using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Tracker)), CanEditMultipleObjects]
public class TrackerInspector : Editor {
	Tracker tracker;
	SerializedProperty activity;
	SerializedProperty alternativePrefab;
	SerializedProperty indicator;
	private void OnEnable () {
		tracker = (Tracker)target;
		activity = serializedObject.FindProperty ("activity");
		alternativePrefab = serializedObject.FindProperty ("indicatorToBeChanged");
		indicator = serializedObject.FindProperty ("indicator");
	}
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		Line (Color.grey, 2f, 5);
		EditorGUILayout.PropertyField (activity);
		if (activity.enumValueIndex == 1) {
			EditorGUILayout.ObjectField (alternativePrefab);
			tracker.activity = ActionList.showDirectionWhenOutside;
			if (indicator.objectReferenceValue == null)
				EditorGUILayout.HelpBox ("Assign an indicator GameObject", MessageType.Warning);
			if (alternativePrefab.objectReferenceValue == null)
				EditorGUILayout.HelpBox ("Assign a direction indicator GameObject to use.", MessageType.Warning);
		}
		serializedObject.ApplyModifiedProperties ();
	}
	public static void Line (Color color, float thickness, int padding) {
		Rect r = EditorGUILayout.GetControlRect (GUILayout.Height (padding + thickness));
		r.height = thickness;
		r.y += padding / 2;
		r.x -= 2;
		r.width -= 2;
		EditorGUI.DrawRect (r, color);
	}
}

[CustomEditor (typeof (ProCompass)), CanEditMultipleObjects]
public class CompassInspector : Editor {
	ProCompass compass;
	bool showHelp = false, showMessage = false;
	string helpText = "";
	private void OnEnable () {
		compass = (ProCompass)target;
	}
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		if (compass.player == null) {
			helpText = "Assign a player";
			showHelp = true;
		} else if (compass.player == null || !compass.essentialObjects.rotationalObjects || !compass.essentialObjects.background || !compass.essentialObjects.indicators || !compass.essentialObjects.offsideIndicators || !compass.essentialObjects.directions) {
			helpText = "Some GameObjects are not assigned in Essential Objects section. The functions of compass may not work properly";
			showHelp = true;
		} else if (compass.enableMinimap && compass.mapSprite == null) {
			helpText = "Minimap is set to show but the main graphic is missing. Assign a map sprite";
			showHelp = true;
		} else if (compass.enableMinimap && (compass.area.x == 0 || compass.area.y == 0)) {
			helpText = "Having x or y or both component of area equal to zero means there will be no minimap. Set it to larger size according to yuor scene. Otherwise, you can uncheck Enable Minimap";
			showHelp = true;
		} else if (!compass.essentialObjects.background.GetComponent<UnityEngine.UI.Mask> ()) {
			helpText = "Background in the Essential Objects must contain Mask component";
			showHelp = true;
		} else showHelp = false;

		if (showHelp) {
			GUILayout.Space (10);
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (!showMessage && GUILayout.Button ("Show error and warnings", GUILayout.Width (180))) {
				showMessage = true;
			}
			GUILayout.EndHorizontal ();
		} else showMessage = false;

		if (showMessage) {
			EditorGUILayout.HelpBox (helpText, MessageType.Warning);
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Hide Help", GUILayout.Width (100))) {
				GUILayout.Space (10);
				showMessage = false;
			}
			GUILayout.EndHorizontal ();
		}
	}
}