#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XUtliPoolLib;
using XEditor;
using System.IO;

public class ReplaceEquip : MonoBehaviour
{
    [Serializable]
    public class ReplacePair
    {
        public string srcMeshName;
        public string replaceEquipName;
        public Texture2D replaceTex;
        public bool isShareTex;
    }
    public GameObject fbx;
    public string srcName;
    public string replaceName;
    public string srcDir;
    public string targetDir;
    [SerializeField]
    public ReplacePair[] ReplaceMeshList;
    static List<SkinnedMeshRenderer> smrList = new List<SkinnedMeshRenderer>();
    static List<MeshFilter> mfList = new List<MeshFilter>();
    static List<ReplacePair> rpList = new List<ReplacePair>();
    public void Refresh()
    {
        if (fbx == null)
            return;
        string fbxPath = AssetDatabase.GetAssetPath(fbx).ToLower();

        int profession = AssetModify.GetProfession(fbxPath);
        if (profession < 0)
        {
            Debug.LogError("profession not found:"+ fbxPath);
            return;
        }
        string srcDirName = AssetModify.MakeEquipDir(fbx.name, profession);
        if (string.IsNullOrEmpty(srcDirName))
        {
            Debug.LogError("equip dir not generate");
            return;
        }
        srcDir = "Equipments/" + srcDirName;
        string targetDirName = srcDirName.Replace(srcName, replaceName);
        targetDir = "Equipments/" + targetDirName;

        GameObject fbxIns = GameObject.Instantiate<GameObject>(fbx);
        rpList.Clear();
        smrList.Clear();
        fbxIns.GetComponentsInChildren<SkinnedMeshRenderer>(smrList);
        
        foreach (SkinnedMeshRenderer smr in smrList)
        {
            ReplacePair rp = new ReplacePair();
            rp.srcMeshName = smr.name;
            if(smr.name.ToLower().EndsWith("_weapon"))
            {
                rp.replaceEquipName = AssetModify.MakeWeaponName(profession);
            }
            else
            {
                rp.replaceEquipName = AssetModify.MakeEquipName(smr.sharedMesh.name, profession, targetDirName);
            }            
            if (smr.sharedMaterial != null)
            {
                Texture2D tex = smr.sharedMaterial.mainTexture as Texture2D;
                string texPath = AssetDatabase.GetAssetPath(tex).ToLower();
                string targetTexPath = texPath.Replace(srcName, replaceName);
                rp.replaceTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath);
            }
            rpList.Add(rp);
        }
        smrList.Clear();
        mfList.Clear();
        fbxIns.GetComponentsInChildren<MeshFilter>(mfList);
        foreach (MeshFilter mf in mfList)
        {
            ReplacePair rp = new ReplacePair();
            rp.srcMeshName = mf.name;
            rp.replaceEquipName = AssetModify.MakeWeaponName(profession);
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr != null && mr.sharedMaterial != null)
            {
                Texture2D tex = mr.sharedMaterial.mainTexture as Texture2D;
                string texPath = AssetDatabase.GetAssetPath(tex).ToLower();
                string targetTexPath = texPath.Replace(srcName, replaceName);
                rp.replaceTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath);
            }
            rpList.Add(rp);
        }
        mfList.Clear();
        GameObject.DestroyImmediate(fbxIns);
        ReplaceMeshList = rpList.ToArray();
        rpList.Clear();
    }
    
    [MenuItem(@"Assets/Tool/Equipment/InitReplaceEquip")]
    public static void Init()
    {
        GameObject[] objs = Selection.gameObjects;
        AssetModify.InitCombineConfig();
        foreach (GameObject obj in objs)
        {
            string path = AssetDatabase.GetAssetPath(obj).ToLower();
            ReplaceEquip re = null;
            GameObject replaceEquip = null;
            bool save = false;
            if (path.EndsWith(".prefab"))
            {
                re = obj.GetComponent<ReplaceEquip>();
            }
            else if (path.EndsWith(".fbx"))
            {
                replaceEquip = new GameObject(obj.name);
                re = replaceEquip.AddComponent<ReplaceEquip>();
                re.srcName = "01";
                re.replaceName = "02";
                re.fbx = obj;
                save = true;
            }
            if (re == null)
            {
                if (replaceEquip != null)
                {
                    GameObject.DestroyImmediate(replaceEquip);
                }
                continue;
            }
            re.Refresh();
            if (save)
            {
                int index = path.LastIndexOf(".");
                if (index > 0)
                {
                    path = path.Substring(0, index) + ".prefab";
                    XEditor.AssetModify.CreateOrReplacePrefab(replaceEquip, path);
                }
            }

            if (replaceEquip != null)
            {
                GameObject.DestroyImmediate(replaceEquip);
            }
        }
    }
    public void Process(bool make = true)
    {
        if (ReplaceMeshList != null)
        {
            string saveRootPath = "Assets/Resources/" + targetDir + "/";
            if (make && !Directory.Exists(saveRootPath))
            {
                Directory.CreateDirectory(saveRootPath);
            }
            AssetModify.usedTex.Clear();
            foreach (ReplacePair rp in ReplaceMeshList)
            {
                Texture2D tex = rp.replaceTex;
                if (tex != null)
                {
                    if (rp.replaceEquipName.EndsWith("_weapon"))
                    {
                        if(make)
                        {
                            string srcWeaponPrefab = string.Format("Assets/Resources/{0}/{1}.prefab", srcDir, rp.replaceEquipName);
                            string targetWeaponPrefab = string.Format("Assets/Resources/{0}/{1}.prefab", targetDir, rp.replaceEquipName);
                            GameObject srcWeapon = AssetDatabase.LoadAssetAtPath<GameObject>(srcWeaponPrefab);
                            if (srcWeapon != null)
                            {

                                Renderer render = srcWeapon.GetComponent<Renderer>();
                                if (render != null)
                                {
                                    GameObject targetWeapon = GameObject.Instantiate<GameObject>(srcWeapon);
                                    Renderer targetRender = targetWeapon.GetComponent<Renderer>();
                                    targetWeapon.name = rp.replaceEquipName;
                                    Material desMat = new Material(render.sharedMaterial);
                                    desMat.name = rp.replaceTex.name;
                                    desMat.mainTexture = rp.replaceTex;
                                    AssetModify.DefaultCompressTex(rp.replaceTex, AssetDatabase.GetAssetPath(rp.replaceTex), true, true);
                                    string srcMatPath = AssetDatabase.GetAssetPath(render.sharedMaterial);
                                    int index = srcMatPath.LastIndexOf("/");
                                    string targetMatPath = string.Format("{0}/{1}.mat", srcMatPath.Substring(0, index), desMat.name);
                                    Material newMat = AssetModify.CreateOrReplaceAsset<Material>(desMat, targetMatPath);
                                    targetRender.sharedMaterial = newMat;
                                    AssetModify.CreateOrReplacePrefab(targetWeapon, targetWeaponPrefab);
                                    GameObject.DestroyImmediate(targetWeapon);
                                }
                            }
                            else
                            {
                                Debug.LogError("null prefab:" + srcWeaponPrefab);
                            }
                        }                        
                    }
                    else
                    {
                        string srcMeshPath = srcDir + "/" + rp.replaceEquipName;
                        string replaceMeshPath = targetDir + "/" + rp.replaceEquipName;
                        AssetModify.PartTexInfo partTexInfo = null;
                        if (!AssetModify.usedTex.TryGetValue(tex.GetHashCode(), out partTexInfo))
                        {
                            string srcTexPath = AssetDatabase.GetAssetPath(tex);
                            partTexInfo = new AssetModify.PartTexInfo();
                            if (rp.isShareTex)
                            {                                
                                partTexInfo.texPath = "Equipments/" + targetDir + "/" + rp.replaceEquipName;
                                partTexInfo.isAlpha = File.Exists(partTexInfo.texPath + "_A.png");
                            }
                            else
                            {
                                bool isAlpha = false;
                                if(make)
                                {
                                    string targetTexPath = saveRootPath + rp.replaceEquipName + ".tga";
                                    AssetDatabase.CopyAsset(srcTexPath, targetTexPath);
                                    Texture2D mainTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath);
                                    Texture2D alphaTex = null;
                                    if (File.Exists("Assets/Resources/Equipments/" + targetDir + "/" + rp.replaceEquipName + "_A.png"))
                                    {
                                        alphaTex = AssetModify.ConvertTexRtex(mainTex);
                                    }
                                    AssetModify.DefaultCompressTex(mainTex, targetTexPath, true, true);
                                    isAlpha = alphaTex != null;
                                }
                                else
                                {
                                    isAlpha = File.Exists(replaceMeshPath + "_A.png");
                                }

                                partTexInfo.texPath = replaceMeshPath;
                                partTexInfo.isAlpha = isAlpha;
                                AssetModify.usedTex.Add(tex.GetHashCode(), partTexInfo);
                            }
                        }
                        string prefix = "";
                        int index = rp.replaceEquipName.IndexOf("_");
                        if (index >= 0)
                        {
                            prefix = rp.replaceEquipName.Substring(0, index);
                        }
                        AssetModify.AddPart(srcMeshPath, replaceMeshPath, srcDir.Replace("Equipments/",""), partTexInfo.texPath, tex.width == 1024, partTexInfo.isAlpha, rp.isShareTex, prefix);
                    }
                }
                else
                {
                    Debug.LogError("null tex:" + rp.srcMeshName);
                }
            }
        }
    }
    [MenuItem(@"Assets/Tool/Equipment/MakeReplaceEquip")]
    public static void MakeReplaceEquip()
    {
        AssetModify.InitCombineConfig();
        AssetModify.LoadMeshPartInfo();
        GameObject[] objs = Selection.gameObjects;

        for (int i = 0; i < objs.Length; ++i)
        {
            GameObject obj = objs[i];
            EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "Process GameObject", i, objs.Length), obj.name, (float)i / objs.Length);
            string path = AssetDatabase.GetAssetPath(obj).ToLower();
            ReplaceEquip re = null;
            if (path.EndsWith(".prefab"))
            {
                re = obj.GetComponent<ReplaceEquip>();
            }
            if (re == null)
            {
                continue;
            }
            re.Process();
        }
        AssetModify.SaveEquipInfo();       
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Finish", "All objects processed finish", "OK");
    }
    //public void RefreshConfig()
    //{
    //    if (ReplaceMeshList != null)
    //    {
    //        string saveRootPath = "Assets/Resources/" + targetDir + "/";
    //        AssetModify.usedTex.Clear();
    //        foreach (ReplacePair rp in ReplaceMeshList)
    //        {
    //            Texture2D tex = rp.replaceTex;
    //            if (tex != null)
    //            {
    //                if (!rp.replaceEquipName.EndsWith("_weapon"))
    //                {
    //                    string srcMeshPath = srcDir + "/" + rp.srcMeshName;
    //                    string replaceMeshPath = targetDir + "/" + rp.replaceEquipName;
    //                    AssetModify.PartTexInfo partTexInfo = null;
    //                    if (!AssetModify.usedTex.TryGetValue(tex.GetHashCode(), out partTexInfo))
    //                    {
    //                        string srcTexPath = AssetDatabase.GetAssetPath(tex);
    //                        partTexInfo = new AssetModify.PartTexInfo();
    //                        if (rp.isShareTex)
    //                        {                                
    //                            partTexInfo.texPath = "Equipments/" + srcDir + "/" + rp.replaceEquipName;
    //                            partTexInfo.isAlpha = File.Exists(partTexInfo.texPath + "_A.png");
    //                        }
    //                        else
    //                        {
    //                            partTexInfo.texPath = replaceMeshPath;
    //                            partTexInfo.isAlpha = File.Exists(partTexInfo.texPath + "_A.png");
    //                            AssetModify.usedTex.Add(tex.GetHashCode(), partTexInfo);
    //                        }
    //                    }
    //                    string prefix = "";
    //                    int index = rp.srcMeshName.IndexOf("_");
    //                    if(index>=0)
    //                    {
    //                        prefix = rp.srcMeshName.Substring(0, index);
    //                    }
    //                    AssetModify.AddPart(srcMeshPath, replaceMeshPath, rp.replaceEquipName, partTexInfo.texPath, tex.width == 1024, partTexInfo.isAlpha, rp.isShareTex, prefix);
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogError("null tex:" + fbx.name);
    //            }
    //        }
    //    }
    //}
}

