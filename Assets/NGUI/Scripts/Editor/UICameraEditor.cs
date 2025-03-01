//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UICamera))]
public class UICameraEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		UICamera cam = target as UICamera;
		GUILayout.Space(3f);

		serializedObject.Update();

		if (UICamera.eventHandler != cam)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("eventType"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("eventReceiverMask"), new GUIContent("Event Mask"));
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.HelpBox("All other settings are inherited from the First Camera.", MessageType.Info);

			if (GUILayout.Button("Select the First Camera"))
			{
				Selection.activeGameObject = UICamera.eventHandler.gameObject;
			}
		}
		else
		{
			SerializedProperty mouse = serializedObject.FindProperty("useMouse");
			SerializedProperty touch = serializedObject.FindProperty("useTouch");
			SerializedProperty keyboard = serializedObject.FindProperty("useKeyboard");
			SerializedProperty controller = serializedObject.FindProperty("useController");

			EditorGUILayout.PropertyField(serializedObject.FindProperty("eventType"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("eventReceiverMask"), new GUIContent("Event Mask"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("debug"));

            NGUIEditorTools.DrawProperty("TouchFx", serializedObject, "touchFx");

			EditorGUI.BeginDisabledGroup(!mouse.boolValue && !touch.boolValue);
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("allowMultiTouch"));
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(!mouse.boolValue);
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stickyTooltip"));

				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("tooltipDelay"));
				GUILayout.Label("seconds", GUILayout.MinWidth(60f));
				GUILayout.EndHorizontal();
			}
			EditorGUI.EndDisabledGroup();

			GUILayout.BeginHorizontal();
			SerializedProperty rd = serializedObject.FindProperty("rangeDistance");
			EditorGUILayout.PropertyField(rd, new GUIContent("Raycast Range"));
			GUILayout.Label(rd.floatValue < 0f ? "unlimited" : "units", GUILayout.MinWidth(60f));
			GUILayout.EndHorizontal();

			NGUIEditorTools.SetLabelWidth(80f);

			if (NGUIEditorTools.DrawHeader("Event Sources"))
			{
				NGUIEditorTools.BeginContents();
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(mouse, new GUIContent("Mouse"), GUILayout.MinWidth(100f));
					EditorGUILayout.PropertyField(touch, new GUIContent("Touch"), GUILayout.MinWidth(100f));
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(keyboard, new GUIContent("Keyboard"), GUILayout.MinWidth(100f));
					EditorGUILayout.PropertyField(controller, new GUIContent("Controller"), GUILayout.MinWidth(100f));
					GUILayout.EndHorizontal();
				}
				NGUIEditorTools.EndContents();
			}

			if ((mouse.boolValue || touch.boolValue) && NGUIEditorTools.DrawHeader("Thresholds"))
			{
				NGUIEditorTools.BeginContents();
				{
					EditorGUI.BeginDisabledGroup(!mouse.boolValue);
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseDragThreshold"), new GUIContent("Mouse Drag"), GUILayout.Width(120f));
					GUILayout.Label("pixels");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseClickThreshold"), new GUIContent("Mouse Click"), GUILayout.Width(120f));
					GUILayout.Label("pixels");
					GUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();

					EditorGUI.BeginDisabledGroup(!touch.boolValue);
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("touchDragThreshold"), new GUIContent("Touch Drag"), GUILayout.Width(120f));
					GUILayout.Label("pixels");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("touchClickThreshold"), new GUIContent("Touch Tap"), GUILayout.Width(120f));
					GUILayout.Label("pixels");
					GUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();
				}
				NGUIEditorTools.EndContents();
			}

			if ((mouse.boolValue || keyboard.boolValue || controller.boolValue) && NGUIEditorTools.DrawHeader("Axes and Keys"))
			{
				NGUIEditorTools.BeginContents();
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalAxisName"), new GUIContent("Horizontal"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalAxisName"), new GUIContent("Vertical"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollAxisName"), new GUIContent("Scroll"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("submitKey0"), new GUIContent("Submit 1"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("submitKey1"), new GUIContent("Submit 2"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("cancelKey0"), new GUIContent("Cancel 1"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("cancelKey1"), new GUIContent("Cancel 2"));
				}
				NGUIEditorTools.EndContents();
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
