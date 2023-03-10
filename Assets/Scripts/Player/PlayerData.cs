using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
	public static PlayerData instance;

	public bool interacting;

	public bool inCutscene;

	private void Start() {
		instance = this;

		interacting = false;
		inCutscene = false;
	}
}
