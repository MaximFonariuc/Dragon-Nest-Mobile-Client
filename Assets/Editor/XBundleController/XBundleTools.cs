using UnityEngine;
using System.Collections;
using XUtliPoolLib;
using System.IO;
using UnityEditor;
using XUpdater;
using XEditor;
using System.Collections.Generic;
using System.Text;

public class XBundleTools : XSingleton<XBundleTools>
{
    public class UpdatedFile
    {
        public string name;
        public string location;
        public string status;

        public bool fetched = false;
    }

    public static string BundleRoot = "Assets/Bundle/Android/";
    public static string ResRoot = "Assets/Resources/";

    public static string BundleManifest = "";

    private XVersionData _manifest = null;
    private XVersionData _current = null;

    private List<UpdatedFile> _update_files_res = new List<UpdatedFile>();
    private List<UpdatedFile> _update_files_ab = new List<UpdatedFile>();
    private List<UpdatedFile> _update_files_sce = new List<UpdatedFile>();
    private List<UpdatedFile> _update_files_fmod = new List<UpdatedFile>();
    private List<UpdatedFile> _update_files_otr = new List<UpdatedFile>();

    private bool _need_rebuild = false;

    public XVersionData Manifest { get { return _manifest; } }

    public List<UpdatedFile> ResUpdateFiles { get { return _update_files_res; } }
    public List<UpdatedFile> ABUpdateFiles { get { return _update_files_ab; } }
    public List<UpdatedFile> SceUpdateFiles { get { return _update_files_sce; } }
    public List<UpdatedFile> FMODUpdateFiles { get { return _update_files_fmod; } }
    public List<UpdatedFile> OtrUpdateFiles { get { return _update_files_otr; } }

    public XVersionData CurrentVersion { get { return _current; } }
    public XVersionData NextVersion { get { return _current.Increment(_need_rebuild); } }

    public bool Rebuild { get { return _need_rebuild; } }

    public string Version { get { return CurrentVersion.ToString(); } }
    public string Version_Next { get { return NextVersion.ToString(); } }

