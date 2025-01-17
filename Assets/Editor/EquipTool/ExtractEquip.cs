using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XEditor;
using XUtliPoolLib;

public class ExtractFashion {

    const string targetFolder = "Equipments";

    //[MenuItem("Assets/ExtractEquip")]
    private static void ExtractEquipFromFBX()
    {
        Object[] imported = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        
        //ChangeMaterial();

        //EquipBones _equipBones = new EquipBones();

        //XEquipBoneLibrary.Read();
        
        if (imported.Length > 0)
        {
            if (Directory.Exists("Assets/Resources/" + targetFolder) == false)
            {
                AssetDatabase.CreateFolder("Assets/Resources", targetFolder);
            }

            if (Directory.Exists("Assets/Resources/" + targetFolder +"/weapon") == false)
            {
                AssetDatabase.CreateFolder("Assets/Resources/"+targetFolder, "weapon");
            }

            

            foreach (Object o in imported)
            {
                if (!(o is GameObject)) continue;

                GameObject characterFBX = (GameObject)o;

                if (characterFBX.name.Contains("wing") || characterFBX.name.Contains("tail") || characterFBX.name.Contains("spirit") || characterFBX.name.Contains("fishing"))
                {
                    SetupEntityEquip(AssetDatabase.GetAssetPath(characterFBX), characterFBX);
                    continue;
                }
                
                // 蒙皮网格
                foreach (SkinnedMeshRenderer smr in characterFBX.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    smr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    smr.receiveShadows = false;
                    
                    //EquipBones.RowData data = new EquipBones.RowData();
                    //data.EquipName = smr.name;

                    //foreach (Transform t in smr.bones)
                    //{
                    //    data.Bones.Add(t.name);
                    //}
                    //XEquipBoneLibrary.ChangeData(smr.name, data);
                    //_equipBones.Table.Add(data);

                    GameObject rendererClone = (GameObject)PrefabUtility.InstantiatePrefab(smr.gameObject);

                    if (rendererClone.transform.parent != null)
                    {
                        GameObject rendererParent = rendererClone.transform.parent.gameObject;
                        rendererClone.transform.parent = null;
                        Object.DestroyImmediate(rendererParent);
                    }
                    rendererClone.transform.localPosition = Vector3.zero;

                    rendererClone.layer = LayerMask.NameToLayer("Role");

                    Component.DestroyImmediate(rendererClone.GetComponent<Animator>());
                    string p;

                    if (rendererClone.name.Contains("weapon"))
                    {
                        p = "Assets/Resources/" + targetFolder + "/weapon/" + rendererClone.name + ".prefab";
                    }
                    else
                    {
                        p = "Assets/Resources/" + targetFolder + "/" + rendererClone.name + ".prefab";
                    }


                    Object tempPrefab = PrefabUtility.CreateEmptyPrefab(p);
                    PrefabUtility.ReplacePrefab(rendererClone, tempPrefab);
                    Object.DestroyImmediate(rendererClone);
                }

                // 普通mesh,现在只有武器
                foreach (MeshRenderer mr in characterFBX.GetComponentsInChildren<MeshRenderer>(true))
                {
                    mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                    GameObject rendererClone = (GameObject)PrefabUtility.InstantiatePrefab(mr.gameObject);

                    if (rendererClone.transform.parent != null)
                    {
                        GameObject rendererParent = rendererClone.transform.parent.gameObject;
                        rendererClone.transform.parent = null;
                        Object.DestroyImmediate(rendererParent);
                    }
                    Component.DestroyImmediate(rendererClone.GetComponent<Animator>());
                    rendererClone.transform.localPosition = Vector3.zero;

                    rendererClone.layer = LayerMask.NameToLayer("Role");

                    string p;

                    if (rendererClone.name.Contains("weapon"))
                    {
                        p = "Assets/Resources/" + targetFolder + "/weapon/" + rendererClone.name + ".prefab";
                    }
                    else
                    {
                        p = "Assets/Resources/" + targetFolder + "/" + rendererClone.name + ".prefab";
                    }

                    Object tempPrefab = PrefabUtility.CreateEmptyPrefab(p);
                    PrefabUtility.ReplacePrefab(rendererClone, tempPrefab);
                    Object.DestroyImmediate(rendererClone);
                }
            }

            //XEquipBoneLibrary.WriteToFile();
            
            //using (FileStream writer = new FileStream(path, FileMode.Create))
            //{
            //    StreamWriter sw = new StreamWriter(writer, Encoding.Unicode);

            //    _equipBones.Comment = new List<string>();
            //    _equipBones.Comment.Add("EquipName");
            //    _equipBones.Comment.Add("bones");

            //    _equipBones.WriteFile(sw);

            //    sw.Flush();
            //    sw.Close ();
            //}

            AssetDatabase.Refresh();
        }
        else
        {
            EditorUtility.DisplayDialog("Generate Equip", " Select the FBX folder in the Project pane to process all equip.", "Ok");
        }
    }

    static void ChangeMaterial()
    {
        Object[] materials = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);

        Shader MobileDiffuseShader = Shader.Find("Custom/Common/MobileDiffuse");
        foreach (Material mat in materials)
        {
            if(!mat.name.Contains("wing") && !mat.name.Contains("tail"))
                mat.shader = MobileDiffuseShader;
        }
    }

    static void SetupEntityEquip(string importedPath, GameObject prefab)
    {
        if (prefab.name.Contains("_bandpose"))
        {
            // 把bandpose做成prefab
            GameObject rendererClone = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            rendererClone.layer = LayerMask.NameToLayer("Role");

            for(int i = 0; i < rendererClone.transform.childCount; i++)
            {
                rendererClone.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Role");
            }

            Animator animator = rendererClone.GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("Controller/XMinorAnimator") as RuntimeAnimatorController;

            SkinnedMeshRenderer[] smr = rendererClone.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < smr.Length; i++)
            {
                smr[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                smr[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                smr[i].receiveShadows = false;
            }
            

            Object tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Prefabs/Equipment/" + rendererClone.name + ".prefab");
            PrefabUtility.ReplacePrefab(rendererClone, tempPrefab);
            Object.DestroyImmediate(rendererClone);
        }
        else
        {
            // 提取animation clip
            Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(importedPath);

            string assetname = prefab.name;
            int index = assetname.IndexOf('_');
            int index2 = assetname.Substring(index + 1).IndexOf('_');
            string folder = assetname.Substring(0, index + index2 + 1);
            string targetPath = "Assets/Resources/Animation/" + folder;

            if (Directory.Exists(targetPath) == false)
            {
                AssetDatabase.CreateFolder("Assets/Resources/Animation", folder);
            }

            foreach (Object o in allObjects)
            {
                AnimationClip oClip = o as AnimationClip;

                if (oClip == null) continue;

                //if(oClip.name != name) continue;
                string copyPath = targetPath + "/" + oClip.name + ".anim";


                AnimationClip newClip = new AnimationClip();
                
                EditorUtility.CopySerialized(oClip, newClip);

                newClip.wrapMode = WrapMode.Loop;
                
                //SerializedObject serializedObject = new UnityEditor.SerializedObject(newClip);
                //SerializedProperty p = serializedObject.FindProperty("mLoopTime");
                //p.boolValue = true;

                AssetDatabase.CreateAsset(newClip, copyPath);
                AssetDatabase.Refresh();
            }
        }
    }

}
