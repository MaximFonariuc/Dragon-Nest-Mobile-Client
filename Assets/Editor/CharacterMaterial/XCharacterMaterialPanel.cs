using UnityEditor;
using UnityEngine;

namespace XEditor
{

    [CustomEditor(typeof(XCharacterMaterial))]
    class XCharacterMaterialPanel : Editor
	{
        private XCharacterMaterial _hoster = null;
        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            if (_hoster == null) 
                _hoster = target as XCharacterMaterial;
            if(_hoster!=null)
            {
                _hoster.OnAttached();
            }
        }

        public override void OnInspectorGUI()
        {
            if(_hoster!=null)
            {
                EditorGUILayout.BeginHorizontal();
                _hoster.mat = EditorGUILayout.ObjectField(_hoster.mat,typeof(Material),false) as Material;
                EditorGUILayout.EndHorizontal();
            }
        }
	}
}
