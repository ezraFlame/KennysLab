using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification, INotificationOptionProvider
{
	public Dialog dialog;

	private bool retroactive = false;
	private bool emitOnce = false;

	public PropertyName id => new PropertyName();

	public NotificationFlags flags => 
		(retroactive ? NotificationFlags.Retroactive : default) |
		(emitOnce ? NotificationFlags.TriggerOnce : default);
}
