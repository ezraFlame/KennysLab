using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnimation : DialogAnimation
{
	public override Vector3 GetOffset(int charIndex) {
		return new Vector3(0, Mathf.Sin(charIndex + (Time.time * 15)) * 2.5f, 0);
	}

	public override string AnimName => "wave";
}
