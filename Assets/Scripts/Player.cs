using System;

using UnityEngine;

public class Player: MonoBehaviour
{
	public Rigidbody2D rb;
	
	public float speed;
	public float jumpForce;

	public float jumpForgiveness;

	[ReadOnly]
	public bool jumped;
	[ReadOnly]
	public bool grounded;
	[ReadOnly]
	public float timeSinceGrounded;
	
	void FixedUpdate() {
		float dx = Input.GetAxisRaw("Horizontal");
		dx *= speed;
		dx *= Time.deltaTime;
		
		// ground check
		grounded = false;
		timeSinceGrounded += Time.deltaTime;
		Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale + new Vector3(0.2f, 0.2f), transform.rotation.eulerAngles.z);
		foreach (var collider in colliders)
			if (collider.gameObject != gameObject)
			{
				grounded = true;
				timeSinceGrounded = 0;
				break;
			}

		jumped &= timeSinceGrounded <= jumpForgiveness;

		if (!jumped && timeSinceGrounded <= jumpForgiveness && Input.GetButton("Jump"))
			Jump();
		
		rb.AddForce(new Vector2(dx, 0), ForceMode2D.Impulse);
	}

	public void Jump() {
		rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		jumped = true;
	}
}
