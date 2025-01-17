//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIProgressBar))]
#else
[CustomEditor(typeof(UIProgressBar), true)]
#endif
public class UIProgressBarEditor : UIWidgetContainerEditor
{
	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);

		serializedObject.Update();

		GUILayout.Space(3f);

		DrawLegacyFields();

		GUILayout.BeginHorizontal();
		SerializedProperty sp = NGUIEditorTools.DrawProperty("Steps", serializedObject, "numberOfSteps", GUILayout.Width(110f));
		if (sp.intValue == 0) GUILayout.Label("= unlimited");
		GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        NGUIEditorTools.DrawProperty("DynamicThreshold", serializedObject, "mDynamicThreshold", GUILayout.Width(150f));
        GUILayout.EndHorizontal();

        NGUIEditorTools.SetLabelWidth(120f);
        UIProgressBar bar = target as UIProgressBar;
        bool bHTAE = EditorGUILayout.Toggle("HideThumbAtEnds", bar.bHideThumbAtEnds);
        if (bHTAE != bar.bHideThumbAtEnds)
            bar.bHideThumbAtEnds = bHTAE;

        bool bHTAE1 = EditorGUILayout.Toggle("HideFgAtEnds", bar.bHideFgAtEnds);
        if (bHTAE1 != bar.bHideFgAtEnds) bar.bHideFgAtEnds = bHTAE1;

        bool fillDirstate = EditorGUILayout.Toggle("UseFillDir", bar.UseFillDir);
        if (fillDirstate != bar.UseFillDir)
            bar.UseFillDir = fillDirstate;

		OnDrawExtraFields();

		if (NGUIEditorTools.DrawHeader("Appearance"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.DrawProperty("Foreground", serializedObject, "mFG");
			NGUIEditorTools.DrawProperty("Background", serializedObject, "mBG");
            NGUIEditorTools.DrawProperty("Dynamicground", serializedObject, "mDG");
            NGUIEditorTools.DrawProperty("FullFx", serializedObject, "mFullFx");
            NGUIEditorTools.DrawProperty("KeepFx", serializedObject, "mFx");
			NGUIEditorTools.DrawProperty("Thumb", serializedObject, "thumb");

			GUILayout.BeginHorizontal();
			NGUIEditorTools.DrawProperty("Direction", serializedObject, "mFill");
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();

			OnDrawAppearance();
			NGUIEditorTools.EndContents();
		}

		UIProgressBar sb = target as UIProgressBar;
		NGUIEditorTools.DrawEvents("On Value Change", sb, sb.onChange);
		serializedObject.ApplyModifiedProperties();
	}

	protected virtual void DrawLegacyFields()
	{
		UIProgressBar sb = target as UIProgressBar;
		float val = EditorGUILayout.Slider("Value", sb.value, 0f, 1f);
		float alpha = EditorGUILayout.Slider("Alpha", sb.alpha, 0f, 1f);

		if (sb.value != val ||
			sb.alpha != alpha)
		{
			NGUIEditorTools.RegisterUndo("Progress Bar Change", sb);
			sb.value = val;
			sb.alpha = alpha;
			NGUITools.SetDirty(sb);

			for (int i = 0; i < UIScrollView.list.size; ++i)
			{
				UIScrollView sv = UIScrollView.list[i];

				if (sv.horizontalScrollBar == sb || sv.verticalScrollBar == sb)
				{
					NGUIEditorTools.RegisterUndo("Progress Bar Change", sv);
					sv.UpdatePosition();
				}
			}
		}
	}

	protected virtual void OnDrawExtraFields () { }
	protected virtual void OnDrawAppearance () { }
}
