using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeAnimation : DialogAnimation
{
	public override Vector3 GetOffset(int charIndex) {
		Vector3 offset = new Vector3();

		offset.x += (Mathf.PerlinNoise((charIndex + Time.time) * 10f, 0) - 0.5f) * 4f;
		offset.y += (Mathf.PerlinNoise((charIndex + Time.time) * 10f, 1000) - 0.5f) * 4f;

		return offset;
	}

	public override string AnimName => "shake";
}
