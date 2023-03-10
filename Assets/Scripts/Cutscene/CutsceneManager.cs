using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager instance;

	private PlayableDirector director;

	private void Start() {
		instance = this;

		director = GetComponent<PlayableDirector>();
	}

	private void Update() {
		if (director.time >= director.duration)
		{
			PlayerData.instance.inCutscene = false;
		}
	}

	public void PlayCutscene(PlayableAsset cutscene = null) {
		if (cutscene != null)
		{
			director.Stop();
			director.playableAsset = cutscene;
			director.Play();
		} else
		{
			director.Resume();
		}

		PlayerData.instance.inCutscene = true;
	}

	public void PauseCutscene() {
		director.Pause();
	}
}
