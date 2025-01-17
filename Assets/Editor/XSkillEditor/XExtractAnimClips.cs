using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace XEditor
{
    public class XExtractAnimClips : MonoBehaviour
	{
        public static string assetname = "";

        static string path = "";
        static readonly string dictionary = "Animation";

        static Dictionary<string, int> uniqe = new Dictionary<string, int>();

        [MenuItem("XEditor/Extract Animation Clips/Extract From Clips")]
        static void ExtractAnimationClipsA()
        {
            ExtractFromClips();
        }

        //true means enable the validation, not means the initialized value for validation.
        [MenuItem("XEditor/Extract Animation Clips/Extract From Clips", true)]
        static bool ValidateExtractAnimationClipsA()
        {
            if (Selection.activeObject == null) return false;

            UnityEngine.Object[] imported = Selection.GetFiltered(
                typeof(UnityEngine.AnimationClip),
                SelectionMode.Unfiltered |
                SelectionMode.ExcludePrefab |
                SelectionMode.Deep);

            return (imported.Length > 0);
        }

        [MenuItem("XEditor/Extract Animation Clips/Extract From FBX")]
        static void ExtractAnimationClipsB()
        {
            ExtractFromFBX();
        }

        //true means enable the validation, not means the initialized value for validation.
        [MenuItem("XEditor/Extract Animation Clips/Extract From FBX", true)]
        static bool ValidateExtractAnimationClipsB()
        {
            if (Selection.activeObject == null) return false;

            UnityEngine.Object[] imported = Selection.GetFiltered(
                typeof(UnityEngine.GameObject),
                SelectionMode.Unfiltered);

            return (imported.Length > 0);
        }

        static void ExtractFromFBX()
        {
            path = XEditorPath.GetPath(dictionary);

            UnityEngine.Object[] imported = Selection.GetFiltered(
                typeof(UnityEngine.GameObject),
                SelectionMode.Unfiltered);

            uniqe.Clear();

            foreach (UnityEngine.Object o in imported)
            {
                assetname = GetAssetsName(o);

                if (assetname != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));

                    if (!uniqe.ContainsKey(guid))
                    {
                        UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid));
                        foreach (UnityEngine.Object obj in objs)
                        {
                            DuplicateClips(obj as AnimationClip, assetname);
                        }

                        uniqe.Add(guid, 0);
                    }
                }
            }
        }

        static void ExtractFromClips()
        {
            path = XEditorPath.GetPath(dictionary);
            assetname = GetAssetsName(Selection.activeObject);

            if (assetname != null) BuildPrefixName();
        }

        static string GetAssetsName(UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            int last = path.LastIndexOf('.');
            string subfix = path.Substring(last, path.Length - last).ToLower();

            if (subfix == ".fbx")
            {
                int n = path.LastIndexOf('/') + 1;
                return path.Substring(n, last - n);
            }
            else
            {
                return null;
            }
        }

        public static void Execute(string selectionName)
        {
            UnityEngine.Object[] imported = Selection.GetFiltered(
                typeof(UnityEngine.AnimationClip),
                SelectionMode.Unfiltered |
                SelectionMode.ExcludePrefab |
                SelectionMode.Deep);

            if (imported.Length <= 0) return;

            foreach (UnityEngine.Object o in imported)
            {
                DuplicateClips(o as AnimationClip, selectionName);
            }
        }

        static void DuplicateClips(AnimationClip oClip, string selectionName)
        {
            if (oClip != null && path != "")
            {
                string des = selectionName + "_" + oClip.name + ".anim";
                string copyPath = path + des;

                if (System.IO.File.Exists(copyPath))
                {
                    if (!EditorUtility.DisplayDialog("Are you sure?",
                    "The AnimationClip named: \"" + des + "\" already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                        return;
                }

                CopyClip(oClip, copyPath);
            }
        }

        static void CopyClip(AnimationClip src, string copyPath)
        {
            AnimationClip newClip = new AnimationClip();

            EditorUtility.CopySerialized(src, newClip);

            AssetDatabase.CreateAsset(newClip, copyPath);
            AssetDatabase.Refresh();
        }

        static void BuildPrefixName()
        {
            EditorWindow.GetWindow<XPrefixName>("Prefix");
        }
	}

    public class XPrefixName : EditorWindow
    {
        private string _prefix = "";

        public String Prefix { get { return _prefix; } }

        public void OnGUI()
        {
            GUILayout.Label("Write a prefix for the extracted animations' name.");
            _prefix = EditorGUILayout.TextField("Prefix Name: ", _prefix);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Using Mine"))
            {
                if (_prefix == "") _prefix = "Default";

                XExtractAnimClips.Execute(_prefix);
                Close();
            }

            if (GUILayout.Button("Using \"" + XExtractAnimClips.assetname + "\""))
            {
                XExtractAnimClips.Execute(XExtractAnimClips.assetname);
                Close();
            }
            EditorGUILayout.EndHorizontal();

            this.Repaint();
        }
    }
}