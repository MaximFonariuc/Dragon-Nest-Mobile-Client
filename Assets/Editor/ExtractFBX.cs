using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections.Generic;

public class ExtractFBX
{
    //const string targetFolder = "Animation";
    //static void CopyClip(string importedPath, string assetname)
    //{
    //    //AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
    //    Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(importedPath);

    //    int index = assetname.IndexOf('_');
    //    int index2 = assetname.Substring(index + 1).IndexOf('_');
    //    string folder = assetname.Substring(0, index + index2 + 1);
    //    string targetPath = "Assets/Resources/" + targetFolder + "/" + folder;

    //    if (Directory.Exists(targetPath) == false)
    //    {
    //        AssetDatabase.CreateFolder("Assets/Resources/" + targetFolder, folder);
    //    }

    //    string staticTargetPath = "Assets/Resources/" + targetFolder + "/" + folder;

    //    foreach (Object o in allObjects)
    //    {
    //        AnimationClip oClip = o as AnimationClip;

    //        if (oClip == null || oClip.name.StartsWith("__preview__Take 001")) continue;

    //        //if(oClip.name != name) continue;
    //        string copyPath = targetPath + "/" + oClip.name + ".anim";
    //        string staticPath = staticTargetPath + "/" + oClip.name + ".anim";
    //        if (File.Exists(staticPath))
    //            copyPath = staticPath;

    //        AnimationClip newClip = new AnimationClip();

    //        EditorUtility.CopySerialized(oClip, newClip);

    //        AssetDatabase.CreateAsset(newClip, copyPath);

    //        //XEditor.AssetModify._ReduceKeyFrame(newClip, "");

    //    }
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //}

    //[MenuItem("Assets/ExtractFBX")]
    //static void ExtractAnimationClipFromFBX()
    //{
    //    Object[] imported = Selection.GetFiltered(typeof(object), SelectionMode.Unfiltered | SelectionMode.Deep);

    //    if (imported.Length > 0)
    //    {
    //        string importedPath = AssetDatabase.GetAssetPath(imported[0]);

    //        if (importedPath != null)
    //        {
    //            if (Directory.Exists("Assets/Resources/" + targetFolder) == false)
    //            {
    //                AssetDatabase.CreateFolder("Assets/Resources", targetFolder);
    //            }
    //            ModelImporter modelImport = AssetImporter.GetAtPath(importedPath) as ModelImporter;
    //            if (modelImport != null)
    //            {
    //                if (modelImport.animationCompression != ModelImporterAnimationCompression.Optimal)
    //                {
    //                    modelImport.animationCompression = ModelImporterAnimationCompression.Optimal;
    //                }
    //                CopyClip(importedPath, imported[0].name);
    //            }





    //        }
    //    }
    //}
}
