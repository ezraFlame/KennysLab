using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ExitPoint : MonoBehaviour
{
	public string entrancePointName;

	public string sceneToLoad;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player"))
		{
			RoomManager.instance.LoadRoom(entrancePointName, sceneToLoad);
		}
	}
}
