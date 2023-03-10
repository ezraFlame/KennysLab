using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;

    private float x, y;

    public Vector2 previousDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        previousDirection = Vector2.down;
    }

    void Update()
    {
		x = 0;
		y = 0;

        if (!PlayerData.instance.interacting && !PlayerData.instance.inCutscene)
        {
			x = Input.GetAxisRaw("Horizontal");
			y = Input.GetAxisRaw("Vertical");

			if (x > 0)
				previousDirection = Vector2.right;
			else if (x < 0)
				previousDirection = Vector2.left;
			if (y > 0)
				previousDirection = Vector2.up;
			else if (y < 0)
				previousDirection = Vector2.down;
		}
	}

	private void FixedUpdate() {
		rb.velocity = new Vector2(x, y) * speed;
	}
}
