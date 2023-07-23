using UnityEngine;

public class Tracker : MonoBehaviour {

	[Tooltip ("The indicator of this gameObject on the compass")]
	public GameObject indicator;
	[Tooltip ("Should the indicator rotate as this gameObject rotates")]
	public bool allowRotation = false;

	[HideInInspector]
	public ActionList activity;
	[HideInInspector]
	public GameObject indicatorToBeChanged;

	public Transform target {
		get {
			return transform;
		}
	}
}
