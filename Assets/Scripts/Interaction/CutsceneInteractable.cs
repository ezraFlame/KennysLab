using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneInteractable : Interactable
{
	public PlayableAsset cutscene;

	public override void Interact() {
		CutsceneManager.instance.PlayCutscene(cutscene);
	}
}
