using UnityEngine;

public class GameManager: MonoBehaviour
{
	public static GameManager instance;
	public Transform goal;

	void Awake() {
		instance = this;
	}
}
