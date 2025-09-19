using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

class MsdkProcessBuild
{
    [PostProcessBuild(980)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.Android) {
            return;
        }
        if (!Directory.Exists(pathToBuiltProject)) {
            return;
        }

        string srcAssets = MsdkEnv.Instance.PATH_LIBRARYS_ANDROID + "/assets";
        string[] needDeleteFiles = Directory.GetFiles(srcAssets);
        string path = Path.GetFullPath(pathToBuiltProject);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs) {
            string destAssets = dir + "/assets";
            if (!File.Exists(destAssets + "/msdkconfig.ini")) {
                continue;
            }
            Debug.Log("Export Android project. Delete duplicate msdk resource in " + destAssets);
            foreach (string srcFile in needDeleteFiles) {
                string fileName = Path.GetFileName(srcFile);
                if (string.IsNullOrEmpty(fileName) || fileName.Equals("msdkconfig.ini")
                    || fileName.EndsWith(".meta") || fileName.Equals("msdk_agreement.txt")) {
                    continue;
                }
                string needDeleteFile = Path.Combine(destAssets, fileName);
                if (!File.Exists(needDeleteFile)) {
                    Debug.Log("Can't find " + needDeleteFile + "to delete.");
                    continue;
                }
                try {
                    File.Delete(needDeleteFile);
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
        }
    }

}
