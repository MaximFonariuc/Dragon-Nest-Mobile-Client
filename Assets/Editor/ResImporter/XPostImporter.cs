using UnityEngine;
using UnityEditor;
using XEditor;

public class XPostImporter : AssetPostprocessor
{
    XModelImporterData xdata = null;
    public void OnPreprocessModel()
    {
        if (!SelectPlatformEditor.isBuild)
        {

            ModelImporter modelimporter = (ModelImporter)assetImporter;
            XResModelImportEditor.Sets = XDataIO<XModelImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Model/ResourceImportXML.xml");
            xdata = null;
            if (XResModelImportEditor.Sets != null)
            {
                foreach (XModelImporterData data in XResModelImportEditor.Sets.ModelSet)
                {
                    if (data.model == modelimporter.assetPath)
                    {
                        xdata = data;
                        break;
                    }
                }
            }
            modelimporter.isReadable = false;
            modelimporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            if (xdata != null)
            {
                modelimporter.meshCompression = xdata.compression;
                modelimporter.importNormals = xdata.normal;
                modelimporter.importTangents = xdata.tangent;
            }
            else
            {
                if (modelimporter.meshCompression == ModelImporterMeshCompression.Off)
                {
                    modelimporter.meshCompression = ModelImporterMeshCompression.Medium;
                }
                modelimporter.importNormals = ModelImporterNormals.Import;
                modelimporter.importTangents = ModelImporterTangents.None;
            }

            if (assetPath.IndexOf("Assets/Equipment/Sprite") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Tile") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Wing") >= 0 ||
                 assetPath.IndexOf("Assets/Equipment/Else") >= 0)
            {
                modelimporter.isReadable = false;
            }
            else if (assetPath.IndexOf("Assets/Equipment/") >= 0)
            {
                modelimporter.isReadable = true;
            }
            else if (assetPath.IndexOf("XScene/") >= 0)
            {
                modelimporter.meshCompression = ModelImporterMeshCompression.Off;
                modelimporter.importNormals = ModelImporterNormals.Import;
                modelimporter.importTangents = ModelImporterTangents.CalculateMikk;
            }
        }
    }
    /// <summary>
    /// 对于模型，删除共享材质，保证不会将包括Standard这样的材质打包到游戏中
    /// </summary>
    /// 
    public void OnPostprocessModel(GameObject model)
    {
        //Debug.LogWarning("Post Import Asset:" + assetPath);
#if !UNITY_BUILD
        if (!SelectPlatformEditor.isBuild)
        {
            if (assetPath.IndexOf("Assets/Creatures/") >= 0 ||
                assetPath.IndexOf("Assets/XScene/") >= 0 ||
                assetPath.IndexOf("Assets/Effect/") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Sprite") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Tail") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Wing") >= 0 ||
                assetPath.IndexOf("Assets/Equipment/Else") >= 0)
            {
                ModelImporter modelimporter = (ModelImporter)assetImporter;
                if (xdata != null)
                    AssetModify.PreExportMeshAvatar(xdata.compression, xdata.tangent);
                else
                    AssetModify.PreExportMeshAvatar(ModelImporterMeshCompression.Medium, ModelImporterTangents.None);
                AssetModify.ExportMeshAvatar(modelimporter, assetPath, null);
            }
            xdata = null;
        }
#endif
    }


    public void OnPreprocessTexture()
    {
        if (!SelectPlatformEditor.isBuild)
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
        	int index = assetPath.LastIndexOf("/");
            if (index != 0)
            {
                string path = assetPath.Substring(0, index);
                if (path == "Assets/Resources/atlas")
                {
                    textureImporter.npotScale = TextureImporterNPOTScale.None;
                    textureImporter.mipmapEnabled = false;
                    AssetModify.SetTexImportSetting(textureImporter, "Standalone", 2048, TextureImporterFormat.RGBA32);
                }
                else if (!path.StartsWith("Assets/Resources/atlas/UI") && !path.StartsWith("Assets/Resources/Equipments") && (path.StartsWith("Assets/Resources/") ||
                    path.StartsWith("Assets/Creatures/") || path.StartsWith("XScene")))
                {
                    bool overrideTex = false;
                    TextureImporterPlatformSettings tips = textureImporter.GetPlatformTextureSettings("Android");
                    if (tips.overridden)
                    {
                        tips = textureImporter.GetPlatformTextureSettings("iPhone");
                        overrideTex = tips.overridden;
                    }
                    if (!overrideTex)
                    {
                        Debug.LogError(string.Format("贴图没有设置平台格式，会导致内存暴增,选中贴图ctrl+1进行默认压缩：{0}", assetPath));
                    }
                }
            }
        }
            
        //if (assetPath.StartsWith("Assets/Resources/atlas"))
        //{
        //    TextureImporter textureImporter = assetImporter as TextureImporter;
        //    if (textureImporter != null)
        //    {
        //        textureImporter.npotScale = TextureImporterNPOTScale.None;
        //    }
        //}
        //if (assetPath.IndexOf("atlas/") < 0 && !assetPath.StartsWith("Assets/Resources/Equipments/") && !assetPath.EndsWith(".exr"))
        //{
        //    TextureImporter textureImporter = assetImporter as TextureImporter;
        //    if (textureImporter != null)
        //    {
        //        TextureImporterPlatformSettings tips = textureImporter.GetPlatformTextureSettings("Android");
        //        if (!tips.overridden)
        //        {
        //            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        //            if (tex != null)
        //            {
        //                AssetModify.DefaultCompressTex(tex, assetPath, false, false);
        //            }
        //        }
        //    }
        //}
           
        }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (!SelectPlatformEditor.isBuild)
        {
            AssetModify.PostImportAssets(importedAssets, deletedAssets, false);
        }
    }
}


//public class FileModificationWarning : UnityEditor.AssetModificationProcessor
//{
//    static string[] OnWillSaveAssets(string[] paths)
//    {
//        //for (int i = 0; i < paths.Length; ++i)
//        //{
//        //    string path = paths[i];
//        //    if (path.EndsWith(".prefab"))
//        //    {
//        //        AssetModify.FixPrefab(path);
//        //    }
//        //}
//        return paths;
//    }
//}