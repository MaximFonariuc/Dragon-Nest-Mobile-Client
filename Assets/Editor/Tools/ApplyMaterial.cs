using UnityEngine;
using System.Collections;
using UnityEditor;

public class ApplyMaterial : EditorWindow
{

    public static string[] config =
	{
		".PNG",".JPG",".TGA"
	};

    [MenuItem(@"XEditor/Tools/ApplyMaterial")]
    static void Applay()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        ApplyMaterial window = (ApplyMaterial)EditorWindow.GetWindowWithRect(typeof(ApplyMaterial), wr, true, "widow name");
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("批量关联材质"))
        {
            ApplayMatrials(true);
        }
        if (GUILayout.Button("批量删除关联"))
        {
            ApplayMatrials(false);
        }
    }

    void ApplayMatrials(bool isAdd)
    {
        if (Selection.activeGameObject != null)
        {
            foreach (GameObject g in Selection.gameObjects)
            {
                Renderer[] renders = g.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renders)
                {
                    if (r != null)
                    {
                        foreach (Object o in r.sharedMaterials)
                        {
                            string path = AssetDatabase.GetAssetPath(o);
                            Material m = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;

                            if (isAdd)
                            {
                                if (m.mainTexture == null)
                                {
                                    Texture t = GetTexture(m.name);
                                    if (t != null)
                                    {
                                        m.mainTexture = t;
                                    }
                                    else
                                    {
                                        Debug.Log("材质名:" + o.name + " 材质替换失败，请检查资源");
                                    }
                                }
                            }
                            else
                            {
                                m.mainTexture = null;

                            }
                        }
                    }
                }
            }

            this.ShowNotification(new GUIContent("批量关联材质贴图成功"));
        }
        else
        {
            this.ShowNotification(new GUIContent("没有选择游戏对象"));
        }
    }

    static Texture GetTexture(string name)
    {
        foreach (string suffix in config)
        {
            Texture t = AssetDatabase.LoadAssetAtPath("Assets/XScene/modlelib/DN_modlelib" + name + suffix, typeof(Texture)) as Texture;
            if (t != null)
                return t;
        }
        return null;
    }
}