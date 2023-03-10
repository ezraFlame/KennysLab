using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
	public static RoomManager instance;

	public Dictionary<string, Vector3> entrancePoints;

	private void Start() {
		instance = this;

		GetEntrancePoints();
	}

	public void LoadRoom(string entrancePoint, string sceneName) {
		StartCoroutine(_LoadRoom(entrancePoint, sceneName));
	}

	private IEnumerator _LoadRoom(string entrancePoint, string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName);

		GetEntrancePoints();

		GameObject.FindGameObjectWithTag("Player").transform.position = entrancePoints[entrancePoint];
	}

	private void GetEntrancePoints() {
		entrancePoints = new Dictionary<string, Vector3>();

		foreach (EntrancePoint entrance in FindObjectsOfType<EntrancePoint>())
		{
			entrancePoints.Add(entrance.entrancePointName, entrance.transform.position);
		}
	}
}
