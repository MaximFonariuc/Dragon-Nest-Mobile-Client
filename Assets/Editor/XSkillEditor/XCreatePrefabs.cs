using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace XEditor
{
    public class XCreatePrefabs : MonoBehaviour
	{
        static readonly string dictionary = "Prefabs";

        [MenuItem("XEditor/Generate Prefab")]
        static public void Prefabs()
        {
            GameObject[] objs = Selection.gameObjects;
            foreach (GameObject go in objs)
            {
                string localPath = XEditorPath.GetPath(dictionary) + go.name + ".prefab";
                if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "The prefab already exists. Do you want to overwrite it?",
                        "Yes",
                        "No"))
                        CreateNew(go, localPath);
                }
                else
                    CreateNew(go, localPath);
            }
        }

        //true means enable the validation, not means the initialized value for validation.
        [MenuItem("XEditor/Generate Prefab", true)]
        static bool ValidatePrefabs()
        {
            return (Selection.activeObject != null);
        }

        static void CreateNew(GameObject obj, string localPath)
        {
            UnityEngine.GameObject prefab = PrefabUtility.CreatePrefab(localPath, obj);
            //PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);

            prefab.AddComponent<CharacterController>();
            prefab.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }
	}
}
