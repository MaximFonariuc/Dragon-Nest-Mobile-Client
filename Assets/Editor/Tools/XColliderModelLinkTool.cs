using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using XUtliPoolLib;


public class XColliderModelLinkTool : MonoBehaviour 
{

    [MenuItem(@"XEditor/2.5D Model Collider Linker")]
    static void SkillCreater()
    {
        EditorWindow.GetWindow<XLinkerEditor>(@"Linker Editor");
    }
}

internal class XLinkerEditor : EditorWindow
{
    private GUIStyle _labelstyle = null;
    private GameObject _collider = null;

    private List<Renderer> _renders = new List<Renderer>();

    void OnGUI()
    {
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontSize = 13;
        }

        GUILayout.Label(@"Select collider:", _labelstyle);

        EditorGUI.BeginChangeCheck();
        _collider = EditorGUILayout.ObjectField("Collider: ", _collider, typeof(GameObject), true) as GameObject;
        if (EditorGUI.EndChangeCheck())
        {
            if (_collider == null) _renders.Clear();
            else
            {
                XColliderRenderLinker linker = _collider.GetComponent("XColliderRenderLinker") as XColliderRenderLinker;

                if (linker != null && linker.renders!=null)
                {
                    foreach (Renderer r in linker.renders)
                    {
                        if (!_renders.Contains(r)) _renders.Add(r);
                    }
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label(@"Select models:", _labelstyle);
        for (int i = 0; i < _renders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _renders[i] = EditorGUILayout.ObjectField("Model " + i + ": ", _renders[i], typeof(Renderer), true) as Renderer;
            if (GUILayout.Button("-", GUILayout.MaxWidth(30), GUILayout.MinWidth(30)))
            {
                _renders.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Render"))
        {
            _renders.Add(null);
        }
        if (GUILayout.Button("Confirm") && _renders != null && _collider != null)
        {
            XColliderRenderLinker linker = _collider.GetComponent("XColliderRenderLinker") as XColliderRenderLinker;

            if (linker == null)
            {
                linker = _collider.AddComponent<XColliderRenderLinker>() as XColliderRenderLinker;
            }
            
            if (_renders.Count > 0)
            {
                linker.renders = _renders.ToArray();
            }
            else
                linker.renders = null;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Replce"))
        {
            XEditor.AssetModify.Process45colliders();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create BoxCollider"))
        {
            CreateBoxCollider();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void CreateBoxCollider()
    {
        Object[] selected = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);

        if (selected.Length > 0)
        {
            float min_x = 9999;
            float min_y = 9999;
            float min_z = 9999;

            float max_x = -9999;
            float max_y = -9999;
            float max_z = -9999;

            GameObject go = Resources.Load("Prefabs/empty") as GameObject;
            GameObject c = Object.Instantiate(go) as GameObject;
            c.name = "TransparentCollider";
            c.layer = LayerMask.NameToLayer("45Transparent");
            XColliderRenderLinker linker = c.AddComponent<XColliderRenderLinker>();
            List<Renderer> _CacheRenderer = new List<Renderer>();
            
            foreach (Object o in selected)
            {
                if (!(o is GameObject)) continue;

                GameObject sceneObj = (GameObject)o;

                if (sceneObj.name.Contains("grass")) continue;

                MeshRenderer[] mr = sceneObj.GetComponentsInChildren<MeshRenderer>();

                for(int i = 0; i < mr.Length; i++)
                {
                    Bounds b = mr[i].bounds;

                    min_x = Mathf.Min(min_x, b.min.x);
                    min_y = Mathf.Min(min_y, b.min.y);
                    min_z = Mathf.Min(min_z, b.min.z);

                    max_x = Mathf.Max(max_x, b.max.x);
                    max_y = Mathf.Max(max_y, b.max.y);
                    max_z = Mathf.Max(max_z, b.max.z);

                    _CacheRenderer.Add(mr[i]);
                }
            }

            if (_CacheRenderer.Count > 0)
            {
                GameObject scene = GameObject.Find("Scene");

                Transform t = scene.transform.Find("45");

                GameObject node;
                if (t == null)
                {
                    GameObject gg = Resources.Load("Prefabs/empty") as GameObject;
                    node = Object.Instantiate(gg) as GameObject;
                    node.layer = LayerMask.NameToLayer("45Transparent");
                    node.name = "45";
                    node.transform.parent = scene.transform;
                    node.transform.position = Vector3.zero;
                    node.transform.localScale = Vector3.one;
                    node.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    node = t.gameObject;
                }
                
                c.AddComponent<BoxCollider>();

                c.transform.parent = node.transform;
                c.transform.position = new Vector3((min_x + max_x) / 2, (min_y + max_y) / 2, (min_z + max_z) / 2);
                c.transform.localScale = new Vector3((max_x - min_x), (max_y - min_y), (max_z - min_z));
                c.transform.localRotation = Quaternion.identity;

                linker.renders = _CacheRenderer.ToArray();
            }

            

        }
    }
}
