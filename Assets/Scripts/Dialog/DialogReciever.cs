using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogReciever : MonoBehaviour, INotificationReceiver
{

	public void OnNotify(Playable origin, INotification notification, object context) {
		if (notification is DialogMarker dialogMarker)
		{
			Dialog dialog = dialogMarker.dialog;
			DialogManager.instance.StartDialog(dialog);
			CutsceneManager.instance.PauseCutscene();
		}
	}
}
