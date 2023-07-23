using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionList {
	doNothing,
	showDirectionWhenOutside,
	clipIndicator
}

[System.Serializable]
public struct EssentialObjects {
	[Tooltip ("The gameObject which will rotate as the compass")]
	public GameObject rotationalObjects;
	
	[Tooltip ("Background of the compass")]
	public GameObject background;
	
	[Tooltip ("The indicators of the compass will be spawned in this gameObject")]
	public GameObject indicators;
	
	[Tooltip ("The alternative indicators that are set to be chenged are spawned in this gameObject")]
	public GameObject offsideIndicators;
	
	[Tooltip ("GameObject that contains direction signs or other things in the compass")]
	public GameObject directions;
}

public class ProCompass: MonoBehaviour {

	[Tooltip ("Player of the compass or the gameObject under which the compass will work")]
	public Transform player;

	[Space]
	
	[Tooltip ("List of trackers that will indicate their position in the compass")]
	public List<Tracker> trackers = new List<Tracker> ();
	
	[Tooltip ("Essential gameObjects in compass gameObject")]
	public EssentialObjects essentialObjects;

	[Space]
	
	[Header ("Appearance")]
	
	[Tooltip ("Lock rotation of the objects in 'Directions' gameObject")]
	public bool lockDirectionRotation = true;
	
	[Tooltip ("Offset of the indicators that are being clipped inside compass")]
	public float clippingOffset = 15f;
	
	[Tooltip ("Offset of the indicators outside compass")]
	public float offset = 7.5f;
	
	[Tooltip ("Field of view of the compass")]
	[Range (1f, 300f)]
	public float fieldOfView = 50f;

	[Tooltip ("Set the scale or size of indicators on compass. Set it to 1 to use the actual size of the indicator prefab used in trackers. This value can not be changed in play mode")]
	[Range (0.01f, 3f)]
	public float indicatorScale = 1f;
	
	[Space]
	
	[Header ("Minimap")]
	
	[Tooltip ("Turn on/off minimap")]
	public bool enableMinimap;
	
	[Tooltip ("Sprite of the minimap")]
	public Sprite mapSprite;
	
	[Tooltip ("The area that the map will cover in the scene")]
	public Vector2 area;

	[Tooltip ("Calibrate or move the position of the minimap with respect to player")]
	public Vector2 moveMinimapBy;

	[Tooltip ("Select layers to show layer wise gameObjects")]
	public LayerMask layerMask = ~0;

	private float range;
	
	private GameObject minimap;

	private Dictionary<int, bool> map = new Dictionary<int, bool> ();
	private Dictionary<int, Transform> ind = new Dictionary<int, Transform> ();

	private Vector3 dir;

	Vector3 position {
		get {
			return player.position;
		}
	}

	private void Start () {
		if (!player) {
			Debug.LogError ("Player of the compass is not assigned", gameObject);
			enabled = false;
			return;
		} else if (!essentialObjects.rotationalObjects || !essentialObjects.background || !essentialObjects.indicators || !essentialObjects.offsideIndicators) {
			enabled = false;
			Debug.LogError ("Necessary components of essential objects are not assigned. Please assign them properly.", gameObject);
			return;
		} else if (!GetComponent<RectTransform> ()) {
			enabled = false;
			Debug.LogError ("Compass needs RectTransform component to work, but there isn't any RectTransform attached to '" + gameObject.name + "'. The object must be an UI object.", gameObject);
			return;
		}

		range = GetComponent<RectTransform> ().sizeDelta.x / 2;

		InitializeMinimap ();
		
		for (short i = 0; i < trackers.Count; i++) {
			if (trackers[i] == null) {
				trackers.RemoveAt (i);
				i--;
				continue;
			}
			AddIndicatorInternal (trackers[i]);
		}
	}

	private void Update () {
		dir.z = player.eulerAngles.y;
		essentialObjects.rotationalObjects.transform.localEulerAngles = dir;
		for (int i = 0; i < trackers.Count; i++) {
			if (!trackers[i]) {
				UnityEngine.Debug.LogWarning ("A gameObject with Tracker is destroyed or removed whthout removing it from the compass.");
				RemoveIndicator (trackers[i]);
			} else
				Indicate (trackers [i]);
		}
		ShowMinimap ();
		LockRotation ();
	}

