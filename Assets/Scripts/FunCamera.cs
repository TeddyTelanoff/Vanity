using System;

using UnityEngine;

public class FunCamera: MonoBehaviour
{
	public Transform follow;
	public float lowest;
	[Range(0.01f, 1)]
	public float lerp;

	void Update() {
		transform.position = Vector3.Lerp(transform.position,
										  new Vector3(transform.position.x, Mathf.Max(follow.position.y, lowest),
													  transform.position.z), lerp * Time.deltaTime);
	}
}
