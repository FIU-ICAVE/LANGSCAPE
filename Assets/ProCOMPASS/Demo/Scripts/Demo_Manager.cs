using UnityEngine;
using UnityEngine.UI;

public class Demo_Manager : MonoBehaviour {

	public Tracker[] trackers;

	public ProCompass compass;

	public string[] descriptions;

	public Text textBox;

	public Button next;
	public Button prev;

	public Toggle directionRotation;

	public Slider clippingOffset;
	public Slider offset;
	public Slider fieldOfView;

	public GameObject customUI;
	public GameObject arrow;

	int curIndex = 0;

	public void Start () {
		ShowMessage ();
		arrow.SetActive (false);
		prev.interactable = false;
		next.interactable = true;
		compass.enableMinimap = false;
		foreach (Tracker i in trackers) {
			i.GetComponent<Collider> ().enabled = false;
			i.GetComponent<Demo_TrackerAddOrRemove> ().com = compass;
		}
	}

	public void Button_Next () {
		if (curIndex == descriptions.Length - 2) 
			next.interactable = false; 
		prev.interactable = true;
		curIndex++;
		ShowMessage ();
	}

	public void Button_Prev () {
		if (curIndex == 1) 
			prev.interactable = false;
		next.interactable = true;
		curIndex--;
		ShowMessage ();
	}
	
	bool done = false;

	public void ShowMessage () {
		textBox.text = descriptions[curIndex];
		if (curIndex == 2 || curIndex == 5) {
			arrow.SetActive (true);
		} else arrow.SetActive (false);
		if (curIndex == 0)
			StartCoroutine (One ());
		else if (curIndex == 2) {
			StartCoroutine (Two ());
		} else if (curIndex == 6) {
			StartCoroutine (Three ());
		} else if (curIndex == 5) {
			compass.enableMinimap = true;
		}
		if (curIndex == 7 && !done) {
			foreach (Tracker i in trackers) {
				i.gameObject.GetComponent<Collider> ().enabled = true;
			}
			done = true;
		}
	}

	System.Collections.IEnumerator One () {
		while (curIndex == 0) {
			textBox.text  = descriptions[curIndex] + "<b>" + compass.DirectionAngle ().ToString (".0") + "°</b>";
			yield return null;
		}
	}

	System.Collections.IEnumerator Two () {
		int ind = 0;
		while (ind < trackers.Length) {
			compass.AddIndicator (trackers[ind++]);
			yield return new WaitForSeconds (0.2f);
		}
	}

	System.Collections.IEnumerator Three () {
		customUI.SetActive (true);
		directionRotation.isOn = compass.lockDirectionRotation;
		clippingOffset.value = compass.clippingOffset;
		offset.value = compass.offset;
		fieldOfView.value = compass.fieldOfView;
		while (curIndex == 6) {
			compass.lockDirectionRotation = directionRotation.isOn;
			compass.clippingOffset = clippingOffset.value;
			compass.offset = offset.value;
			compass.fieldOfView = fieldOfView.value;
			yield return null;
		}
		customUI.SetActive (false);
	}
}
