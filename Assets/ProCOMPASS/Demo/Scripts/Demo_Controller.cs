using UnityEngine;

public class Demo_Controller : MonoBehaviour {

	Transform player;

	public Transform cam;

	public float walkSpeed = 1f;

	bool isRunning = false;

	void Start () {
		player = GetComponent<Transform> ();
	}

	void Update () {

		float yRot = Input.GetAxis("Mouse X") * 2.5f;
		float xRot = Input.GetAxis("Mouse Y") * 2.5f;

		player.localRotation *= Quaternion.Euler (0, yRot, 0);
		cam.localRotation *= Quaternion.Euler (-xRot, 0, 0);

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			Move (-walkSpeed, 0);
		}
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			Move (walkSpeed, 0);
		}
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			Move (0, walkSpeed);
		}
		if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			Move (0, -walkSpeed);
		}

		if (Input.GetKey (KeyCode.LeftShift))
			isRunning = true;
		else isRunning = false;
	}

	void Move (float x, float z) {
		transform.Translate (new Vector3 (x, 0, z) * Time.deltaTime * (isRunning? 2 : 1));
	}
}
