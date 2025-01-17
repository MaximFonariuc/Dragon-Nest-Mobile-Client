using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(UIDummy), true)]
public class UIDummyInspector : UIWidgetInspector
{
    UIDummy mDummy;

    protected override void OnEnable()
    {
        base.OnEnable();
        mDummy = target as UIDummy;
    }
    protected override void DrawCustomProperties()
    {
        GUILayout.BeginHorizontal();
        bool calcRenderQueue = EditorGUILayout.Toggle("RQ", mDummy.calcRenderQueue, GUILayout.Width(100f));
        GUILayout.Label("Calculate RenderQueue");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("RenderQueue:" + mDummy.RenderQueue);
        GUILayout.EndHorizontal();
        if (mDummy.calcRenderQueue != calcRenderQueue)
        {
            mDummy.calcRenderQueue = !mDummy.calcRenderQueue;
            EditorUtility.SetDirty(mDummy);
        }
        base.DrawCustomProperties();
    }
}
