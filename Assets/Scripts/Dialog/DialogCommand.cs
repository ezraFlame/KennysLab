using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogCommandType
{
	AnimStart,
	AnimEnd,
	Speed,
	Pause
}

public struct DialogCommand
{
	public int position;
	public DialogCommandType type;
	public string value;
}