[CanEditMultipleObjects, CustomEditor(typeof(ReplaceEquip))]
public class ReplaceEquipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(100)))
        {
            AssetModify.InitCombineConfig();
            for (int i = 0; i < targets.Length; ++i)
            {
                ReplaceEquip re = targets[i] as ReplaceEquip;
                if (re != null)
                {
                    re.Refresh();
                    GameObject go = GameObject.Instantiate<GameObject>(re.gameObject);
                    PrefabUtility.ReplacePrefab(go, re.gameObject, ReplacePrefabOptions.ReplaceNameBased);
                    GameObject.DestroyImmediate(go);
                }

            }

        }
        if (GUILayout.Button("Make", GUILayout.MaxWidth(100)))
        {           
            if(targets.Length>0)
            {
                AssetModify.InitCombineConfig();
                AssetModify.LoadMeshPartInfo();
                for (int i = 0; i < targets.Length; ++i)
                {
                    ReplaceEquip re = targets[i] as ReplaceEquip;
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "Process GameObject", i, targets.Length), "", (float)i / targets.Length);
                    if (re != null)
                    {
                        re.Process();
                    }
                }
                AssetModify.SaveEquipInfo();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Finish", "All objects processed finish", "OK");
            }
           
        }
       
    }
}
public class EquipPathInfo : IComparable<EquipPathInfo>
{
    public string name = "";
    public string texPath = "";
    public byte type = 0;
    public string newName = "";
    public string newTexPath = "";
    public Texture2D replaceTex = null;
    public int CompareTo(EquipPathInfo other)
    {
        if (other == null)
            return 1;
        return name.CompareTo(other.name);
    }
}

#endif