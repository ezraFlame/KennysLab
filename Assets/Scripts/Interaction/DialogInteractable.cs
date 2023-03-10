using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogInteractable : Interactable
{
	public Dialog dialog;

	public override void Interact() {
		DialogManager.instance.StartDialog(dialog);
	}
}
