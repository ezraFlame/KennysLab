using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
	public float interactDistance;

	private PlayerMovement playerMovement;

	private void Start() {
		playerMovement = GetComponent<PlayerMovement>();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (!PlayerData.instance.interacting && !PlayerData.instance.inCutscene)
			{
				CheckForInteractables(playerMovement.previousDirection);
			} else
			{
				DialogManager.instance.DisplayNextSentence();
			}
		}
	}

	private void CheckForInteractables(Vector2 direction) {
		BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

		RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0, direction, interactDistance);

		foreach (RaycastHit2D hit in hits)
		{
			GameObject go = hit.transform.gameObject;

			Interactable interactable;

			if (go.TryGetComponent<Interactable>(out interactable))
			{
				interactable.Interact();
			}
		}
	}
}