	// mechanism for showing an indicator on the compass
	private void Indicate (Tracker i) {
		int id = i.GetInstanceID ();

		if ((layerMask.value & (1 << i.gameObject.layer)) == 0) {
			ind[id].gameObject.SetActive (false);
			return;
		} else if (!ind[id].gameObject.activeSelf) {
			ind[id].gameObject.SetActive (true);
		}

		float deltaX = (i.target.position.x - position.x) * range / fieldOfView;
		float deltaZ = (i.target.position.z - position.z) * range / fieldOfView;

		float distance = Mathf.Sqrt ((deltaZ * deltaZ) + (deltaX * deltaX));

		if (Mathf.Abs (distance) > range - clippingOffset) {

			float finalX, finalZ;

			if (i.activity == ActionList.doNothing) {
				ind[id].gameObject.SetActive (false);
				return;
			} else if (i.activity == ActionList.showDirectionWhenOutside) {
				finalX = (deltaX * (range + offset)) / distance;
				finalZ = (deltaZ * (range + offset)) / distance;

				if (map [id]) {
					ind [id] = SwapIndicator (ind [id]);
					map [id] = false;
				}
			} else {
				finalX = (deltaX * (range - clippingOffset)) / distance;
				finalZ = (deltaZ * (range - clippingOffset)) / distance;
			}

			ind[id].localPosition = new Vector3 (finalX, finalZ, 0);
		} else {
			if (i.activity == ActionList.doNothing)
				ind[id].gameObject.SetActive (true);
			else if (i.activity == ActionList.showDirectionWhenOutside && !map [id]) {
				ind[id] = SwapIndicator (ind[id]);
				map [id] = true;
			}
			ind[id].localPosition = new Vector3 (deltaX, deltaZ, 0);
		}

		if (i.activity == ActionList.showDirectionWhenOutside && !map[id]) {
			float theta = 270 + Mathf.Atan2 (deltaZ, deltaX) * Mathf.Rad2Deg;
			ind[id].localEulerAngles = new Vector3 (0, 0, theta);
		} else if (i.allowRotation) {
			ind[id].localEulerAngles = new Vector3 (0, 0, i.target.eulerAngles.y);
		} else {
			ind[id].eulerAngles = Vector3.zero;
		}
	}

	// adds indicator of a tracker to associated list and dictionary
	// to identify the indicator of a gameObject, instance id of its tracker component is used everywhere in this script
	private void AddIndicatorInternal (Tracker i) {
		if (!i.indicator) {
			Debug.LogError ("Indicator of Tracker on gameObject '" + i.gameObject.name + "' is not set.", i.gameObject);
			trackers.RemoveAt (trackers.IndexOf (i));
			return;
		}
		GameObject temp = Instantiate (i.indicator, Vector3.zero, Quaternion.identity, essentialObjects.indicators.transform);
		temp.transform.localScale = i.indicator.transform.localScale * indicatorScale;
		ind.Add (i.GetInstanceID (), temp.transform);
		if (i.activity == ActionList.showDirectionWhenOutside) {
			if (!i.indicatorToBeChanged) {
				Debug.LogError ("The activity of Tracker in \"" + i.gameObject.name + "\" was set to show direction when it is outside of compass. But the alternative indicator or direction prefab is null. This function will not work properly.", i.gameObject);
			} else {
				GameObject alternativeIndicator = Instantiate (i.indicatorToBeChanged, temp.transform);
				alternativeIndicator.gameObject.SetActive (false);
				alternativeIndicator.transform.localPosition = Vector3.zero;
				alternativeIndicator.transform.localScale = Vector2.one;
				alternativeIndicator.transform.SetAsFirstSibling ();
			}
			map.Add (i.GetInstanceID (), true);
		}
	}

	bool isMinimapEnabled = false;
	RectTransform compassRect;

