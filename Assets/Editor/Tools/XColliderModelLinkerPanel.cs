using UnityEditor;
using UnityEngine;

namespace XEditor
{
    [CustomEditor(typeof(XColliderRenderLinker))]
    class XColliderModelLinkerPanel : Editor
	{
        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();

        //    if (GUILayout.Button("Hello"))
        //    {
        //        EditorUtility.SetDirty(target);
        //    }
        //}
	}
}
