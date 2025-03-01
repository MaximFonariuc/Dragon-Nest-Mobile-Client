//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenPosition))]
public class TweenPositionEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenPosition tw = target as TweenPosition;
		GUI.changed = false;

		Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
		Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);

	    bool nox = EditorGUILayout.Toggle("nox", tw.nox);
	    bool noy = EditorGUILayout.Toggle("noy", tw.noy);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
		    tw.nox = nox;
		    tw.noy = noy;
			NGUITools.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
