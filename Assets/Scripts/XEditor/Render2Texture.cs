#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Camera))]
public class Render2Texture : MonoBehaviour
{

    public Camera cameraCache;
    public MeshRenderer mr;

    public Texture2D Render(Texture src, Texture tex1, int width, int height, Vector2 tile, Vector2 offset, string shaderName)
    {
        if (cameraCache != null && mr != null)
        {
            Shader shader = Shader.Find(shaderName);
            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", src);
            if (mat.HasProperty("_Tex1"))
                mat.SetTexture("_Tex1", tex1);
            mat.SetTextureScale("_MainTex", tile);
            mat.SetTextureOffset("_MainTex", offset);
            width = width <= 0 ? src.width : width;
            height = height <= 0 ? src.height : height;
            RenderTexture current = RenderTexture.active;
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            cameraCache.targetTexture = rt;
            mr.sharedMaterial = mat;
            cameraCache.Render();
            Texture2D des = new Texture2D(src.width, src.height);
            Graphics.SetRenderTarget(rt);
            des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            des.Apply();
            GameObject.DestroyImmediate(mat);            
            Graphics.SetRenderTarget(current);
            RenderTexture.ReleaseTemporary(rt);
          
            return des;
        }
        return null;
    }

    public static Texture2D ScaleTexture(Texture src, string despath, int width, int height, Vector2 tile, Vector2 offset, string shaderName)
    {
        Texture2D tex = null;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/EditorRes/Render2Tex.prefab");
        if(prefab!=null)
        {
            GameObject render2Tex = GameObject.Instantiate<GameObject>(prefab);
            Render2Texture r2t = render2Tex.GetComponent<Render2Texture>();
            if (r2t != null)
            {
                tex = r2t.Render(src, null, width, height, tile, offset, shaderName);
                if (tex != null && !string.IsNullOrEmpty(despath))
                {
                    byte[] bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(despath, bytes);
                    AssetDatabase.Refresh();
                }
            }
            GameObject.DestroyImmediate(render2Tex);
        }
        return tex;
    }

    public static Texture2D CompactTexture(Texture tex0, Texture tex1, int width, int height, Vector2 tile, Vector2 offset, string shaderName)
    {
        Texture2D tex = null;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/EditorRes/Render2Tex.prefab");
        if (prefab != null)
        {
            GameObject render2Tex = GameObject.Instantiate<GameObject>(prefab);
            Render2Texture r2t = render2Tex.GetComponent<Render2Texture>();
            if (r2t != null)
            {
                tex = r2t.Render(tex0, tex1, width, height, tile, offset, shaderName);
            }
            GameObject.DestroyImmediate(render2Tex);
        }
        return tex;
    }
}
#endif