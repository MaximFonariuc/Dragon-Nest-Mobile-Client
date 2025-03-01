﻿//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector class used to edit UISpriteAnimations.
/// </summary>

[CustomEditor(typeof(UISpriteAnimation))]
public class UISpriteAnimationInspector : Editor
{
	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.DrawSeparator();
		NGUIEditorTools.SetLabelWidth(80f);
		UISpriteAnimation anim = target as UISpriteAnimation;

		int fps = EditorGUILayout.IntField("Framerate", anim.framesPerSecond);
		fps = Mathf.Clamp(fps, 0, 60);

		if (anim.framesPerSecond != fps)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.framesPerSecond = fps;
			EditorUtility.SetDirty(anim);
		}

		string namePrefix = EditorGUILayout.TextField("Name Prefix", (anim.namePrefix != null) ? anim.namePrefix : "");
        if (anim.namePrefix != namePrefix)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.namePrefix = namePrefix;
			EditorUtility.SetDirty(anim);
		}

		bool loop = EditorGUILayout.Toggle("Loop", anim.loop);
        if (anim.loop != loop)
		{
			NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
			anim.loop = loop;
			EditorUtility.SetDirty(anim);
		}

        int interval = EditorGUILayout.IntField("Interval", anim.loopinterval);
        if (anim.loopinterval != interval)
        {
            NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
            anim.loopinterval = interval;
            EditorUtility.SetDirty(anim);
        }

        bool disableWhenFinish = EditorGUILayout.Toggle("DisableWhenFinish", anim.disableWhenFinish);
        if (anim.disableWhenFinish != disableWhenFinish)
        {
            NGUIEditorTools.RegisterUndo("Sprite Animation Change", anim);
            anim.disableWhenFinish = disableWhenFinish;
            EditorUtility.SetDirty(anim);
        }

	}
}