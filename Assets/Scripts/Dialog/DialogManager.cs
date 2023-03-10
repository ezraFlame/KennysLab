using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    private Queue<string> sentences;

    public GameObject textBox;
    public TMP_Text nameText;
    public TMP_Text dialogText;

    public float typeDelay;

    public bool textAnimating;

    public List<DialogCommand> commands;

    public Dictionary<string, DialogAnimation> dialogAnimations;

	private void Start()
    {
        sentences = new Queue<string>();

        instance = this;

        textBox.SetActive(false);

        commands = new List<DialogCommand>();

		RegisterDialogAnimations();
	}

	public void StartDialog(Dialog dialog) {
        sentences.Clear();

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }

        nameText.text = dialog.npcName;

		textBox.SetActive(true);

		commands = new List<DialogCommand>();

		DisplayNextSentence();

        PlayerData.instance.interacting = true;
	}

    public void DisplayNextSentence() {
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        string sentence = sentences.Dequeue();
        commands = DialogCommandParser.ParseText(sentence, out string parsedSentence);
        StopAllCoroutines();
        StartCoroutine(TypeText(parsedSentence));
    }

    public IEnumerator TypeText(string text) {
		dialogText.text = text;

		dialogText.ForceMeshUpdate();

        TextAnimInfo[] animInfo = BuildTextAnimInfos(commands);

        TMP_TextInfo textInfo = dialogText.textInfo;

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        Color32[][] originalColors = new Color32[textInfo.meshInfo.Length][];
        for (int i = 0; i < originalColors.Length; i++)
        {
            Color32[] colors = textInfo.meshInfo[i].colors32;
            originalColors[i] = new Color32[colors.Length];
            Array.Copy(colors, originalColors[i], colors.Length);
        }

        float currentTypeDelay = typeDelay;
        float pauseTime = 0f;

        int visibleCharIndex = 0;
        while (true)
        {
            pauseTime = 0f;

            if (visibleCharIndex < textInfo.characterCount)
            {
                ExecuteCommandsAtIndex(commands, visibleCharIndex, ref currentTypeDelay, ref pauseTime);
            }

            for (int j = 0; j < textInfo.characterCount; j++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[j];
                if (charInfo.isVisible)
                {
                    int vertexIndex = charInfo.vertexIndex;
                    int materialIndex = charInfo.materialReferenceIndex;

                    Color32[] destinationColors = textInfo.meshInfo[materialIndex].colors32;
                    Color32 color = j < visibleCharIndex ? originalColors[materialIndex][vertexIndex] : new Color32(0, 0, 0, 0);
                    destinationColors[vertexIndex] = color;
                    destinationColors[vertexIndex + 1] = color;
                    destinationColors[vertexIndex + 2] = color;
                    destinationColors[vertexIndex + 3] = color;

                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    Vector3 offset = Vector3.zero;

                    for (int k = 0; k < animInfo.Length; k++)
                    {
                        TextAnimInfo info = animInfo[k];
                        if (j >= info.startIndex && j < info.endIndex)
                        {
                            DialogAnimation anim = dialogAnimations[info.animType];

                            if (anim != null)
                            {
                                offset = anim.GetOffset(j);
                            }
                        }
					}

					destinationVertices[vertexIndex] = sourceVertices[vertexIndex] + offset;
					destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
					destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
					destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
				}
            }

            dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo info = textInfo.meshInfo[i];
                info.mesh.vertices = info.vertices;
                dialogText.UpdateGeometry(info.mesh, i);
            }

            visibleCharIndex++;
            if (pauseTime != 0)
            {
                yield return new WaitForSeconds(pauseTime);
            } else
            {
				yield return new WaitForSeconds(currentTypeDelay);
			}
        }
    }
    private void EndDialog() {
        textBox.SetActive(false);

        PlayerData.instance.interacting = false;

        if (PlayerData.instance.inCutscene)
        {
            CutsceneManager.instance.PlayCutscene();
        }
	}

    private TextAnimInfo[] BuildTextAnimInfos(List<DialogCommand> commands) {
        List<TextAnimInfo> tempResult = new List<TextAnimInfo>();
        List<DialogCommand> animStartCommands = new List<DialogCommand>();
        List<DialogCommand> animEndCommands = new List<DialogCommand>();

        for (int i = 0; i < commands.Count; i++)
        {
            DialogCommand command = commands[i];
            if (command.type == DialogCommandType.AnimStart)
            {
                animStartCommands.Add(command);
                commands.RemoveAt(i);
                i--;
            } else if (command.type == DialogCommandType.AnimEnd)
            {
                animEndCommands.Add(command);
                commands.RemoveAt(i);
                i--;
            }
        }

        if (animStartCommands.Count != animEndCommands.Count)
        {
            Debug.LogError("You don't have the same number of animation start tags (" + animStartCommands.Count + ") and animation end tags (" + animEndCommands.Count + ").");
        } else
        {
            for (int i = 0; i < animStartCommands.Count; i++)
            {
                DialogCommand startCommand = animStartCommands[i];
                DialogCommand endCommand = animEndCommands[i];
                tempResult.Add(new TextAnimInfo
                {
                    startIndex = startCommand.position,
                    endIndex = endCommand.position,
                    animType = startCommand.value
                });
            }
        }

        return tempResult.ToArray();
    }

    private void RegisterDialogAnimations() {
        dialogAnimations = new Dictionary<string, DialogAnimation>();

        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        Type baseType = typeof(DialogAnimation);

        foreach (Type type in currentAssembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(baseType))
            {
                continue;
            }

            DialogAnimation animation = Activator.CreateInstance(type) as DialogAnimation;

            if (animation != null)
            {
                dialogAnimations.Add(animation.AnimName, animation);
            }
        }
    }

    private void ExecuteCommandsAtIndex(List<DialogCommand> commands, int visibleCharIndex, ref float delay, ref float pauseTime) {
        for (int i = 0; i < commands.Count; i++)
        {
            DialogCommand command = commands[i];
            if (command.position == visibleCharIndex)
            {
                switch (command.type)
                {
                    case DialogCommandType.Speed:
                        if (command.value == "def")
                        {
                            delay = typeDelay;
                            break;
                        }
                        float lastDelay = delay;
                        if (!float.TryParse(command.value, out delay))
                        {
                            delay = lastDelay;
                        }
                        break;
                    case DialogCommandType.Pause:
                        pauseTime = float.Parse(command.value);
                        break;
                }
                commands.RemoveAt(i);
                i--;
            }
        }
    }
}

public struct TextAnimInfo
{
    public int startIndex;
    public int endIndex;
    public string animType; //make this an enum later
}