    static public bool InitVersionBytes()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.Android:
                {
                    BundleRoot = "Assets/Bundle/Android/";
                    ResRoot = "Assets/Resources/android-version.bytes";
                } break;
            case UnityEditor.BuildTarget.iOS:
                {
                    BundleRoot = "Assets/Bundle/IOS/";
                    ResRoot = "Assets/Resources/ios-version.bytes";
                } break;
            default:
                {
                    EditorUtility.DisplayDialog("Error", "You current platform is " + EditorUserBuildSettings.activeBuildTarget, "OK");
                    return false;
                };
        }

        return true;
    }

    public bool OnInit()
    {
        if (!InitVersionBytes())
        {
            return false;
        }

        //check branch
        string branch = XGitExtractor.CurrentBranch();
        if (!branch.StartsWith("release") && !branch.StartsWith("stable-"))
        {
            EditorUtility.DisplayDialog("Error", "You current branch is " + branch, "OK");
            return false;
        }

        //if (!XGitExtractor.IsWorkingSpaceCleaning())
        //{
        //    EditorUtility.DisplayDialog("Error", "Git working space is not clean!", "OK");
        //    return false;
        //}

        //check path
        if (!Directory.Exists(BundleRoot))
        {
            XEditorPath.BuildPath(BundleRoot.Substring(7, BundleRoot.Length - 7), "Assets");
        }

        _need_rebuild = false;

        BundleManifest = BundleRoot + "manifest.assetbundle";
        //load version
        if (!LoadVersion()) return false;
        //load manifest
        LoadManifest();
        //fetching the latest update
        if (!FetchNewlyUpdate()) return false;
        //pull branch
        XGitExtractor.Pull(branch);

        return true;
    }

    public List<string> GetAbs()
    {
        List<string> abs = new List<string>();

        foreach (UpdatedFile file in _update_files_ab)
        {
            string ui = file.location + file.name;

            if (abs.Contains(ui))
            {
                EditorUtility.DisplayDialog("Error", "Duplicated ui file: " + ui, "OK");
                return null;
            }
            else
                abs.Add(ui);
        }

        return abs;
    }

    public List<string> GetScenes()
    {
        List<string> scenes = new List<string>();

        foreach (UpdatedFile file in _update_files_sce)
        {
            string scene = file.location + file.name;

            if (scenes.Contains(scene))
            {
                EditorUtility.DisplayDialog("Error", "Duplicated scene meta file: " + scene, "OK");
                return null;
            }
            else
                scenes.Add(scene);
        }

        return scenes;
    }

    public List<string> GetFmods()
    {
        List<string> fmods = new List<string>();

        foreach (UpdatedFile file in _update_files_fmod)
        {
            string fmod = file.location + file.name;

            if (fmods.Contains(fmod))
            {
                EditorUtility.DisplayDialog("Error", "Duplicated fmod file: " + fmod, "OK");
            }
            else
                fmods.Add(fmod);
        }

        return fmods;
    }

    public List<UnityEngine.Object> GetObjects()
    {
        List<UnityEngine.Object> os = new List<Object>();

        foreach (XBundleTools.UpdatedFile m in _update_files_res)
        {
            if (m.name.Contains("-version.bytes"))
            {
                m.fetched = false;
                continue;
            }

            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath("Assets/" + m.location + m.name, typeof(UnityEngine.Object));
            if (o != null)
            {
                m.fetched = true;
                os.Add(o);
            }
            else
                m.fetched = false;
        }

        return os;
    }

    private bool LoadVersion()
    {
        if (!File.Exists(ResRoot))
        {
            //write a brand new with version x.0.0
            File.WriteAllBytes(ResRoot, Encoding.ASCII.GetBytes(XUpdater.XUpdater.Major_Version + ".0.0"));
        }

        string version = ASCIIEncoding.ASCII.GetString(File.ReadAllBytes(ResRoot));
        _current = XVersionData.Convert2Version(version);

        return _current != null;
    }

    public void UpdateVersion(string version)
    {
        File.WriteAllBytes(ResRoot, Encoding.ASCII.GetBytes(version));
        LoadVersion();
    }

    public void LoadManifest()
    {
        if (!File.Exists(BundleManifest))
        {
            //write a brand new manifest with version
            XVersionMgr.BuildEmptyManifest(new XVersionData());
        }

        AssetDatabase.Refresh();
        _manifest = XVersionMgr.GetManifest();
    }

    public bool FetchNewlyUpdate()
    {
        _update_files_res.Clear();
        _update_files_otr.Clear();
        _update_files_ab.Clear();
        _update_files_sce.Clear();
        _update_files_fmod.Clear();

        //newly
        if (_current.IsNewly)
        {
            _need_rebuild = true;
            return true;
        }
        else
        {
            XGitExtractor.Run(Version);
            _need_rebuild = CalculateVersionDiff();

            return true;// ExtractUis();
        }
    }

    /*private bool ExtractUis()
    {
        _update_extract_ui.Sort();

        for (int i = 0; i < _update_extract_ui.Count; i++)
        {
            if (_update_extract_ui[i].EndsWith(".png") || _update_extract_ui[i].EndsWith(".mat"))
            {
                string root = _update_extract_ui[i].Substring(0, _update_extract_ui[i].Length - 4);
                string finder = root + ".prefab";
                int index = _update_extract_ui.BinarySearch(finder);
                
                if(index < 0)
                {
                    //maybe is only a png file.
                    string prefab = Application.dataPath + "/" + root + ".prefab";
                    if (File.Exists(prefab))
                    {
                        EditorUtility.DisplayDialog("Error", prefab + " not changed while png or mat changed!", "OK");
                        return false;
                    }
                }
                else
                {
                    //is a atlas file combined png and perfab
                    _update_extract_ui.RemoveAt(i); i--;
                }
            }
        }

        return true;
    }*/

    private bool CalculateVersionDiff()
    {
        bool notrebuild = true;

        foreach (string m in XGitExtractor.M_files)
        {
            UpdatedFile uf = new UpdatedFile();

            int idx = m.LastIndexOf('/');

            uf.name = m.Substring(idx + 1, m.Length - idx - 1);
            uf.location = m.Remove(m.LastIndexOf('/') + 1);
            uf.status = "M";
            uf.fetched = false;

            if (uf.location.StartsWith("Resources/"))
            {
                //if (uf.location.Contains("Resources/CutScene") || uf.location.Contains("Resources/lua") || uf.location.Contains("Resources/SkillPackage") || uf.location.Contains("Resources/Table") || uf.name == "XMainClient.bytes")
                if (uf.name == "XMainClient.bytes")
                {
                    _update_files_res.Add(uf);
                }
                else if(uf.name == "FMODStudioSettings.asset")
                {
                    _update_files_fmod.Add(uf);
                }
                else
                {
                    _update_files_ab.Add(uf);
                }
            }
            else if (uf.location.StartsWith("XScene/"))
            {
                if(uf.name.EndsWith(".unity"))
                {
                    _update_files_ab.Add(uf);
                }
                else
                {
                    _update_files_otr.Add(uf);
                }
            }
            else if(uf.location.StartsWith("StreamingAssets/"))
            {
                if (uf.name.EndsWith(".bank"))
                {
                    _update_files_fmod.Add(uf);
                }
                else
                {
                    _update_files_otr.Add(uf);
                }
            }
            else _update_files_otr.Add(uf);

            notrebuild = notrebuild && CanUpdated(uf.location);
        }
        foreach (string m in XGitExtractor.A_files)
        {
            UpdatedFile uf = new UpdatedFile();

            int idx = m.LastIndexOf('/');

            uf.name = m.Substring(idx + 1, m.Length - idx - 1);
            uf.location = m.Remove(m.LastIndexOf('/') + 1);
            uf.status = "A";
            uf.fetched = false;

            if (uf.location.StartsWith("Resources/"))
            {
                //if (uf.location.Contains("Resources/CutScene") || uf.location.Contains("Resources/lua") || uf.location.Contains("Resources/SkillPackage") || uf.location.Contains("Resources/Table") || uf.name == "XMainClient.bytes")
                if (uf.name == "XMainClient.bytes")
                {
                    _update_files_res.Add(uf);
                }
                else
                {
                    _update_files_ab.Add(uf);
                }
            }
            else if (uf.location.StartsWith("XScene/"))
            {
                if (uf.name.EndsWith(".unity"))
                {
                    _update_files_ab.Add(uf);
                }
                else
                {
                    _update_files_otr.Add(uf);
                }
            }
            else if (uf.location.StartsWith("StreamingAssets/"))
            {
                if (uf.name.EndsWith(".bank") || uf.name.EndsWith(".cfg"))
                {
                    _update_files_fmod.Add(uf);
                }
                else
                {
                    _update_files_otr.Add(uf);
                }
            }
            else _update_files_otr.Add(uf);

            notrebuild = notrebuild && CanUpdated(uf.location);
        }

        return !notrebuild;
    }

    private bool CanUpdated(string location)
    {
        if (location.StartsWith("Resources/"))
        {
            return !(location.Contains("Controller/") || location.Contains("StaticUI") || location.Contains("StaticAnimation") || location.Contains("Shader"));
        }
        else
        {
            return location.StartsWith("Creatures/") ||
                location.StartsWith("XScene/") ||
                location.StartsWith("Equipment/") ||
                location.Contains("/SkillPackage/") ||
                location.Contains("/Curve/") ||
                location.StartsWith("Bundle/") ||
                location.StartsWith("ABSystem/") ||
                location.StartsWith("AssetBundles/") ||
                location.StartsWith("StreamingAssets/") ||
                location.StartsWith("Table/");
        }
    }
}