	// shows the minimap
	private void ShowMinimap () {
		if (enableMinimap) {
			if (!minimap) {
				InitializeMinimap ();
				return;
			}
			if (!isMinimapEnabled) {
				minimap.gameObject.SetActive (true);
				isMinimapEnabled = true;
			}
			float deltaX = position.x * range / fieldOfView;
			float deltaZ = position.z * range / fieldOfView;

			minimap.transform.localPosition = new Vector3 (-deltaX + moveMinimapBy.x, -deltaZ + moveMinimapBy.y, 0);
			compassRect.sizeDelta = area * range / fieldOfView;
		} else if (isMinimapEnabled && minimap) {
			minimap.gameObject.SetActive (false);
			isMinimapEnabled = false;
		}
	}

	// creates a temporary map gameObject and sets sizes
	private void InitializeMinimap () {
		if (enableMinimap) {
			if (!mapSprite) {
				Debug.LogError ("Please assign the parameters of minimap in the compass corectly.", gameObject);
				enableMinimap = false;
				return;
			}
			if (area.x == 0 || area.y == 0) {
				Debug.LogError ("The area of the minimap can't be zero.", gameObject);
				enableMinimap = false;
				return;
			}
			if (!essentialObjects.background.gameObject.GetComponent<Mask> ()) {
				essentialObjects.background.gameObject.AddComponent<Mask> ();
				Debug.Log ("As there was no 'Mask' component attached to '" + essentialObjects.background.name + "', a Mask component was added.");
			}
			minimap = new GameObject ("Minimap", typeof (RectTransform), typeof (Image));
			minimap.GetComponent<Image> ().sprite = mapSprite;
			minimap.transform.SetParent (essentialObjects.background.transform);
			minimap.transform.localScale = Vector3.one;
			minimap.transform.SetAsFirstSibling ();
			compassRect = minimap.GetComponent<RectTransform> ();
		}
	}

	// swaps the child and parent gameObject
	private Transform SwapIndicator (Transform original) {
		Transform duplicate = null;
		if (original.parent.gameObject == essentialObjects.indicators) {
			original.SetParent (essentialObjects.offsideIndicators.transform);
		} else original.SetParent (essentialObjects.indicators.transform);
		if (original.childCount > 0) {
			duplicate = original.GetChild (0);
			duplicate.SetParent (original.parent);
			original.SetParent (duplicate);
			original.localPosition = Vector3.zero;
			duplicate.gameObject.SetActive (true);
			original.gameObject.SetActive (false);
		}
		return duplicate? duplicate : original;
	}

	private void LockRotation () {
		if (essentialObjects.directions)
			foreach (Transform child in essentialObjects.directions.transform) {
				if (lockDirectionRotation)
					child.eulerAngles = Vector3.zero;
				else
					child.localEulerAngles = Vector3.zero;
			}
	}

	/// <summary>
	/// Adds the indicator to compass.
	/// </summary>
	/// <param name="i">Tracker attached with gameObject.</param>
	public void AddIndicator (Tracker i) {
		if (!trackers.Contains (i)) {
			trackers.Add (i);
			AddIndicatorInternal (i);
		}
	}

	/// <summary>
	/// Removes the indicator from compass.
	/// </summary>
	/// <param name="i">Tracker attached with gameObject.</param>
	public void RemoveIndicator (Tracker i) {
		int id = i.GetInstanceID ();
		if (!ind.ContainsKey (id)) {
			Debug.LogError ("Failed to remove indicator. Tracker with gameObject, '" + i.gameObject.name + "' was not found in the Dictionary of indicaotrs. This means the following Tracker was not added to this compass before or somehow deleted externally", i.gameObject);
			return;
		}
		trackers.RemoveAt (trackers.IndexOf (i));
		Destroy (ind[id].gameObject);
		ind.Remove (id);
		if (map.ContainsKey (id)) map.Remove (id);
	}

	/// <summary>
	/// Returns the direction angle of the compass.
	/// </summary>
	/// <returns>The angle of the compass.</returns>
	public float DirectionAngle () {
		return float.Parse (dir.z.ToString ("F"));
	}
}