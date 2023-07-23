using UnityEngine;

public class Demo_TrackerAddOrRemove : MonoBehaviour {

	public ProCompass com;

	bool removed = false;

	public GameObject show;

	void OnTriggerEnter () {
		show.SetActive (true);
	}

	void OnTriggerExit () {
		show.SetActive (false);
	}

	void OnTriggerStay () {
		if (Input.GetKey (KeyCode.R) && !removed) {
			com.RemoveIndicator (GetComponent<Tracker> ());
			removed = true;
		} else if (Input.GetKey (KeyCode.Q) && removed) {
			com.AddIndicator (GetComponent<Tracker> ());
			removed = false;
		}
		if (!removed)
			show.GetComponent<UnityEngine.UI.Text> ().text = "Press 'R' to remove its indicator from compass";
		else show.GetComponent<UnityEngine.UI.Text> ().text = "Press 'Q' to add its indicator to compass";
	}
}
