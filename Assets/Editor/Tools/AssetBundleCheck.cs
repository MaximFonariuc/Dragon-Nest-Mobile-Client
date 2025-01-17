using UnityEngine;
using UnityEditor;
using System.IO;

namespace XEditor
{
    public class AssetBundleCheck
    {
        [MenuItem(@"Assets/VerifyBundleTable")]
        private static void VerifyBundleTable()
        {
            string _file = EditorUtility.OpenFilePanel("Select AssetBundle", "Assets/StreamingAssets/update", "ab");
            if(!string.IsNullOrEmpty(_file))
            {
                _VerifyBundleTable(_file, EditorUserBuildSettings.activeBuildTarget.ToString());
            }
           
        }

        private static void _VerifyBundleTable(string path, string platform)
        {
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes != null)
            {
                AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
                if (ab != null)
                {
                    UnityEngine.Object[] objs = ab.LoadAllAssets(typeof(TextAsset));
                    if (objs != null)
                    {
                        try
                        {
                            foreach (UnityEngine.Object obj in objs)
                            {
                                TextAsset ta = obj as TextAsset;
                                byte[] bundleTableBytes = ta.bytes;
                                if (bundleTableBytes != null)
                                {
                                    TextAsset pcTable = Resources.Load<TextAsset>("Table/" + obj.name);
                                    if (pcTable == null)
                                    {
                                        pcTable = Resources.Load<TextAsset>("Table/EnhanceAttribute/" + obj.name);
                                        if (pcTable == null)
                                        {
                                            pcTable = Resources.Load<TextAsset>("Table/Recharge/" + obj.name);
                                        }
                                    }
                                    if (pcTable != null)
                                    {
                                        byte[] pcTableBytes = pcTable.bytes;

                                        if (pcTableBytes.Length == bundleTableBytes.Length)
                                        {
                                            bool hasError = false;
                                            for (int i = 0; i < pcTableBytes.Length; ++i)
                                            {
                                                if (pcTableBytes[i] != bundleTableBytes[i])
                                                {
                                                    Debug.LogError(string.Format("table not same as pc:{0} index:{1} platform:{2}", obj.name, i, platform));
                                                    hasError = true;
                                                    break;
                                                }
                                            }
                                            if (!hasError)
                                            {
                                                Debug.Log(string.Format("table {0} same as pc!", obj.name));
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError(string.Format("table not same as pc:{0} pc length:{1} {2} length:{3}", obj.name, pcTableBytes.Length, platform, bundleTableBytes.Length));
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning(string.Format("table {0}  platform {1} not loaded error!", obj.name, platform));
                                    }
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(e.Message);
                        }

                        ab.Unload(true);
                    }
                    else
                    {
                        Debug.LogError(string.Format("not TextAsset objs {0} platform {1} error!", path, platform));
                    }
                }
                else
                {
                    Debug.LogError(string.Format("load ab {0} platform {1} error!", path, platform));
                }
            }
            else
            {
                Debug.LogError(string.Format("load {0} platform {1} error!", path, platform));
            }
        }

    }
}
