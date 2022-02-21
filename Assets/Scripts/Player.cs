using System;
using System.Collections;

using UnityEngine;

public class Player: MonoBehaviour
{
	public Rigidbody2D rb;
	
	public float speed;
	public float jumpForce;

	public float jumpForgiveness;
	public float jumpGiveness;
	

	[ReadOnly]
	public bool jumped;
	[ReadOnly]
	public bool canJump;
	[ReadOnly]
	public bool grounded;
	[ReadOnly]
	public bool orbded;
	[ReadOnly]
	public float timeSinceGrounded;
	[ReadOnly]
	public float timeSinceOrbded;
	[ReadOnly]
	public float timeSinceJumpAttempt;

	[Header("Timmy")]
	public float timmyTimeout;
	[ReadOnly]
	public float timeSinceTimmy;

	void Update() {
		timeSinceJumpAttempt += Time.deltaTime;
		if (Input.GetButtonDown("Jump"))
			timeSinceJumpAttempt = 0;
	}

	void FixedUpdate() {
		float dx = Input.GetAxisRaw("Horizontal");
		dx *= speed;
		dx *= Time.deltaTime;

		timeSinceTimmy += Time.deltaTime;
		
		// ground check
		grounded = false;
		timeSinceGrounded += Time.deltaTime;
		timeSinceOrbded += Time.deltaTime;
		Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale + new Vector3(0.2f, 0.2f), transform.rotation.eulerAngles.z);
		foreach (var collider in colliders)
			if (collider.gameObject != gameObject)
			{
				int layer = collider.gameObject.layer;
				if (layer == LayerMask.NameToLayer("Ground"))
				{
					grounded = true;
					timeSinceGrounded = 0;
				}
				else if (layer == LayerMask.NameToLayer("Orb"))
				{
					orbded = true;
					timeSinceOrbded = 0;
				}
				else if (layer == LayerMask.NameToLayer("Goal"))
				{
					print("goal");
					StartCoroutine(Goal(collider.transform));
				}
				else if (layer == LayerMask.NameToLayer("TimmyTrigger"))
				{
					if (timeSinceTimmy >= timmyTimeout && canJump)
					{
						Jump();
						float dist = GameManager.instance.goal.position.x - transform.position.x;
						dx = speed * Mathf.Sign(dist);

						timeSinceTimmy = 0;
					}
				}
			}

		jumped &= timeSinceGrounded <= jumpForgiveness && timeSinceOrbded <= jumpForgiveness;

		canJump = false;
		if (!jumped)
		{
			if (timeSinceGrounded <= jumpForgiveness)
			{
				canJump = true;
				if (Input.GetButton("Jump"))
					Jump();
			}
			else if (timeSinceOrbded <= jumpForgiveness)
			{
				canJump = true;
				if (timeSinceJumpAttempt <= jumpGiveness)
					Jump();
			}
		}

		rb.AddForce(new Vector2(dx, 0), ForceMode2D.Impulse);
	}

	public void Jump() {
		rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		jumped = true;
	}

	public void TryJump() {
		if (canJump)
			Jump();
	}

	IEnumerator Goal(Transform goal) {
		Time.timeScale = 0.1f;

		for (float t = 0; t < 2; t += Time.unscaledDeltaTime)
		{
			transform.position = Vector3.Lerp(transform.position,
											  goal.transform.position, 0.69f * Time.unscaledDeltaTime);

			transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 120 * Time.unscaledDeltaTime);
			yield return new WaitForEndOfFrame();
		}

		Time.timeScale = 0;
	}
}
