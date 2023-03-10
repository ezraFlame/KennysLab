using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogAnimation
{
	public DialogAnimation() { }

	public abstract Vector3 GetOffset(int charIndex);

	public virtual string AnimName => GetType().Name;
}
