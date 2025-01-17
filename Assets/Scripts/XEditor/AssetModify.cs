#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XUtliPoolLib;
using System;
using System.Text;
using System.Reflection;
using UnityEditor.SceneManagement;
using System.Xml.Serialization;

namespace XEditor
{
    public enum ETextureCompress
    {
        Compress,
        TrueColor,
        RGB16
    }
    public enum ETextureSize
    {
        Original,
        Half,
        Quarter,
        X32,
        X64,
        X128,
        X256,
        X512,
    }


    public class AssetModify : EditorWindow
    {
        public delegate void EnumAssetPreprocessCallback(string path);

        public delegate bool EnumAssetImportCallback<T, I>(T obj, I assetImporter, string path)
            where T : UnityEngine.Object where I : UnityEditor.AssetImporter;

        public delegate void EnumAssetCallback<T>(T obj, string path)
            where T : UnityEngine.Object;

        public class ObjectInfo
        {
            public UnityEngine.Object obj = null;
            public string path = "";
        }
        public interface IAssetLoadCallback
        {
            bool verbose { get; }
            List<ObjectInfo> GetObjects(string dir);

            void PreProcess(string path);
            bool Process(UnityEngine.Object asset, string path);
            void PostProcess(string path);
        }
        public class BaseAssetLoadCallback<T> where T : UnityEngine.Object
        {
            public bool is_verbose = true;
            public string extFilter = "";
            public string extFilter1 = "";
            protected List<ObjectInfo> m_Objects = new List<ObjectInfo>();
            private static string assetsRoot = "Assets/";
            public BaseAssetLoadCallback(string ext)
            {
                extFilter = ext;
            }
            public BaseAssetLoadCallback(string ext, string ext1)
            {
                extFilter = ext;
                extFilter1 = ext1;
            }

            public bool verbose { get { return is_verbose; } }
            private void GetObjectsInfolder(FileInfo[] files)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    FileInfo file = files[i];
                    string fileName = file.FullName.Replace("\\", "/");
                    int index = fileName.IndexOf(assetsRoot);
                    fileName = fileName.Substring(index);
                    ObjectInfo oi = new ObjectInfo();
                    oi.path = fileName;
                    oi.obj = AssetDatabase.LoadAssetAtPath<T>(fileName);
                    m_Objects.Add(oi);
                }

            }
            private void GetObjectsInfolder(string path)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles(extFilter, SearchOption.AllDirectories);
                GetObjectsInfolder(files);
                if (!string.IsNullOrEmpty(extFilter1))
                {
                    files = di.GetFiles(extFilter1, SearchOption.AllDirectories);
                    GetObjectsInfolder(files);
                }

            }

            private void GetObjectsInfolder(UnityEditor.DefaultAsset folder)
            {
                string path = AssetDatabase.GetAssetPath(folder);
                GetObjectsInfolder(path);
            }

            public List<ObjectInfo> GetObjects(string dir)
            {
                m_Objects.Clear();
                if(string.IsNullOrEmpty(dir))
                {
                    UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
                    for (int i = 0; i < objs.Length; ++i)
                    {
                        UnityEngine.Object obj = objs[i];
                        if (obj is UnityEditor.DefaultAsset)
                        {
                            GetObjectsInfolder(obj as UnityEditor.DefaultAsset);
                        }
                        else
                        {
                            if (obj is T)
                            {
                                string path = AssetDatabase.GetAssetPath(obj);
                                ObjectInfo oi = new ObjectInfo();
                                oi.obj = obj;
                                oi.path = path;
                                m_Objects.Add(oi);
                            }
                        }
                    }
                }
                else
                {
                    GetObjectsInfolder(dir);
                }
                return m_Objects;
            }
        }

        public class AssetLoadCallback<T, I> : BaseAssetLoadCallback<T>, IAssetLoadCallback
            where T : UnityEngine.Object where I : UnityEditor.AssetImporter
        {
            public EnumAssetPreprocessCallback preprocess = null;
            public EnumAssetImportCallback<T, I> cb = null;
            
            public AssetLoadCallback(string ext)
                :base(ext)
            {
            }
            public AssetLoadCallback(string ext, string ext1)
                 : base(ext, ext1)
            {
            }

            public virtual void PreProcess(string path)
            {
                if (preprocess != null)
                {
                    preprocess(path);
                }
            }
            public virtual bool Process(UnityEngine.Object asset, string path)
            {
                T obj = asset as T;
                if (cb != null && obj != null)
                {
                    I assetImporter = AssetImporter.GetAtPath(path) as I;
                    return cb(obj, assetImporter, path);
                }
                return false;
            }

            public virtual void PostProcess(string path)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        public class AssetLoadCallback<T> : BaseAssetLoadCallback<T>, IAssetLoadCallback where T : UnityEngine.Object
        {
            public EnumAssetCallback<T> cb = null;
            public AssetLoadCallback(string ext)
                : base(ext)
            {
            }
            public AssetLoadCallback(string ext, string ext1)
                 : base(ext, ext1)
            {
            }
            public virtual void PreProcess(string path)
            {

            }
            public virtual bool Process(UnityEngine.Object asset, string path)
            {
                T obj = asset as T;
                if (cb != null && obj != null)
                {
                    cb(obj, path);
                }
                return false;
            }

            public virtual void PostProcess(string path)
            {
            }
        }

        public delegate bool EnumFbxCallback<GameObject, ModelImporter>(GameObject fbx, ModelImporter modelImporter, string path);
        public delegate bool EnumTex2DCallback<Texture2D, TextureImporter>(Texture2D tex, TextureImporter textureImporter, string path);

        private static AssetLoadCallback<GameObject, ModelImporter> enumFbx = new AssetLoadCallback<GameObject, ModelImporter>("*.fbx");
        public static AssetLoadCallback<Texture2D, TextureImporter> enumTex2D = new AssetLoadCallback<Texture2D, TextureImporter>("*.png", "*.tga");

        public delegate void EnumPrefabCallback<GameObject>(GameObject prefab, string path);
        public delegate void EnumTxtCallback<TextAsset>(TextAsset txt, string path);
        public delegate void EnumMaterialCallback<Material>(Material mat, string path);
        public delegate void EnumMeshCallback<Mesh>(Mesh mesh, string path);
        public delegate void EnumAnimationCallback<AnimationClip>(AnimationClip animClip, string path);
        public delegate void EnumBytesCallback<TextAsset>(TextAsset bytes, string path);

        private static AssetLoadCallback<GameObject> enumPrefab = new AssetLoadCallback<GameObject>("*.prefab");
        private static AssetLoadCallback<TextAsset> enumTxt = new AssetLoadCallback<TextAsset>("*.bytes", "*.txt");
        private static AssetLoadCallback<Material> enumMat = new AssetLoadCallback<Material>("*.mat");
        private static AssetLoadCallback<Mesh> enumMesh = new AssetLoadCallback<Mesh>("*.asset");
        private static AssetLoadCallback<AnimationClip> enumAnimationClip = new AssetLoadCallback<AnimationClip>("*.anim");

        public static T CreateOrReplaceAsset<T>(T asset, string path) where T : UnityEngine.Object
        {
            T existingAsset = null;
            if (asset is Texture2D)
            {
                Texture2D tex = asset as Texture2D;
                byte[] png = tex.EncodeToPNG();
                path = path.ToLower();
                if (path.EndsWith(".tga"))
                {
                    path = path.Replace(".tga", ".png");
                }
                File.WriteAllBytes(path, png);
                existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            else
            {
                existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (existingAsset == null)
                {
                    if (asset is Texture2D)
                    {
                        Texture2D tex = asset as Texture2D;
                        byte[] png = tex.EncodeToPNG();
                        path = path.ToLower();
                        if (path.EndsWith(".tga"))
                        {
                            path = path.Replace(".tga", ".png");
                        }
                        File.WriteAllBytes(path, png);
                        existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(asset, path);
                        AssetDatabase.SaveAssets();
                        existingAsset = asset;
                    }

                }
                else
                {
                    EditorUtility.CopySerializedIfDifferent(asset, existingAsset);
                }

            }           

            return existingAsset;
        }

        public static void CreateOrReplacePrefab(GameObject prefab, string path)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (go == null)
            {
                PrefabUtility.CreatePrefab(path, prefab, ReplacePrefabOptions.ReplaceNameBased);
            }
            else
            {
                PrefabUtility.ReplacePrefab(prefab, go);
            }
        }
        public static void ProcessRender(Renderer render, bool on = false)
        {
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.shadowCastingMode = on ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = on;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            render.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }
        public static void ProcessSkinMesh(SkinnedMeshRenderer smr)
        {
            ProcessRender(smr);
            smr.skinnedMotionVectors = false;
            smr.updateWhenOffscreen = false;
        }

        public static int ReplaceAvatar(Avatar avatar, out Avatar newAvatar)
        {
            newAvatar = avatar;
            string path = AssetDatabase.GetAssetPath(avatar);
            path = path.ToLower();
            if (path.EndsWith(".fbx"))
            {
                int index = path.LastIndexOf("/");
                if (index >= 0)
                {
                    string dir = path.Substring(0, index);
                    string name = path.Substring(index + 1);
                    index = name.LastIndexOf(".");
                    name = name.Substring(0, index);
                    string avatarPath = string.Format("{0}/{1}_avatar.asset", dir, name);
                    if (avatarPath != path)
                    {
                        newAvatar = AssetDatabase.LoadAssetAtPath<Avatar>(avatarPath);
                        if (newAvatar == null || avatar == newAvatar)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
            return 0;
        }
        public static bool CheckRender(Renderer render)
        {
            return render.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off &&
            render.receiveShadows == false &&
            render.reflectionProbeUsage == UnityEngine.Rendering.ReflectionProbeUsage.Off &&
            render.motionVectorGenerationMode == MotionVectorGenerationMode.ForceNoMotion &&
            render.lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.Off;
        }
        public static bool CheckSkinMesh(SkinnedMeshRenderer smr)
        {
            return CheckRender(smr) &&
            smr.skinnedMotionVectors == false &&
            smr.updateWhenOffscreen == false;
        }

        public static void GetGameObjectComponent<T>(GameObject go, List<T> lst) where T : Component
        {
            T com = go.GetComponent<T>();
            if (com != null)
            {
                lst.Add(com);
            }
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                GetGameObjectComponent<T>(child.gameObject, lst);
            }
        }

        public static void EnumAsset<T>(IAssetLoadCallback cb, string title, string dir = "") where T : UnityEngine.Object
        {
            if (cb != null)
            {
                List<ObjectInfo> objInfoLst = cb.GetObjects(dir);
                for (int i = 0; i < objInfoLst.Count; ++i)
                {
                    ObjectInfo oi = objInfoLst[i];
                    T asset = oi.obj as T;
                    if (asset != null)
                    {
                        cb.PreProcess(oi.path);
                        if (cb.Process(asset, oi.path))
                        {
                            cb.PostProcess(oi.path);
                        }
                    }
                    if (cb.verbose)
                        EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, objInfoLst.Count), oi.path, (float)i / objInfoLst.Count);
                }
            }

            AssetDatabase.Refresh();
            if (cb.verbose)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Finish", "All assets processed finish", "OK");
            }
        }

        #region texture
        private static int scaleSize = 2;
        private static TextureImporterPlatformSettings tips = null;
        public static int GetSize(int srcSize, ETextureSize size)
        {
            switch (size)
            {
                case ETextureSize.Original:
                    return srcSize;
                case ETextureSize.Half:
                    return srcSize / 2;
                case ETextureSize.Quarter:
                    return srcSize / 4;
                case ETextureSize.X32:
                    return 32;
                case ETextureSize.X64:
                    return 64;
                case ETextureSize.X128:
                    return 128;
                case ETextureSize.X256:
                    return 256;
                case ETextureSize.X512:
                    return 512;
            }
            return srcSize;
        }
        public static void MakeTexReadable(Texture2D tex, TextureImporter texImporter, bool readable)
        {
            texImporter.isReadable = readable;
            string path = AssetDatabase.GetAssetPath(tex);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static bool HasAlpha(Texture2D tex, TextureImporter texImporter, bool forceConvert = false)
        {
            if (forceConvert)
            {
                texImporter.isReadable = true;
                SetTexImportSetting(texImporter, "", 1024, TextureImporterFormat.RGBA32);
                string path = AssetDatabase.GetAssetPath(tex);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            }
            if (texImporter.isReadable)
            {
                Color[] colors = tex.GetPixels();
                for (int j = 0; j < colors.Length; j++)
                    if (colors[j].a < 1f)
                    {
                        return true;
                    }
                return false;
            }
            return texImporter.DoesSourceTextureHaveAlpha();
        }
        public static void GetTexFormat(bool isAtlas, string userData, out ETextureCompress format, out ETextureSize size)
        {
            format = ETextureCompress.Compress;
            size = isAtlas ? ETextureSize.Original : ETextureSize.Half;
            if (!string.IsNullOrEmpty(userData))
            {
                string[] str = userData.Split(' ');
                if (str.Length > 0)
                {
                    try
                    {
                        format = (ETextureCompress)int.Parse(str[0]);
                    }
                    catch (Exception)
                    {

                    }
                }
                if (str.Length > 1)
                {
                    try
                    {
                        size = (ETextureSize)int.Parse(str[1]);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        public static string GetCurrentPlatformName()
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "iPhone";
#else
            return "Standalone";
#endif
        }
        public static void SetTexImportSetting(TextureImporter texImporter, string platform, int size, TextureImporterFormat format, bool overrideOpt = true)
        {
            if (tips == null)
            {
                tips = new TextureImporterPlatformSettings();
            }
            if (string.IsNullOrEmpty(platform))
            {
                tips.name = GetCurrentPlatformName();
            }
            else
            {
                tips.name = platform;
            }
            
            tips.overridden = overrideOpt;
            tips.maxTextureSize = size;
            tips.format = format;
            texImporter.SetPlatformTextureSettings(tips);
        }

        [MenuItem(@"Assets/Tool/Texture/Compress", false, 0)]
        private static void Compress()
        {
            Rect wr = new Rect(0, 0, 300, 400);
            TextureCommonCompress window = (TextureCommonCompress)EditorWindow.GetWindowWithRect(typeof(TextureCommonCompress), wr, true, "压缩贴图");
            window.Show();
        }

        public static Texture2D MakeAlphaRTex(string alphaTexPath, int size, Texture2D src)
        {
            Texture2D alphaTex = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false);
            for (int y = 0, ymax = src.height; y < ymax; ++y)
            {
                for (int x = 0, xmax = src.width; x < xmax; ++x)
                {
                    Color c0 = src.GetPixel(x, y);
                    Color a = new Color(c0.a, 1, 1, 1);
                    alphaTex.SetPixel(x, y, a);
                }
            }

            byte[] bytes = alphaTex.EncodeToPNG();

            File.WriteAllBytes(alphaTexPath, bytes);
            AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);

            TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaTexPath) as TextureImporter;
            if (alphaTextureImporter != null)
            {
                int alphaSize = size;
                alphaTextureImporter.textureType = TextureImporterType.Default;
                alphaTextureImporter.anisoLevel = 0;
                alphaTextureImporter.mipmapEnabled = false;
                alphaTextureImporter.isReadable = false;
                alphaTextureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                SetTexImportSetting(alphaTextureImporter, BuildTarget.Android.ToString(), alphaSize, TextureImporterFormat.ETC_RGB4);
                SetTexImportSetting(alphaTextureImporter, "iPhone", alphaSize, TextureImporterFormat.PVRTC_RGB4);
                AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
            }
            return AssetDatabase.LoadAssetAtPath(alphaTexPath, typeof(Texture2D)) as Texture2D;
        }

        public static Texture2D ConvertTexRtex(Texture2D src)
        {
            string texPath = AssetDatabase.GetAssetPath(src);
            int index = texPath.LastIndexOf('.');
            string alphaTexPath = texPath.Substring(0, index) + "_A.png";
            int size = src.width > src.height ? src.width : src.height;
            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;

            texImporter.textureType = TextureImporterType.Default;
            texImporter.anisoLevel = 0;
            texImporter.mipmapEnabled = src.width > 256 && src.height > 256;
            texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            texImporter.wrapMode = TextureWrapMode.Repeat;
            texImporter.filterMode = FilterMode.Bilinear;
            SetTexImportSetting(texImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA32);
            SetTexImportSetting(texImporter, "iPhone", size, TextureImporterFormat.RGBA32);
            SetTexImportSetting(texImporter, "Standalone", size, TextureImporterFormat.RGBA32);
            texImporter.isReadable = true;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
            Texture2D alphaTex = null;
            if (HasAlpha(src, texImporter))
            {
                alphaTex = MakeAlphaRTex(alphaTexPath, size / scaleSize, src);
            }
            SetTexImportSetting(texImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
            SetTexImportSetting(texImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
            if (alphaTex != null)
            {
                SetTexImportSetting(texImporter, "Standalone", size, TextureImporterFormat.DXT5);
            }
            else
            {
                SetTexImportSetting(texImporter, "Standalone", size, TextureImporterFormat.DXT1);
            }
            texImporter.isReadable = false;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
            return alphaTex;
        }

        public static void DefaultCompressTex(Texture2D src, string path, bool disableMipmap, bool forceSquare)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            isSeperateAlpha = !path.StartsWith("Assets/Effect");
            isForceSquare = forceSquare;
            isDisableMipmap = disableMipmap;
            _DefaultCompress(src, textureImporter, path);
            isDisableMipmap = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        public static void EnableMipmap(Texture2D src, string path, bool enable)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.mipmapEnabled)
            {
                textureImporter.mipmapEnabled = enable;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        static bool isSeperateAlpha = true;
        static bool isDisableMipmap = false;
        static bool isForceSquare = false;
        public static bool _DefaultCompress(Texture2D tex, TextureImporter textureImporter, string path)
        {
            if (path.IndexOf("_A.") >= 0 || path.IndexOf("_NRM.") >= 0)
                return false;
            int size = tex.width > tex.height ? tex.width : tex.height;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.anisoLevel = 0;
            textureImporter.mipmapEnabled = isDisableMipmap ? false : tex.height > 256 && tex.height > 256;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.filterMode = FilterMode.Bilinear;
            SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA32);
            int iosSize = size;
            if (isForceSquare)
            {
                iosSize = tex.width > tex.height ? tex.height : tex.width;
            }
            SetTexImportSetting(textureImporter, "iPhone", iosSize, TextureImporterFormat.RGBA32);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            bool hasAlpha = false;
            if (HasAlpha(tex, textureImporter))
            {
                hasAlpha = true;
                if (isSeperateAlpha)
                {
                    int extIndex = path.LastIndexOf(".");
                    if (extIndex >= 0)
                    {
                        string alphaTexPath = path.Substring(0, extIndex) + "_A.png";
                        MakeAlphaRTex(alphaTexPath, size / 2, tex);
                    }
                }

            }
            if (isSeperateAlpha || !hasAlpha)
            {
                SetTexImportSetting(textureImporter, "Standalone", size, TextureImporterFormat.DXT1);
                SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                if (tex.width / tex.height >= 4 || tex.height / tex.width >= 4)
                {
                    SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.RGB16);
                }
                else
                {
                    SetTexImportSetting(textureImporter, "iPhone", iosSize, TextureImporterFormat.PVRTC_RGB4);
                }
               
            }
            else
            {
                SetTexImportSetting(textureImporter, "Standalone", size, TextureImporterFormat.DXT5);
                SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA16);
                if (tex.width / tex.height >= 4 || tex.height / tex.width >= 4)
                {
                    SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.RGB16);
                }
                else
                {
                    SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGBA4);
                }
                
            }
            textureImporter.isReadable = false;
            return true;
        }

        public static void DefaultCompress(Texture2D tex, TextureImporter textureImporter, string path, bool mipmap)
        {
            if (path.IndexOf("_A.") >= 0 || path.IndexOf("_NRM.") >= 0)
                return;
            int size = tex.width > tex.height ? tex.width : tex.height;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.anisoLevel = 0;
            textureImporter.mipmapEnabled = tex.height > 256 && tex.height > 256 && mipmap;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.filterMode = FilterMode.Bilinear;
            SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA32);
            SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.RGBA32);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            bool hasAlpha = false;
            if (HasAlpha(tex, textureImporter))
            {
                hasAlpha = true;
                if (isSeperateAlpha)
                {
                    int extIndex = path.LastIndexOf(".");
                    if (extIndex >= 0)
                    {
                        string alphaTexPath = path.Substring(0, extIndex) + "_A.png";
                        MakeAlphaRTex(alphaTexPath, size / 2, tex);
                    }
                }

            }
            if (isSeperateAlpha || !hasAlpha)
            {
                SetTexImportSetting(textureImporter, "Standalone", size, TextureImporterFormat.DXT1);
                SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
            }
            else
            {
                SetTexImportSetting(textureImporter, "Standalone", size, TextureImporterFormat.DXT5);
                SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA16);
                SetTexImportSetting(textureImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGBA4);
            }
            textureImporter.isReadable = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        [MenuItem(@"Assets/Tool/Texture/DefaultCompress %1", false, 0)]
        private static void DefaultCompress()
        {
            isSeperateAlpha = true;
            enumTex2D.cb = _DefaultCompress;
            EnumAsset<Texture2D>(enumTex2D, "DefaultCompress");
        }
        [MenuItem(@"Assets/Tool/Texture/DefaultCompressNoSeperate %5", false, 0)]
        private static void DefaultCompressNoSeperate()
        {
            isSeperateAlpha = false;
            enumTex2D.cb = _DefaultCompress;
            EnumAsset<Texture2D>(enumTex2D, "DefaultCompressNoSeperate");
        }
        [MenuItem(@"Assets/Tool/Texture/TextureCombine", false, 0)]
        private static void CombineTex()
        {
            Rect wr = new Rect(0, 0, 800, 800);
            TextureCombine window = (TextureCombine)EditorWindow.GetWindowWithRect(typeof(TextureCombine), wr, true, "合并贴图");
            window.Show();
        }

        private static bool _SplitTex(Texture2D tex, TextureImporter textureImporter, string path)
        {
            TextureCombine.SplitTexture(tex, textureImporter, true);
            return false;
        }

        [MenuItem(@"Assets/Tool/Texture/SplitTex", false, 0)]
        public static void SplitTex()
        {
            enumTex2D.cb = _SplitTex;
            EnumAsset<Texture2D>(enumTex2D, "SplitTex");
        }

        [MenuItem(@"Assets/Tool/Texture/ShadowTex", false, 0)]
        public static void ShadowTex()
        {
            int width = 32;
            int height = 32;
            Color[] edgeColor = new Color[]
            {   Color.white,
                new Color(0.8f, 0.8f, 0.8f, 0.8f),
                new Color(0.6f, 0.6f, 0.6f, 0.6f),
                new Color(0.4f, 0.4f, 0.4f, 0.4f),
                new Color(0.2f, 0.2f, 0.2f, 0.2f)
            };
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    bool inColorRange = false;
                    for (int i = 0; i < edgeColor.Length; ++i)
                    {
                        if(x == i || x == width - 1 - i || y == i || y == height - 1 - i)
                        {
                            tex.SetPixel(x, y, edgeColor[i]);
                            inColorRange = true;
                            break;
                        }
                    }
                    if(!inColorRange)
                    {
                        tex.SetPixel(x, y, Color.black);
                    }
                }
            }

            CreateOrReplaceAsset<Texture2D>(tex, "Assets/Resources/Shader/Shadow/ShadowMask.png");
            AssetDatabase.Refresh();
        }

        private static bool _Find2ValueAlphaTex(Texture2D tex, TextureImporter textureImporter, string path)
        {
            textureImporter.isReadable = true;
            TextureImporterPlatformSettings tips = textureImporter.GetPlatformTextureSettings(GetCurrentPlatformName());
            TextureImporterFormat format = tips.format;
            SetTexImportSetting(textureImporter, "", tips.maxTextureSize, TextureImporterFormat.RGBA32);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            if (textureImporter.isReadable)
            {
                bool not2Value = false;
                Color[] colors = tex.GetPixels();
                for (int j = 0; j < colors.Length; j++)
                {
                    float a = colors[j].a;
                    if (a < 0.99f && a > 0.09)
                    {
                        not2Value = true;
                        break;
                    }
                }
                if (!not2Value)
                {
                    Debug.LogWarning("2 value tex:" + path);
                }
            }
            textureImporter.isReadable = false;
            SetTexImportSetting(textureImporter, "", tips.maxTextureSize, format);
            return true;
        }

        [MenuItem(@"Assets/Tool/Texture/Find2ValueAlphaTex", false, 0)]
        public static void Find2ValueAlphaTex()
        {
#if UNITY_ANDROID
            enumTex2D.cb = _Find2ValueAlphaTex;
            EnumAsset<Texture2D>(enumTex2D, "Find2ValueAlphaTex");

#else
            Debug.LogError("Only supported in android platform");
#endif
        }
        #endregion texture
        #region UI

        private static bool onlyAtlas = false;

        public struct ConvertInfo
        {
            public bool mipmap;
            public bool isAlpha;
            public Texture2D srcTex;
            public TextureImporter textureImporter;
            public string path;
            public string name;
            public string resourcePath;
            public string alphaResourcePath;
            public string alphaTexPath;
            public ETextureCompress srcFormat;
            public ETextureSize alphaSize;
            public bool isAtlas;
        }

        //public struct TexProcessInfo
        //{
        //    public string name;
        //    public string path;
        //    public TextureImporter textureImporter;
        //    public bool halfSize;
        //}


        private static bool GetTexPath(ref ConvertInfo ci)
        {
            ci.isAlpha = ci.path.IndexOf("_A.png") >= 0;
            if (ci.isAlpha)
                return false;
            ci.name = ci.path;
            ci.alphaResourcePath = ci.path;
            ci.alphaTexPath = ci.path;
            int extIndex = ci.name.LastIndexOf(".");
            if (extIndex >= 0)
            {
                ci.name = ci.name.Substring(0, extIndex);
                string relativePath = "Assets/Resources/";
                ci.name = ci.name.Substring(relativePath.Length);
                ci.resourcePath = ci.name;

                int nameIndex = ci.name.LastIndexOf("/");
                if (nameIndex >= 0)
                {
                    ci.name = ci.name.Substring(nameIndex + 1);
                }
                if (!ci.isAlpha)
                {
                    if (ci.path.StartsWith("Assets/Resources/atlas/UI"))
                    {
                        ci.alphaResourcePath = "atlas/UI/Alpha/" + ci.name + "_A";
                        ci.alphaTexPath = "Assets/Resources/atlas/UI/Alpha/" + ci.name + "_A.png";
                    }
                    else
                    {
                        ci.alphaResourcePath = ci.resourcePath + "_A";
                        ci.alphaTexPath = ci.path.Replace(".png", "_A.png");
                    }
                }
                return true;
            }
            return false;
        }

        private static void _ProcessTexture(ref ConvertInfo ci, bool import, bool isUI = false)
        {
            ci.textureImporter.textureType = TextureImporterType.Default;
            ci.textureImporter.anisoLevel = 0;
            ci.textureImporter.mipmapEnabled = ci.mipmap;
            ci.textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            ci.textureImporter.wrapMode = TextureWrapMode.Clamp;
            ci.textureImporter.filterMode = FilterMode.Bilinear;
            ci.textureImporter.isReadable = false;
            int srcSize = 1024;
            if (ci.srcTex != null)
            {
                srcSize = ci.srcTex.width > ci.srcTex.height ? ci.srcTex.width : ci.srcTex.height;
            }
            //XUITextureImporterData data = ci.dataSet.Find(ci.name);
            //if (data != null)
            {
                if (ci.isAlpha)
                {
                    int size = GetSize(srcSize, ci.alphaSize);
                    SetTexImportSetting(ci.textureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.Alpha8);
                    SetTexImportSetting(ci.textureImporter, "iPhone", size, TextureImporterFormat.Alpha8);
                    SetTexImportSetting(ci.textureImporter, "Standalone", size, TextureImporterFormat.Alpha8);
                }
                else
                {
                    if (ci.srcFormat == ETextureCompress.TrueColor)
                    {
                        SetTexImportSetting(ci.textureImporter, BuildTarget.Android.ToString(), srcSize, TextureImporterFormat.RGB24);
                        SetTexImportSetting(ci.textureImporter, "iPhone", srcSize, TextureImporterFormat.RGB24);
                        //SetTexImportSetting(ci.textureImporter, BuildTarget.StandaloneWindows.ToString(), srcSize, TextureImporterFormat.RGB24);
                    }
                    else if (ci.srcFormat == ETextureCompress.RGB16)
                    {
                        SetTexImportSetting(ci.textureImporter, BuildTarget.Android.ToString(), srcSize, TextureImporterFormat.RGB16);
                        SetTexImportSetting(ci.textureImporter, "iPhone", srcSize, TextureImporterFormat.RGB16);
                        //SetTexImportSetting(ci.textureImporter, BuildTarget.StandaloneWindows.ToString(), srcSize, TextureImporterFormat.RGB16);
                    }
                    else
                    {
                        SetTexImportSetting(ci.textureImporter, BuildTarget.Android.ToString(), srcSize, TextureImporterFormat.ETC_RGB4);
                        if (ci.srcTex.width / ci.srcTex.height >= 4 || ci.srcTex.height / ci.srcTex.width >= 4)
                        {
                            SetTexImportSetting(ci.textureImporter, "iPhone", srcSize, TextureImporterFormat.RGB16);
                        }
                        else
                        {
                            SetTexImportSetting(ci.textureImporter, "iPhone", srcSize, TextureImporterFormat.PVRTC_RGB4);
                        }

                        // ci.textureImporter.SetPlatformTextureSettings("Standalone", srcSize, TextureImporterFormat.DXT1);
                    }
                    if (isUI)
                    {
                        SetTexImportSetting(ci.textureImporter, "Standalone", 4096, TextureImporterFormat.RGBA32);
                    }
                }
            }

            if (import)
            {
                AssetDatabase.ImportAsset(ci.path, ImportAssetOptions.ForceUpdate);
            }
        }

        private static void _ProcessAlpha(ref ConvertInfo ci, bool halfSize)
        {
            File.Copy(ci.path, ci.alphaTexPath, true);

            AssetDatabase.ImportAsset(ci.alphaTexPath, ImportAssetOptions.ForceUpdate);
            ci.name = ci.name + "_A";
            ci.path = ci.alphaTexPath;
            ci.isAlpha = true;
            ci.mipmap = false;
            ci.textureImporter = AssetImporter.GetAtPath(ci.alphaTexPath) as TextureImporter;
            _ProcessTexture(ref ci, halfSize);
        }

        private static void _CompressAtlas(ConvertInfo ci, Texture2D aTex, GameObject atlas, Material mat)
        {
            int size = ci.srcTex.width > ci.srcTex.height ? ci.srcTex.height : ci.srcTex.width;
            if (size >= 512)
            {
                Texture2D newTex = null;
                string texResPath = "Assets/Resources/" + ci.resourcePath;
                if (ci.srcFormat != ETextureCompress.Compress && aTex.width != ci.srcTex.width / 2)
                {
                    newTex = MakeCompressTexture(texResPath + "Half.png", ci.srcTex, false);
                }
                else
                {
                    return;
                }

                Texture2D newAlphaTex = MakeCompressTexture(texResPath + "Half_A.png", aTex, true);


                Material newMat = null;
                if (mat.shader.name == "Custom/UI/Additive")
                {
                    newMat = new Material(Shader.Find("Custom/UI/Additive"));
                }
                else
                {
                    newMat = new Material(Shader.Find("Custom/UI/SeparateColorAlpha"));
                }
                newMat.mainTexture = newTex;
                newMat.SetTexture("_Mask", newAlphaTex);
                newMat = CreateOrReplaceAsset<Material>(newMat, "Assets/Resources/" + ci.resourcePath + "Half.mat");

                GameObject newAtlas = GameObject.Instantiate(atlas) as GameObject;
                UIAtlas a = newAtlas.GetComponent<UIAtlas>();
                a.spriteMaterial = newMat;
                PrefabUtility.CreatePrefab("Assets/Resources/" + ci.resourcePath + "Half.prefab", newAtlas, ReplacePrefabOptions.ReplaceNameBased);
                GameObject.DestroyImmediate(newAtlas);
            }
        }
        private static Texture2D MakeCompressTexture(string desTex, Texture src, bool isAlpha, int scale = 1, bool mipmap = false)
        {
            if (isAlpha)
            {
                TextureCombine.ScaleTexture(src, desTex, src.width / 2, src.height / 2, "Custom/Editor/TexScaleAlpha");
            }
            else
                TextureCombine.ScaleTexture(src, desTex, src.width / scale, src.height / scale, "Custom/Editor/TexScale");

            TextureImporter newTexImporter = AssetImporter.GetAtPath(desTex) as TextureImporter;
            Texture2D newTex = AssetDatabase.LoadAssetAtPath(desTex, typeof(Texture)) as Texture2D;

            ConvertInfo newTexCI = new ConvertInfo();
            newTexCI.isAlpha = isAlpha;
            newTexCI.srcTex = newTex;
            newTexCI.textureImporter = newTexImporter;
            newTexCI.path = desTex;
            newTexCI.srcFormat = ETextureCompress.Compress;
            newTexCI.alphaSize = ETextureSize.Original;
            newTexCI.mipmap = mipmap;
            _ProcessTexture(ref newTexCI, true);
            return newTex;
        }

        private static Texture2D MakeHalfTexture(string desTex, Texture2D src, bool mipmap = false)
        {
            TextureCombine.ScaleTexture(src, desTex, src.width / 2, src.height / 2, "Custom/Effect/TexScale");

            TextureImporter newTexImporter = AssetImporter.GetAtPath(desTex) as TextureImporter;

            Texture2D newTex = AssetDatabase.LoadAssetAtPath(desTex, typeof(Texture)) as Texture2D;

            int srcSize = newTex.width > newTex.height ? newTex.width : newTex.height; ;

            newTexImporter.textureType = TextureImporterType.Default;
            newTexImporter.anisoLevel = src.anisoLevel;
            newTexImporter.mipmapEnabled = (src.mipmapCount > 1);
            newTexImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            newTexImporter.wrapMode = src.wrapMode;
            newTexImporter.filterMode = src.filterMode;
            newTexImporter.isReadable = false;
            SetTexImportSetting(newTexImporter, BuildTarget.Android.ToString(), srcSize, (TextureImporterFormat)src.format);
            SetTexImportSetting(newTexImporter, "iPhone", srcSize, (TextureImporterFormat)src.format);

            AssetDatabase.ImportAsset(desTex, ImportAssetOptions.ForceUpdate);

            return newTex;
        }

        public static bool _CompressSeparateAlpha(Texture2D tex, TextureImporter textureImporter, string path)
        {
            if (path.IndexOf("_A.png") >= 0 || path.IndexOf("Half.png") >= 0)
                return false;
            ConvertInfo ci = new ConvertInfo();
            ci.srcTex = tex;
            ci.textureImporter = textureImporter;
            ci.path = path;

            //ci.dataSet = dataSet;
            if (GetTexPath(ref ci))
            {
                Material mat = Resources.Load(ci.resourcePath, typeof(Material)) as Material;
                GameObject atlas = Resources.Load(ci.resourcePath, typeof(GameObject)) as GameObject;
                bool isAtlas = mat != null && atlas != null;
                GetTexFormat(mat != null, textureImporter.userData, out ci.srcFormat, out ci.alphaSize);
                ci.isAtlas = isAtlas;
                if (isAtlas)
                {
                    Texture2D aTex = null;
                    //atlas
                    if (mat.shader.name == "Custom/UI/RGBA" ||
                        mat.shader.name == "Custom/UI/SeparateColorAlpha" ||
                        mat.shader.name == "Custom/UI/Additive" ||
                        mat.shader.name == "Unlit/Separate Alpha Colored Mask")
                    {
                        ci.alphaResourcePath = ci.resourcePath + "_A";

                        int index = ci.path.LastIndexOf(".");
                        if (index >= 0)
                        {
                            ci.alphaTexPath = ci.path.Substring(0, index);
                        }
                        else
                        {
                            ci.alphaTexPath = ci.path;
                        }
                        ci.alphaTexPath += "_A.png";
                        _ProcessTexture(ref ci, true, true);
                        _ProcessAlpha(ref ci, true);
                        if (mat.shader.name != "Custom/UI/Additive")
                            mat.shader = Shader.Find("Custom/UI/SeparateColorAlpha");

                        mat.SetTexture("_MainTex", ci.srcTex);
                        aTex = Resources.Load(ci.alphaResourcePath, typeof(Texture)) as Texture2D;
                        mat.SetTexture("_Mask", aTex);
                    }
                    else
                    {
                        if (mat.shader.name == "Custom/UI/WhiteAnim")
                            ci.isAlpha = false;
                        _ProcessTexture(ref ci, true, true);
                    }
                    //_CompressAtlas(ci, aTex, atlas, mat);
                }
                else if (!onlyAtlas)
                {
                    if (!HasAlpha(tex, ci.textureImporter, true))
                    {
                        AssetDatabase.DeleteAsset(ci.alphaTexPath);
                        _ProcessTexture(ref ci, true, true);
                    }
                    else
                    {
                        _ProcessTexture(ref ci, true, true);
                        _ProcessAlpha(ref ci, true);
                    }
                }
            }
            return false;
        }

        [MenuItem(@"Assets/Tool/UI/Compress&SeparateAlpha", false, 0)]
        public static void CompressSeparateAlpha()
        {
            enumTex2D.cb = _CompressSeparateAlpha;
            EnumAsset<Texture2D>(enumTex2D, "CompressSeparateAlpha");
        }
        public static bool _CompressAtlas(Texture2D tex, TextureImporter textureImporter, string path)
        {
            if (path.IndexOf("_A.png") >= 0 || path.IndexOf("Half.png") >= 0)
                return false;
            ConvertInfo ci = new ConvertInfo();
            ci.srcTex = tex;
            ci.textureImporter = textureImporter;
            ci.path = path;

            //ci.dataSet = dataSet;
            if (GetTexPath(ref ci))
            {
                Material mat = Resources.Load(ci.resourcePath, typeof(Material)) as Material;
                GameObject atlas = Resources.Load(ci.resourcePath, typeof(GameObject)) as GameObject;
                bool isAtlas = mat != null && atlas != null;
                GetTexFormat(mat != null, textureImporter.userData, out ci.srcFormat, out ci.alphaSize);
                if (isAtlas)
                {
                    ci.alphaResourcePath = ci.resourcePath + "_A";
                    Texture2D aTex = Resources.Load(ci.alphaResourcePath, typeof(Texture)) as Texture2D;
                    _CompressAtlas(ci, aTex, atlas, mat);
                }
            }
            return false;
        }

        [MenuItem(@"Assets/Tool/UI/CompressAtlas", false, 0)]
        public static void CompressAtlas()
        {
            enumTex2D.cb = _CompressAtlas;
            EnumAsset<Texture2D>(enumTex2D, "CompressAtlas");
        }

        public static void CompressSeparateAlpha(TextureImporter textureImporter)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath(textureImporter.assetPath, typeof(Texture2D)) as Texture2D;
            if (tex != null)
            {
                _CompressSeparateAlpha(tex, textureImporter, textureImporter.assetPath);
            }
        }

        [MenuItem(@"Assets/Tool/UI/SeparateAlpha", false, 0)]
        private static void SeparateAlpha()
        {
            onlyAtlas = true;
            enumTex2D.cb = _CompressSeparateAlpha;
            EnumAsset<Texture2D>(enumTex2D, "SeparateAlpha");
            onlyAtlas = false;
        }

        [MenuItem(@"Assets/Tool/UI/Compress&SeparateAlphaHere", false, 0)]
        private static void CompressSeparateAlphaHere()
        {
            enumTex2D.cb = _CompressSeparateAlpha;
            EnumAsset<Texture2D>(enumTex2D, "CompressSeparateAlphaHere");
        }

        //[MenuItem(@"Assets/Tool/UI/SeparateAlphaHere")]
        //private static void SeparateAlphaHere()
        //{
        //    XImageImporterSet dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
        //    if (dataSet == null)
        //        dataSet = new XImageImporterSet();
        //    else
        //    {
        //        dataSet.Init();
        //    }
        //    inCommonAlphaFolder = false;
        //    onlyAtlas = true;
        //    EnumTextures(_CompressSeparateAlpha, "SeparateAlphaHere", dataSet);
        //    onlyAtlas = false;
        //}

        [MenuItem(@"Assets/Tool/UI/CheckUISameName", false, 0)]
        private static void CheckUIName()
        {
            Dictionary<string, List<string>> names = new Dictionary<string, List<string>>();
            UnityEngine.Object[] textures = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
            if (textures != null)
            {
                for (int i = 0; i < textures.Length; ++i)
                {
                    Texture2D tex = textures[i] as Texture2D;
                    if (tex != null)
                    {
                        List<string> list;
                        string name = tex.name.ToLower();
                        if (!names.TryGetValue(name, out list))
                        {
                            list = new List<string>();
                            names.Add(name, list);
                        }
                        list.Add(AssetDatabase.GetAssetPath(tex));
                    }
                }
            }
            foreach (KeyValuePair<string, List<string>> name in names)
            {
                if (name.Value.Count > 1)
                {
                    Debug.LogWarning(string.Format("======Same Name Tex:{0}======", name.Key));
                    foreach (string path in name.Value)
                    {
                        Debug.LogWarning(path);
                    }
                }
            }
        }

        //private static bool _ReplaceUIMat(Material material, string path)
        //{
        //    if (material.shader.name == "Unlit/Transparent Colored")
        //    {
        //        material.shader = Shader.Find("Unlit/Separate Alpha Colored");
        //    }
        //    if (material.shader.name == "Unlit/Separate Alpha Colored")
        //    {
        //        string alphaResourcePath = "atlas/UI/Alpha/" + material.name + "_A";
        //        Texture2D aTex = Resources.Load(alphaResourcePath, typeof(Texture)) as Texture2D;
        //        material.SetTexture("_Mask", aTex);
        //    }
        //    return false;
        //}

        //[MenuItem(@"Assets/Tool/UI/ReplaceUIMat")]
        //private static void ReplaceUIMat()
        //{
        //    EnumMaterial(_ReplaceUIMat, "ReplaceUIMat");
        //}

        //[MenuItem(@"Assets/Tool/UI/GenerateAtlasAlpha")]
        //private static void GenerateAtlasAlpha()
        //{
        //    XImageImporterSet dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
        //    if (dataSet == null)
        //        dataSet = new XImageImporterSet();
        //    else
        //    {
        //        dataSet.Init();
        //    }
        //    inCommonAlphaFolder = true;
        //    onlyAtlas = true;
        //    EnumTextures(_CompressSeparateAlpha, "SeparateAlpha", dataSet);
        //    onlyAtlas = false;
        //}

        public static void SeparateTexture(string[] files, bool isHighQuality = false)
        {
            //XImageImporterSet dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
            //if (dataSet == null)
            //    dataSet = new XImageImporterSet();
            //else
            //{
            //    dataSet.Init();
            //}
            int i = 0;
            foreach (string strPath in files)
            {
                string strTempPath = strPath.Replace(@"\", "/");
                strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(@strTempPath, typeof(UnityEngine.Object));
                Texture2D tex = obj as Texture2D;
                if (tex != null)
                {
                    string path = AssetDatabase.GetAssetPath(tex);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    _CompressSeparateAlpha(tex, textureImporter, path);
                    EditorUtility.DisplayProgressBar(i.ToString(), tex.name, 1.0f * i / files.Length);
                }
                ++i;
            }
            EditorUtility.ClearProgressBar();
        }


        //public static bool PreCheckImportUITexture(ref ConvertInfo ci)
        //{
        //    if (manulConvert || atlasProcess)
        //    {
        //        return false;
        //    }

        //    if (GetTexPath(ref ci))
        //    {
        //        Material mat = Resources.Load(ci.resourcePath, typeof(Material)) as Material;
        //        if (mat == null)
        //        {
        //            ci.srcTex = Resources.Load(ci.resourcePath, typeof(Texture2D)) as Texture2D;
        //            if (!ci.isAlpha)
        //            {
        //                if (ci.srcTex != null)
        //                {
        //                    lastRGBTex.SrcSize = ci.srcTex.width > ci.srcTex.height ? ci.srcTex.width : ci.srcTex.height;
        //                    lastRGBTex.SrcName = ci.name;
        //                }
        //                else
        //                {
        //                    lastRGBTex.SrcSize = 1024;
        //                }
        //            }
        //            ci.dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
        //            if (ci.dataSet == null)
        //                ci.dataSet = new XImageImporterSet();
        //            else
        //            {
        //                ci.dataSet.Init();
        //            }
        //            return true;
        //        }
        //    }
        //    lastRGBTex.SrcSize = -1;
        //    return false;
        //}

        //public static void PreImportUITexture(ref ConvertInfo ci)
        //{
        //    if (ci.isAlpha)
        //    {
        //        _ProcessTexture(ref ci, true, false);
        //        lastRGBTex.SrcSize = -1;
        //    }
        //    else
        //    {
        //        _ProcessTexture(ref ci, false, false);
        //        if (!ci.textureImporter.DoesSourceTextureHaveAlpha())
        //        {
        //            AssetDatabase.DeleteAsset(ci.alphaTexPath);
        //        }
        //        else
        //        {
        //            File.Copy(ci.path, ci.alphaTexPath, true);
        //            AssetDatabase.ImportAsset(ci.alphaTexPath, ImportAssetOptions.ForceSynchronousImport);
        //        }
        //    }
        //}

        public static bool atlasProcess = false;
        //public static void PostImportUIAtlas(Texture2D tex)
        //{
        //    atlasProcess = true;
        //    string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
        //    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        //    if (path.IndexOf("_A.png") >= 0)
        //    {
        //        atlasProcess = false;
        //        return;
        //    }

        //    ConvertInfo ci = new ConvertInfo();
        //    ci.srcTex = tex;
        //    ci.textureImporter = textureImporter;
        //    ci.path = path;
        //    //ci.dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
        //    //if (ci.dataSet == null)
        //    //    ci.dataSet = new XImageImporterSet();
        //    //else
        //    //{
        //    //    ci.dataSet.Init();
        //    //}
        //    if (GetTexPath(ref ci))
        //    {
        //        Material mat = Resources.Load(ci.resourcePath, typeof(Material)) as Material;
        //        if (mat != null)
        //        {
        //            //atlas
        //            if (mat.shader.name == "Unlit/Transparent Colored" ||
        //                mat.shader.name == "Unlit/Separate Alpha Colored" ||
        //                mat.shader.name == "Unlit/Particles/Additive" ||
        //                mat.shader.name == "Unlit/Separate Alpha Colored Mask")
        //            {
        //                _ProcessTexture(ref ci, false);
        //                _ProcessAlpha(ref ci, false);

        //                mat.shader = Shader.Find("Unlit/Separate Alpha Colored");

        //                mat.SetTexture("_MainTex", ci.srcTex);
        //                Texture2D aTex = Resources.Load(ci.alphaResourcePath, typeof(Texture)) as Texture2D;
        //                mat.SetTexture("_Mask", aTex);
        //            }
        //            else
        //            {
        //                _ProcessTexture(ref ci, false);
        //            }
        //        }
        //    }
        //    atlasProcess = false;
        //}


        [MenuItem(@"Assets/Tool/UI/FindAtlas&Texture", false, 0)]
        public static void FindAtlasTexture()
        {
            Rect wr = new Rect(0, 0, 600, 800);
            TextureStatis window = (TextureStatis)EditorWindow.GetWindowWithRect(typeof(TextureStatis), wr, true, "查找贴图");
            window.ScanGameObject();
            window.Show();
        }

        private static Dictionary<int, List<AlphaTexTask>> atlasTasks = new Dictionary<int, List<AlphaTexTask>>();
        private static Dictionary<int, List<AlphaTexTask>> texTasks = new Dictionary<int, List<AlphaTexTask>>();
        public class AlphaTexTask
        {
            public Texture2D srcTex;
            public Material mat;
            public AlphaTexTask(Texture2D t, Material m)
            {
                srcTex = t;
                mat = m;
            }
        }

        private static void InitAlphaTask()
        {
            atlasTasks[1024] = new List<AlphaTexTask>();
            atlasTasks[512] = new List<AlphaTexTask>();
            atlasTasks[256] = new List<AlphaTexTask>();
            atlasTasks[128] = new List<AlphaTexTask>();
            atlasTasks[64] = new List<AlphaTexTask>();
            atlasTasks[32] = new List<AlphaTexTask>();

            texTasks[1024] = new List<AlphaTexTask>();
            texTasks[512] = new List<AlphaTexTask>();
            texTasks[256] = new List<AlphaTexTask>();
            texTasks[128] = new List<AlphaTexTask>();
            texTasks[64] = new List<AlphaTexTask>();
            texTasks[32] = new List<AlphaTexTask>();
        }

        private static Shader alphaMask = Shader.Find("Unlit/Separate Alpha Colored Mask");
        private static void CombineAlphaTex(int size,
            AlphaTexTask task0,
            AlphaTexTask task1,
            AlphaTexTask task2,
            AlphaTexTask task3)
        {
            string name = "";
            if (task0.srcTex != null)
            {
                name = task0.srcTex.name;
            }
            if (task1 != null && task1.srcTex != null)
            {
                name += "_" + task1.srcTex.name;
            }
            if (task2 != null && task2.srcTex != null)
            {
                name += "_" + task2.srcTex.name;
            }
            if (task3 != null && task3.srcTex != null)
            {
                name += "_" + task3.srcTex.name;
            }
            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            for (int y = 0, ymax = size; y < ymax; ++y)
            {
                for (int x = 0, xmax = size; x < xmax; ++x)
                {
                    Color c0 = Color.black;
                    Color c1 = Color.black;
                    Color c2 = Color.black;
                    Color c3 = Color.black;
                    if (task0.srcTex != null)
                    {
                        c0 = task0.srcTex.GetPixel(x, y);
                    }
                    if (task1 != null && task1.srcTex != null)
                    {
                        c1 = task1.srcTex.GetPixel(x, y);
                    }
                    if (task2 != null && task2.srcTex != null)
                    {
                        c2 = task2.srcTex.GetPixel(x, y);
                    }
                    if (task3 != null && task3.srcTex != null)
                    {
                        c3 = task3.srcTex.GetPixel(x, y);
                    }
                    Color a = new Color(c0.a, c1.a, c2.a, c3.a);
                    tex.SetPixel(x, y, a);
                }
            }

            byte[] bytes = tex.EncodeToPNG();
            string alphaTexPath = "Assets/Resources/atlas/UI/Alpha/" + name + "_A.png";
            File.WriteAllBytes(alphaTexPath, bytes);
            AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
            TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaTexPath) as TextureImporter;
            if (alphaTextureImporter != null)
            {
                alphaTextureImporter.textureType = TextureImporterType.Default;
                alphaTextureImporter.anisoLevel = 0;
                alphaTextureImporter.mipmapEnabled = false;
                alphaTextureImporter.isReadable = false;
                alphaTextureImporter.npotScale = TextureImporterNPOTScale.ToLarger;
                alphaTextureImporter.wrapMode = TextureWrapMode.Clamp;
                SetTexImportSetting(alphaTextureImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA16);
                SetTexImportSetting(alphaTextureImporter, "iPhone", size, TextureImporterFormat.RGBA16);
                AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(alphaTexPath, typeof(UnityEngine.Object));
                Texture2D newTex = obj as Texture2D;
                if (newTex != null)
                {
                    if (task0.mat != null)
                    {
                        task0.mat.shader = alphaMask;
                        task0.mat.SetTexture("_Mask", newTex);
                        task0.mat.SetVector("_Channel", new Vector4(1, 0, 0, 0));
                    }
                    if (task1 != null && task1.mat != null)
                    {
                        task1.mat.shader = alphaMask;
                        task1.mat.SetTexture("_Mask", newTex);
                        task1.mat.SetVector("_Channel", new Vector4(0, 1, 0, 0));
                    }
                    if (task2 != null && task2.mat != null)
                    {
                        task2.mat.shader = alphaMask;
                        task2.mat.SetTexture("_Mask", newTex);
                        task2.mat.SetVector("_Channel", new Vector4(0, 0, 1, 0));
                    }
                    if (task3 != null && task3.mat != null)
                    {
                        task3.mat.shader = alphaMask;
                        task3.mat.SetTexture("_Mask", newTex);
                        task3.mat.SetVector("_Channel", new Vector4(0, 0, 0, 1));
                    }
                }

            }
            if (task0.srcTex != null)
            {
                string path = AssetDatabase.GetAssetPath(task0.srcTex);
                TextureImporter srcImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (srcImporter != null)
                {
                    srcImporter.isReadable = false;
                    SetTexImportSetting(srcImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                    SetTexImportSetting(srcImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            if (task1 != null && task1.srcTex != null)
            {
                string path = AssetDatabase.GetAssetPath(task1.srcTex);
                TextureImporter srcImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (srcImporter != null)
                {
                    srcImporter.isReadable = false;
                    SetTexImportSetting(srcImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                    SetTexImportSetting(srcImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            if (task2 != null && task2.srcTex != null)
            {
                string path = AssetDatabase.GetAssetPath(task2.srcTex);
                TextureImporter srcImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (srcImporter != null)
                {
                    srcImporter.isReadable = false;
                    SetTexImportSetting(srcImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                    SetTexImportSetting(srcImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
            if (task3 != null && task3.srcTex != null)
            {
                string path = AssetDatabase.GetAssetPath(task3.srcTex);
                TextureImporter srcImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (srcImporter != null)
                {
                    srcImporter.isReadable = false;
                    SetTexImportSetting(srcImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.ETC_RGB4);
                    SetTexImportSetting(srcImporter, "iPhone", size, TextureImporterFormat.PVRTC_RGB4);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }


        private static int SortByName(AlphaTexTask a, AlphaTexTask b)
        {
            if (a.srcTex != null && b.srcTex != null)
            {
                return string.Compare(a.srcTex.name, b.srcTex.name);
            }
            if (a.srcTex != null && b.srcTex == null)
            {
                return 1;
            }
            if (a.srcTex == null && b.srcTex != null)
            {
                return -1;
            }
            return 1;
        }
        private static void PostProcessAlpha()
        {
            foreach (KeyValuePair<int, List<AlphaTexTask>> pair in atlasTasks)
            {
                int size = pair.Key;
                List<AlphaTexTask> list = pair.Value;
                list.Sort(SortByName);
                for (int i = 0, imax = list.Count; i < imax; i += 4)
                {
                    AlphaTexTask task0 = list[i];
                    AlphaTexTask task1 = null;
                    AlphaTexTask task2 = null;
                    AlphaTexTask task3 = null;
                    if ((i + 1) < imax)
                    {
                        task1 = list[i + 1];
                    }
                    if ((i + 2) < imax)
                    {
                        task2 = list[i + 2];
                    }
                    if ((i + 3) < imax)
                    {
                        task3 = list[i + 3];
                    }
                    CombineAlphaTex(size, task0, task1, task2, task3);
                }
            }
        }
        private static void PreConvertTex2Mobile(Texture2D srcTex, TextureImporter textureImporter, string path)
        {
            int srcSize = srcTex.width > srcTex.height ? srcTex.width : srcTex.height;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.anisoLevel = 0;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = true;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.filterMode = FilterMode.Bilinear;
            SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), srcSize, TextureImporterFormat.RGBA32);
            SetTexImportSetting(textureImporter, "iPhone", srcSize, TextureImporterFormat.RGBA32);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        private static void _ProcessAtlasAlpha(Texture2D srcTex, Material mat, string path, string alphaTexPath)
        {
            int srcSize = srcTex.width > srcTex.height ? srcTex.width : srcTex.height;
            int size = srcSize;

            List<AlphaTexTask> taskList = null;
            if (mat != null)
            {
                if (atlasTasks.TryGetValue(size, out taskList))
                {
                    taskList.Add(new AlphaTexTask(srcTex, mat));
                }
            }
            else
            {
                if (texTasks.TryGetValue(size, out taskList))
                {
                    taskList.Add(new AlphaTexTask(srcTex, mat));
                }
            }
        }

        private static bool _CombineAlpha(Texture2D tex, TextureImporter textureImporter, string path)
        {
            if (path.IndexOf("_A.png") >= 0)
                return false;
            int extIndex = path.LastIndexOf(".");
            //1.check mat first
            string matPath = path;
            if (extIndex >= 0)
            {
                matPath = path.Substring(0, extIndex);
                string relativePath = "Assets/Resources/";
                matPath = matPath.Substring(relativePath.Length);

                //string alphaResourcePath = inCommonAlphaFolder ? "atlas/UI/Alpha/" + tex.name + "_A" : matPath + "_A";
                string alphaTexPath = "Assets/Resources/atlas/UI/Alpha/" + tex.name + "_A.png";


                Material mat = Resources.Load(matPath, typeof(Material)) as Material;
                if (mat != null)
                {
                    //atlas
                    if (mat.shader.name == "Custom/UI/RGBA" ||
                        mat.shader.name == "Custom/UI/SeparateColorAlpha")
                    {
                        PreConvertTex2Mobile(tex, textureImporter, path);
                        _ProcessAtlasAlpha(tex, mat, path, alphaTexPath);
                    }
                    else if (mat.shader.name == "Custom/UI/Additive")
                    {
                        PreConvertTex2Mobile(tex, textureImporter, path);
                        _ProcessAtlasAlpha(tex, mat, path, alphaTexPath);
                    }
                    else
                    {
                    }
                }
            }
            return false;
        }


        //[MenuItem(@"Assets/Tool/UI/CombineAlpha")]
        //private static void CombineAlpha()
        //{
        //    InitAlphaTask();
        //    XImageImporterSet dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
        //    if (dataSet == null)
        //        dataSet = new XImageImporterSet();
        //    else
        //    {
        //        dataSet.Init();
        //    }
        //    inCommonAlphaFolder = true;
        //    EnumTextures(_CombineAlpha, "CombineAlpha", dataSet, PostProcessAlpha);
        //}
        #endregion UI
        #region fbx           

        private static bool _MakeFbxReadOnly(GameObject fbx, ModelImporter modelImporter, string path)
        {
            bool change = modelImporter.isReadable == false;
            modelImporter.isReadable = false;
            if(modelImporter.meshCompression == ModelImporterMeshCompression.Low|| modelImporter.meshCompression == ModelImporterMeshCompression.Off)
            {
                if (path.ToLower().Contains("_bandpose"))
                    Debug.LogError(path);
            }
            return false;
        }

        [MenuItem(@"Assets/Tool/Fbx/MakeFbxReadOnly", false, 0)]
        private static void MakeFbxReadOnly()
        {
            enumFbx.cb = _MakeFbxReadOnly;
            EnumAsset<GameObject>(enumFbx, "MakeFbxReadOnly");
        }
        //private static void CheckMeshVertex(Mesh mesh)
        //{
        //    List<Vector3> newPos = ListPool<Vector3>.Get();
        //    List<Vector2> newUV = ListPool<Vector2>.Get();
        //    List<Vector2> newUV2 = ListPool<Vector2>.Get();
        //    Vector3[] pos = mesh.vertices;
        //    Vector2[] uv = mesh.uv;
        //    Vector2[] uv2 = mesh.uv2;
        //    int index = 0;
        //    Vector3 invalidPos = new Vector3(-100001, -100000, -100000);
        //    while(index< pos.Length)
        //    {
        //        Vector3 posStart = pos[index];
        //        if (posStart.x > -100000)
        //        {
        //            for (int i = index + 1; i < pos.Length; ++i)
        //            {
        //                Vector3 p = pos[i] - posStart;
        //                p.x = Mathf.Abs(p.x);
        //                p.y = Mathf.Abs(p.y);
        //                p.z = Mathf.Abs(p.z);
        //                if (p.x < 0.01f && p.y < 0.01f && p.z < 0.01f)
        //                {
        //                    pos[i] = invalidPos;
        //                    newUV.Add(uv[i]);
        //                    newUV2.Add(uv2[i]);
        //                    newPos.Add(posStart);
        //                    index++;
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            index++;
        //        }
        //    }
        //    if(newPos.Count%4==0)
        //    {
        //        List<int> newIndex = ListPool<int>.Get();
        //        for (int i = 0; i < newPos.Count; i += 4)
        //        {
        //            Vector3 p0 = newPos[i];
        //            Vector3 p1 = newPos[i + 1];
        //            Vector3 p2 = newPos[i + 2];
        //            Vector3 p3 = newPos[i + 3];
        //            newIndex.Add(i);
        //            newIndex.Add(i + 1);
        //            newIndex.Add(i + 2);

        //            newIndex.Add(i + 1);
        //            newIndex.Add(i + 0);
        //            newIndex.Add(i + 3);
        //        }
        //        mesh.triangles = newIndex.ToArray();
        //        mesh.vertices = newPos.ToArray();
        //        mesh.uv = newUV.ToArray();
        //        mesh.uv2 = newUV2.ToArray();
        //        ListPool<int>.Release(newIndex);
        //    }

        //    ListPool<Vector3>.Release(newPos);


        //}


        public class ExportInfo
        {
            public bool needNormal = false;
            public bool needColor = false;
            public bool needExportAnim = true;
            public bool needUV2 = false;
            public bool notNeedExportMesh = false;
            public ModelImporterMeshCompression compress = ModelImporterMeshCompression.Medium;
            public ModelImporterTangents tangents = ModelImporterTangents.None;
        }

        public static ExportInfo fbxExportInfo = new ExportInfo();
        public static void PreExportMeshAvatar(ModelImporterMeshCompression compress, ModelImporterTangents tangents)
        {
            fbxExportInfo.compress = compress;
            fbxExportInfo.tangents = tangents;
        }

        public static void PreExportMeshAvatar(string path)
        {
            string lowPath = path.ToLower();
            bool needNormal = lowPath.Contains("_normal");
            bool needColor = lowPath.Contains("_color");
            bool isCreature = lowPath.StartsWith("assets/creatures/");
            bool isEffect = lowPath.StartsWith("assets/effect/");
            bool isSene = lowPath.StartsWith("assets/xscene/");
            bool isEuip = lowPath.StartsWith("assets/equipment/") &&
                !lowPath.StartsWith("assets/equipment/tail/") &&
                !lowPath.StartsWith("assets/equipment/wing/") &&
                !lowPath.StartsWith("assets/equipment/spirit/") &&
                !lowPath.StartsWith("assets/equipment/else/");

            fbxExportInfo.needNormal = needNormal;
            fbxExportInfo.needColor = needColor|| isEffect;
            fbxExportInfo.needExportAnim = !isCreature;
            fbxExportInfo.needUV2 = isEffect|| isSene;
            fbxExportInfo.notNeedExportMesh = isEuip;
            fbxExportInfo.compress = ModelImporterMeshCompression.Medium;
        }

        private static bool _ExportMeshAvatar(GameObject fbx, ModelImporter modelImporter, string path)
        {
            if (fbx == null)
                return false;
            PreExportMeshAvatar(path);
            string name = fbx.name.ToLower();
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            int index = path.LastIndexOf("/");
            if (index >= 0)
            {
                string dir = path.Substring(0, index);
                if (name.EndsWith("_bandpose"))
                {
                    name = name.Replace("_bandpose", "");
                }
                bool hasSkin = false;
                List<Renderer> renderLst = ListPool<Renderer>.Get();
                go.GetComponentsInChildren<Renderer>(true, renderLst);
                for (int i = 0; i < renderLst.Count; ++i)
                {
                    Renderer render = renderLst[i];
                    Mesh mesh = null;
                    Shader shader = render.sharedMaterial != null ? render.sharedMaterial.shader : null;
                    if (render is SkinnedMeshRenderer)
                    {
                        SkinnedMeshRenderer smr = render as SkinnedMeshRenderer;
                        mesh = smr.sharedMesh;
                        hasSkin = true;
                    }
                    else if (render is MeshRenderer)
                    {
                        MeshFilter mf = render.GetComponent<MeshFilter>();
                        mesh = mf.sharedMesh;

                    }
                    if (mesh != null && !fbxExportInfo.notNeedExportMesh)
                    {
                        string newMeshName = string.Format("{0}_{1}", name, i);
                        string meshPath = string.Format("{0}/{1}.asset", dir, newMeshName);
                        Mesh newMesh = UnityEngine.Object.Instantiate<Mesh>(mesh);
                        newMesh.name = newMeshName;
                        if (!hasSkin && !fbxExportInfo.needNormal)
                        {
                            newMesh.normals = null;
                        }
                        if (!fbxExportInfo.needUV2 && !hasSkin)
                        {
                            if (!(shader != null && shader.name.EndsWith("UV2")))
                                newMesh.uv2 = null;
                        }
                        if (!fbxExportInfo.needColor)
                        {
                            newMesh.colors = null;
                        }
                        newMesh.tangents = null;
                        newMesh.uv3 = null;
                        newMesh.uv4 = null;
                        
                        newMesh.UploadMeshData(true);
                        MeshUtility.SetMeshCompression(newMesh, fbxExportInfo.compress);
                        MeshUtility.Optimize(newMesh);
                        CreateOrReplaceAsset<Mesh>(newMesh, meshPath);
                    }
                }
                ListPool<Renderer>.Release(renderLst);
                if (hasSkin)
                {
                    Animator ator = go.GetComponent<Animator>();
                    if (ator != null && ator.avatar != null)
                    {
                        string avatarPath = dir + "/" + fbx.name + "_avatar.asset";
                        Avatar avatar = UnityEngine.Object.Instantiate<Avatar>(ator.avatar);
                        avatar.name = fbx.name + "_avatar";
                        CreateOrReplaceAsset<Avatar>(avatar, avatarPath);
                    }
                }
                if (fbxExportInfo.needExportAnim)
                {
                    UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(path);
                    int animIndex = 0;
                    for (int i = 0; i < allObjects.Length; ++i)
                    {
                        AnimationClip anim = allObjects[i] as AnimationClip;
                        if (anim != null && !anim.name.StartsWith("__preview"))
                        {
                            string newAnimName = string.Format("{0}_{1}", name, animIndex++);
                            string animPath = string.Format("{0}/{1}.anim", dir, newAnimName);
                            AnimationClip newClip = new AnimationClip();
                            EditorUtility.CopySerialized(anim, newClip);
                            newClip.name = newAnimName;
                            CreateOrReplaceAsset<AnimationClip>(newClip, animPath);
                        }
                    }
                }

            }

            GameObject.DestroyImmediate(go);
            return false;
        }




        //[MenuItem(@"Assets/Tool/Fbx/FindBandpose", false, 0)]
        //private static void FindBandpose()
        //{
        //    string path = "Folder\tfbx\r\n";
        //    DirectoryInfo di = new DirectoryInfo("Assets/Creatures");
        //    DirectoryInfo[] subDirs = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
        //    foreach (DirectoryInfo subDir in subDirs)
        //    {
        //        FileInfo[] files = subDir.GetFiles("*_bandpose.fbx", SearchOption.TopDirectoryOnly);
        //        if (files.Length > 1)
        //        {
        //            path += subDir.FullName + "\t";
        //            foreach (FileInfo file in files)
        //            {
        //                path += file.Name + "|";
        //            }
        //            path += "\r\n";
        //        }
        //        else if (files.Length == 0)
        //        {
        //            Debug.LogError(subDir.FullName);
        //        }
        //    }
        //    File.WriteAllText("Assets/Resources/a.txt", path);
        //}
        public static void ExportMeshAvatar(ModelImporter modelimporter, string path, GameObject fbx)
        {
            if (fbx == null)
                fbx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            _ExportMeshAvatar(fbx, modelimporter, path);
        }


        static string targetAnimFolder = "Animation";
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
        private static bool _ExtractAnimationClipFromFBX(GameObject fbx, ModelImporter modelImporter, string path)
        {
            if (modelImporter.animationCompression != ModelImporterAnimationCompression.Optimal)
            {
                modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }


            int index = fbx.name.IndexOf('_');
            if(index>=0)
            {
                index = fbx.name.IndexOf('_', index + 1);
                if(index >= 0)
                {
                    string folderName = fbx.name.Substring(0, index);
                    string targetPath = "Assets/Resources/" + targetAnimFolder + "/" + folderName;

                    if (!Directory.Exists(targetPath))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources/" + targetAnimFolder, folderName);
                    }
                    UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(path);
                    for (int i = 0; i < allObjects.Length; ++i)
                    {
                        AnimationClip anim = allObjects[i] as AnimationClip;
                        if (anim != null && !anim.name.StartsWith("__preview"))
                        {
                            
                            if (string.IsNullOrEmpty(anim.name))
                            {
                                Debug.LogError(path);
                            }
                            else
                            {
                                AnimationClip newClip = new AnimationClip();
                                EditorUtility.CopySerialized(anim, newClip);
                                newClip.name = anim.name;
                                string newClipPath = targetPath + "/" + anim.name + ".anim";
                                _ReduceKeyFrame(newClip, newClipPath);
                                CreateOrReplaceAsset<AnimationClip>(newClip, newClipPath);
                            }
                        }
                    }
                }

            }

            return false;
        }

        [MenuItem("Assets/ExtractFBX")]
        private static void ExtractAnimationClipFromFBX()
        {
            if (!Directory.Exists("Assets/Resources/" + targetAnimFolder))
            {
                AssetDatabase.CreateFolder("Assets/Resources", targetAnimFolder);
            }

            enumFbx.cb = _ExtractAnimationClipFromFBX;
            EnumAsset<GameObject>(enumFbx, "ExtractAnimationClipFromFBX");
          
        }

        #endregion fbx
        #region curve
        [MenuItem(@"Assets/Tool/Curve/Scan", false, 0)]
        private static void ScanCurve()
        {
            UnityEngine.Object[] curves = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);

            if (curves != null)
            {
                using (FileStream desStream = new FileStream("Assets/Resources/Curves.bytes", FileMode.Create))
                {
                    int count = curves.Length;
                    BinaryWriter writer = new BinaryWriter(desStream);
                    writer.Write(count);
                    for (int i = 0; i < curves.Length; ++i)
                    {
                        UnityEngine.Object curve = curves[i];
                        string path = AssetDatabase.GetAssetPath(curve);
                        path = path.Replace("Assets/Resources/Server/", "");
                        writer.Write(path);
                        EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", path, i, curves.Length), path, (float)i / curves.Length);
                    }
                }

            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }

        [MenuItem(@"Assets/Tool/Curve/Read", false, 0)]
        private static void ReadCurve()
        {
            UnityEngine.Object[] curves = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);

            if (curves != null)
            {
                using (FileStream desStream = new FileStream("Assets/Resources/Curves.bytes", FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(desStream);
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; ++i)
                    {
                        string path = reader.ReadString();
                        Debug.Log(path);
                        EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", path, i, curves.Length), path, (float)i / curves.Length);
                    }
                }

            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }
        #endregion
        #region animation

        private static bool ComputerKeyDerivative(Keyframe preKey,Keyframe midKey, Keyframe currentKey)
        {
            float ddx0 = (midKey.value - preKey.value);
            float ddx1 = (currentKey.value - midKey.value);
            float derivativeValue = Mathf.Abs(ddx1 - ddx0);

            float ddxInTangent0 = midKey.inTangent - preKey.inTangent;
            float ddxInTangent1 = currentKey.inTangent - midKey.inTangent;
            float derivativeInTangent = Mathf.Abs(ddxInTangent1 - ddxInTangent0);

            float ddxOutTangent0 = midKey.outTangent - preKey.outTangent;
            float ddxOutTangent1 = currentKey.outTangent - midKey.outTangent;
            float derivativeOutTangent = Mathf.Abs(ddxOutTangent1 - ddxOutTangent0);

            return derivativeValue < 0.01f &&
               derivativeInTangent < 0.01f &&
                derivativeOutTangent < 0.01f;
        }

        public static void _ReduceKeyFrame(AnimationClip animClip, string path)
        {
            errorLog = "";
            int reduceCurveCount = 0;
            bool isMount = path.Contains("_Mount_");
            List<int> removeIndex = ListPool<int>.Get();
            EditorCurveBinding[] curveBinding = AnimationUtility.GetCurveBindings(animClip);
            for (int i = 0; i < curveBinding.Length; ++i)
            {
                var binding = curveBinding[i];
                AnimationCurve curve = AnimationUtility.GetEditorCurve(animClip, binding);
                if (curve.keys.Length > 1)
                {
                    removeIndex.Clear();
                    bool scale = binding.propertyName.StartsWith("m_LocalScale");
                    bool pos = binding.propertyName.StartsWith("m_LocalPosition");

                    Keyframe[] keys = new Keyframe[curve.keys.Length];
                    for (int j = 0; j < curve.keys.Length; ++j)
                    {
                        Keyframe key = curve.keys[j];
                        if (scale || pos)
                        {
                            key.value = (float)Math.Round(key.value, 4);
                        }
                        else
                        {
                            key.value = key.value;
                        }
                        key.inTangent = (float)Math.Round(key.inTangent, 3);
                        key.outTangent = (float)Math.Round(key.outTangent, 3);
                        keys[j] = key;
                    }

                    Keyframe preKey = keys[0];
                    Keyframe midKey = keys[1];

                    float defaultValue = scale ? 1 : 0;
                    bool isDefaultValue = Mathf.Abs(preKey.value - defaultValue) < 0.01f&& Mathf.Abs(midKey.value - defaultValue) < 0.01f;

                    for (int j = 2; j < keys.Length; ++j)
                    {
                        Keyframe key = keys[j];
                        if (ComputerKeyDerivative(preKey, midKey, key))
                        {
                            removeIndex.Add(j - 1);
                        }


                        float defaultError = Mathf.Abs(key.value - defaultValue);
                        float defaultErrorPercent = scale ? defaultError / defaultValue : defaultError;
                        if (defaultErrorPercent > 0.01f)
                            isDefaultValue = false;

                        preKey = midKey;
                        midKey = key;
                    }
                    curve.keys = keys;

                    if (isDefaultValue)
                    {
                        if (isMount && pos && binding.propertyName == "m_LocalPosition.z" && !binding.path.Contains("/"))
                        {
                            Debug.Log("not opt curve" + binding.path);
                        }
                        else
                        {
                            AnimationUtility.SetEditorCurve(animClip, binding, null);
                            reduceCurveCount++;
                            if (!(pos || scale))
                            {
                                errorLog += string.Format("{0}:{1}\r\n", binding.path, binding.propertyName);
                            }
                        }                       
                    }
                    else
                    {
                        for (int j = removeIndex.Count - 1; j >= 0; --j)
                        {
                            curve.RemoveKey(removeIndex[j]);
                        }
                        if (removeIndex.Count > 0)
                            reduceCurveCount++;

                        AnimationUtility.SetEditorCurve(animClip, binding, curve);
                    }
                    
                }
                else
                {
                    AnimationUtility.SetEditorCurve(animClip, binding, null);
                }
            }
            if (reduceCurveCount > 0)
            {
                Debug.LogWarning(string.Format("{0} reduceCurveCount/total:{1}/{2}\r\n{3}", path, reduceCurveCount, curveBinding.Length, errorLog));
            }
            ListPool<int>.Release(removeIndex);
        }

        [MenuItem(@"Assets/Tool/Animation/ReduceKeyFrame", false, 0)]
        private static void ReduceKeyFrame()
        {
            enumAnimationClip.cb = _ReduceKeyFrame;
            EnumAsset<AnimationClip>(enumAnimationClip, "ReduceKeyFrame");
        }

        private static void _ConvertAnimation2Legacy(AnimationClip animClip, string path)
        {
            bool loop = animClip.isLooping;
            animClip.legacy = true;
            if (loop)
                animClip.wrapMode = WrapMode.Loop;
        }

        [MenuItem(@"Assets/Tool/Animation/ConvertAnimator2Legacy", false, 0)]
        private static void ConvertAnimation2Legacy()
        {
            enumAnimationClip.cb = _ConvertAnimation2Legacy;
            EnumAsset<AnimationClip>(enumAnimationClip, "ConvertAnimation2Legacy");
        }
        static AssetBundleBuild[] testAnimBundle = new AssetBundleBuild[1];
        static string[] testAnimBundlePath = new string[1];
        private static void _TestAnimBundle(AnimationClip animClip, string path)
        {
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = animClip.name;
            abb.assetBundleVariant = "";
            testAnimBundlePath[0] = path;
            abb.assetNames = testAnimBundlePath;
            testAnimBundle[0] = abb;
            BuildPipeline.BuildAssetBundles("Assets/Test", testAnimBundle, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem(@"Assets/Tool/Animation/TestAnimBundle", false, 0)]
        private static void TestAnimBundle()
        {
            enumAnimationClip.cb = _TestAnimBundle;
            EnumAsset<AnimationClip>(enumAnimationClip, "TestAnimBundle");
        }

        [MenuItem(@"Assets/Tool/Animation/RefreshAnim", false, 0)]
        private static void RefreshAnim()
        {
            using (FileStream desStream = new FileStream(@"Assets/Resources/anim.bytes", FileMode.Open))
            {
                BinaryReader sr = new BinaryReader(desStream);
                int count = sr.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    string path = sr.ReadString();
                    bool loopPose = sr.ReadBoolean();
                    //int index = path.LastIndexOf("/");
                    //string dir = path.Substring(0, index);
                    //string animName = path.Substring(index + 1).Replace(".anim", ".fbx");
                    //index = dir.LastIndexOf("/");
                    //string dirName = dir.Substring(index + 1);
                    //string fbxPath = string.Format("Assets/Creatures/{0}/{1}", dirName, animName);
                    AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    SerializedObject serializedClip = new SerializedObject(animClip);
                    SerializedProperty sp = serializedClip.FindProperty("m_AnimationClipSettings");
                    if (sp != null)
                    {
                        SerializedProperty subsp0 = sp.FindPropertyRelative("m_LoopTime");
                        if (subsp0 != null)
                        {
                            subsp0.boolValue = true;
                        }
                        SerializedProperty subsp1 = sp.FindPropertyRelative("m_LoopBlend");
                        if (subsp1 != null)
                        {
                            subsp1.boolValue = loopPose;
                        }
                    }
                    serializedClip.ApplyModifiedProperties();
                    EditorUtility.DisplayProgressBar(string.Format("RefreshAnim-{0}/{1}", i, count), path, (float)i / count);
                    //ModelImporter modelImporter = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
                    //if (modelImporter != null)
                    //{
                    //    ModelImporterClipAnimation[] clipAnimations = modelImporter.clipAnimations;
                    //    for (int j = 0; j < clipAnimations.Length; ++j)
                    //    {
                    //        ModelImporterClipAnimation mica = clipAnimations[j];
                    //        mica.loop = true;
                    //        mica.loopTime = true;
                    //        mica.loopPose = loopPose;
                    //    }
                    //    modelImporter.clipAnimations = clipAnimations;
                    //    modelImporter.animationCompression = ModelImporterAnimationCompression.KeyframeReductionAndCompression;
                    //    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        
                    //}
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Finish", "All assets processed finish", "OK");
            }
        }
        //public static string relativeRoot = "Assets/Animation/";
        //private static string platform = "Android";
        ////private static BuildTarget target = BuildTarget.Android;

        //private static void _BuildBundle(AnimationClip clip, string path)
        //{
        //    string relativeDir = path.Substring(relativeRoot.Length);
        //    int index = relativeDir.LastIndexOf("/");
        //    relativeDir = relativeDir.Substring(0, index);
        //    string targetDir = string.Format("Assets/AssetBundle/{0}/Animation/{1}", platform, relativeDir);
        //    if (!Directory.Exists(targetDir))
        //    {
        //        Directory.CreateDirectory(targetDir);
        //    }
        //    string targetPath = string.Format("{0}/{1}.bundle", targetDir, clip.name);

        //    BuildPipeline.BuildAssetBundle(clip,
        //                             null,
        //                             targetPath,
        //                             BuildAssetBundleOptions.UncompressedAssetBundle |
        //                             BuildAssetBundleOptions.CompleteAssets |
        //                             BuildAssetBundleOptions.DeterministicAssetBundle,
        //                             EditorUserBuildSettings.activeBuildTarget);
        //}

        //[MenuItem(@"Assets/Tool/Animation/BuildBundleAndroid")]
        //private static void BuildBundleAndroid()
        //{
        //    platform = "Android";
        //    EnumAnimation(_BuildBundle, "BuildBundleAndroid");
        //}

        //[MenuItem(@"Assets/Tool/Animation/BuildBundleIOS")]
        //private static void BuildBundleIOS()
        //{
        //    platform = "IOS";
        //    EnumAnimation(_BuildBundle, "BuildBundleIOS");
        //}
        //[MenuItem(@"Assets/Tool/Animation/BuildBundlePC")]
        //private static void BuildBundlePC()
        //{
        //    platform = "PC";
        //    EnumAnimation(_BuildBundle, "BuildBundlePC");
        //}
        //public static void BuildBundle()
        //{
        //    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        //    {
        //        platform = "Android";
        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
        //    {
        //        platform = "IOS";
        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        //    {
        //        platform = "PC";

        //    }
        //    else
        //    {
        //        return;
        //    }
        //    DirectoryInfo animDir = new DirectoryInfo(AssetModify.relativeRoot);
        //    FileInfo[] files = animDir.GetFiles("*.anim", SearchOption.AllDirectories);
        //    if (files != null)
        //    {
        //        FileInfo root = new FileInfo(Application.dataPath);
        //        string rootPath = root.FullName.Replace("\\", "/");
        //        int index = rootPath.LastIndexOf("/");
        //        rootPath = rootPath.Substring(0, index + 1);
        //        for (int i = 0, imax = files.Length; i < imax; ++i)
        //        {
        //            FileInfo file = files[i];
        //            string clipPath = file.FullName.Replace("\\", "/");
        //            string relativePath = clipPath.Substring(rootPath.Length);
        //            AnimationClip clip = AssetDatabase.LoadAssetAtPath(relativePath, typeof(AnimationClip)) as AnimationClip;
        //            if (clip != null)
        //            {
        //                _BuildBundle(clip, relativePath);
        //                EditorUtility.DisplayProgressBar(string.Format("Build Anim Bundle-{0}/{1}", i, imax), relativePath, (float)i / imax);
        //            }
        //        }
        //        EditorUtility.ClearProgressBar();
        //    }
        //}
        //private static void CreateDir(DirectoryInfo di, string parentDirName)
        //{
        //    DirectoryInfo[] subdirs = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
        //    if (subdirs != null)
        //    {
        //        for (int i = 0, imax = subdirs.Length; i < imax; ++i)
        //        {
        //            DirectoryInfo subdir = subdirs[i];
        //            string desDir = parentDirName + subdir.Name;
        //            if (!Directory.Exists(desDir))
        //            {
        //                Directory.CreateDirectory(desDir);
        //                //AssetDatabase.CreateFolder(parentDirName, subdir.Name);
        //                //AssetDatabase.Refresh();
        //            }
        //            CreateDir(subdir, parentDirName + "/" + subdir.Name + "/");
        //        }
        //    }
        //}

        //public static void CopyBundle()
        //{
        //    string srcABRoot = "";
        //    string desABRoot = "";
        //    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
        //    {
        //        srcABRoot = "Assets/AssetBundle/IOS/Animation/";
        //        desABRoot = "Assets/StreamingAssets/IOS/Animation/";
        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        //    {
        //        srcABRoot = "Assets/AssetBundle/Android/Animation/";
        //        desABRoot = "Assets/StreamingAssets/Android/Animation/";

        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        //    {
        //        srcABRoot = "Assets/AssetBundle/PC/Animation/";
        //        desABRoot = "Assets/StreamingAssets/PC/Animation/";
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    if (!Directory.Exists(desABRoot))
        //    {
        //        Directory.CreateDirectory(desABRoot);
        //    }
        //    AssetDatabase.Refresh();
        //    DirectoryInfo animDir = new DirectoryInfo(AssetModify.relativeRoot);
        //    CreateDir(animDir, desABRoot);
        //    AssetDatabase.Refresh();
        //    FileInfo[] files = animDir.GetFiles("*.anim", SearchOption.AllDirectories);
        //    if (files != null)
        //    {
        //        for (int i = 0, imax = files.Length; i < imax; ++i)
        //        {
        //            FileInfo file = files[i];
        //            string relativePath = file.FullName.Substring(animDir.FullName.Length);
        //            string relativeABPath = relativePath.Replace(".anim", ".bundle");
        //            string srcPath = srcABRoot + relativeABPath;
        //            string desPath = desABRoot + relativeABPath;
        //            if (File.Exists(srcPath))
        //            {
        //                try
        //                {
        //                    File.Copy(srcPath, desPath, true);
        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.Log(e.Message);
        //                }
        //            }
        //            else
        //            {
        //                UnityEngine.Object clip = AssetDatabase.LoadAssetAtPath(relativePath, typeof(AnimationClip));
        //                BuildPipeline.BuildAssetBundle(clip,
        //                             null,
        //                             desPath,
        //                             BuildAssetBundleOptions.UncompressedAssetBundle |
        //                             BuildAssetBundleOptions.CompleteAssets |
        //                             BuildAssetBundleOptions.DeterministicAssetBundle,
        //                             EditorUserBuildSettings.activeBuildTarget);
        //            }
        //            EditorUtility.DisplayProgressBar(string.Format("Copy Anim Bundle-{0}/{1}", i, imax), desPath, (float)i / imax);
        //        }
        //        //string staticAnimationDir = "Assets/Resources/StaticAnimation/Main_Camera";
        //        //DirectoryInfo staticCameraDir = new DirectoryInfo(staticAnimationDir);
        //        //for()
        //        AssetDatabase.SaveAssets();
        //        AssetDatabase.Refresh();
        //        EditorUtility.ClearProgressBar();
        //    }
        //}

        //public static void DeleteBundle()
        //{
        //    string desABRoot = "Assets/StreamingAssets/";
        //    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
        //    {
        //        desABRoot += "IOS/Animation";
        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        //    {
        //        desABRoot += "Android/Animation";
        //    }
        //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        //    {
        //        desABRoot += "PC/Animation";
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    AssetDatabase.DeleteAsset(desABRoot);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}
        #endregion animation
        #region Material            

        private static void _Cutoff2RMat(Material mat, string path)
        {
            if (mat.shader.name.Contains("CutoutDiffuse") ||
                mat.shader.name.Contains("TransparentDiffuse"))
            {
                Texture2D tex = mat.mainTexture as Texture2D;
                if (tex != null)
                {
                    Texture2D alphaTex = ConvertTexRtex(tex);
                    if (mat.shader.name.Contains("RimLight/CutoutDiffuse"))
                        mat.shader = Shader.Find("Custom/RimLight/CutoutDiffuseMaskR");
                    else if (mat.shader.name.Contains("CutoutDiffuse") && mat.shader.name.Contains("NoLight"))
                        mat.shader = Shader.Find("Custom/Common/CutoutDiffuseMaskRNoLight");
                    else if (mat.shader.name.Contains("CutoutDiffuse"))
                        mat.shader = Shader.Find("Custom/Common/CutoutDiffuseMaskR");
                    else if (mat.shader.name.Contains("Common/TransparentDiffuse") && mat.shader.name.Contains("NoLight"))
                        mat.shader = Shader.Find("Custom/Common/TransparentDiffuseMaskRNoLight");
                    else if (mat.shader.name.Contains("Common/TransparentDiffuse"))
                        mat.shader = Shader.Find("Custom/Common/TransparentDiffuseMaskR");
                    else if (mat.shader.name.Contains("Transparent/Cutout/Diffuse"))
                        mat.shader = Shader.Find("Custom/Scene/CutoutDiffuseMaskLM");
                    mat.SetTexture("_Mask", alphaTex);

                }
            }
        }

        [MenuItem(@"Assets/Tool/Material/Cutoff2RMat2 %2", false, 0)]
        private static void Cutoff2RMat2()
        {
            enumMat.cb = _Cutoff2RMat;
            EnumAsset<Material>(enumMat, "Cutoff2RMat");
        }

        [MenuItem(@"Assets/Tool/Material/Cutoff2RMat", false, 0)]
        private static void Cutoff2RMat()
        {
            scaleSize = 1;
            enumMat.cb = _Cutoff2RMat;
            EnumAsset<Material>(enumMat, "Cutoff2RMat");
            scaleSize = 2;
        }

        private static HashSet<string> matName = new HashSet<string>();
        private static Dictionary<string, List<string>> matTexName = new Dictionary<string, List<string>>();
        private static void _FindSameMat(Material mat, string path)
        {
            if (matName.Contains(mat.name))
            {
                Debug.Log(string.Format("Same Mat:{0}", mat.name));
            }
            else
            {
                matName.Add(mat.name);
            }
            Texture tex = mat.mainTexture;
            if (tex != null)
            {
                List<string> matList = null;
                if (matTexName.TryGetValue(tex.name, out matList))
                {
                    matList.Add(mat.name);
                }
                else
                {
                    matList = new List<string>();
                    matList.Add(mat.name);
                    matTexName.Add(tex.name, matList);
                }
            }
        }

        [MenuItem(@"Assets/Tool/Material/FindSameMat", false, 0)]
        private static void FindSameMat()
        {
            matName.Clear();
            matTexName.Clear();
            enumMat.cb = _FindSameMat;
            EnumAsset<Material>(enumMat, "FindSameMat");
            foreach (KeyValuePair<string, List<string>> kvp in matTexName)
            {
                if (kvp.Value.Count > 1)
                {
                    Debug.Log(string.Format("Tex:{0}----------------------", kvp.Key));
                    foreach (string mat in kvp.Value)
                    {
                        Debug.Log(string.Format("Mat:{0}", mat));
                    }
                }
            }
            matName.Clear();
            matTexName.Clear();
        }

        //private static void _FixMat(Material mat, string path)
        //{
        //    //if (mat.shader==null||mat.shader.name.Contains("Hidden/InternalErrorShader"))
        //    //{
        //    //    mat.shader = Shader.Find("Unlit/Texture");
        //    //}
        //}

        //[MenuItem(@"Assets/Tool/Material/FixMat", false, 0)]
        //private static void FixMat()
        //{
        //    enumMat.cb = _FixMat;
        //    EnumAsset<Material>(enumMat, "FixMat");
        //}



        private static List<ShaderValue> shaderValue = new List<ShaderValue>();
        private static void _ClearMat(Material mat, string path)
        {
            shaderValue.Clear();
            ShaderValue.GetShaderValue(mat, shaderValue);
            Material emptyMat = new Material(mat.shader);
            mat.CopyPropertiesFromMaterial(emptyMat);
            UnityEngine.Object.DestroyImmediate(emptyMat);
            for (int i = 0; i < shaderValue.Count; ++i)
            {
                ShaderValue sv = shaderValue[i];
                sv.SetValue(mat);
            }
            string name = mat.shader.name;
            if (name == "Custom/RimLight/Diffuse" ||
                name == "Custom/Common/Diffuse" ||
                name == "Custom/RimLight/CutoutDiffuseMaskR" ||
                name == "Custom/Common/CutoutDiffuseMaskR")
            {
                mat.SetColor("_Color", new Color(1, 1, 1, 1));
            }
            if (!mat.name.EndsWith("_RQ"))
                mat.renderQueue = -1;
        }


        [MenuItem(@"Assets/Tool/Material/ClearMat", false, 0)]
        private static void ClearMat()
        {
            enumMat.cb = _ClearMat;
            EnumAsset<Material>(enumMat, "ClearMat");
            AssetDatabase.SaveAssets();
        }
        [MenuItem(@"Assets/Tool/Material/FindUnusedShader", false, 0)]
        private static void FindUnusedShader()
        {
            DirectoryInfo di = new DirectoryInfo("Assets/Resources/Shader");
            FileInfo[] files = di.GetFiles("*.shader", SearchOption.AllDirectories);
            Dictionary<Shader, int> shaderUse = new Dictionary<Shader, int>();
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i].FullName;
                path = path.Replace("\\","/");
                int index = path.IndexOf("Assets/Resources/");
                path = path.Substring(index);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (!shaderUse.ContainsKey(shader))
                    shaderUse.Add(shader, 0);
            }
            di = new DirectoryInfo("Assets/");
            files = di.GetFiles("*.mat", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i].FullName;
                path = path.Replace("\\", "/");
                int index = path.IndexOf("Assets/");
                path = path.Substring(index);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null)
                {
                    int count = 0;
                    if (shaderUse.TryGetValue(mat.shader, out count))
                    {
                        count++;
                        shaderUse[mat.shader] = count;
                    }
                    else
                    {
                        Debug.LogError(string.Format("Not custum shader:{0}", mat.shader.name));
                    }
                }
            }
            var it = shaderUse.GetEnumerator();
            while (it.MoveNext())
            {
                if (it.Current.Value == 0)
                {
                    Debug.LogError(string.Format("No use shader:{0}", it.Current.Key.name));
                }
            }
        }

        private static void _FindMat(Material mat, string path)
        {
            if (mat.shader.name == "Mobile/Diffuse")
            {
                mat.shader = Shader.Find("Custom/Scene/DiffuseLM");
            }
        }

        [MenuItem(@"Assets/Tool/Material/FindMat", false, 0)]
        private static void FindMat()
        {
            enumMat.cb = _FindMat;
            EnumAsset<Material>(enumMat, "FindMat");
        }
        public static void GetMatTex(Material mat, List<Texture> lst)
        {
            if (mat != null)
            {
                Shader shader = mat.shader;
                int count = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < count; ++i)
                {
                    string name = ShaderUtil.GetPropertyName(shader, i);
                    ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
                    switch (type)
                    {
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            Texture tex = mat.GetTexture(name);
                            if (tex != null)
                                lst.Add(tex);
                            break;
                    }
                }
            }

        }

        #endregion Material
        #region scene

        private delegate bool EnumSceneCallback(UnityEngine.SceneManagement.Scene scene);
        private static void EnumAllScene(EnumSceneCallback cb, string title)
        {
            if (cb != null)
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < scenes.Length; ++i)
                {
                    EditorBuildSettingsScene scene = scenes[i];
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, scenes.Length), scene.path, (float)i / scenes.Length);
                    UnityEngine.SceneManagement.Scene s = EditorSceneManager.OpenScene(scene.path);

                    bool save = cb(s);
                    if (save)
                    {
                        EditorSceneManager.SaveScene(s);
                    }
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All scenes processed finish", "OK");
        }

        public static void Process45colliders()
        {
            GameObject colliders = GameObject.Find("45collider");
            if (colliders != null)
            {
                for (int i = 0, imax = colliders.transform.childCount; i < imax; ++i)
                {
                    Transform child = colliders.transform.GetChild(i);
                    XColliderModelLinker modleLink = child.GetComponent<XColliderModelLinker>();
                    if (modleLink != null && modleLink.Models.Count > 0)
                    {
                        XColliderRenderLinker renderLink = child.gameObject.GetComponent<XColliderRenderLinker>();
                        if (renderLink == null)
                        {
                            renderLink = child.gameObject.AddComponent<XColliderRenderLinker>();
                        }
                        if (renderLink != null)
                        {
                            List<Renderer> tmp = new List<Renderer>();
                            for (int j = 0, jmax = modleLink.Models.Count; j < jmax; ++j)
                            {
                                Renderer r = modleLink.Models[j].GetComponent<Renderer>();
                                if (r != null && !tmp.Contains(r))
                                {
                                    tmp.Add(r);
                                }
                            }
                            if (tmp.Count > 0)
                            {
                                renderLink.renders = tmp.ToArray();
                            }
                            else
                                renderLink.renders = null;
                        }
                        UnityEngine.Object.DestroyImmediate(modleLink);
                    }
                }
            }
        }


        private static HashSet<Material> processedMat = new HashSet<Material>();
        private static void _PrcessSceneMat(Material mat, bool recover)
        {
            if (!processedMat.Contains(mat) && mat.shader != null)
            {
                if (mat.shader.name.Contains("Transparent/Cutout/Diffuse") ||
                    mat.shader.name.Contains("CutoutDiffuseMaskLM_VMove") ||
                    mat.shader.name == "Custom/Scene/CutoutDiffuseMaskLM")
                {
                    Texture2D tex = mat.mainTexture as Texture2D;
                    if (tex != null)
                    {
                        if (recover)
                        {
                            string texPath = AssetDatabase.GetAssetPath(tex);
                            int size = tex.width > tex.height ? tex.width : tex.height;
                            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;
                            texImporter.textureType = TextureImporterType.Default;
                            texImporter.anisoLevel = 0;
                            texImporter.mipmapEnabled = tex.width > 256 && tex.height > 256;
                            texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                            texImporter.wrapMode = TextureWrapMode.Repeat;
                            texImporter.filterMode = FilterMode.Bilinear;
                            //SetTexImportSetting(texImporter, BuildTarget.Android.ToString(), size, TextureImporterFormat.RGBA16);
                            //SetTexImportSetting(texImporter, "iPhone", size, TextureImporterFormat.RGBA16);
                            SetTexImportSetting(texImporter, "Standalone", size, TextureImporterFormat.RGBA32);
                            texImporter.isReadable = false;
                            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
                            mat.shader = Shader.Find("Legacy Shaders/Transparent/Cutout/Diffuse");
                        }
                        else
                        {
                            Texture2D alphaTex = ConvertTexRtex(tex);

                            mat.shader = Shader.Find("Custom/Scene/CutoutDiffuseMaskLM");
                            mat.SetTexture("_Mask", alphaTex);
                        }

                    }
                    processedMat.Add(mat);
                }
            }

        }

        private static void _HalfSceneMatTex(Material mat, string path)
        {
            if (!processedMat.Contains(mat) && mat.shader != null)
            {
                processedMat.Add(mat);
                Texture2D tex = mat.mainTexture as Texture2D;
                string texturePath = AssetDatabase.GetAssetPath(tex);
                texturePath = texturePath.Replace("Half", "");
                Texture2D fullTex = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                int index = texturePath.LastIndexOf(".");
                if (index > 0)
                {
                    string ext = texturePath.Substring(index);
                    string halfTexturePath = texturePath.Substring(0, index) + "Half" + ext;
                    Texture2D halfTex = AssetDatabase.LoadAssetAtPath(halfTexturePath, typeof(Texture2D)) as Texture2D;
                    if (halfTex == null)
                        halfTex = MakeHalfTexture(halfTexturePath, fullTex, true);
                    mat.mainTexture = halfTex;
                }
                else
                {
                    return;
                }
            }
        }

        [MenuItem(@"Assets/Tool/Scene/HalfSceneMatTex", false, 0)]
        private static void HalfSceneMatTex()
        {
            processedMat.Clear();
            enumMat.cb = _HalfSceneMatTex;
            EnumAsset<Material>(enumMat, "HalfMatTex");
            processedMat.Clear();
        }

        private static void _PrcessSceneMat(Material mat, string path)
        {
            bool cutout = mat.HasProperty("_Cutoff");
            bool isTransparent = mat.renderQueue >= 3000;
            if (cutout || isTransparent)
            {
                Texture2D tex = mat.mainTexture as Texture2D;
                if (tex != null)
                {
                    Texture2D alphaTex = ConvertTexRtex(tex);
                    if (cutout && mat.shader != Shader.Find("Custom/Scene/CutoutDiffuseMaskLM_VMove"))
                        mat.shader = Shader.Find("Custom/Scene/CutoutDiffuseMaskLM");
                    mat.SetTexture("_Mask", alphaTex);
                }
            }
        }

        [MenuItem(@"Assets/Tool/Scene/PrcessSceneMat", false, 0)]
        public static void PrcessSceneMat()
        {
            enumMat.cb = _PrcessSceneMat;
            EnumAsset<Material>(enumMat, "PrcessSceneMat");
            AssetDatabase.SaveAssets();
        }

        private static void ProcessSceneObject(Transform t, bool bake)
        {
            int flag = (int)GameObjectUtility.GetStaticEditorFlags(t.gameObject);
            if ((flag & (int)StaticEditorFlags.ContributeGI) != 0)
            {
                Renderer render = t.GetComponent<Renderer>();
                if (render != null)
                {
                    if (render is SkinnedMeshRenderer)
                    {
                        SkinnedMeshRenderer smr = render as SkinnedMeshRenderer;
                        smr.skinnedMotionVectors = false;
                        smr.updateWhenOffscreen = false;
                    }
                    else
                    {
                        render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                        render.shadowCastingMode = bake ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                        render.receiveShadows = bake;
                        render.reflectionProbeUsage = bake ? UnityEngine.Rendering.ReflectionProbeUsage.BlendProbesAndSkybox : UnityEngine.Rendering.ReflectionProbeUsage.Off;
                        render.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    }
                    MeshFilter mf = t.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        Mesh mesh = mf.sharedMesh;
                        string assetPath = AssetDatabase.GetAssetPath(mesh).ToLower();
                        if (bake)
                        {
                            if (assetPath.EndsWith(".asset"))
                            {
                                bool find = false;
                                int index = assetPath.LastIndexOf("_");
                                string fbxPath = assetPath.Substring(0, index) + ".fbx";
                                string indexStr = assetPath.Substring(index + 1).Replace(".asset", "");
                                int meshIndex = -1;
                                int.TryParse(indexStr, out meshIndex);
                                if (meshIndex >= 0)
                                {
                                    GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
                                    if (fbx != null)
                                    {
                                        GameObject fbxObj = GameObject.Instantiate<GameObject>(fbx);
                                        List<Renderer> renderLst = ListPool<Renderer>.Get();
                                        fbxObj.GetComponentsInChildren<Renderer>(true, renderLst);
                                        if (meshIndex < renderLst.Count)
                                        {
                                            Renderer r = renderLst[meshIndex];
                                            if (r is MeshRenderer)
                                            {
                                                MeshFilter fbxMf = r.GetComponent<MeshFilter>();
                                                if (fbxMf != null && fbxMf.sharedMesh != null)
                                                {
                                                    mf.sharedMesh = fbxMf.sharedMesh;
                                                    find = true;
                                                }
                                            }
                                        }
                                        ListPool<Renderer>.Release(renderLst);
                                        GameObject.DestroyImmediate(fbxObj);
                                    }
                                }

                                if (!find)
                                {
                                    Debug.LogError("fbx mesh not found:" + t.name);
                                }
                            }
                        }
                        else if (assetPath.EndsWith(".fbx"))
                        {

                            Mesh newMesh = null;
                            int result = FindMesh(mesh, false, out newMesh);
                            if (result == 1)
                            {
                                mf.sharedMesh = newMesh;
                            }
                            else if (result == -1)
                            {
                                Debug.LogError("mesh not found:" + t.name);
                            }
                        }
                    }
                }
            }
            for (int i = t.childCount - 1; i >= 0; --i)
            {
                Transform child = t.GetChild(i);
                ProcessSceneObject(child, bake);
            }
        }

        //private static HashSet<Mesh> processedMesh = new HashSet<Mesh>();
        //private static void InnerProcessMat(Transform t,bool bake,bool processMesh, bool processMat)
        //{
        //    InnerProcessSceneObject(t, bake);
        //    MeshRenderer meshRender = t.GetComponent<MeshRenderer>();
        //    if (meshRender != null && meshRender.enabled)
        //    {
        //        if(processMat)
        //        {
        //            for (int i = 0, imax = meshRender.sharedMaterials.Length; i < imax; ++i)
        //            {
        //                Material mat = meshRender.sharedMaterials[i];
        //                if (mat != null)
        //                    _PrcessSceneMat(mat, bake);
        //                else
        //                {
        //                    Debug.LogError("Null mat:" + meshRender.name);
        //                }
        //            }
        //        }               
        //    }
        //    if(processMesh)
        //    {
        //        MeshFilter mf = t.GetComponent<MeshFilter>();
        //        if (mf != null && mf.sharedMesh != null)
        //        {
        //            Mesh m = mf.sharedMesh;
        //            if (!processedMesh.Contains(m))
        //            {
        //                string modelPath = AssetDatabase.GetAssetPath(m);
        //                ModelImporter modelImporter = AssetImporter.GetAtPath(modelPath) as ModelImporter;
        //                if (modelImporter != null)
        //                {
        //                    AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate);
        //                }
        //            }
        //        }
        //    }

        //    for (int i = t.childCount - 1; i >= 0; --i)
        //    {
        //        Transform child = t.GetChild(i);
        //        InnerProcessMat(child, bake, processMesh, processMat);
        //    }
        //}
        //private static void _MaterialInFolder2Original(Material mat, string path)
        //{
        //    _PrcessSceneMat(mat, true);
        //}
        //private static bool _MeshInFolderImport(GameObject fbx, ModelImporter modelImporter, string path)
        //{
        //    if (modelImporter == null)
        //        return false;
        //    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive);
        //    return false;
        //}
        //[MenuItem(@"Assets/Tool/Scene/恢复目录中的材质到烘焙状态")]
        //private static void MeshMaterialInFolder2Original()
        //{
        //    bAccordingSettings = false;
        //    enumFbx.cb = _MeshInFolderImport;
        //    EnumAsset<GameObject>(enumFbx, "MeshInFolder2Original");
        //    bAccordingSettings = true;

        //    processedMat.Clear();
        //    enumMat.cb = _MaterialInFolder2Original;
        //    EnumAsset<Material>(enumMat, "MaterialInFolder2Original");
        //    processedMat.Clear();
        //}

        //private static void _MaterialInFolder2Compress(Material mat, string path)
        //{
        //    _PrcessSceneMat(mat, false);
        //}

        //[MenuItem(@"Assets/Tool/Scene/转换目录中的材质转到运行状态")]
        //private static void MaterialInFolder2Compress()
        //{
        //    bAccordingSettings = true;
        //    enumFbx.cb = _MeshInFolderImport;
        //    EnumAsset<GameObject>(enumFbx, "MeshInFolder2Compress");

        //    processedMat.Clear();
        //    enumMat.cb = _MaterialInFolder2Compress;
        //    EnumAsset<Material>(enumMat, "MaterialInFolder2Compress");
        //    processedMat.Clear();
        //}

        [MenuItem(@"GameObject/Scene/准备烘焙", false, 0)]
        public static void PrepareBake()
        {
            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            GameObject[] roots = s.GetRootGameObjects();
            for (int i = 0, imax = roots.Length; i < imax; ++i)
            {
                Transform t = roots[i].transform;
                ProcessSceneObject(t, true);
            }
            UnityEngine.Object[] terrains = UnityEngine.Object.FindObjectsOfType(typeof(Terrain));
            if (terrains != null)
            {
                for (int j = 0, imax = terrains.Length; j < imax; ++j)
                {
                    Terrain terrain = terrains[j] as Terrain;
                    terrain.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
                    terrain.Flush();
                }
            }
            EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        }
        [MenuItem(@"GameObject/Scene/结束烘焙", false, 0)]
        public static void EndBake()
        {
            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            GameObject[] roots = s.GetRootGameObjects();
            for (int i = 0, imax = roots.Length; i < imax; ++i)
            {
                Transform t = roots[i].transform;
                ProcessSceneObject(t, false);
            }
            FixCurrentScene();
            EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        }

        //[MenuItem(@"Assets/Tool/Scene/恢复场景中选中的fbx和材质到烘焙状态")]
        //private static void SelectMeshMaterial2Original()
        //{
        //    processedMat.Clear();
        //    processedMesh.Clear();
        //    bAccordingSettings = false;
        //    UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        //    GameObject[] roots = s.GetRootGameObjects();
        //    for (int i = 0, imax = roots.Length; i < imax; ++i)
        //    {
        //        Transform t = roots[i].transform;
        //        InnerProcessMat(t, true, true, true);
        //    }
        //    processedMat.Clear();
        //    processedMesh.Clear();
        //    bAccordingSettings = true;
        //    EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        //}
        //[MenuItem(@"Assets/Tool/Scene/转换场景中选中的fbx和材质转到运行状态")]
        //private static void SelectMeshMaterial2Compress()
        //{
        //    processedMat.Clear();
        //    processedMesh.Clear();
        //    bAccordingSettings = true;
        //    UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        //    GameObject[] roots = s.GetRootGameObjects();
        //    for (int i = 0, imax = roots.Length; i < imax; ++i)
        //    {
        //        Transform t = roots[i].transform;
        //        InnerProcessMat(t, false, true, true);
        //    }
        //    processedMat.Clear();
        //    processedMesh.Clear();
        //    bAccordingSettings = true;
        //    EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        //}

        private static bool InnerProcessSceneObject(Transform t)
        {
            bool change = false;
            Renderer render = t.GetComponent<Renderer>();
            if (render != null && t.gameObject.layer == 9 && render.sharedMaterial.shader.name == "Custom/Scene/DiffuseLM")
            {
                render.sharedMaterial.shader = Shader.Find("Custom/Scene/DiffuseTerrainLM");
            }
            //Animator animator = t.GetComponent<Animator>();
            //if (animator != null && render != null)
            //{
            //    errorLog += t.name + ":无用animator\r\n";
            //    if (needFix)
            //        UnityEngine.Object.DestroyImmediate(animator);
            //}
            //bool hasMesh = false;
            //Mesh mesh = null;
            //MeshFilter mf = null;
            //if (render != null)
            //{
            //    PrefabType prefabType = PrefabUtility.GetPrefabType(render.gameObject);
            //    if (prefabType == PrefabType.MissingPrefabInstance)
            //    {
            //        PrefabUtility.DisconnectPrefabInstance(render.gameObject);
            //    }                
            //    {

            //        SkinnedMeshRenderer smr = null;
            //        ParticleSystemRenderer psr = null;
            //        change = render.lightProbeUsage != UnityEngine.Rendering.LightProbeUsage.Off ||
            //           render.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off ||
            //           render.receiveShadows ||
            //           render.reflectionProbeUsage != UnityEngine.Rendering.ReflectionProbeUsage.Off ||
            //           render.motionVectorGenerationMode != MotionVectorGenerationMode.ForceNoMotion;
            //        if (render is SkinnedMeshRenderer)
            //        {
            //            smr = render as SkinnedMeshRenderer;
            //            change |= smr.skinnedMotionVectors ||
            //            smr.updateWhenOffscreen;
            //            mesh = smr.sharedMesh;
            //            hasMesh = true;
            //        }
            //        else if (render is MeshRenderer)
            //        {
            //            mf = render.GetComponent<MeshFilter>();
            //            if (mf != null)
            //                mesh = mf.sharedMesh;
            //            hasMesh = true;
            //        }
            //        else if (render is ParticleSystemRenderer)
            //        {
            //            psr = render as ParticleSystemRenderer;
            //            if (psr.renderMode == ParticleSystemRenderMode.Mesh)
            //            {
            //                mesh = psr.mesh;
            //                hasMesh = true;
            //            }
            //            else if (psr.mesh != null)
            //            {
            //                errorLog += t.name + ":Particle未优化\r\n";
            //                if (needFix)
            //                {
            //                    psr.mesh = null;
            //                    change = true;
            //                }
            //            }
            //        }
            //        if (change)
            //        {
            //            errorLog += t.name + ":render未优化\r\n";
            //            if (needFix)
            //            {
            //                render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            //                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //                render.receiveShadows = false;
            //                render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            //                render.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            //                if (smr != null)
            //                {
            //                    smr.skinnedMotionVectors = false;
            //                    smr.updateWhenOffscreen = false;
            //                }
            //            }
            //        }
            //        if (mesh != null)
            //        {
            //            Mesh newMesh = null;
            //            int result = FindMesh(mesh, false, out newMesh);
            //            if (result == 1)
            //            {
            //                errorLog += t.name + ":mesh未优化\r\n";
            //                if (needFix)
            //                {
            //                    if (smr != null)
            //                    {
            //                        smr.sharedMesh = newMesh;
            //                    }
            //                    else if (mf != null)
            //                    {
            //                        mf.sharedMesh = newMesh;
            //                    }
            //                    else if (psr != null)
            //                    {
            //                        psr.mesh = newMesh;
            //                    }
            //                    change = true;
            //                }
            //            }
            //            else if (result == -1)
            //            {
            //                errorLog += t.name + ":mesh没有找到mesh\r\n";
            //            }
            //        }
            //        else if (hasMesh)
            //        {
            //            errorLog += t.name + ":mesh为空\r\n";
            //            if (needFix)
            //            {
            //                if (smr != null)
            //                {
            //                    GameObject.DestroyImmediate(smr);
            //                }
            //                else if (mf != null)
            //                {
            //                    GameObject.DestroyImmediate(mf);
            //                    GameObject.DestroyImmediate(render);
            //                }
            //                else if (psr != null)
            //                {
            //                    GameObject.DestroyImmediate(psr);
            //                }
            //                change = true;
            //            }
            //        }
            //    }
            //}

            //MeshCollider mc = t.GetComponent<MeshCollider>();
            //if (mc != null)
            //{
            //    if (hasMesh && mesh == null)
            //    {
            //        if (needFix)
            //            GameObject.DestroyImmediate(mc);
            //    }
            //    else if (render != null && !render.enabled)
            //    {
            //        if (needFix)
            //        {
            //            GameObject.DestroyImmediate(render);
            //            if(mf!=null)
            //            {
            //                GameObject.DestroyImmediate(mf);
            //            }
            //        }

            //    }
            //    else
            //    {
            //        mesh = mc.sharedMesh;
            //        if (mesh != null)
            //        {
            //            Mesh newMesh = null;
            //            int result = FindMesh(mesh, false, out newMesh);
            //            if (result == 1)
            //            {
            //                errorLog += t.name + ":meshcollider未优化\r\n";
            //                if (needFix)
            //                {
            //                    mc.sharedMesh = newMesh;
            //                    change = true;
            //                }
            //            }
            //            else if (result == -1)
            //            {
            //                errorLog += t.name + ":meshcollider没有找到mesh\r\n";
            //            }
            //        }
            //    }

            //}
            for (int i = t.childCount - 1; i >= 0; --i)
            {
                Transform child = t.GetChild(i);
                change |= InnerProcessSceneObject(child);
            }
            return change;
        }


        private static bool _FixScene(UnityEngine.SceneManagement.Scene scene)
        {
            bool change = false;
            errorLog = "";
            //GameObject camera = GameObject.Find("Main Camera");
            //if (camera != null)
            //{
            //    FxPro fxPro = camera.GetComponent<FxPro>();
            //    if (fxPro != null && fxPro.enabled)
            //    {
            //        errorLog += "fxPro未优化\r\n";
            //        if (needFix)
            //            fxPro.enabled = false;
            //        change = true;
            //    }
            //    FMOD_Listener fmod_Listener = camera.GetComponent<FMOD_Listener>();
            //    if (fmod_Listener != null)
            //    {
            //        errorLog += "FMOD_Listener未优化\r\n";
            //        if (needFix)
            //            UnityEngine.Object.DestroyImmediate(fmod_Listener);
            //        change = true;
            //    }
            //}


            //UnityEngine.Object[] terrains = UnityEngine.Object.FindObjectsOfType(typeof(Terrain));
            //if (terrains != null)
            //{
            //    GameObject lightGo = GameObject.Find("MainLight");
            //    if(lightGo != null)
            //    {
            //        Light light = lightGo.GetComponent<Light>();
            //        if(light!=null)
            //        {
            //            light.shadows = LightShadows.None;
            //            lightGo.layer = 9;
            //        }
            //    }
            //    for (int j = 0, imax = terrains.Length; j < imax; ++j)
            //    {
            //        Terrain terrain = terrains[j] as Terrain;
            //        if (needFix)
            //        {
            //            terrain.materialType = Terrain.MaterialType.Custom;
            //            string terrainMat = "Assets/XScene/TerrainTextures/Custom_Terrain_Diffuse.mat";
            //            if(terrain.terrainData.alphamapLayers > 4)
            //                terrainMat = "Assets/XScene/TerrainTextures/Custom_Terrain_Diffuse8.mat";
            //            terrain.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>(terrainMat);
            //            terrain.bakeLightProbesForTrees = false;
            //            terrain.collectDetailPatches = false;
            //            if (terrain.basemapDistance < 1000)
            //            {
            //                terrain.basemapDistance = 1000;
            //                change = true;
            //            }
            //            else if (terrain.basemapDistance > 1000)
            //            {
            //                terrain.basemapDistance = 1000;
            //                change = true;
            //            }
            //            else
            //            {
            //                terrain.basemapDistance = 1001;
            //                change = true;
            //            }
            //            if (terrain.terrainData.baseMapResolution > 16)
            //            {
            //                terrain.terrainData.baseMapResolution = 16;
            //                terrain.Flush();
            //                change = true;
            //            }
            //        }
            //    }
            //}
            GameObject[] roots = scene.GetRootGameObjects();

            for (int j = 0, imax = roots.Length; j < imax; ++j)
            {
                Transform t = roots[j].transform;
                change |= InnerProcessSceneObject(t);
            }
            if (errorLog != "")
            {
                Debug.LogError(errorLog + ":" + scene.name);
            }
            return change && needFix;
        }

        [MenuItem(@"Assets/Tool/Scene/FixScene", false, 0)]
        private static void FixScene()
        {
            needFix = true;
            EnumAllScene(_FixScene, "FixScene");
            needFix = true;
        }

        [MenuItem(@"Assets/Tool/Scene/ScanScene", false, 0)]
        private static void ScanScene()
        {
            needFix = false;
            EnumAllScene(_FixScene, "ScanScene");
            needFix = true;
        }

        [MenuItem(@"GameObject/Scene/FixCurrentScene", false, 0)]
        private static void FixCurrentScene()
        {
            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            bool change = _FixScene(s);
            if (change)
                EditorSceneManager.SaveScene(s);
        }
        private static void _InnerFindSceneMat(Transform t, ref int cutOutCount, ref int totalCount)
        {
            Renderer render = t.GetComponent<Renderer>();
            if (render != null)
            {
                if(!(render is ParticleSystemRenderer))
                {
                    Material[] mats = render.sharedMaterials;
                    if (mats != null)
                    {
                        for (int i = 0; i < mats.Length; ++i)
                        {
                            Material mat = mats[i];
                            if (mat != null)
                            {
                                if (mat.renderQueue > 2000)
                                    cutOutCount++;
                            }
                            totalCount++;
                        }
                    }
                }
                
            }
            for (int i = t.childCount - 1; i >= 0; --i)
            {
                Transform child = t.GetChild(i);
                _InnerFindSceneMat(child,ref cutOutCount, ref totalCount);
            }
        }
        private static bool _FindSceneMat(UnityEngine.SceneManagement.Scene scene)
        {
            GameObject sceneObj = GameObject.Find("Scene");
            if(sceneObj!=null)
            {
                int totalCount = 0;
                int cutOutCount = 0;
                _InnerFindSceneMat(sceneObj.transform, ref cutOutCount, ref totalCount);
                string str = string.Format("{0}:{1}/{2}({3:P})\r\n", scene.name, cutOutCount, totalCount, (float)cutOutCount / totalCount);
                errorLog += str;
                Debug.LogError(str);
            }
            return false;
        }
        [MenuItem(@"GameObject/Scene/FindSceneMat", false, 0)]
        private static void FindSceneMat()
        {
            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            _FindSceneMat(s);

        }
        [MenuItem(@"Assets/Tool/Scene/ScanSceneMat", false, 0)]
        private static void ScanSceneMat()
        {
            errorLog = "";
            EnumAllScene(_FindSceneMat, "ScanSceneMat");
            File.WriteAllText("Assets/XScene/CutoutMat.txt",errorLog);
        }

        //class MeshCombineInfo
        //{
        //    public Mesh mesh;
        //    public Matrix4x4 matrix;
        //    public MeshFilter mf;
        //}
        //private static void _CollectMatMesh(Transform child, Dictionary<Material, List<MeshCombineInfo>> matMesh)
        //{
        //    int flag = (int)GameObjectUtility.GetStaticEditorFlags(child.gameObject);
        //    if ((flag & (int)StaticEditorFlags.BatchingStatic) != 0)
        //    {
        //        MeshRenderer mr = child.GetComponent<MeshRenderer>();
        //        if (mr != null)
        //        {
        //            Material mat = mr.sharedMaterial;
        //            if (mat != null)
        //            {
        //                MeshFilter mf = child.GetComponent<MeshFilter>();
        //                if (mf != null)
        //                {
        //                    Mesh mesh = mf.sharedMesh;
        //                    if (mesh != null && mesh.isReadable)
        //                    {
        //                        List<MeshCombineInfo> meshList = null;
        //                        if (!matMesh.TryGetValue(mat, out meshList))
        //                        {
        //                            meshList = new List<MeshCombineInfo>();
        //                            matMesh.Add(mat, meshList);
        //                        }
        //                        MeshCombineInfo mci = new MeshCombineInfo();
        //                        mci.mesh = mesh;
        //                        mci.matrix = child.localToWorldMatrix;
        //                        mci.mf = mf;
        //                        meshList.Add(mci);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    for (int i = 0; i < child.childCount; ++i)
        //    {
        //        Transform grandChild = child.GetChild(i);
        //        _CollectMatMesh(grandChild, matMesh);
        //    }
        //}
        //class MeshCombineInfo
        //{
        //    public Mesh mesh;
        //    public MeshFilter mf;
        //}
        //private static void _CollectMatMesh(Transform child, Dictionary<Mesh, List<MeshFilter>> meshs)
        //{
        //    int flag = (int)GameObjectUtility.GetStaticEditorFlags(child.gameObject);
        //    if ((flag & (int)StaticEditorFlags.BatchingStatic) != 0)
        //    {
        //        MeshRenderer mr = child.GetComponent<MeshRenderer>();
        //        if (mr != null)
        //        {
        //            Material mat = mr.sharedMaterial;
        //            if (mat != null)
        //            {
        //                MeshFilter mf = child.GetComponent<MeshFilter>();
        //                if (mf != null)
        //                {
        //                    Mesh mesh = mf.sharedMesh;
        //                    if (mesh != null && mesh.name.StartsWith("Combined Mesh"))
        //                    {
        //                        List<MeshFilter> meshList = null;
        //                        if (!meshs.TryGetValue(mesh, out meshList))
        //                        {
        //                            meshList = new List<MeshFilter>();
        //                            meshs.Add(mesh, meshList);
        //                        }
        //                        meshList.Add(mf);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    for (int i = 0; i < child.childCount; ++i)
        //    {
        //        Transform grandChild = child.GetChild(i);
        //        _CollectMatMesh(grandChild, meshs);
        //    }
        //}

        //[MenuItem(@"GameObject/Scene/BakeSceneMesh", false, 0)]
        //private static void BakeSceneMesh()
        //{
        //    //Dictionary<Material, List<MeshCombineInfo>> matMesh = new Dictionary<Material, List<MeshCombineInfo>>();
        //    Dictionary<Mesh, List<MeshFilter>> meshs = new Dictionary<Mesh, List<MeshFilter>>();
        //    UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        //    GameObject go = GameObject.Find("Scene");
        //    if(go!=null)
        //    {
        //        Transform t = go.transform;
        //        for(int i=0;i<t.childCount;++i)
        //        {
        //            Transform child = t.GetChild(i);
        //            _CollectMatMesh(child, meshs);
        //        }
        //    }
        //    int index = s.path.LastIndexOf("/");
        //    string sceneDir = s.path.Substring(0, index);
        //    var it = meshs.GetEnumerator();
        //    int meshIndex = 0;
        //    while (it.MoveNext())
        //    {
        //        Mesh mesh = it.Current.Key;
        //        string Path = string.Format("{0}/BakedMesh_{1}.asset", sceneDir, meshIndex++);
        //        CreateOrReplaceAsset<Mesh>(mesh, Path);
        //        List<MeshFilter> meshList = it.Current.Value;
        //        for (int i = 0; i < meshList.Count; ++i)
        //        {
        //            MeshFilter mf = meshList[i];
        //            mf.sharedMesh = mesh;
        //        }
        //    }
        //    //List<CombineInstance> ciList = new List<CombineInstance>();
        //    //var it = matMesh.GetEnumerator();
        //    //while(it.MoveNext())
        //    //{
        //    //    List<MeshCombineInfo> meshList = it.Current.Value;
        //    //    if(meshList.Count>10)
        //    //    {
        //    //        ciList.Clear();
        //    //        int vertexCount = 0;
        //    //        int combineIndex = 0;
        //    //        for (int i = 0; i < meshList.Count; ++i)
        //    //        {
        //    //            MeshCombineInfo mci = meshList[i];
        //    //            if (vertexCount + mci.mesh.vertexCount > 60000)
        //    //            {
        //    //                Mesh newMesh = new Mesh();
        //    //                newMesh.CombineMeshes(ciList.ToArray(), true, true);
        //    //                string path = string.Format("{0}/{1}_{2}.asset", sceneDir, it.Current.Key.name, combineIndex++);
        //    //                CreateOrReplaceAsset<Mesh>(newMesh, path);
        //    //                ciList.Clear();
        //    //            }
        //    //            CombineInstance ci = new CombineInstance();
        //    //            ci.mesh = mci.mesh;
        //    //            //ci.transform = mci.matrix;
        //    //            ciList.Add(ci);
        //    //        }
        //    //        Mesh lastMesh = new Mesh();
        //    //        lastMesh.CombineMeshes(ciList.ToArray(), true, false);
        //    //        string lastPath = string.Format("{0}/{1}.asset", sceneDir, it.Current.Key.name, combineIndex);
        //    //        CreateOrReplaceAsset<Mesh>(lastMesh, lastPath);
        //    //        for (int i = 0; i < meshList.Count; ++i)
        //    //        {
        //    //            MeshCombineInfo mci = meshList[i];
        //    //            mci.mf.sharedMesh = lastMesh;
        //    //        }
        //    //    }

        //    //}
        //}

        //private static bool ReplaceSceneMesh(Transform t,string sceneName)
        //{
        //    if (!t.gameObject.activeSelf)
        //        return false;
        //    Renderer render = t.GetComponent<Renderer>();
        //    bool hasMesh = false;
        //    Mesh mesh = null;
        //    if (render != null)
        //    {
        //        MeshFilter mf = null;
        //        SkinnedMeshRenderer smr = null;
        //        if (render is SkinnedMeshRenderer)
        //        {
        //            smr = render as SkinnedMeshRenderer;
        //            hasMesh = true;
        //        }
        //        else if (render is MeshRenderer)
        //        {
        //            mf = render.GetComponent<MeshFilter>();
        //            if (mf != null)
        //                mesh = mf.sharedMesh;
        //            hasMesh = true;
        //        }
        //        if (mesh != null)
        //        {
        //            Mesh newMesh = null;
        //            int result = FindMesh(mesh, false, out newMesh);
        //            if (newMesh != null)
        //            {
        //                string path = AssetDatabase.GetAssetPath(newMesh);
        //                path = path.Replace("XScene", "XSceneIOS");
        //                newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        //            }
        //            if (newMesh == null)
        //            {
        //                Debug.LogError(string.Format("Mesh not found:{0} scene:{1}", t.name, sceneName));
        //            }
        //            if (smr != null)
        //            {
        //                smr.sharedMesh = newMesh;
        //            }
        //            else if (mf != null)
        //            {
        //                mf.sharedMesh = newMesh;
        //            }
        //        }
        //    }

        //    MeshCollider mc = t.GetComponent<MeshCollider>();
        //    if (mc != null)
        //    {
        //        if (hasMesh && mesh == null)
        //        {
        //        }
        //        else
        //        {
        //            mesh = mc.sharedMesh;
        //            if (mesh != null)
        //            {
        //                Mesh newMesh = null;
        //                int result = FindMesh(mesh, false, out newMesh);
        //                if (newMesh != null)
        //                {
        //                    string path = AssetDatabase.GetAssetPath(newMesh);
        //                    path = path.Replace("XScene", "XSceneIOS");
        //                    newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        //                }
        //                if (newMesh == null)
        //                {
        //                    Debug.LogError(string.Format("Collider Mesh not found:{0} scene:{1}", t.name, sceneName));
        //                }
        //                mc.sharedMesh = newMesh;
        //            }
        //        }

        //    }
        //    List<GameObject> notVisibleNode = new List<GameObject>();
        //    for (int i = t.childCount - 1; i >= 0; --i)
        //    {
        //        Transform child = t.GetChild(i);
        //        bool isVisible = ReplaceSceneMesh(child, sceneName);
        //        if(!isVisible)
        //        {
        //            notVisibleNode.Add(child.gameObject);
        //        }
        //    }
        //    for (int i = 0; i < notVisibleNode.Count; ++i)
        //    {
        //        GameObject.DestroyImmediate(notVisibleNode[i]);
        //    }
        //    return true;
        //}
        //private static bool _GenIOSSene(UnityEngine.SceneManagement.Scene scene)
        //{            
        //    GameObject[] roots = scene.GetRootGameObjects();

        //    for (int j = 0, imax = roots.Length; j < imax; ++j)
        //    {
        //        Transform t = roots[j].transform;
        //        if(t.name=="Scene")
        //        {
        //            ReplaceSceneMesh(t, scene.name);
        //        }
        //    }
        //    return true;
        //}
        //private static void CreateDir(string path)
        //{
        //    if (Directory.Exists(path))
        //        return;
        //    int index = path.LastIndexOf("/");
        //    string newDirPath = path.Substring(0, index);
        //    CreateDir(newDirPath);
        //    Directory.CreateDirectory(path);

        //}
        //private static void _CopyMesh(Mesh mesh, string path)
        //{
        //    //string newPath = path.Replace("xscene", "XSceneIOS");
        //    //int index = newPath.LastIndexOf("/");
        //    //string newDirPath = newPath.Substring(0, index);
        //    //CreateDir(newDirPath);
        //    string data = File.ReadAllText(path);
        //    data = data.Replace("m_IsReadable: 1", "m_IsReadable: 0");
        //    File.WriteAllText(path, data);

        //}

        //[MenuItem(@"Assets/Tool/Scene/GenIOSSene", false, 0)]
        //private static void GenIOSSene()
        //{
        //    enumMesh.cb = _CopyMesh;
        //    EnumAsset<Mesh>(enumMesh, "CopyMesh", "Assets/XScene");

        //    //EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        //    //for (int i = 0; i < scenes.Length; ++i)
        //    //{
        //    //    EditorBuildSettingsScene scene = scenes[i];
        //    //    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "GenIOSSene", i, scenes.Length), scene.path, (float)i / scenes.Length);
        //    //    UnityEngine.SceneManagement.Scene s = EditorSceneManager.OpenScene(scene.path);

        //    //    _GenIOSSene(s);                
        //    //    EditorSceneManager.SaveScene(s, "Assets/XSceneIOS/" + s.name + ".unity");
        //    //}
        //    //AssetDatabase.Refresh();
        //    //AssetDatabase.SaveAssets();
        //    //EditorUtility.ClearProgressBar();
        //    //EditorUtility.DisplayDialog("Finish", "All scenes processed finish", "OK");
        //}

        //public static void RemoveLightmapBakeThing(string scenePath)
        //{
        //    if (EditorApplication.OpenScene(scenePath))
        //    {
        //        GameObject scene = GameObject.Find(@"Scene");
        //        if (scene != null)
        //        {
        //            List<GameObject> lst = new List<GameObject>();
        //            for (int i = 0, imax = scene.transform.childCount; i < imax; ++i)
        //            {
        //                Transform t = scene.transform.GetChild(i);
        //                if (!t.gameObject.activeSelf)
        //                {
        //                    lst.Add(t.gameObject);
        //                }
        //            }
        //            for (int i = 0, imax = lst.Count; i < imax; ++i)
        //            {
        //                GameObject.DestroyImmediate(lst[i]);
        //            }
        //            if (lst.Count > 0)
        //                EditorApplication.SaveScene(scenePath);
        //        }


        //    }
        //}

        //public static void RemoveLightmapBakeThing(string[] scenes)
        //{
        //    foreach (string scenePath in scenes)
        //    {
        //        RemoveLightmapBakeThing(scenePath);
        //    }
        //}
        #endregion
        #region prefab
        private static Mesh CompareMesh(Mesh srcMesh, Mesh mesh, string dir, string name, int i)
        {
            if (srcMesh != null && srcMesh.name == mesh.name)
            {
                string newMeshPath = string.Format("{0}/{1}_{2}.asset", dir, name, i);
                return AssetDatabase.LoadAssetAtPath<Mesh>(newMeshPath);
            }
            return null;
        }
        private static int FindMesh(Mesh mesh, bool isSkin, out Mesh newMesh)
        {
            newMesh = mesh;
            string meshPath = AssetDatabase.GetAssetPath(mesh);
            int index = meshPath.LastIndexOf(".");
            if (index > 0)
            {
                int dirindex = meshPath.LastIndexOf("/");
                string dir = meshPath;
                if (dirindex >= 0)
                {
                    dir = meshPath.Substring(0, dirindex);
                }
                string ext = meshPath.Substring(index).ToLower();
                if (ext == ".fbx")
                {
                    GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
                    string name = fbx.name.ToLower();
                    if (name.EndsWith("_bandpose"))
                    {
                        name = name.Replace("_bandpose", "");
                    }
                    GameObject go = GameObject.Instantiate(fbx) as GameObject;
                    List<Renderer> renderLst = ListPool<Renderer>.Get();
                    go.GetComponentsInChildren<Renderer>(true, renderLst);
                    for (int i = 0; i < renderLst.Count; ++i)
                    {
                        Renderer render = renderLst[i];
                        Mesh srcMesh = null;
                        if (isSkin)
                        {
                            if (render is SkinnedMeshRenderer)
                            {
                                SkinnedMeshRenderer smr = render as SkinnedMeshRenderer;
                                srcMesh = smr.sharedMesh;
                            }
                        }
                        else
                        {
                            if (render is MeshRenderer)
                            {
                                MeshFilter mf = render.GetComponent<MeshFilter>();
                                srcMesh = mf.sharedMesh;
                            }
                        }
                        if (srcMesh != null)
                        {
                            Mesh findMesh = CompareMesh(srcMesh, mesh, dir, name, i);
                            if (findMesh != null)
                            {
                                newMesh = findMesh;
                                break;
                            }
                        }
                    }
                    ListPool<Renderer>.Release(renderLst);
                    GameObject.DestroyImmediate(go);
                    if (newMesh == mesh)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        private static int FindAvatar(Avatar avatar, out Avatar newAvatar)
        {
            newAvatar = avatar;
            string avatarPath = AssetDatabase.GetAssetPath(avatar);
            int index = avatarPath.LastIndexOf(".");
            if (index > 0)
            {
                int dirindex = avatarPath.LastIndexOf("/");
                string dir = avatarPath;
                if (dirindex >= 0)
                {
                    dir = avatarPath.Substring(0, dirindex);
                }
                string ext = avatarPath.Substring(index).ToLower();
                if (ext == ".fbx")
                {
                    GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(avatarPath);
                    string name = fbx.name.ToLower();
                    if (name.EndsWith("_bandpose"))
                    {
                        name = name.Replace("_bandpose", "");
                    }
                    GameObject go = GameObject.Instantiate(fbx) as GameObject;

                    GameObject.DestroyImmediate(go);
                    if (newAvatar == avatar)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        private static bool needFix = true;
        private static string errorLog = "";
        public static void FixPrefab(string path, GameObject go = null, bool isPrefab = false)
        {
            if (go == null && isPrefab)
                return;
            if (go == null)
                go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
                return;
            path = path.ToLower();
            bool isFx = path.StartsWith("assets/resources/prefabs/bullets") || path.StartsWith("assets/resources/effects");
            errorLog = "";
            GameObject prefab = isPrefab ? go : GameObject.Instantiate(go) as GameObject;
            prefab.name = go.name;
            Animator ator = prefab.GetComponentInChildren<Animator>();

            List<Renderer> renderLst = ListPool<Renderer>.Get();
            prefab.GetComponentsInChildren<Renderer>(true, renderLst);
            bool change = false;
            bool hasSkin = false;
            for (int i = 0; i < renderLst.Count; ++i)
            {
                Renderer render = renderLst[i];
                if (render != null)
                {
                    MeshFilter mf = null;
                    SkinnedMeshRenderer smr = null;
                    ParticleSystemRenderer psr = null;
                    Mesh mesh = null;
                    if (!render.gameObject.activeSelf)
                    {
                        errorLog += render.name + ":render被影藏\r\n";
                    }
                    if (!CheckRender(render))
                    {
                        errorLog += render.name + ":mesh设置错误\r\n";
                        if (needFix)
                        {
                            ProcessRender(render);
                            change = true;
                        }
                    }
                    if (render is SkinnedMeshRenderer)
                    {
                        hasSkin = true;
                        smr = render as SkinnedMeshRenderer;
                        mesh = smr.sharedMesh;
                        if (!CheckSkinMesh(smr))
                        {
                            errorLog += render.name + ":skinmesh设置错误\r\n";
                            if (needFix)
                            {
                                ProcessSkinMesh(smr);
                                change = true;
                            }
                        }
                    }
                    else if (render is MeshRenderer)
                    {
                        mf = render.GetComponent<MeshFilter>();
                        if (mf != null)
                            mesh = mf.sharedMesh;
                    }
                    else if (render is ParticleSystemRenderer)
                    {
                        psr = render as ParticleSystemRenderer;

                        ParticleSystem ps = render.GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            var main = ps.main;
                            var startSize = main.startSize;
                            var startSizeZ = main.startSizeZ;
                            startSizeZ.mode = startSize.mode;
                            if (startSize.constantMin != startSizeZ.constantMin ||
                                startSize.constantMax != startSizeZ.constantMax)
                            {
                                bool scaleError = false;
                                if (startSize.constantMin > startSize.constantMax)
                                {
                                    scaleError = true;
                                    errorLog += render.name + ":Particle Min>Max未设置\r\n";
                                }
                                else
                                {
                                    errorLog += render.name + ":ParticleScale未设置\r\n";
                                }
                                if (needFix)
                                {
                                    if (scaleError)
                                    {
                                        startSizeZ.constantMin = startSize.constantMax;
                                        startSizeZ.constantMax = startSize.constantMin;
                                        main.startSize = startSizeZ;
                                    }
                                    else
                                    {
                                        startSizeZ.constantMin = startSize.constantMin;
                                        startSizeZ.constantMax = startSize.constantMax;
                                    }

                                    main.startSizeZ = startSizeZ;
                                    change = true;
                                }
                            }
                        }

                        if (psr.renderMode == ParticleSystemRenderMode.Mesh)
                        {
                            mesh = psr.mesh;
                        }
                        else if (psr.mesh != null)
                        {
                            errorLog += render.name + ":Particle未优化\r\n";
                            if (needFix)
                            {
                                psr.mesh = null;
                                change = true;
                            }
                        }
                    }

                    if (mesh != null)
                    {
                        Mesh newMesh = null;
                        int result = FindMesh(mesh, smr!=null, out newMesh);
                        if (result == 1)
                        {
                            errorLog += render.name + ":mesh未优化\r\n";
                            if (needFix)
                            {
                                if (smr != null)
                                {
                                    smr.sharedMesh = newMesh;
                                }
                                else if (mf != null)
                                {
                                    mf.sharedMesh = newMesh;
                                }
                                else if (psr != null)
                                {
                                    psr.mesh = newMesh;
                                }
                                change = true;
                            }
                        }
                        else if (result == -1)
                        {
                            errorLog += render.name + ":mesh没有找到mesh\r\n";
                        }
                    }
                    MeshCollider mc = render.GetComponent<MeshCollider>();
                    if (mc != null)
                    {
                        mesh = mc.sharedMesh;
                        if (mesh != null)
                        {
                            Mesh newMesh = null;
                            int result = FindMesh(mesh, false, out newMesh);
                            if (result == 1)
                            {
                                errorLog += render.name + "meshcollider未优化\r\n";
                                if (needFix)
                                {
                                    mc.sharedMesh = newMesh;
                                    change = true;
                                }
                            }
                            else if (result == -1)
                            {
                                errorLog += render.name + "meshcollider没有找到mesh\r\n";
                            }
                        }
                    }
                }
            }
            if (!isFx && renderLst.Count > 1)
            {
                Debug.LogWarning(string.Format("Too many draw call:{0} {1}", renderLst.Count, path));
            }
            ListPool<Renderer>.Release(renderLst);
            if (ator != null)
            {
                if (ator.avatar != null)
                {
                    if (hasSkin)
                    {
                        Avatar newAvatar = null;
                        int result = ReplaceAvatar(ator.avatar, out newAvatar);
                        if (result == 1)
                        {
                            errorLog += ator.name + ":avatar没有优化\r\n";
                            if (needFix && newAvatar != null)
                            {
                                ator.avatar = newAvatar;
                                change = true;
                            }

                        }
                        else if (result == -1)
                        {
                            errorLog += ator.name + ":avatar没有找到avatar\r\n";
                        }
                    }
                    else
                    {
                        errorLog += ator.name + ":avatar可以为空\r\n";
                        if (needFix)
                        {
                            ator.avatar = null;
                            change = true;
                        }
                    }
                }
                else if (hasSkin)
                {
                    if (ator.runtimeAnimatorController != null)
                    {
                        string name = ator.runtimeAnimatorController.name;
                        if (name == "XAnimator" ||
                             name == "XCamera" ||
                             name == "XMinorAnimator")
                            errorLog += "avatar空\r\n";
                    }
                    else
                    {
                        errorLog += "avatarController空\r\n";
                    }
                }

            }
            BoxCollider box = prefab.GetComponent<BoxCollider>();
            if (box != null && ator != null)
            {
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    errorLog += "Rigidbody没有优化\r\n";
                    if (needFix)
                    {
                        rb = prefab.AddComponent<Rigidbody>();
                        change = true;
                    }

                }
                if (rb != null && (rb.useGravity != false || rb.isKinematic != true))
                {
                    errorLog += "Rigidbody设置错误\r\n";
                    if (needFix)
                    {

                        rb.useGravity = false;
                        rb.isKinematic = true;
                        change = true;
                    }
                }

            }


            if (change && needFix&& !isPrefab)
                PrefabUtility.ReplacePrefab(prefab, go, ReplacePrefabOptions.ReplaceNameBased);
            if (errorLog != "")
            {
                Debug.LogError(errorLog + ":" + path);
            }
            if (!isPrefab)
                GameObject.DestroyImmediate(prefab);
        }
        private static void _FixPrefab(GameObject go, string path)
        {
            FixPrefab(path, go);
        }

        [MenuItem(@"Assets/FixPrefab", false, 0)]
        private static void FixPrefab()
        {
            enumPrefab.cb = _FixPrefab;
            EnumAsset<GameObject>(enumPrefab, "FixPrefab");
        }

        private static void _CheckPrefab(GameObject go, string path)
        {
            FixPrefab(path, go);
        }
        [MenuItem(@"Assets/CheckPrefab", false, 0)]
        private static void CheckPrefab()
        {
            needFix = false;
            enumPrefab.cb = _CheckPrefab;
            EnumAsset<GameObject>(enumPrefab, "CheckPrefab");
            needFix = true;
        }

        [MenuItem(@"Assets/DisconnectPrefab", false, 0)]
        private static void DisconnectPrefab()
        {
            GameObject go = Selection.activeGameObject;
            if (go != null)
            {
                PrefabUtility.DisconnectPrefabInstance(go);
                PrefabUtility.CreatePrefab("Assets/TMPPrefab.prefab", go, ReplacePrefabOptions.ConnectToPrefab);
                AssetDatabase.DeleteAsset("Assets/TMPPrefab.prefab");
            }

        }
        [MenuItem(@"Assets/Tool/Prefab/SetPetTag", false, 0)]
        private static void SetPetTag()
        {
            TextAsset ta0 = Resources.Load<TextAsset>(@"Table/PetInfo");
            TextAsset ta1 = Resources.Load<TextAsset>(@"Table/XEntityPresentation");
            if (ta0 != null && ta1 != null)
            {
                CVSReader.Init();
                XBinaryReader.Init();
                XBinaryReader piReader = XBinaryReader.Get();
                piReader.Init(ta0);
                PetInfoTable pi = new PetInfoTable();
                pi.ReadFile(piReader);
                XBinaryReader.Return(piReader);

                XBinaryReader presentReader = XBinaryReader.Get();
                presentReader.Init(ta1);
                XEntityPresentation ep = new XEntityPresentation();
                ep.ReadFile(presentReader);
                XBinaryReader.Return(presentReader);

                for (int i = 0; i < pi.Table.Length; ++i)
                {
                    uint present_id = pi.Table[i].presentID;
                    XEntityPresentation.RowData row = ep.GetByPresentID(present_id);
                    if (row != null)
                    {
                        string path = "Prefabs/" + row.Prefab;
                        GameObject go = Resources.Load<GameObject>(path);
                        if (go != null)
                        {
                            bool change = false;
                            List<Renderer> renderLst = ListPool<Renderer>.Get();
                            GameObject mount = GameObject.Instantiate<GameObject>(go);
                            mount.GetComponentsInChildren<Renderer>(renderLst);
                            for (int j = 0; j < renderLst.Count; ++j)
                            {
                                Renderer render = renderLst[j];
                                if (render.sharedMaterial.renderQueue >= 3000 || render is ParticleSystemRenderer)
                                {
                                    if (render.gameObject.tag != "Mount_BindedRes")
                                    {
                                        render.gameObject.tag = "Mount_BindedRes";
                                        change = true;
                                    }
                                }
                                else
                                {
                                    if (render.gameObject.tag != "Mount")
                                    {
                                        render.gameObject.tag = "Mount";
                                        change = true;
                                    }
                                }
                            }
                            ListPool<Renderer>.Release(renderLst);
                            if (change)
                                PrefabUtility.ReplacePrefab(mount, go, ReplacePrefabOptions.ReplaceNameBased);
                            GameObject.DestroyImmediate(mount);
                        }
                    }
                }
            }

        }

        private static void AddCCPrefab(BuffTable buff, XEntityStatistics entityStatistics, XEntityPresentation entityPresentation, HashSet<string> transformPrefab)
        {
            for (int i = 0; i < buff.Table.Length; ++i)
            {
                BuffTable.RowData row = buff.Table[i];
                if (row.BuffState != null)
                {
                    for (int j = 0; j < row.BuffState.Length; ++j)
                    {
                        byte buffState = row.BuffState[j];
                        if (buffState == 9)
                        {
                            //变身
                            int statisticsID = Math.Abs(row.StateParam);
                            XEntityStatistics.RowData data = entityStatistics.GetByID((uint)statisticsID);
                            if (data != null)
                            {
                                XEntityPresentation.RowData present_data = entityPresentation.GetByPresentID(data.PresentID);
                                if (present_data != null)
                                {
                                    transformPrefab.Add(present_data.Prefab.ToLower());
                                }
                            }
                        }
                    }
                }
            }
        }
        [MenuItem(@"Assets/Tool/Prefab/RefreshCharacterController", false, 0)]
        private static void RefreshCharacterController()
        {
            TextAsset ta0 = Resources.Load<TextAsset>(@"Table/BuffList");
            TextAsset ta1 = Resources.Load<TextAsset>(@"Table/BuffListPVP");
            TextAsset ta2 = Resources.Load<TextAsset>(@"Table/XEntityStatistics");
            TextAsset ta3 = Resources.Load<TextAsset>(@"Table/XEntityPresentation");
            if (ta0 != null && ta1 != null)
            {

                CVSReader.Init();
                XBinaryReader.Init();

                XBinaryReader buffReader = XBinaryReader.Get();
                buffReader.Init(ta0);
                BuffTable buff = new BuffTable();
                buff.ReadFile(buffReader);
                XBinaryReader.Return(buffReader);

                XBinaryReader buffPvPReader = XBinaryReader.Get();
                buffPvPReader.Init(ta1);
                BuffTable buffPvP = new BuffTable();
                buffPvP.ReadFile(buffPvPReader);
                XBinaryReader.Return(buffPvPReader);

                XBinaryReader XEntityStatisticsPReader = XBinaryReader.Get();
                XEntityStatisticsPReader.Init(ta2);
                XEntityStatistics entityStatistics = new XEntityStatistics();
                entityStatistics.ReadFile(XEntityStatisticsPReader);
                XBinaryReader.Return(XEntityStatisticsPReader);

                XBinaryReader XEntityPresentationReader = XBinaryReader.Get();
                XEntityPresentationReader.Init(ta3);
                XEntityPresentation entityPresentation = new XEntityPresentation();
                entityPresentation.ReadFile(XEntityPresentationReader);
                XBinaryReader.Return(XEntityPresentationReader);

                HashSet<string> transformPrefab = new HashSet<string>();
                AddCCPrefab(buff, entityStatistics, entityPresentation, transformPrefab);
                AddCCPrefab(buffPvP, entityStatistics, entityPresentation, transformPrefab);

                DirectoryInfo di = new DirectoryInfo("Assets/Resources/Prefabs");
                FileInfo[] files = di.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; ++i)
                {
                    string path = files[i].FullName.Replace("\\", "/");
                    int index = path.IndexOf("Assets/Resources/Prefabs/");
                    path = path.Substring(index);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab.name.StartsWith("Mount_"))
                    {
                        CharacterController cc = prefab.GetComponent<CharacterController>();
                        if (cc == null)
                        {
                            Debug.LogError("miss CharacterController prefab:" + prefab.name);
                        }
                    }
                    else
                    {
                        index = path.LastIndexOf("/");
                        string name = path.Substring(index + 1);
                        name = name.Replace(".prefab", "").ToLower();
                        CharacterController cc = prefab.GetComponent<CharacterController>();
                        if (transformPrefab.Contains(name))
                        {
                            if (cc == null)
                                Debug.LogError("miss CharacterController prefab:" + name);
                        }
                        else
                        {
                            if (cc != null && (!name.StartsWith("player") && !name.StartsWith("zj")))
                            {
                                GameObject.DestroyImmediate(cc, true);
                                Debug.LogError("not need CharacterController prefab:" + name);
                            }

                        }
                    }

                }
                AssetDatabase.Refresh();
            }

        }
        #endregion
        #region equip
        [MenuItem(@"Assets/Tool/Creatures/OptmizeCreatures", false, 0)]
        private static void OptmizeGameObject()
        {
            Rect wr = new Rect(0, 0, 600, 800);
            SelectBones window = (SelectBones)EditorWindow.GetWindowWithRect(typeof(SelectBones), wr, true, "隐藏骨骼");
            window.Init();
            window.Show();
        }
        [MenuItem(@"Assets/Tool/Equipment/OptmizeEquipment", false, 0)]
        private static void OptmizeEquipGameObject()
        {
            Rect wr = new Rect(0, 0, 600, 800);
            SelectEquipBones window = (SelectEquipBones)EditorWindow.GetWindowWithRect(typeof(SelectEquipBones), wr, true, "隐藏骨骼");
            window.Init();
            window.Show();
        }

        public static int GetUVOffset(int profession, string meshName, CombineConfig config, int depth = 0)
        {
            int index = meshName.LastIndexOf("_");
            if (index >= 0)
            {
                string name = meshName.Substring(index).ToLower();
                if (name.StartsWith(config.BodyString))
                {
                    return (int)EPartType.EUpperBody;
                }
                if (name.StartsWith(config.LegString))
                {
                    return (int)EPartType.ELowerBody;
                }
                if (name.StartsWith(config.GloveString))
                {
                    return (int)EPartType.EGloves;
                }
                if (name.StartsWith(config.BootString))
                {
                    return (int)EPartType.EBoots;
                }
                if (name.StartsWith(config.HeadString) ||
                    name.StartsWith(config.FaceString))
                {
                    return (int)EPartType.EFace;
                }
                if (name.StartsWith(config.HairString))
                {
                    return (int)EPartType.EHair;
                }
                if (name.StartsWith(config.HelmetString) ||
                   name.StartsWith("_helmat"))
                {
                    return (int)EPartType.EHeadgear;
                }
                if (name.StartsWith(config.SecondaryWeapon[profession]))
                {
                    return (int)EPartType.ESecondaryWeapon;
                }
                if (depth < 3)
                {
                    depth++;
                    return GetUVOffset(profession, meshName.Substring(0, index), config, depth);
                }
                return -1;
            }
            else
            {
                return -1;
            }

        }
        private static void ReCalculateUV(Mesh mesh, int uvOffsetX)
        {
            if (uvOffsetX >= 0)
            {
                Vector2[] uv = mesh.uv;
                for (int i = 0, imax = mesh.uv.Length; i < imax; ++i)
                {
                    //计算新uv
                    Vector2 tmp = uv[i];
                    tmp.x = tmp.x - Mathf.Floor(tmp.x);
                    tmp.x += uvOffsetX;
                    tmp.y = tmp.y - Mathf.Floor(tmp.y);
                    uv[i] = tmp;
                }
                mesh.uv = uv;
            }
        }
        private static void ProcessMeshAsset(Mesh mesh, Material mat, int profession, string saveRootPath, string dirName, string srcMeshName, string meshName, string fbxDir,bool export = true)
        {
            Texture2D tex = mat.mainTexture as Texture2D;
            int uvOffsetX = GetUVOffset(profession, srcMeshName, s_CombineConfig);
            if (uvOffsetX >= 0)
            {
                if (export)
                    ReCalculateUV(mesh, uvOffsetX);
            }
            else
            {
                XDebug.singleton.AddErrorLog(string.Format("Find UV Error:{0} {1}", meshName, dirName));
            }
            if (export)
            {
                mesh.uv2 = null;
                mesh.uv3 = null;
                mesh.uv4 = null;
                mesh.colors = null;
                mesh.colors32 = null;
                mesh.tangents = null;
            }

            string newMeshPath = "Equipments/" + dirName + "/" + meshName;
            string meshPath = saveRootPath + meshName + ".asset";
            string prefix = s_CombineConfig.EquipPrefix[profession];
            if (tex == null)
            {
                Debug.LogError("null tex:" + meshName);
                AddPart(newMeshPath, newMeshPath, "", "", false, false, false, prefix);
            }
            else
            {
                if (export)
                {
                    MeshUtility.Optimize(mesh);
                    CreateOrReplaceAsset<Mesh>(mesh, meshPath);
                }

                PartTexInfo partTexInfo = null;
                bool shareTex = false;
                if (!usedTex.TryGetValue(tex.GetHashCode(), out partTexInfo))
                {
                    string srcTexPath = AssetDatabase.GetAssetPath(tex);
                    int index = srcTexPath.LastIndexOf("/");
                    string srcTexDir = srcTexPath.Substring(0, index);
                    if (srcTexDir.ToLower() != fbxDir.ToLower())
                    {
                        //共享贴图的逻辑
                        int srcTexProfession = GetProfession(srcTexPath.ToLower());
                        string srcprefix = s_CombineConfig.EquipPrefix[srcTexProfession];
                        string partSuffix = s_CombineConfig.PartSuffix[uvOffsetX];
                        string srcTexName = saveRootPath + srcprefix + partSuffix + ".tga";
                        if (File.Exists(srcTexName))
                        {
                            //Debug.LogWarning(string.Format("Src Tex already exist:{0} {1}", srcTexName, meshPath));
                            partTexInfo = new PartTexInfo();
                            partTexInfo.texPath = "Equipments/" + dirName + "/" + srcprefix + partSuffix;
                            partTexInfo.isAlpha = File.Exists(saveRootPath + srcprefix + partSuffix + "_A.png");
                            shareTex = true;
                        }
                        else
                        {
                            Debug.LogError(string.Format("Src Tex not exist:{0} {1}", srcTexName, meshPath));
                            return;
                        }
                    }
                    else
                    {
                        bool isAlpha = false;
                        if(export)
                        {
                            string targetTexPath = saveRootPath + meshName + ".tga";
                            AssetDatabase.CopyAsset(srcTexPath, targetTexPath);
                            Texture2D mainTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath);
                            Texture2D alphaTex = null;
                            if (mat.renderQueue > 2000)
                            {
                                alphaTex = ConvertTexRtex(mainTex);
                            }
                            DefaultCompressTex(AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath), targetTexPath, true, true);
                            isAlpha = alphaTex != null;
                        }
                        else
                        {
                            isAlpha = File.Exists(saveRootPath + meshName + "_A.png");
                        }
                        partTexInfo = new PartTexInfo();
                        partTexInfo.texPath = newMeshPath;
                        partTexInfo.isAlpha = isAlpha;
                        usedTex.Add(tex.GetHashCode(), partTexInfo);
                    }
                }

                AddPart(newMeshPath, newMeshPath, "", partTexInfo.texPath, tex.width == 1024, partTexInfo.isAlpha, shareTex, prefix);
            }
        }

        private static void SaveSkinWeaponAsset(SkinnedMeshRenderer smr, Mesh mesh, string saveRootPath, int profession)
        {
            string prefix = s_CombineConfig.EquipPrefix[profession];
            mesh.name = prefix + "weapon";
            string weaponMeshPath = saveRootPath + prefix + "weapon.asset";

            Material mat = smr.sharedMaterial;
            if (mat.shader != null && mat.shader.name.EndsWith("UV2"))
            {

            }
            else
            {
                mesh.uv2 = null;
            }

            mesh.uv3 = null;
            mesh.uv4 = null;
            mesh.colors = null;
            mesh.colors32 = null;
            mesh.tangents = null;
            MeshUtility.Optimize(mesh);
            mesh.UploadMeshData(true);

            Mesh loadMesh = CreateOrReplaceAsset<Mesh>(mesh, weaponMeshPath);

            GameObject weaponGo = new GameObject(mesh.name);
            SkinnedMeshRenderer newSmr = weaponGo.AddComponent<SkinnedMeshRenderer>();
            newSmr.sharedMesh = loadMesh;
            ProcessSkinMesh(newSmr);
            newSmr.sharedMaterial = smr.sharedMaterial;
            newSmr.localBounds = mesh.bounds;
            newSmr.gameObject.layer = LayerMask.NameToLayer("Role");
            DefaultCompressTex(newSmr.sharedMaterial.mainTexture as Texture2D, AssetDatabase.GetAssetPath(newSmr.sharedMaterial.mainTexture), true, true);
            string weaponPath = weaponMeshPath.Replace(".asset", ".prefab");
            CreateOrReplacePrefab(weaponGo, weaponPath);
            GameObject.DestroyImmediate(weaponGo);
        }
        public static string MakeEquipDir(string name, int profession)
        {
            string prefix = s_CombineConfig.EquipPrefix[profession];
            name = name.ToLower();
            if (name.StartsWith("player_") || name.StartsWith("cl_normal02_"))
            {
                return "Player";
            }
            else if (name.StartsWith(prefix) && (name.EndsWith("_bandpose") || name.EndsWith("_onepart_bandpose")))
            {
                name = name.Substring(prefix.Length);
                int index = -1;
                if (name.EndsWith("_onepart_bandpose"))
                {
                    index = name.LastIndexOf("_onepart_bandpose");
                }
                else
                {
                    index = name.LastIndexOf("_bandpose");
                }
                name = name.Substring(0, index);
                return name;
            }
            else
            {
                Debug.LogError("name not correct:" + name);
                return "";
            }
        }
        public static string MakeEquipName(string name, int profession, string dirName)
        {
            string prefix = s_CombineConfig.EquipPrefix[profession];
            string replacePrefix = s_CombineConfig.EquipPrefixReplace[profession];
            name = name.ToLower();
            if (name.Contains("_helmat"))
                name = name.Replace("_helmat", s_CombineConfig.HelmetString);

            int partIndex = GetUVOffset(profession, name, s_CombineConfig);
            if (partIndex < 0)
            {
                Debug.LogError("error part:" + name);
                return "";
            }

            string partSuffix = s_CombineConfig.PartSuffix[partIndex];
            string defaultName = prefix + partSuffix;
            if (partIndex == (int)EPartType.EFace && name.Contains(s_CombineConfig.HeadString))
            {
                name = name.Replace(s_CombineConfig.HeadString, s_CombineConfig.FaceString);
            }
            if (partIndex == 6)
            {
                return defaultName;
            }


            if (name.StartsWith(replacePrefix))
            {
                if (dirName == "Player")
                {
                    name = name.Replace(replacePrefix, prefix);
                }
                else
                {
                    name = defaultName;
                }

                return name;
            }
            return defaultName;
        }

        public static string MakeWeaponName(int profession)
        {
            string prefix = s_CombineConfig.EquipPrefix[profession];
            return prefix + "weapon";
        }
        public static int GetProfession(string path)
        {
            path = path.ToLower();
            int profession = -1;
            for (int i = 0; i < s_CombineConfig.EquipFolderName.Length; ++i)
            {
                if (path.Contains(s_CombineConfig.EquipFolderName[i]))
                {
                    profession = i;
                    break;
                }
            }
            return profession;
        }
        private static CombineConfig s_CombineConfig = null;
        public static void InitCombineConfig()
        {
            s_CombineConfig = CombineConfig.GetConfig();
        }
        public class PartTexInfo
        {
            public string texPath = "";
            public bool isAlpha = false;
        }
        public static Dictionary<int, PartTexInfo> usedTex = new Dictionary<int, PartTexInfo>();

        private static bool _SaveSkinAsset(GameObject fbx, ModelImporter modelImporter, string path)
        {
            if (modelImporter == null)
                return false;
            int profession = GetProfession(path);
            if (profession < 0)
            {
                modelImporter.isReadable = false;
                return true;
            }
            string dirName = MakeEquipDir(fbx.name, profession);
            if (string.IsNullOrEmpty(dirName))
            {
                return false;
            }
            int pathIndex = path.LastIndexOf("/");
            string fbxDir = path.Substring(0, pathIndex);
            string saveRootPath = "Assets/Resources/Equipments/" + dirName + "/";
            if (!Directory.Exists(saveRootPath))
            {
                Directory.CreateDirectory(saveRootPath);
            }
            modelImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            usedTex.Clear();
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            List<SkinnedMeshRenderer> smrList = ListPool<SkinnedMeshRenderer>.Get();
            go.GetComponentsInChildren<SkinnedMeshRenderer>(true, smrList);

            foreach (SkinnedMeshRenderer smr in smrList)
            {
                Mesh mesh = UnityEngine.Object.Instantiate(smr.sharedMesh) as Mesh;
                mesh.name = smr.sharedMesh.name;

                if (smr.sharedMesh.name.EndsWith("weapon"))
                {
                    SaveSkinWeaponAsset(smr, mesh, saveRootPath, profession);
                }
                else
                {
                    string meshName = MakeEquipName(mesh.name, profession, dirName);
                    if (string.IsNullOrEmpty(meshName))
                    {
                        continue;
                    }
                    mesh.name = meshName;
                    string meshPath = saveRootPath + meshName + ".asset";

                    ProcessMeshAsset(mesh, smr.sharedMaterial, profession, saveRootPath, dirName, smr.sharedMesh.name, meshName, fbxDir);
                }
            }
            ListPool<SkinnedMeshRenderer>.Release(smrList);
            modelImporter.isReadable = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            List<MeshFilter> meshList = ListPool<MeshFilter>.Get();
            go.GetComponentsInChildren<MeshFilter>(meshList);
            string prefix = s_CombineConfig.EquipPrefix[profession];

            foreach (MeshFilter mf in meshList)
            {
                Mesh mesh = UnityEngine.Object.Instantiate(mf.sharedMesh) as Mesh;
                MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Material mat = mr.sharedMaterial;
                    if (mat.shader != null && mat.shader.name.EndsWith("UV2"))
                    {

                    }
                    else
                    {
                        mesh.uv2 = null;
                    }
                }


                mesh.uv3 = null;
                mesh.uv4 = null;
                mesh.colors = null;
                mesh.colors32 = null;
                mesh.tangents = null;
                mesh.name = prefix + "weapon";
                mesh.UploadMeshData(true);
                string weaponMeshPath = saveRootPath + prefix + "weapon.asset";
                Mesh loadMesh = CreateOrReplaceAsset<Mesh>(mesh, weaponMeshPath);
                mf.sharedMesh = loadMesh;
                DefaultCompressTex(mr.sharedMaterial.mainTexture as Texture2D, AssetDatabase.GetAssetPath(mr.sharedMaterial.mainTexture), true, true);
                ProcessRender(mr);
                mr.gameObject.layer = LayerMask.NameToLayer("Role");
                CreateOrReplacePrefab(mr.gameObject, saveRootPath + mesh.name + ".prefab");
            }
            ListPool<MeshFilter>.Release(meshList);
            GameObject.DestroyImmediate(go);
            return false;
        }

        public static XMeshPartList s_meshPartLis = new XMeshPartList();
        public static void LoadMeshPartInfo()
        {
            s_meshPartLis.Load();
            if (s_meshPartLis.meshPartsInfo == null)
                s_meshPartLis.meshPartsInfo = new Dictionary<uint, byte>();
            if (s_meshPartLis.replaceMeshPartsInfo == null)
                s_meshPartLis.replaceMeshPartsInfo = new Dictionary<uint, string>();
            if (s_meshPartLis.replaceTexPartsInfo == null)
                s_meshPartLis.replaceTexPartsInfo = new Dictionary<uint, string>();

        }
        [MenuItem(@"Assets/Tool/Equipment/SaveSkinAsset %3", false, 0)]
        private static void SaveSkinAsset()
        {
            s_CombineConfig = CombineConfig.GetConfig();
            LoadMeshPartInfo();
            enumFbx.cb = _SaveSkinAsset;
            EnumAsset<GameObject>(enumFbx, "SaveSkinAsset");
            SaveEquipInfo();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static bool _ExportEquipAvatar(GameObject fbx, ModelImporter modelImporter, string path)
        {
            AssetModify.PreExportMeshAvatar(modelImporter.meshCompression, ModelImporterTangents.None);
            AssetModify.ExportMeshAvatar(modelImporter, path, null);
            return false;
        }
        [MenuItem(@"Assets/Tool/Equipment/ExportEquipAvata", false, 0)]
        private static void ExportMeshAvatar()
        {
            enumFbx.cb = _ExportEquipAvatar;
            EnumAsset<GameObject>(enumFbx, "ExportEquipAvata");
            enumFbx.preprocess = null;
        }
        private static void _RefreshConfig(GameObject fbx, string path)
        {
            int profession = GetProfession(path);
            if (profession < 0)
            {
                return;
            }
            string dirName = MakeEquipDir(fbx.name, profession);
            if (string.IsNullOrEmpty(dirName))
            {
                return;
            }

            int pathIndex = path.LastIndexOf("/");
            string fbxDir = path.Substring(0, pathIndex);
            string saveRootPath = "Assets/Resources/Equipments/" + dirName + "/";
            if (!Directory.Exists(saveRootPath))
            {
                return;
            }
            usedTex.Clear();
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            List<SkinnedMeshRenderer> smrList = ListPool<SkinnedMeshRenderer>.Get();
            go.GetComponentsInChildren<SkinnedMeshRenderer>(true, smrList);

            foreach (SkinnedMeshRenderer smr in smrList)
            {
                Mesh mesh = smr.sharedMesh;
                if (!smr.sharedMesh.name.ToLower().EndsWith("weapon") && smr.sharedMaterial != null)
                {
                    string meshName = MakeEquipName(mesh.name, profession, dirName);
                    if (string.IsNullOrEmpty(meshName))
                    {
                        continue;
                    }
                    ProcessMeshAsset(mesh, smr.sharedMaterial, profession, saveRootPath, dirName, smr.sharedMesh.name, meshName, fbxDir, false);
                    //string meshPath = saveRootPath + meshName + ".asset";
                    //if (File.Exists(meshPath))
                    //{
                    //    string newMeshPath = "Equipments/" + dirName + "/" + meshName;

                    //    Texture2D tex = smr.sharedMaterial.mainTexture as Texture2D;
                    //    int uvOffsetX = GetUVOffset(profession, smr.sharedMesh.name, s_CombineConfig);
                    //    if (uvOffsetX < 0)
                    //    {
                    //        Debug.LogError(string.Format("Find UV Error:{0} {1}", meshName, dirName));
                    //    }

                    //    if (tex == null)
                    //    {
                    //        Debug.LogError("null tex:" + path);
                    //        AddPart(newMeshPath, newMeshPath, "", "", false, false, false);
                    //    }
                    //    else
                    //    {
                    //        bool shareTex = false;
                    //        PartTexInfo partTexInfo = null;
                    //        if (usedTex.TryGetValue(tex.GetHashCode(), out partTexInfo))
                    //        {
                    //            Debug.LogWarning(string.Format("Share tex with:{0} {1}", partTexInfo.texPath, meshPath));
                    //        }
                    //        else
                    //        {
                    //            string srcTexPath = AssetDatabase.GetAssetPath(tex);
                    //            int index = srcTexPath.LastIndexOf("/");
                    //            string srcTexDir = srcTexPath.Substring(0, index).ToLower();
                    //            if (srcTexDir != fbxDir)
                    //            {
                    //                int srcTexProfession = GetProfession(srcTexPath.ToLower());
                    //                string prefix = s_CombineConfig.EquipPrefix[srcTexProfession];
                    //                string partSuffix = s_CombineConfig.PartSuffix[uvOffsetX];
                    //                string srcTexName = saveRootPath + prefix + partSuffix + ".tga";
                    //                if (File.Exists(srcTexName))
                    //                {
                    //                    Debug.LogWarning(string.Format("Src Tex already exist:{0} {1}", srcTexName, meshPath));
                    //                    partTexInfo = new PartTexInfo();
                    //                    partTexInfo.texPath = "Equipments/" + dirName + "/" + prefix + partSuffix;
                    //                    partTexInfo.isAlpha = File.Exists(saveRootPath + prefix + partSuffix + "_A.png");
                    //                    shareTex = true;
                    //                }
                    //                else
                    //                {
                    //                    Debug.LogError(string.Format("Src Tex not exist:{0} {1}", srcTexName, meshPath));
                    //                    continue;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                string targetAlphaTexPath = saveRootPath + meshName + "_A.png";
                    //                partTexInfo = new PartTexInfo();
                    //                partTexInfo.texPath = newMeshPath;
                    //                partTexInfo.isAlpha = File.Exists(targetAlphaTexPath);
                    //                usedTex.Add(tex.GetHashCode(), partTexInfo);
                    //            }
                    //        }

                    //        AddPart(newMeshPath, newMeshPath, "", partTexInfo.texPath, tex.width == 1024, partTexInfo.isAlpha, shareTex);
                    //    }
                    //}
                }
            }
            ListPool<SkinnedMeshRenderer>.Release(smrList);
            GameObject.DestroyImmediate(go);
        }
        private static void _RefreshExtraConfig(GameObject fbx, string path)
        {
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            ReplaceEquip re = go.GetComponent<ReplaceEquip>();
            if (re != null)
            {
                re.Process(false);
            }
            GameObject.DestroyImmediate(go);
        }
        [MenuItem(@"Assets/Tool/Equipment/RefreshConfig", false, 0)]
        private static void RefreshConfig()
        {
            s_CombineConfig = CombineConfig.GetConfig();
            s_meshPartLis.meshPartsInfo = new Dictionary<uint, byte>();
            s_meshPartLis.replaceMeshPartsInfo = new Dictionary<uint, string>();
            s_meshPartLis.replaceTexPartsInfo = new Dictionary<uint, string>();

            List<EquipPathInfo> meshParts = new List<EquipPathInfo>();
            foreach (string equipFolderName in s_CombineConfig.EquipFolderName)
            {
                string path = "Assets/Equipment" + equipFolderName;
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*.fbx", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; ++i)
                {
                    FileInfo fi = files[i];
                    string filename = fi.FullName;
                    filename = filename.Replace("\\", "/");
                    int index = filename.IndexOf("Assets/Equipment/");
                    filename = filename.Substring(index);
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "Process Fbx", i, files.Length), filename, (float)i / files.Length);

                    GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(filename);

                    _RefreshConfig(fbx, filename);
                }

                files = di.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; ++i)
                {
                    FileInfo fi = files[i];
                    string filename = fi.FullName;
                    filename = filename.Replace("\\", "/");
                    int index = filename.IndexOf("Assets/Equipment/");
                    filename = filename.Substring(index);
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "Process Fbx", i, files.Length), filename, (float)i / files.Length);

                    GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(filename);

                    _RefreshExtraConfig(fbx, filename);
                }
            }


            SaveEquipInfo();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All objects processed finish", "OK");
        }

        [MenuItem(@"Assets/Tool/Equipment/PrintSpecialPart", false, 0)]
        private static void PrintSpecialPart()
        {
            s_CombineConfig = CombineConfig.GetConfig();
            LoadMeshPartInfo();
            Debug.LogWarning("==================ReplaceMesh");
            var itReplace = s_meshPartLis.replaceMeshPartsInfo.GetEnumerator();
            while (itReplace.MoveNext())
            {
                Debug.LogWarning(itReplace.Current.Value);

            }
            Debug.LogWarning("==================ReplaceTex");
            var itReplaceTex = s_meshPartLis.replaceTexPartsInfo.GetEnumerator();
            while (itReplaceTex.MoveNext())
            {
                Debug.LogWarning(itReplaceTex.Current.Value);
            }
        }
        private static string GetDefaultEquipPart(DefaultEquip.RowData de, EPartType part)
        {
            switch (part)
            {
                case EPartType.EFace:
                    return de.Face;
                case EPartType.EHair:
                    return de.Hair;
                case EPartType.EUpperBody:
                    return de.Body;
                case EPartType.ELowerBody:
                    return de.Leg;
                case EPartType.EGloves:
                    return de.Glove;
                case EPartType.EBoots:
                    return de.Boots;
                case EPartType.ESecondaryWeapon:
                    return de.SecondWeapon;
                case EPartType.EHeadgear:
                    return de.Helmet;
                case EPartType.EMainWeapon:
                    return de.Weapon;
            }
            return "";
        }

        private static int ConvertPart(int fp)
        {
            switch (fp)
            {
                case 0:
                    return (int)EPartType.EHeadgear;
                case 1:
                    return (int)EPartType.EUpperBody;
                case 2:
                    return (int)EPartType.ELowerBody;
                case 3:
                    return (int)EPartType.EGloves;
                case 4:
                    return (int)EPartType.EBoots;
                case 5:
                    return (int)EPartType.EMainWeapon;
                case 6:
                    return (int)EPartType.ESecondaryWeapon;
                case 7:
                    return (int)EPartType.EWings;
                case 8:
                    return (int)EPartType.ETail;
                case 9:
                    return (int)EPartType.EDecal;
                case 10:
                    return (int)EPartType.EFace;
                case 11:
                    return (int)EPartType.EHair;
            }
            return -1;
        }

        private static string GetEquipPrefabModel(FashionList.RowData data, int profID)
        {
            switch (profID)
            {
                case 0:
                    return data.ModelPrefabWarrior;
                case 1:
                    return data.ModelPrefabArcher;
                case 2:
                    return data.ModelPrefabSorcer;
                case 3:
                    return data.ModelPrefabCleric;
                case 4:
                    return data.ModelPrefab5;
                case 5:
                    return data.ModelPrefab6;
                case 6:
                    return data.ModelPrefab7;
                //case 8:
                //    return data.ModelPrefab8;
                default:
                    return string.Empty;
            }
        }
        private static void CheckMesh(string meshPath, string meshResPath, uint professionIndex,int part)
        {
            if (meshPath.EndsWith("_weapon"))
            {
                if (!File.Exists(meshPath + ".prefab"))
                {
                    Debug.LogError("prefab not found:" + meshPath);
                }
            }
            else
            {
                byte partType = 0;
                string replaceMeshLocation = null;
                string replaceTexLocation = null;
                if (s_meshPartLis.GetMeshInfo(meshResPath, (int)professionIndex, part,"", out partType, ref replaceMeshLocation, ref replaceTexLocation))
                {
                    if (replaceMeshLocation != null)
                    {
                        if (!File.Exists("Assets/Resources/" + replaceMeshLocation + ".asset"))
                        {
                            Debug.LogError("replace mesh not found:" + replaceMeshLocation);
                        }
                    }
                    else if (!File.Exists(meshPath + ".asset"))
                    {
                        Debug.LogError("mesh not found:" + meshPath);
                    }
                    if (replaceTexLocation != null)
                    {
                        if (!File.Exists("Assets/Resources/" + replaceTexLocation + ".tga"))
                        {
                            Debug.LogError("replace tex not found:" + replaceTexLocation);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(meshPath))
                {
                    Debug.LogError("mesh not found in config:" + meshPath);
                }
            }

        }
        [MenuItem(@"Assets/Tool/Equipment/TestEquip", false, 0)]
        private static void TestEquip()
        {
            s_CombineConfig = CombineConfig.GetConfig();
            LoadMeshPartInfo();
            XBinaryReader.Init();
            XBinaryReader reader = XBinaryReader.Get();
            DefaultEquip defaultEquip = new DefaultEquip();
            FashionList fashionList = new FashionList();
            TextAsset ta = Resources.Load<TextAsset>(@"Table/DefaultEquip");
            if (ta != null)
            {
                reader.Init(ta);
                defaultEquip.ReadFile(reader);
                XBinaryReader.Return(reader);

                for (int i = 0; i < defaultEquip.Table.Length; ++i)
                {
                    DefaultEquip.RowData de = defaultEquip.Table[i];
                    string weaponPoint = de.WeaponPoint != null ? de.WeaponPoint[0] : "";


                    int professionIndex = -1;
                    for (int j = 0; j < s_CombineConfig.EquipPrefix.Length; ++j)
                    {
                        DefaultEquip.RowData startDe = defaultEquip.Table[j];
                        if (startDe.WeaponPoint != null && startDe.WeaponPoint[0] == weaponPoint)
                        {
                            professionIndex = j;
                            break;
                        }
                    }
                    if (professionIndex < 0)
                    {
                        Debug.LogError("professionIndex not found:" + i.ToString());
                        continue;
                    }
                    string proPrefix = s_CombineConfig.EquipPrefix[professionIndex];
                    for (int j = 0; j < s_CombineConfig.PartSuffix.Length; ++j)
                    {
                        string partSuffix = s_CombineConfig.PartSuffix[j];
                        string path = GetDefaultEquipPart(de, (EPartType)j);
                        string meshPath = "";
                        string meshResPath = "";
                        if (string.IsNullOrEmpty(path))
                        {
                            meshPath = string.Format("Assets/Resources/Equipments/Player/{0}{1}", proPrefix, partSuffix);
                            meshResPath = string.Format("Equipments/Player/{0}{1}", proPrefix, partSuffix);
                        }
                        else if (path.StartsWith("/"))
                        {
                            meshPath = string.Format("Assets/Resources/Equipments{0}/{1}{2}", path, proPrefix, partSuffix);
                            meshResPath = string.Format("Equipments{0}/{1}{2}", path, proPrefix, partSuffix);
                        }
                        else if (path == "E")
                        {
                            meshPath = "";
                            meshResPath = "";
                        }
                        else
                        {
                            meshPath = string.Format("Assets/Resources/Equipments/Player/{0}{1}", proPrefix, path);
                            meshResPath = string.Format("Equipments/Player/{0}{1}", proPrefix, path);
                        }
                        CheckMesh(meshPath, meshResPath, (uint)professionIndex,j);
                    }
                }
            }

            ta = Resources.Load<TextAsset>(@"Table/FashionList");
            if (ta != null)
            {
                reader.Init(ta);
                fashionList.ReadFile(reader);

                for (int i = 0; i < fashionList.Table.Length; ++i)
                {
                    FashionList.RowData data = fashionList.Table[i];
                    int index = ConvertPart(data.EquipPos);

                    if (index >= 0 && index < s_CombineConfig.PartSuffix.Length)
                    {
                        for (int j = 0; j < s_CombineConfig.EquipPrefix.Length; ++j)
                        {
                            string proPrefix = s_CombineConfig.EquipPrefix[j];
                            string partSuffix = s_CombineConfig.PartSuffix[index];
                            string equipPath = GetEquipPrefabModel(data, j);
                            string meshPath = "";
                            string meshResPath = "";
                            if (equipPath == "E" || string.IsNullOrEmpty(data.SuitName) && string.IsNullOrEmpty(equipPath))
                            {
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(data.SuitName))
                                {

                                    meshPath = string.Format("Assets/Resources/Equipments/{0}/{1}{2}", equipPath, proPrefix, partSuffix);
                                    meshResPath = string.Format("Equipments/{0}/{1}{2}", equipPath, proPrefix, partSuffix);
                                }
                                else
                                {
                                    meshPath = string.Format("Assets/Resources/Equipments/{0}/{1}{2}", data.SuitName, proPrefix, partSuffix);
                                    meshResPath = string.Format("Equipments/{0}/{1}{2}", data.SuitName, proPrefix, partSuffix);
                                }
                            }
                            CheckMesh(meshPath, meshResPath, (uint)j, index);
                        }

                    }
                }
            }
            XBinaryReader.Return(reader);
        }

        [MenuItem(@"Assets/Tool/Equipment/MakeMesh", false, 0)]
        private static void MakeMesh()
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(Mesh), SelectionMode.Assets);
            if(objs!=null&& objs.Length>0)
            {
                Mesh mesh = objs[0] as Mesh;
                Mesh newMesh = UnityEngine.Object.Instantiate<Mesh>(mesh);
                newMesh.name = "CombineMesh";
                newMesh.Clear(true);
                CreateOrReplaceAsset<Mesh>(newMesh, "Assets/Resources/Equipments/CombineMesh.asset");
            }
        }

        private static bool _Save3Part(GameObject fbx, ModelImporter modelImporter, string path)
        {
            if (modelImporter == null)
                return false;
            string saveRootPath = "Assets/Resources/Prefabs/Equipment/";
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            List<Renderer> renderLst = ListPool<Renderer>.Get();
            go.GetComponentsInChildren<Renderer>(true, renderLst);
            foreach (Renderer r in renderLst)
            {
                ProcessRender(r);
            }
            ListPool<Renderer>.Release(renderLst);
            Animator animator = go.GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("Controller/XMinorAnimator") as RuntimeAnimatorController;

            go.layer = LayerMask.NameToLayer("Role");
            CreateOrReplacePrefab(go, saveRootPath + fbx.name + ".prefab");

            GameObject.DestroyImmediate(go);
            modelImporter.isReadable = false;
            return true;
        }

        [MenuItem(@"Assets/Tool/Equipment/Save3Part %4", false, 0)]
        private static void Save3Part()
        {
            enumFbx.cb = _Save3Part;
            EnumAsset<GameObject>(enumFbx, "Save3Part");
        }


        public static void AddPart(string meshPath, string replaceMesh,string replaceMeshDir, string texPath, bool isOnePart, bool hasAlpha, bool shareTex,string prefix)
        {
            uint hash = XCommon.singleton.XHash(meshPath);
            if (string.IsNullOrEmpty(texPath) || hash == 0)
            {
                s_meshPartLis.meshPartsInfo.Remove(hash);
                s_meshPartLis.replaceMeshPartsInfo.Remove(hash);
                return;
            }
            byte type = 0;
            if (isOnePart)
            {
                if (hasAlpha)
                {
                    type = XMeshPartList.CutoutPart;
                }
                else
                {
                    type = XMeshPartList.OnePart;
                }
            }
            else
            {
                type = XMeshPartList.NormalPart;
            }
            string location = meshPath;
            if (meshPath != replaceMesh)
            {
                location = replaceMesh;
                type |= XMeshPartList.ReplacePart;
                hash = XCommon.singleton.XHash(replaceMesh);
                s_meshPartLis.replaceMeshPartsInfo[hash] = replaceMeshDir;
                Debug.LogWarning(string.Format("ReplaceMesh:\r\nsrcmesh:{0}\r\nreplacemesh:{1}\r\nsavedmesh:{2}", meshPath, replaceMesh, replaceMeshDir));
            }
            if (shareTex)
            {
                type |= XMeshPartList.ShareTex;
                int i = location.LastIndexOf("/");
                if (i >= 0)
                {
                    location = location.Substring(0, i);
                    if (location.Contains(prefix))
                    {
                        Debug.LogError(string.Format("path:{0} contain profession prefix:{1},please rename!", location, prefix));
                    }
                }
            }
            else if (meshPath != texPath)
            {
                if (meshPath != replaceMesh && replaceMesh != texPath||
                    meshPath == replaceMesh)
                {                    
                    type |= XMeshPartList.ReplaceTex;
                    string newTexPath = "";
                    if(texPath.Contains("/Player"))
                    {
                        int index = texPath.LastIndexOf("/");
                        newTexPath = texPath.Substring(index + 1);
                    }
                    else
                    {
                        int index = texPath.LastIndexOf("/");
                        newTexPath = texPath.Substring(index);
                    }
                    Debug.LogWarning(string.Format("ReplaceTex:\r\ntex:{0}\r\nmesh:{1}\r\nsavedmesh:{2}", texPath, replaceMesh, newTexPath));
                    s_meshPartLis.replaceTexPartsInfo[hash] = newTexPath;
                }                   
            }
            s_meshPartLis.meshPartsInfo[hash] = type;
            //if (s_meshPartLis.meshParts.ContainsKey(hash))
            //{
            //    Debug.LogError("duplicate hash:" + meshPath);
            //}
        }

        public static void SaveEquipInfo()
        {
            try
            {
                using (FileStream desStream = new FileStream(@"Assets/Resources/Equipments/equipmentInfo.bytes", FileMode.Create))
                {
                    BinaryWriter writer = new BinaryWriter(desStream);
                    int professionCount = s_CombineConfig.EquipPrefix.Length;
                    writer.Write(professionCount);
                    for (int i = 0; i < professionCount; ++i)
                    {
                        writer.Write(s_CombineConfig.EquipPrefix[i]);
                    }

                    int partSuffixCount = s_CombineConfig.PartSuffix.Length;
                    writer.Write(partSuffixCount);
                    for (int i = 0; i < partSuffixCount; ++i)
                    {
                        writer.Write(s_CombineConfig.PartSuffix[i]);
                    }
                    //shared tex prefix
                    writer.Write(s_CombineConfig.EquipPrefix[0]);
                    List<string> stringTable = new List<string>();
                    Dictionary<string, ushort> stringHash = new Dictionary<string, ushort>();
                    var itReplace = s_meshPartLis.replaceMeshPartsInfo.GetEnumerator();
                    while (itReplace.MoveNext())
                    {
                        if(!stringHash.ContainsKey(itReplace.Current.Value))
                        {
                            stringHash.Add(itReplace.Current.Value, (ushort)stringTable.Count);
                            stringTable.Add(itReplace.Current.Value);
                        }
                    }
                    var itReplaceTex = s_meshPartLis.replaceTexPartsInfo.GetEnumerator();
                    while (itReplaceTex.MoveNext())
                    {
                        if (!stringHash.ContainsKey(itReplaceTex.Current.Value))
                        {
                            stringHash.Add(itReplaceTex.Current.Value, (ushort)stringTable.Count);
                            stringTable.Add(itReplaceTex.Current.Value);
                        }
                    }
                    writer.Write(stringTable.Count);
                    for (int i = 0; i < stringTable.Count; ++i)
                    {
                        writer.Write(stringTable[i]);
                    }

                    writer.Write(s_meshPartLis.meshPartsInfo.Count);
                    var it = s_meshPartLis.meshPartsInfo.GetEnumerator();
                    while (it.MoveNext())
                    {
                        writer.Write(it.Current.Key);                       
                        writer.Write(it.Current.Value);
                    }
                    writer.Write(s_meshPartLis.replaceMeshPartsInfo.Count);
                    itReplace = s_meshPartLis.replaceMeshPartsInfo.GetEnumerator();
                    while (itReplace.MoveNext())
                    {
                        writer.Write(itReplace.Current.Key);
                        ushort index = 0;
                        stringHash.TryGetValue(itReplace.Current.Value, out index);
                        writer.Write(index);

                    }
                    writer.Write(s_meshPartLis.replaceTexPartsInfo.Count);
                    itReplaceTex = s_meshPartLis.replaceTexPartsInfo.GetEnumerator();
                    while (itReplaceTex.MoveNext())
                    {
                        writer.Write(itReplaceTex.Current.Key);
                        ushort index = 0;
                        stringHash.TryGetValue(itReplaceTex.Current.Value, out index);
                        writer.Write(index);
                    }
                    desStream.Flush();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        //private static bool MakeEquipName(string name, int profession, out string dirName, out string newName)
        //{
        //    string prefix = s_CombineConfig.EquipPrefix[profession];
        //    string replacePrefix = s_CombineConfig.EquipPrefixReplace[profession];
        //    name = name.ToLower();
        //    bool isWeapon = false;
        //    if (name.StartsWith("weapon/"))
        //    {
        //        name = name.Replace("weapon/", "");
        //        isWeapon = true;
        //    }
        //    if (name.StartsWith(replacePrefix))
        //    {
        //        newName = name.Replace(replacePrefix, prefix);
        //        dirName = "Player";
        //        if (isWeapon)
        //        {
        //            newName = "";
        //        }
        //        return true;
        //    }
        //    else if (name.StartsWith(prefix))
        //    {

        //        int suffix = name.LastIndexOf("_");
        //        if (suffix >= 0)
        //        {
        //            newName = prefix + name.Substring(suffix + 1);
        //            dirName = name.Substring(0, suffix);
        //            if (dirName.StartsWith(prefix))
        //                dirName = dirName.Substring(prefix.Length);
        //            if (isWeapon)
        //            {
        //                suffix = dirName.LastIndexOf("_");
        //                if (suffix >= 0)
        //                {
        //                    dirName = dirName.Substring(0, suffix);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            newName = name;
        //            dirName = "";
        //        }

        //    }
        //    else
        //    {
        //        newName = name;
        //        dirName = "";
        //    }
        //    return false;
        //}
        //private static bool MakeDefaultEquipName(string name, int profession, int part, out string newName)
        //{
        //    //string prefix = s_CombineConfig.EquipPrefix[profession];
        //    //string replacePrefix = s_CombineConfig.EquipPrefixReplace[profession];
        //    //string partSuffix = s_CombineConfig.PartSuffix[part];
        //    //string secondSuffix = s_CombineConfig.SecondaryWeapon[profession].Substring(1);
        //    //string defaultPart = prefix + partSuffix;
        //    //name = name.ToLower();
        //    //bool isWeapon = false;
        //    //bool isSceondWeapon = part == 6;
        //    //newName = "";
        //    //if (name.StartsWith("weapon/"))
        //    //{
        //    //    name = name.Replace("weapon/", "");
        //    //    isWeapon = true;
        //    //    defaultPart = prefix + "weapon";
        //    //}
        //    //if (name.StartsWith(replacePrefix))
        //    //{
        //    //    newName = name.Replace(replacePrefix, prefix);
        //    //    if (isSceondWeapon)
        //    //    {
        //    //        defaultPart = prefix + secondSuffix + "_weapon";
        //    //    }
        //    //    if (isWeapon)
        //    //    {
        //    //        int suffix = newName.LastIndexOf("_");
        //    //        newName = prefix + newName.Substring(suffix + 1);
        //    //    }
        //    //    if (newName == defaultPart)
        //    //    {
        //    //        newName = "";
        //    //    }
        //    //    return true;
        //    //}
        //    //else if (name.StartsWith(prefix))
        //    //{

        //    //    int suffix = name.LastIndexOf("_");
        //    //    if (suffix >= 0)
        //    //    {
        //    //        newName = name.Substring(0, suffix);
        //    //        if (newName.StartsWith(prefix))
        //    //            newName = newName.Substring(prefix.Length);
        //    //        if (isWeapon)
        //    //        {
        //    //            suffix = newName.LastIndexOf("_");
        //    //            if (suffix >= 0)
        //    //            {
        //    //                newName = newName.Substring(0, suffix);
        //    //            }
        //    //        }
        //    //    }
        //    //    return true;
        //    //}
        //    //else
        //    //{
        //    //    return false;
        //    //}

        //    string prefix = s_CombineConfig.EquipPrefix[profession];
        //    string replacePrefix = s_CombineConfig.EquipPrefixReplace[profession];
        //    name = name.ToLower();
        //    bool isWeapon = false;
        //    if (name.StartsWith("weapon/"))
        //    {
        //        name = name.Replace("weapon/", "");
        //        isWeapon = true;
        //    }

        //    int index = name.LastIndexOf("/");
        //    if (index >= 0)
        //    {
        //        name = name.Substring(index + 1);
        //    }
        //    if (name.Contains("_helmat"))
        //        name = name.Replace("_helmat", s_CombineConfig.HelmetString);
        //    newName = "";
        //    string defaultName = "";
        //    string defaultName2 = prefix + "head";
        //    string defaultName3 = prefix + s_CombineConfig.SecondaryWeapon[profession].Substring(1);
        //    if (!isWeapon)
        //    {
        //        int partIndex = GetUVOffset(profession, name, s_CombineConfig);
        //        if (partIndex < 0)
        //        {
        //            Debug.LogError("error part:" + name);
        //            return false;
        //        }
        //        string partSuffix = s_CombineConfig.PartSuffix[partIndex];
        //        defaultName = prefix + partSuffix;

        //        //if (partIndex == 6)
        //        //{
        //        //    return true;
        //        //}
        //    }
        //    else
        //    {

        //    }


        //    if (name.StartsWith(replacePrefix))
        //    {
        //        if (isWeapon)
        //        {
        //            newName = "";
        //        }
        //        else
        //        {
        //            newName = name.Replace(replacePrefix, prefix);
        //            if (defaultName == newName || defaultName2 == newName || defaultName3 == newName)
        //                newName = "";
        //        }

        //        return true;
        //    }
        //    if (name.StartsWith(prefix))
        //    {
        //        newName = name.Replace(prefix, "/");
        //        index = newName.LastIndexOf("_");
        //        if (index >= 0)
        //        {
        //            newName = newName.Substring(0, index);
        //        }
        //        if (isWeapon)
        //        {
        //            index = newName.LastIndexOf("_");
        //            if (index >= 0)
        //            {
        //                newName = newName.Substring(0, index);
        //            }
        //        }
        //        return true;
        //    }
        //    newName = name;
        //    return true;
        //}

        //[MenuItem(@"Assets/Tool/Equipment/FashionList")]
        //private static void FashionList()
        //{
        //    s_CombineConfig = CombineConfig.GetConfig();
        //    int lineno = 1;
        //    using (FileStream fs = new FileStream(@"Assets/Table/FashionList.txt", FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
        //    {
        //        if (fs.Length > 0)
        //        {
        //            byte[] bytes = new byte[fs.Length];
        //            if (bytes != null)
        //            {
        //                fs.Read(bytes, 0, (int)fs.Length);
        //                Stream srcStream = new System.IO.MemoryStream(bytes);
        //                using (FileStream desStream = new FileStream(@"Assets/Table/FashionListNew.txt", FileMode.Create))
        //                {
        //                    StreamReader reader = new StreamReader(srcStream);
        //                    StreamWriter writer = new StreamWriter(desStream, new UnicodeEncoding());
        //                    int suitNameIndex = -1;
        //                    int[] professionIndex = new int[6] { -1, -1, -1, -1, -1, -1 };
        //                    string[] professionStr = new string[] { "ModelPrefabWarrior", "ModelPrefabArcher", "ModelPrefabSorcer", "ModelPrefabCleric", "ModelPrefab5", "ModelPrefab6" };
        //                    string[] newNames = new string[] { "", "", "", "", "", "" };
        //                    string[] dirNames = new string[] { "", "", "", "", "", "" };

        //                    while (true)
        //                    {
        //                        bool isParseOK = true;
        //                        string line = reader.ReadLine();
        //                        if (line == null)
        //                            break;

        //                        line = line.TrimEnd(eof);
        //                        string[] cols = line.Split('\t');
        //                        if (lineno == 1)
        //                        {
        //                            writer.WriteLine(line);
        //                            for (int i = 0; i < cols.Length; ++i)
        //                            {
        //                                if (cols[i] == "SuitName")
        //                                {
        //                                    suitNameIndex = i;
        //                                }
        //                                else
        //                                {
        //                                    for(int j=0;j< professionStr.Length;++j)
        //                                    {
        //                                        if(cols[i] == professionStr[j])
        //                                        {
        //                                            professionIndex[j] = i;
        //                                            break;
        //                                        }
        //                                    }                                            
        //                                }                                        
        //                            }
        //                        }
        //                        else if (lineno == 2)
        //                        {
        //                            writer.WriteLine(line);
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < professionIndex.Length; ++i)
        //                            {
        //                                int index = professionIndex[i];
        //                                string prefix = s_CombineConfig.EquipPrefix[i];
        //                                string replacePrefix = s_CombineConfig.EquipPrefixReplace[i];
        //                                string name = cols[index].ToLower();

        //                                string newName = "";
        //                                string dirName = "";
        //                                bool isWeapon = false;
        //                                if (name.StartsWith("weapon/"))
        //                                {
        //                                    name = name.Replace("weapon/", "");
        //                                    isWeapon = true;
        //                                }
        //                                if (name.StartsWith(replacePrefix))
        //                                {
        //                                    //player_xxx_xxx
        //                                    newName = name.Replace(replacePrefix, "");
        //                                    dirName = "Player";
        //                                    if (isWeapon)
        //                                    {
        //                                        newName = "";
        //                                    }
        //                                }
        //                                else if (name.StartsWith(prefix))
        //                                {
        //                                    int suffix = name.LastIndexOf("_");
        //                                    if (suffix >= 0)
        //                                    {
        //                                        newName = name.Substring(suffix + 1);

        //                                        dirName = name.Substring(0, suffix);
        //                                        if (dirName.StartsWith(prefix))
        //                                            dirName = dirName.Substring(prefix.Length);
        //                                        if (isWeapon)
        //                                        {
        //                                            suffix = dirName.LastIndexOf("_");
        //                                            if (suffix >= 0)
        //                                            {
        //                                                dirName = dirName.Substring(0, suffix);
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {                                                
        //                                        newName = name;
        //                                        dirName = "";
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    if (string.IsNullOrEmpty(name))
        //                                    {
        //                                        name = "E";
        //                                    }
        //                                    newName = name;
        //                                    dirName = "";
        //                                }
        //                                newNames[i] = newName;
        //                                dirNames[i] = dirName;
        //                            }
        //                            string dir = "";
        //                            int dirIndex = 0;
        //                            for (; dirIndex < dirNames.Length; ++dirIndex)
        //                            {
        //                                if (dirNames[dirIndex] != "")
        //                                {
        //                                    dir = dirNames[dirIndex];
        //                                    dirIndex++;
        //                                    break;
        //                                }


        //                            }
        //                            for (int i = dirIndex; i < dirNames.Length; ++i)
        //                            {
        //                                if (dirNames[i] != "")
        //                                {
        //                                    if (dir != dirNames[i])
        //                                    {
        //                                        dir = "";
        //                                        break;
        //                                    }
        //                                }

        //                            }
        //                            cols[suitNameIndex] = dir;
        //                            for (int j = 0; j < professionIndex.Length; ++j)
        //                            {
        //                                if (dir == "Player")
        //                                {
        //                                    cols[professionIndex[j]] = newNames[j];
        //                                }
        //                                else if (dir == "")
        //                                {
        //                                    cols[professionIndex[j]] = dirNames[j];
        //                                }
        //                                else
        //                                {
        //                                    cols[professionIndex[j]] = "";

        //                                }

        //                            }                                  

        //                            string newLine = "";
        //                            for (int i = 0; i < cols.Length; ++i)
        //                            {
        //                                newLine += cols[i] + ((i != cols.Length - 1) ? "\t" : "");
        //                            }
        //                            writer.WriteLine(newLine);
        //                        }
        //                        line = null;
        //                        if (!isParseOK) break;

        //                        ++lineno;
        //                    }
        //                    writer.Flush();
        //                }
        //                srcStream.Close();
        //            }
        //        }
        //    }

        //}
        //[MenuItem(@"Assets/Tool/Equipment/DefaultEquip")]
        //private static void DefaultEquip()
        //{
        //    s_CombineConfig = CombineConfig.GetConfig();
        //    int lineno = 1;
        //    using (FileStream fs = new FileStream(@"Assets/Table/DefaultEquip.txt", FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
        //    {
        //        if (fs.Length > 0)
        //        {
        //            byte[] bytes = new byte[fs.Length];
        //            if (bytes != null)
        //            {
        //                fs.Read(bytes, 0, (int)fs.Length);
        //                Stream srcStream = new System.IO.MemoryStream(bytes);
        //                using (FileStream desStream = new FileStream(@"Assets/Table/DefaultEquipNew.txt", FileMode.Create))
        //                {
        //                    StreamReader reader = new StreamReader(srcStream);
        //                    StreamWriter writer = new StreamWriter(desStream, new UnicodeEncoding());
        //                    int[] partIndexes = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        //                    string[] partStr = new string[9] { "Face", "Hair", "Helmet", "Body", "Leg", "Glove", "Boots", "Weapon", "SecondWeapon" };
        //                    string[] professionStr = new string[6] { "Point001_zhanshi", "BoxBone01_archer", "BoxBone01_sorceress|BoxBone02_sorceress", "BoxBone01_Cleric|BoxBone02_Cleric", "~BoxBone01_academic", "BoxBone01_assassin" };
        //                    int WeaponPointIndex = -1;
        //                    while (true)
        //                    {
        //                        bool isParseOK = true;
        //                        string line = reader.ReadLine();
        //                        if (line == null)
        //                            break;

        //                        line = line.TrimEnd(eof);
        //                        string[] cols = line.Split('\t');
        //                        if (lineno == 1)
        //                        {
        //                            writer.WriteLine(line);
        //                            for (int i = 0; i < cols.Length; ++i)
        //                            {
        //                                string partName = cols[i];
        //                                for (int j = 0; j < partStr.Length; ++j)
        //                                {
        //                                    if(partName== partStr[j])
        //                                    {
        //                                        partIndexes[j] = i;
        //                                        break;
        //                                    }
        //                                }
        //                                if(partName== "WeaponPoint")
        //                                {
        //                                    WeaponPointIndex = i;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        else if (lineno == 2)
        //                        {
        //                            writer.WriteLine(line);
        //                        }
        //                        else
        //                        {

        //                            int profession = -1;
        //                            for (int i = 0; i < professionStr.Length; ++i)
        //                            {
        //                                if (professionStr[i] == cols[WeaponPointIndex])
        //                                {
        //                                    profession = i;
        //                                    break;
        //                                }
        //                            }
        //                            if (profession >= 0 && profession < 6)
        //                            {
        //                                for (int j = 0; j < partIndexes.Length; ++j)
        //                                {
        //                                    int partIndex = partIndexes[j];
        //                                    string partName = cols[partIndex].ToLower();
        //                                    if (string.IsNullOrEmpty(partName))
        //                                    {
        //                                        cols[partIndex] = "E";
        //                                        continue;
        //                                    }

        //                                    string newName = "";
        //                                    string prefix = s_CombineConfig.EquipPrefix[profession];
        //                                    string replacePrefix = s_CombineConfig.EquipPrefixReplace[profession];

        //                                    bool isWeapon = false;
        //                                    if (partName.StartsWith("weapon/"))
        //                                    {
        //                                        partName = partName.Replace("weapon/", "");
        //                                        isWeapon = true;
        //                                    }

        //                                    int index = partName.LastIndexOf("/");
        //                                    if (index >= 0)
        //                                    {
        //                                        partName = partName.Substring(index + 1);
        //                                    }
        //                                    if (partName.Contains("_helmat"))
        //                                        partName = partName.Replace("_helmat", s_CombineConfig.HelmetString);

        //                                    string defaultName = "";
        //                                    string defaultName2 = "head";
        //                                    string defaultName3 = s_CombineConfig.SecondaryWeapon[profession].Substring(1);
        //                                    if (!isWeapon)
        //                                    {
        //                                        int uvIndex = GetUVOffset(profession, partName, s_CombineConfig);
        //                                        if (uvIndex < 0)
        //                                        {
        //                                            Debug.LogError(string.Format("error part:{0} line :{1}", partName, lineno));
        //                                            continue;
        //                                        }
        //                                        string partSuffix = s_CombineConfig.PartSuffix[uvIndex];
        //                                        defaultName = partSuffix;
        //                                    }

        //                                    if (partName.StartsWith(replacePrefix))
        //                                    {
        //                                        if (isWeapon)
        //                                        {
        //                                            newName = "";
        //                                        }
        //                                        else
        //                                        {
        //                                            newName = partName.Replace(replacePrefix, "");
        //                                            if (defaultName == newName || defaultName2 == newName || defaultName3 == newName)
        //                                                newName = "";
        //                                        }
        //                                    }
        //                                    else if (partName.StartsWith(prefix))
        //                                    {
        //                                        newName = partName.Replace(prefix, "/");
        //                                        index = newName.LastIndexOf("_");
        //                                        if (index >= 0)
        //                                        {
        //                                            newName = newName.Substring(0, index);
        //                                        }
        //                                        if (isWeapon)
        //                                        {
        //                                            index = newName.LastIndexOf("_");
        //                                            if (index >= 0)
        //                                            {
        //                                                newName = newName.Substring(0, index);
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        newName = partName;
        //                                    }                                           
        //                                    cols[partIndex] = newName;
        //                                }                                       
        //                            }

        //                            string newLine = "";
        //                            for (int i = 0; i < cols.Length; ++i)
        //                            {
        //                                newLine += cols[i] + ((i != cols.Length - 1) ? "\t" : "");
        //                            }
        //                            writer.WriteLine(newLine);
        //                        }
        //                        line = null;
        //                        if (!isParseOK) break;

        //                        ++lineno;
        //                    }
        //                    writer.Flush();
        //                }
        //                srcStream.Close();
        //            }
        //        }
        //    }

        //}

        #endregion
        #region fx
        private static void _ConvertAnimator2Legacy(GameObject go, string path)
        {
            GameObject newGo = GameObject.Instantiate(go) as GameObject;
            List<Animator> animators = new List<Animator>();
            newGo.GetComponentsInChildren<Animator>(animators);
            for (int i = 0; i < animators.Count; ++i)
            {
                Animator animator = animators[i];
                if (animator.avatar != null)
                {
                    Debug.LogWarning("avatar fx:" + path);
                    continue;
                }

                UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                if (ac != null)
                {
                    UnityEditor.Animations.AnimatorControllerLayer[] layers = ac.layers;
                    if (layers == null || layers.Length != 1)
                    {
                        break;
                    }
                    else
                    {
                        foreach (UnityEditor.Animations.AnimatorControllerLayer acl in layers)
                        {
                            UnityEditor.Animations.AnimatorStateMachine asm = acl.stateMachine;

                            UnityEditor.Animations.ChildAnimatorState[] states = asm.states;
                            if (states.Length != 1)
                            {
                                UnityEditor.Animations.AnimatorState defaultState = asm.defaultState;
                                if (defaultState != null)
                                    defaultState.name = "Anim";
                                break;
                            }
                            foreach (UnityEditor.Animations.ChildAnimatorState state in states)
                            {
                                AnimationClip clip = state.state.motion as AnimationClip;
                                if (clip != null)
                                {
                                    bool loop = clip.isLooping;
                                    clip.legacy = true;
                                    if (loop)
                                        clip.wrapMode = WrapMode.Loop;
                                    GameObject animGo = animator.gameObject;
                                    Animation animaton = animGo.GetComponent<Animation>();
                                    if (animaton == null)
                                        animaton = animGo.AddComponent<Animation>();
                                    animaton.playAutomatically = true;
                                    animaton.clip = clip;
                                    animaton.cullingType = AnimationCullingType.BasedOnRenderers;
                                    UnityEngine.Object.DestroyImmediate(animator);
                                }
                                else
                                {
                                    break;
                                }

                            }

                        }
                    }

                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(animator);
                }

            }
            PrefabUtility.ReplacePrefab(newGo, go, ReplacePrefabOptions.ReplaceNameBased);
            GameObject.DestroyImmediate(newGo);
        }
        [MenuItem(@"Assets/Tool/Fx/ConvertAnimator2Legacy", false, 0)]
        private static void ConvertAnimator2Legacy()
        {
            enumPrefab.cb = _ConvertAnimator2Legacy;
            EnumAsset<GameObject>(enumPrefab, "ConvertAnimator2Legacy");
        }

        public static void GetAssetPath(string dir, List<string> assetPath)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] files = di.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (FileInfo fi in files)
            {
                string filePath = fi.FullName;
                filePath = filePath.Replace("\\", "/");
                int index = filePath.IndexOf("Assets/Resources/");
                filePath = filePath.Substring(index);
                assetPath.Add(filePath);
            }
        }
        class AnimRefInfo
        {
            public GameObject go = null;
            public string missingPath = null;
            public int missingType = 0;//0 not found 1 deactive not found active
        }

        private static void FindAnimClip(GameObject instance, ref Transform clipTrans, ref AnimationClip clip)
        {
            Animator ator = instance.GetComponentInChildren<Animator>();
            if (ator != null)
            {
                if (ator.avatar == null)
                {
                    clipTrans = ator.transform;
                    UnityEditor.Animations.AnimatorController ac = ator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                    if (ac != null)
                    {
                        UnityEditor.Animations.AnimatorControllerLayer[] layers = ac.layers;
                        if (layers != null && layers.Length == 1)
                        {
                            UnityEditor.Animations.AnimatorControllerLayer acl = layers[0];
                            if (acl != null)
                            {
                                UnityEditor.Animations.AnimatorStateMachine asm = acl.stateMachine;
                                if (asm != null)
                                {
                                    UnityEditor.Animations.AnimatorState defaultState = asm.defaultState;
                                    if (defaultState != null)
                                    {
                                        clip = defaultState.motion as AnimationClip;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Animation anim = instance.GetComponentInChildren<Animation>();
                if (anim != null)
                {
                    clipTrans = anim.transform;
                    clip = anim.clip;
                }
            }
        }
        private static bool CheckAnimRef(EditorCurveBinding[] curveBinding, Transform clipTrans, Transform currentTrans, ref AnimRefInfo ari)
        {

            if (curveBinding != null && clipTrans != null)
            {
                string path = currentTrans.name;
                while (currentTrans.parent != null && clipTrans != currentTrans.parent)
                {
                    currentTrans = currentTrans.parent;
                    path = string.Format("{0}/{1}", currentTrans.name, path);
                }
                for (int i = 0; i < curveBinding.Length; ++i)
                {
                    var binding = curveBinding[i];
                    if (path == binding.path && binding.propertyName == "m_IsActive")
                    {
                        return true;
                    }

                }
                ari = new AnimRefInfo();
                ari.missingPath = path;
                return false;
            }
            return true;
        }
        private static void _FixAnimRef()
        {

            List<string> assetPath = ListPool<string>.Get();

            Dictionary<string, List<string>> shareAnimFx = new Dictionary<string, List<string>>();
            //Dictionary<string, List<AnimRefInfo>> missAimFx = new Dictionary<string, List<AnimRefInfo>>();
            GetAssetPath("Assets/Resources/Effects", assetPath);
            GetAssetPath("Assets/Resources/Prefabs/Bullets", assetPath);
            GetAssetPath("Assets/Resources/Prefabs/Effects/Default", assetPath);
            GetAssetPath("Assets/Resources/Prefabs/Effects/scene", assetPath);
            for (int i = 0; i < assetPath.Count; ++i)
            {
                string path = assetPath[i];
                EditorUtility.DisplayProgressBar(string.Format("{0}/{1}", i, assetPath.Count), path, (float)i / assetPath.Count);
                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(prefab);
                    Transform clipTrans = null;
                    AnimationClip clip = null;
                    FindAnimClip(instance, ref clipTrans, ref clip);
                    if (clip != null && clipTrans != null)
                    {
                        string animPath = AssetDatabase.GetAssetPath(clip);
                        List<string> fxList = null;
                        if (!shareAnimFx.TryGetValue(animPath, out fxList))
                        {
                            fxList = new List<string>();
                            shareAnimFx.Add(animPath, fxList);
                        }
                        fxList.Add(path);
                    }
                    GameObject.DestroyImmediate(instance);
                }
            }
            for (int i = 0; i < assetPath.Count; ++i)
            {
                bool change = false;
                errorLog = "";
                string path = assetPath[i];
                EditorUtility.DisplayProgressBar(string.Format("{0}/{1}", i, assetPath.Count), path, (float)i / assetPath.Count);
                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(prefab);
                    Transform clipTrans = null;
                    AnimationClip clip = null;
                    string animPath = "";
                    List<AnimRefInfo> missAnim = null;
                    List<string> fxList = null;
                    FindAnimClip(instance, ref clipTrans, ref clip);
                    EditorCurveBinding[] curveBinding = null;
                    if (clip != null && clipTrans != null)
                    {
                        animPath = AssetDatabase.GetAssetPath(clip);
                        shareAnimFx.TryGetValue(animPath, out fxList);

                        curveBinding = AnimationUtility.GetCurveBindings(clip);
                        for (int j = 0; j < curveBinding.Length; ++j)
                        {
                            var binding = curveBinding[j];
                            Transform t = clipTrans.Find(binding.path);
                            if (t == null)
                            {
                                if (missAnim == null)
                                {
                                    missAnim = new List<AnimRefInfo>();
                                }
                                bool found = false;
                                for (int k = 0; k < missAnim.Count; ++k)
                                {
                                    if(missAnim[k].missingPath== binding.path)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if(!found)
                                {
                                    AnimRefInfo ari = new AnimRefInfo();
                                    ari.missingPath = binding.path;
                                    missAnim.Add(ari);
                                }                                   
                            }
                        }
                    }

                    List<Renderer> renderLst = ListPool<Renderer>.Get();
                    instance.GetComponentsInChildren<Renderer>(true, renderLst);
                    for (int j = 0; j < renderLst.Count; ++j)
                    {
                        Renderer render = renderLst[j];
                        if (render != null)
                        {
                            if (!render.gameObject.activeSelf)
                            {
                                AnimRefInfo ari = null;
                                if (!CheckAnimRef(curveBinding, clipTrans, render.transform, ref ari))
                                {
                                    if (missAnim == null)
                                    {
                                        missAnim = new List<AnimRefInfo>();
                                    }
                                    ari.go = render.gameObject;
                                    ari.missingType = 1;
                                    missAnim.Add(ari);
                                }
                            }
                        }
                    }
                    ListPool<Renderer>.Release(renderLst);

                    if (fxList != null && fxList.Count > 1 && missAnim != null)
                    {
                        errorLog += "共用animClip:\r\n";
                        for (int j = 0; j < fxList.Count; ++j)
                        {
                            errorLog += fxList[j] + "\r\n";
                        }
                        Debug.LogError(errorLog);
                    }
                    else if (missAnim != null && curveBinding != null && clip != null)
                    {                        
                        if (fxList != null&& fxList.Count>1)
                        {
                            errorLog += "共用animClip:\r\n";
                            for (int j = 0; j < fxList.Count; ++j)
                            {
                                errorLog += fxList[j] + "\r\n";
                            }
                        }
                        Debug.LogError("miss anim obj:" + errorLog + path);
                        if (needFix)
                        {
                            bool animChange = false;
                            for (int j = 0; j < missAnim.Count; ++j)
                            {
                                AnimRefInfo remove = missAnim[j];
                                if (remove != null)
                                {
                                    for (int k = 0; k < curveBinding.Length; ++k)
                                    {
                                        var binding = curveBinding[k];
                                        if (binding.path == remove.missingPath)
                                        {
                                            AnimationUtility.SetEditorCurve(clip, binding, null);
                                            animChange = true;
                                        }
                                    }
                                    if (remove.go != null)
                                    {
                                        GameObject.DestroyImmediate(remove.go);
                                        change = true;
                                    }
                                }
                            }
                            if (animChange)
                            {
                                CreateOrReplaceAsset<AnimationClip>(clip, animPath);
                            }
                        }

                    }
                    if (change && needFix)
                        PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ReplaceNameBased);
                    GameObject.DestroyImmediate(instance);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All assets processed finish", "OK");
            ListPool<string>.Release(assetPath);
        }

        [MenuItem(@"Assets/Tool/Fx/CheckAnim", false, 0)]
        private static void CheckAnim()
        {
            needFix = false;
            _FixAnimRef();
            needFix = true;
        }

        [MenuItem(@"Assets/Tool/Fx/FixAnim", false, 0)]
        private static void FixAnim()
        {
            needFix = true;
            _FixAnimRef();
            needFix = true;
        }
        #endregion
        #region shader
        class ShaderValue
        {
            public ShaderValue(string n, ShaderUtil.ShaderPropertyType t)
            {
                name = n;
                type = t;
            }
            public string name = "";
            public ShaderUtil.ShaderPropertyType type = ShaderUtil.ShaderPropertyType.Float;

            public virtual void SetValue(Material mat)
            {

            }

            public static void GetShaderValue(Material mat, List<ShaderValue> shaderValueLst)
            {
                Shader shader = mat.shader;
                int count = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < count; ++i)
                {
                    ShaderValue sv = null;
                    string name = ShaderUtil.GetPropertyName(shader, i);
                    ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
                    switch (type)
                    {
                        case ShaderUtil.ShaderPropertyType.Color:
                            sv = new ShaderColorValue(name, type, mat);
                            break;
                        case ShaderUtil.ShaderPropertyType.Vector:
                            sv = new ShaderVectorValue(name, type, mat);
                            break;
                        case ShaderUtil.ShaderPropertyType.Float:
                            sv = new ShaderFloatValue(name, type, mat);
                            break;
                        case ShaderUtil.ShaderPropertyType.Range:
                            sv = new ShaderFloatValue(name, type, mat);
                            break;
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            sv = new ShaderTexValue(name, type, mat);
                            break;
                    }
                    shaderValueLst.Add(sv);
                }
            }
        }

        class ShaderIntValue : ShaderValue
        {
            public ShaderIntValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetInt(n);
            }
            public int value = 0;
            public override void SetValue(Material mat)
            {
                mat.SetInt(name, value);
            }
        }

        class ShaderFloatValue : ShaderValue
        {
            public ShaderFloatValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetFloat(n);
            }
            public float value = 0;
            public override void SetValue(Material mat)
            {
                mat.SetFloat(name, value);
            }
        }

        class ShaderVectorValue : ShaderValue
        {
            public ShaderVectorValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetVector(n);
            }
            public Vector4 value = Vector4.zero;
            public override void SetValue(Material mat)
            {
                mat.SetVector(name, value);
            }
        }
        class ShaderColorValue : ShaderValue
        {
            public ShaderColorValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetColor(n);
            }
            public Color value = Color.black;
            public override void SetValue(Material mat)
            {
                mat.SetColor(name, value);
            }
        }
        class ShaderTexValue : ShaderValue
        {
            public ShaderTexValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetTexture(n);
                offset = mat.GetTextureOffset(n);
                scale = mat.GetTextureScale(n);
            }
            public Texture value = null;
            public Vector2 offset = Vector2.zero;
            public Vector2 scale = Vector2.one;

            public override void SetValue(Material mat)
            {
                mat.SetTexture(name, value);
                mat.SetTextureOffset(name, offset);
                mat.SetTextureScale(name, scale);
            }
        }
        [MenuItem(@"Assets/Tool/Shader/BuildBundle")]
        private static void BuildBundle()
        {
            string targetDir = null;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                targetDir = string.Format("Assets/StreamingAssets/update/Android/AssetBundles/");
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                targetDir = string.Format("Assets/StreamingAssets/update/iOS/AssetBundles/");
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
            {
                targetDir = string.Format("Assets/StreamingAssets/update/AssetBundles/");
            }
            else
            {
                return;
            }
            
            string[] shaderPath = new string[] {
                "Assets/Resources/Shader/Skin/Skin-Effect.shader",
                "Assets/Resources/Shader/Skin/Skin-RimLightBlend8.shader",
                "Assets/Resources/Shader/Skin/Skin-RimLightBlend.shader",
                "Assets/Resources/Shader/Skin/Skin-RimLightBlendCutout.shader",
                "Assets/Resources/Shader/Skin/Skin-RimLightBlendCutout4.shader",
                "Assets/Resources/Shader/Common/FadeMaskRNoLight.shader",
                "Assets/Resources/Shader/Common/TransparentGrayMaskRNoLight.shader",
                "Assets/Resources/Shader/Shadow/CustomShadowR.shader",
                "Assets/Resources/Shader/PostEffects/RadialBlur.shader",
                "Assets/Resources/Shader/PostEffects/BlackWhite.shader",
                "Assets/Resources/Shader/PostEffects/BlurEffectConeTaps.shader",
                "Assets/Resources/Shader/NGUI/SeparateColorAlpha.shader",
                "Assets/Resources/Shader/NGUI/Text.shader",
                "Assets/Resources/Shader/NGUI/Additive.shader",
                "Assets/Resources/Shader/NGUI/ColorTexture.shader",
                "Assets/Resources/Shader/NGUI/RGBA.shader",
                "Assets/Resources/Shader/NGUI/Mask.shader",
                "Assets/Resources/Shader/NGUI/RenderTexture.shader",
                "Assets/Resources/Shader/NGUI/WhiteAnim.shader",
                "Assets/Resources/Shader/NGUI/TextureList2.shader",
                "Assets/Resources/Shader/NGUI/TextureList4.shader",
                "Assets/Resources/Shader/NGUI/Merge.shader",
                "Assets/Resources/Shader/FxPro/BloomPro.shader",
                "Assets/Resources/Shader/FxPro/DOFPro.shader",
                "Assets/Resources/Shader/FxPro/FxPro.shader",
                "Assets/Resources/Shader/FxPro/FxProTap.shader",
                 "Assets/Resources/Shader/FxPro/MobileDOFPro.shader"};

            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = "shaders.ab";
            abb.assetBundleVariant = "";
            abb.assetNames = shaderPath;
            testAnimBundle[0] = abb;
            BuildPipeline.BuildAssetBundles(targetDir, testAnimBundle, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);


        }

        #endregion
        #region Table
        public static void PostImportAssets(string[] importedAssets, string[] deletedAssets, bool warning)
        {
            bool deal = false;
            string tables = "";
            if (importedAssets != null)
            {
                for (int i = 0; i < importedAssets.Length; ++i)
                {
                    string path = importedAssets[i];
                    string tableName = GetTableName(path);
                    if (tableName != "")
                    {
                        tables += tableName + " ";
                    }
                }
                if (tables != "")
                {
                    ExeTable2Bytes(tables);
                    deal = true;
                }

            }
            for (int i = 0; i < deletedAssets.Length; ++i)
            {
                string tableName = GetTableName(deletedAssets[i]);

                if (tableName != "")
                {
                    string des = "Assets/Resources/Table/" + tableName + ".bytes";
                    if (File.Exists(des))
                    {
                        File.Delete(des);
                        deal = true;
                    }
                }
            }
            if (deal)
            {
                AssetDatabase.Refresh();
            }
        }
        private static string GetTableName(string tablePath)
        {
            if (tablePath.StartsWith("Assets/Table/") && tablePath.EndsWith(".txt"))
            {
                string tableName = tablePath.Replace("Assets/Table/", "");
                tableName = tableName.Replace(".txt", "");
                return tableName.Replace("\\", "/");
            }
            return "";
        }
        private static string GetTableName(UnityEngine.Object target)
        {
            return GetTableName(AssetDatabase.GetAssetPath(target));
        }

        private static void ExeTable2Bytes(string tables, string arg0 = "-q -tables ")
        {
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process exep = new System.Diagnostics.Process();
            exep.StartInfo.FileName = @"..\XProject\Shell\Table2Bytes.exe";
            exep.StartInfo.Arguments = arg0 + tables;
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;
            exep.Start();
            string output = exep.StandardOutput.ReadToEnd();
            exep.WaitForExit();
            if (output != "")
            {
                int errorIndex = output.IndexOf("error:");
                if(errorIndex>=0)
                {
                    string errorStr = output.Substring(errorIndex);
                    Debug.LogError(errorStr);
                    Debug.Log(output.Substring(0, errorIndex));
                }
                else
                {
                    Debug.Log(output);
                }
            }
            AssetDatabase.Refresh();
#endif
        }

        public static void Table2Bytes(UnityEngine.Object target)
        {
#if UNITY_EDITOR_WIN
            string tableName = GetTableName(target);
            if (tableName != "")
            {
                ExeTable2Bytes(tableName);
            }
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }
        public static void Bytes2Txt(UnityEngine.Object target)
        {
#if UNITY_EDITOR_WIN
            string tableName = GetTableName(target);
            if (tableName != "")
            {
                ExeTable2Bytes(tableName);
            }
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }
        public static void Table2Bytes(UnityEngine.Object[] targets)
        {
#if UNITY_EDITOR_WIN
            if (targets != null)
            {
                string tables = "";
                for (int i = 0; i < targets.Length; ++i)
                {
                    string tableName = AssetDatabase.GetAssetPath(targets[i]);
                    if (tableName != "")
                    {
                        tables += tableName + " ";
                    }
                }
                if (tables != "")
                {
                    ExeTable2Bytes(tables);
                }
            }
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
#endif
        }
        public static void AllTable2Bytes()
        {
            ExeTable2Bytes("");
            EditorUtility.DisplayDialog("Finish", "All tables processed finish", "OK");
        }
        #endregion
        #region res
        [MenuItem(@"Assets/Tool/Res/FindAsset", false, 0)]
        private static void FindAsset()
        {
            AssetFindEditor window = EditorWindow.GetWindow<AssetFindEditor>("查找资源", true);
            window.position = new Rect(100, 100, 1200, 800);
        }

        [MenuItem(@"Assets/Tool/Res/LowPolygon", false, 0)]
        private static void LowPolygon()
        {
            //Vector3[] oldVerts = mesh.vertices;
            //Vector4[] oldTangents = mesh.tangents;
            //Vector2[] oldUVs = mesh.uv;
            //int[] triangles = mesh.triangles;
            //Vector3[] newVerts = new Vector3[triangles.Length];
            //Vector4[] newTangents = new Vector4[triangles.Length];
            //Vector2[] newUVs = new Vector2[triangles.Length];
            //for (int i = 0; i < triangles.Length; i++)
            //{
            //    newVerts[i] = oldVerts[triangles[i]];
            //    newTangents[i] = oldTangents[triangles[i]];
            //    newUVs[i] = oldUVs[triangles[i]];
            //    triangles[i] = i;
            //}
            //mesh.vertices = newVerts;
            //mesh.tangents = newTangents;
            //mesh.triangles = triangles;
            //mesh.uv = newUVs;
            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();
        }
        #endregion

        #region utility
        public static string GetFolder(string path)
        {
            path = path.Replace("\\", "/");
            int index = path.LastIndexOf("/");
            if(index>=0)
            {
                path = path.Substring(0, index);
            }
            return path;
        }

        public static string MakeAssetPath(string path,string newName,string ext)
        {
            path = GetFolder(path);
            return string.Format("{0}/{1}.{2}", path, newName, ext);
        }
        public static string MakeAssetPathWithFolder(string path, string newName, string ext)
        {
            return string.Format("{0}/{1}.{2}", path, newName, ext);
        }
        #endregion
        [MenuItem(@"Assets/Tool/Test/Test", false, 0)]
        private static void Test()
        {
            string shaderTmpDir = @"..\XProject\ShaderTmp";
            if (Directory.Exists(shaderTmpDir))
            {
                Directory.Delete(@"..\XProject\ShaderTmp", true);
            }
            Directory.CreateDirectory(shaderTmpDir);
            string shaderDir = "XProject/Shader/Scene";
            string replaceShaderDir = "Assets/Resources/Shader/Scene";
            DirectoryInfo shaderReplaceDir = new DirectoryInfo(@"..\XProject\Shader\Scene");
            FileInfo[] files = shaderReplaceDir.GetFiles("*.shader", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string path = fi.FullName.Replace("\\", "/");
                int index = path.IndexOf(shaderDir);
                string shaderPath = path.Substring(index + shaderDir.Length);
                string replaceShaderPath = replaceShaderDir + shaderPath;
                string tmpShaderPath = shaderTmpDir + shaderPath;
                Debug.LogWarning(tmpShaderPath);
                File.Copy(replaceShaderPath, tmpShaderPath);
            }
        }
    }
    public class PlayStateChange
    {
        private bool init = false;
        private bool isPlaying = false;
        public bool isStateChange = false;
        public bool Update()
        {
            if(init)
            {
                isStateChange = isPlaying!= Application.isPlaying;
                isPlaying = Application.isPlaying;
            }
            else
            {
                isPlaying = Application.isPlaying;
                isStateChange = true;
                init = true;
            }
            return isStateChange;
        }
        public void PostUpdate()
        {
            isStateChange = false;
        }
    }
    public class ModelBone
    {
        public bool Check = false;
        public string BoneName = "";
        public string Path = "";
        public ModelBone Parent = null;
        public List<ModelBone> Child = new List<ModelBone>();
    }

    public class SelectBones : EditorWindow
    {
        protected GameObject model = null;
        protected string prefabName = "";

        //protected bool isReadable = false;
        protected bool checkAll = false;
        protected GameObject newGo = null;

        protected List<string> exposedBones = new List<string>();
        protected List<string> exExposedBones = new List<string>();
        protected ModelBone root = new ModelBone();
        protected ModelImporter modelImporter;
        protected string path = "";
        protected Vector2 scrollPos = Vector2.zero;
        protected string creatorFolderName = "";
        protected bool gameResource = true;
        protected string prefabRootPath = "Assets/Resources/Prefabs/";
        public virtual void Init()
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            if (objs != null && objs.Length > 0)
            {
                model = objs[0] as GameObject;
                path = "";
                if (model != null)
                {
                    path = AssetDatabase.GetAssetPath(model);
                    modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                    //revert to default
                    modelImporter.optimizeGameObjects = false;
                    modelImporter.extraExposedTransformPaths = null;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    if (prefabName == "")
                    {
                        prefabName = model.name.ToLower();
                        int index = prefabName.IndexOf("_bandpose");
                        if (index >= 0)
                        {
                            prefabName = prefabName.Substring(0, index);
                        }
                    }
                    FilterBone();
                }
            }
            AssetDatabase.Refresh();
        }
        public void AddExExposeBone(string bone)
        {
            if (!exExposedBones.Contains(bone))
                exExposedBones.Add(bone);
        }
        protected virtual void FilterBone()
        {
            //find skill bone
            SearchSkill(model, path, exposedBones);
            //find bone Hierarchy
            root.BoneName = model.name;
            FindModelBones(model.transform, "", root);
        }


        private void SearchSkill(GameObject go, string fbxfilename, List<string> bones)
        {
            string assetPath = "";
            if (creatorFolderName != "")
            {
                assetPath = Application.dataPath + "/Resources/SkillPackage/" + creatorFolderName;
            }
            else
            {
                string foldername = System.IO.Path.GetDirectoryName(fbxfilename);
                assetPath = Application.dataPath + "/Resources/SkillPackage/" + System.IO.Path.GetFileName(foldername);
            }


            if (!Directory.Exists(assetPath))
                return;

            DirectoryInfo di = new DirectoryInfo(assetPath);

            FileInfo[] fis = di.GetFiles("*.txt");

            foreach (FileInfo s in fis)
            {
                XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData(s.FullName);
                foreach (XFxData fx in skill.Fx)
                {
                    if (!string.IsNullOrEmpty(fx.Bone))
                    {
                        string boneName = fx.Bone;
                        if (go.transform.Find(boneName) != null && !bones.Contains(boneName))
                        {
                            bones.Add(boneName);
                            Debug.Log(string.Format("Skill:{0} Bone:{1}", s.Name, fx.Bone));
                        }

                    }
                }
            }

            return;
        }

        private ModelBone FindModelBones(Transform t, string parentPath, ModelBone parent)
        {
            ModelBone modelBone = null;
            if (t != model.transform)
            {
                modelBone = new ModelBone();
            }
            if (modelBone != null)
            {
                //SkinnedMeshRenderer skin = t.GetComponent<SkinnedMeshRenderer>();
                //MeshRenderer mesh = t.GetComponent<MeshRenderer>();
                modelBone.BoneName = t.name;
                modelBone.Path = parentPath + t.name;
                modelBone.Check = exposedBones.Contains(modelBone.Path) || exExposedBones.Contains(t.name);
                modelBone.Parent = parent;
            }
            if (t != model.transform)
                parentPath = parentPath + t.name + "/";
            for (int i = 0; i < t.childCount; ++i)
            {
                ModelBone childModelBone = FindModelBones(t.GetChild(i), parentPath, modelBone);
                if (modelBone != null)
                    modelBone.Child.Add(childModelBone);
                else
                    parent.Child.Add(childModelBone);
            }
            return modelBone;
        }

        private void FindComponentSkinnedMesh(Transform t, List<SkinnedMeshRenderer> components)
        {
            SkinnedMeshRenderer comp = t.GetComponent<SkinnedMeshRenderer>();
            if (comp != null)
            {
                components.Add(comp);
            }
            for (int i = 0; i < t.childCount; ++i)
            {
                FindComponentSkinnedMesh(t.GetChild(i), components);
            }
        }

        private void FindComponentMesh(Transform t, List<MeshRenderer> components)
        {
            MeshRenderer comp = t.GetComponent<MeshRenderer>();
            if (comp != null)
            {
                components.Add(comp);
            }
            for (int i = 0; i < t.childCount; ++i)
            {
                FindComponentMesh(t.GetChild(i), components);
            }
        }

        private void DrawTree(ModelBone modelBone, int depth)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < depth; ++i)
                GUILayout.Label(" ", GUILayout.ExpandWidth(false));
            modelBone.Check = EditorGUILayout.Toggle(modelBone.Check, GUILayout.Width(30));
            GUILayout.Label(modelBone.BoneName, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                DrawTree(modelBone.Child[i], depth + 1);
            }
        }

        private void ExposedBone(ModelBone modelBone)
        {
            if (modelBone.Check && !exposedBones.Contains(modelBone.Path))
            {
                exposedBones.Add(modelBone.Path);
            }
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                ExposedBone(modelBone.Child[i]);
            }
        }

        private void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; ++i)
            {
                SetLayer(t.GetChild(i), layer);
            }
        }

        private bool CopyValue<T>(GameObject srcGO, GameObject destGO) where T : Component
        {
            T srcCC = srcGO.GetComponent<T>();
            if (srcCC == null)
                return false;

            T destCC = destGO.GetComponent<T>();
            if (destCC == null)
                destCC = destGO.AddComponent<T>();

            UnityEditorInternal.ComponentUtility.CopyComponent(srcCC);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(destCC);

            return true;
        }

        private void CheckAll(ModelBone modelBone)
        {
            modelBone.Check = checkAll;
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                CheckAll(modelBone.Child[i]);
            }
        }
        protected virtual void MakeGameObject()
        {
            ExposedBone(root);
            if (checkAll)
            {
                modelImporter.optimizeGameObjects = false;
                modelImporter.isReadable = false;
            }
            else
            {
                modelImporter.optimizeGameObjects = true;
                modelImporter.isReadable = false;
                modelImporter.extraExposedTransformPaths = exposedBones.ToArray();
            }
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            List<SkinnedMeshRenderer> srcSkins = new List<SkinnedMeshRenderer>();
            List<MeshRenderer> srcMeshs = new List<MeshRenderer>();


            string prefabPath = prefabRootPath + prefabName + ".prefab";

            GameObject srcPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (srcPrefab != null)
            {
                FindComponentSkinnedMesh(srcPrefab.transform, srcSkins);
                FindComponentMesh(srcPrefab.transform, srcMeshs);
            }

            newGo = UnityEngine.Object.Instantiate(model) as GameObject;
            if (newGo != null)
            {
                newGo.name = prefabName;
                List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>();
                FindComponentSkinnedMesh(newGo.transform, skins);
                foreach (SkinnedMeshRenderer skin in skins)
                {
                    AssetModify.ProcessSkinMesh(skin);
                }
                List<MeshRenderer> meshs = new List<MeshRenderer>();
                FindComponentMesh(newGo.transform, meshs);
                foreach (MeshRenderer mesh in meshs)
                {
                    AssetModify.ProcessRender(mesh);
                }
                Animator srcAni = null;
                if (srcPrefab != null)
                {
                    CopyValue<CharacterController>(srcPrefab, newGo);
                    if (CopyValue<BoxCollider>(srcPrefab, newGo))
                    {
                        CopyValue<Rigidbody>(srcPrefab, newGo);
                    }
                    SetLayer(newGo.transform, srcPrefab.layer);
                    srcAni = srcPrefab.GetComponent<Animator>();
                    for (int i = 0; i < srcPrefab.transform.childCount; ++i)
                    {
                        Transform child = srcPrefab.transform.GetChild(i);
                        if (child.name.ToLower() == "hugemonstercolliders")
                        {
                            GameObject childGo = GameObject.Instantiate(child.gameObject) as GameObject;
                            childGo.name = "HugeMonsterColliders";
                            childGo.SetActive(false);
                            childGo.transform.parent = newGo.transform;
                        }
                        else if (child.name.StartsWith("Ty_"))
                        {
                            GameObject childGo = GameObject.Instantiate(child.gameObject) as GameObject;
                            childGo.name = child.name;
                            childGo.transform.parent = newGo.transform;
                        }
                    }
                    UnityEditor.Selection.activeObject = srcPrefab;
                }
                Animator desAni = newGo.GetComponent<Animator>();
                if (desAni != null)
                {
                    if (srcAni != null && srcAni.runtimeAnimatorController != null)
                    {
                        desAni.runtimeAnimatorController = Resources.Load("Controller/" + srcAni.runtimeAnimatorController.name) as RuntimeAnimatorController;
                    }
                    else
                    {
                        desAni.runtimeAnimatorController = Resources.Load("Controller/XAnimator") as RuntimeAnimatorController;
                    }
                }
                AssetModify.FixPrefab(prefabPath, newGo, true);
            }
        }

        protected virtual void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
            DrawTree(root, 0);
            GUILayout.EndScrollView();
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
            //isReadable = EditorGUILayout.ToggleLeft("Is Readable", isReadable);
            bool beforeCheck = checkAll;
            checkAll = EditorGUILayout.ToggleLeft("Check All", checkAll);
            if (beforeCheck != checkAll)
            {
                CheckAll(root);
            }
            gameResource = EditorGUILayout.ToggleLeft("Game Resource", gameResource);
            if (GUILayout.Button("OK", GUILayout.ExpandWidth(false)))
            {
                MakeGameObject();

                this.Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
            {
                this.Close();
            }
        }
    }

    public class SelectEquipBones : SelectBones
    {
        private string spritePoint = "sprite";
        private bool selectChar = false;
        private string charPrefab = "";
        private string selectCharPrefab = "";
        private CombineConfig combineConfig = null;
        public override void Init()
        {
            combineConfig = CombineConfig.GetConfig();
            base.Init();
        }
        protected override void FilterBone()
        {
            if (model != null)
            {
                TextAsset ta = Resources.Load<TextAsset>(@"Table/DefaultEquip");
                if (ta != null)
                {
                    DefaultEquip de = new DefaultEquip();
                    XBinaryReader.Init();
                    XBinaryReader reader = XBinaryReader.Get();
                    reader.Init(ta);
                    //using (MemoryStream ms = new System.IO.MemoryStream(ta.bytes))
                    {
                        de.ReadFile(reader);
                        int id = 0;
                        for (int i = 0; i < combineConfig.BandposeName.Length; ++i)
                        {
                            if (model.name.ToLower() == combineConfig.BandposeName[i].ToLower())
                            {
                                id = i + 1;
                                prefabName = combineConfig.PrefabName[i];
                                charPrefab = prefabName;
                                selectCharPrefab = combineConfig.CreateCharPrefabName[i];
                                creatorFolderName = combineConfig.SkillFolderName[i];
                            }
                        }
                        if (id > 0)
                        {
                            DefaultEquip.RowData data = de.GetByProfID(id);
                            if (data != null)
                            {
                                AddExExposeBone(data.WingPoint);
                                AddExExposeBone(data.TailPoint);
                                if (data.WeaponPoint != null && data.WeaponPoint.Length > 0)
                                    AddExExposeBone(data.WeaponPoint[0]);
                                if (data.WeaponPoint != null && data.WeaponPoint.Length > 1)
                                    AddExExposeBone(data.WeaponPoint[1]);
                                AddExExposeBone(data.FishingPoint);
                                AddExExposeBone(data.FishingPoint);
                            }
                        }
                    }
                    XBinaryReader.Return(reader);
                }
            }
            //isReadable = false;
            base.FilterBone();
        }

        protected override void MakeGameObject()
        {
            if (!gameResource)
                prefabRootPath = "Assets/Editor/EditorResources/Prefabs/";
            base.MakeGameObject();
            if (newGo != null)
            {
                GameObject spriteGo = new GameObject(spritePoint);
                spriteGo.transform.parent = newGo.transform;
                spriteGo.layer = newGo.layer;
                if (gameResource)
                {
                    int count = newGo.transform.childCount;
                    for (int i = count - 1; i >= 0; --i)
                    {
                        Transform child = newGo.transform.GetChild(i);
                        if (child.GetComponent<SkinnedMeshRenderer>() != null ||
                            child.GetComponent<MeshRenderer>() != null)
                        {
                            GameObject.DestroyImmediate(child.gameObject);
                        }
                    }

                    SkinnedMeshRenderer smr = newGo.GetComponent<SkinnedMeshRenderer>();
                    if (smr == null)
                        smr = newGo.AddComponent<SkinnedMeshRenderer>();
                    AssetModify.ProcessSkinMesh(smr);
                    smr.rootBone = newGo.transform;
                    smr.localBounds = new Bounds(new Vector3(0, 0.5f, 0), new Vector3(0.5f, 1.0f, 0.5f));
                    newGo.layer = newGo.layer;
                }
                AssetModify.FixPrefab(prefabRootPath + prefabName + ".prefab", newGo, true);
            }
        }
        protected override void OnGUI()
        {
            bool select = EditorGUILayout.ToggleLeft("Is Select Char", selectChar);
            if (select != selectChar)
            {
                selectChar = select;
                prefabName = selectChar ? selectCharPrefab : charPrefab;

            }
            base.OnGUI();
        }
    }
    public class TextureStatis : EditorWindow
    {
        //class TextureInfo
        //{
        //    public Texture tex;
        //    public GameObject go;
        //}
        private Vector2 scrollPosition = Vector2.zero;
        private List<Texture> textures = new List<Texture>();
        private List<UISprite> sprite = new List<UISprite>();
        private List<UITexture> texs = new List<UITexture>();
        //private List<UITexture> texs = new List<UITexture>();
        public void ScanGameObject()
        {
            textures.Clear();
            GameObject go = Selection.activeGameObject;
            if (go != null)
            {
                sprite.Clear();
                go.GetComponentsInChildren<UISprite>(true, sprite);
                foreach (UISprite s in sprite)
                {
                    if (s != null && s.atlas != null && !textures.Contains(s.atlas.texture))
                    {
                        textures.Add(s.atlas.texture);
                    }
                }
                texs.Clear();
                go.GetComponentsInChildren<UITexture>(true, texs);
                foreach (UITexture t in texs)
                {
                    if (t != null && t.mainTexture != null && !textures.Contains(t.mainTexture))
                        textures.Add(t.mainTexture);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Scan", GUILayout.MaxWidth(150)))
            {
                ScanGameObject();
            }
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            for (int i = 0; i < textures.Count; ++i)
            {
                Texture tex = textures[i];
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(tex.name, tex, typeof(Texture2D), true, GUILayout.MaxWidth(250));
                GUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();
        }
    }

    public class TextureCommonCompress : EditorWindow
    {
        public enum ETextureSize
        {
            X32 = 32,
            X64 = 64,
            X128 = 128,
            X256 = 256,
            X512 = 512,
            X1024 = 1024,

        }
        public enum ETextureCompress
        {
            ECompress,
            E16,
            E32,
            EA8
        }
        protected ETextureSize compressSize = ETextureSize.X64;
        protected TextureImporterFormat iosFormat = TextureImporterFormat.PVRTC_RGB4;
        protected TextureImporterFormat androidFormat = TextureImporterFormat.ETC_RGB4;
        protected ETextureCompress compressType = ETextureCompress.ECompress;
        protected bool genMipmap = false;
        protected TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        protected bool genAlpha = false;
        protected bool genRAlpha = true;
        protected ETextureSize alphaSize = ETextureSize.X64;

        private bool _TextureCompress(Texture2D tex, TextureImporter textureImporter, string path)
        {
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.anisoLevel = 0;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.isReadable = false;
            textureImporter.wrapMode = wrapMode;
            textureImporter.mipmapEnabled = genMipmap;
            switch (compressType)
            {
                case ETextureCompress.ECompress:
                    iosFormat = TextureImporterFormat.PVRTC_RGB4;
                    androidFormat = TextureImporterFormat.ETC_RGB4;
                    break;
                case ETextureCompress.E16:
                    if (AssetModify.HasAlpha(tex, textureImporter))
                    {
                        iosFormat = TextureImporterFormat.RGBA32;
                        androidFormat = TextureImporterFormat.RGBA16;
                    }
                    else
                    {
                        iosFormat = TextureImporterFormat.RGB24;
                        androidFormat = TextureImporterFormat.RGB16;
                    }
                    break;
                case ETextureCompress.E32:
                    if (AssetModify.HasAlpha(tex, textureImporter))
                    {
                        iosFormat = TextureImporterFormat.RGBA32;
                        androidFormat = TextureImporterFormat.RGBA32;
                    }
                    else
                    {
                        iosFormat = TextureImporterFormat.RGB24;
                        androidFormat = TextureImporterFormat.RGB24;
                    }
                    break;
                case ETextureCompress.EA8:
                    iosFormat = TextureImporterFormat.Alpha8;
                    androidFormat = TextureImporterFormat.Alpha8;
                    break;
            }
            AssetModify.SetTexImportSetting(textureImporter, "Standalone", (int)compressSize, genAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1);

            if (genAlpha)
            {
                int extIndex = path.LastIndexOf(".");
                if (extIndex >= 0)
                {
                    string alphaTexPath = path.Substring(0, extIndex) + "_A.png";

                    if (genRAlpha)
                    {
                        AssetModify.SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), (int)compressSize, TextureImporterFormat.RGBA32);
                        AssetModify.SetTexImportSetting(textureImporter, "iPhone", (int)compressSize, TextureImporterFormat.RGBA32);
                        textureImporter.isReadable = true;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        AssetModify.MakeAlphaRTex(alphaTexPath, (int)alphaSize, tex);
                        AssetModify.SetTexImportSetting(textureImporter, BuildTarget.Android.ToString(), (int)compressSize, androidFormat);
                        AssetModify.SetTexImportSetting(textureImporter, "iPhone", (int)compressSize, iosFormat);
                        textureImporter.isReadable = false;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                    else
                    {
                        File.Copy(path, alphaTexPath, true);
                        AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
                        TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaTexPath) as TextureImporter;
                        if (alphaTextureImporter != null)
                        {
                            alphaTextureImporter.textureType = TextureImporterType.Default;
                            alphaTextureImporter.anisoLevel = 0;
                            alphaTextureImporter.mipmapEnabled = false;
                            alphaTextureImporter.isReadable = false;
                            alphaTextureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                            AssetModify.SetTexImportSetting(alphaTextureImporter, BuildTarget.Android.ToString(), (int)alphaSize, TextureImporterFormat.Alpha8);
                            AssetModify.SetTexImportSetting(alphaTextureImporter, "iPhone", (int)alphaSize, TextureImporterFormat.Alpha8);
                            AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
                        }
                    }
                }
            }
            return true;

        }
        private void Compress()
        {
            AssetModify.enumTex2D.cb = _TextureCompress;
            AssetModify.EnumAsset<Texture2D>(AssetModify.enumTex2D, "TextureCompress");
        }
        private void OnGUI()
        {
            //GUILayout.BeginHorizontal();

            if (GUILayout.Button("Compress", GUILayout.MaxWidth(150)))
            {
                Compress();
            }

            compressSize = (ETextureSize)EditorGUILayout.EnumPopup("缩放", compressSize);
            compressType = (ETextureCompress)EditorGUILayout.EnumPopup("压缩格式", compressType);
            wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("采样模式", wrapMode);
            genMipmap = EditorGUILayout.ToggleLeft("GenMipmap", genMipmap);
            genAlpha = EditorGUILayout.ToggleLeft("GenAlpha", genAlpha);
            if (genAlpha)
            {
                genRAlpha = EditorGUILayout.ToggleLeft("Gen R Channel Alpha", genRAlpha);
                alphaSize = (ETextureSize)EditorGUILayout.EnumPopup("alpha缩放", alphaSize);
            }
        }
    }
    public enum ETexChanel
    {
        R,
        G,
        B,
        A
    }
    public struct TexureImportInfo
    {
        public TextureImporterFormat format;
        public int texSize;
        public TextureWrapMode wrapMode;
        public TextureImporter textureImporter;
        public string path;
    }
    public class TextureCombine : EditorWindow
    {
        private Texture2D[] texs = new Texture2D[4];
        private ETexChanel[] texChanels = new ETexChanel[4];
        private TexureImportInfo[] importInfo = new TexureImportInfo[4];
        private TextureImporterFormat desAndroidFormat = TextureImporterFormat.RGBA16;
        private TextureImporterFormat desIOSFormat = TextureImporterFormat.RGBA16;
        private int width = 256;
        private int height = 256;
        private string namepath = "";
        private Vector2 scrollPosition = Vector2.zero;

        private float GetChanel(Texture2D tex, ETexChanel chanel, int x, int y)
        {
            if (tex == null)
                return 0.0f;
            Color c = tex.GetPixel(x, y);
            if (chanel == ETexChanel.R)
                return c.r;
            if (chanel == ETexChanel.G)
                return c.g;
            if (chanel == ETexChanel.B)
                return c.b;
            if (chanel == ETexChanel.A)
                return c.a;
            return 0.0f;
        }

        private void Combine()
        {
            if (namepath != "")
            {
                //Texture2D[] tmpTex = new Texture2D[4];
                for (int i = 0, imax = texs.Length; i < imax; ++i)
                {
                    Texture2D tex = texs[i];
                    if (tex != null)
                    {
                        string path = AssetDatabase.GetAssetPath(tex);
                        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (textureImporter != null)
                        {
                            TexureImportInfo tii;

                            tii.wrapMode = textureImporter.wrapMode;
                            tii.textureImporter = textureImporter;
                            tii.path = path;
                            int texSize = 1024;
                            TextureImporterFormat format;
                            textureImporter.wrapMode = TextureWrapMode.Clamp;
                            textureImporter.GetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), out texSize, out format);
                            AssetModify.SetTexImportSetting(textureImporter, "", texSize, TextureImporterFormat.Alpha8);

                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                            tii.format = format;
                            tii.texSize = texSize;
                            importInfo[i] = tii;
                        }
                    }
                }
                Material mat = new Material(Shader.Find("Custom/Editor/CombineTex"));
                for (int i = 0, imax = texs.Length; i < imax; ++i)
                {
                    mat.SetTexture("_Tex" + i.ToString(), texs[i]);
                    ETexChanel c = texChanels[i];
                    Vector4 chanelMask = Vector4.zero;
                    if (c == ETexChanel.R)
                    {
                        chanelMask = new Vector4(1, 0, 0, 0);
                    }
                    else if (c == ETexChanel.G)
                    {
                        chanelMask = new Vector4(0, 1, 0, 0);
                    }
                    else if (c == ETexChanel.B)
                    {
                        chanelMask = new Vector4(0, 0, 1, 0);
                    }
                    else if (c == ETexChanel.A)
                    {
                        chanelMask = new Vector4(0, 0, 0, 1);
                    }
                    mat.SetVector("_ChanelMask" + i.ToString(), chanelMask);
                }
                RenderTexture current = RenderTexture.active;
                RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                Graphics.SetRenderTarget(rt);
                // Set up the simple Matrix
                GL.PushMatrix();
                GL.LoadOrtho();
                mat.SetPass(0);
                GL.Begin(GL.QUADS);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
                GL.End();
                GL.PopMatrix();

                Texture2D des = new Texture2D(width, height);
                des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                Graphics.SetRenderTarget(current);
                rt.Release();
                GameObject.DestroyImmediate(mat);
                byte[] bytes = des.EncodeToPNG();

                File.WriteAllBytes(namepath, bytes);
                AssetDatabase.ImportAsset(namepath, ImportAssetOptions.ForceUpdate);

                TextureImporter desTexImporter = AssetImporter.GetAtPath(namepath) as TextureImporter;
                if (desTexImporter != null)
                {
                    int size = width > height ? width : height;
                    desTexImporter.textureType = TextureImporterType.Default;
                    desTexImporter.anisoLevel = 0;
                    desTexImporter.mipmapEnabled = false;
                    desTexImporter.isReadable = false;
                    desTexImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                    AssetModify.SetTexImportSetting(desTexImporter, BuildTarget.Android.ToString(), size, desAndroidFormat);
                    AssetModify.SetTexImportSetting(desTexImporter, "iPhone", size, desIOSFormat);
                    AssetDatabase.ImportAsset(namepath, ImportAssetOptions.ForceUpdate);
                }
                for (int i = 0, imax = texs.Length; i < imax; ++i)
                {
                    Texture2D tex = texs[i];
                    if (tex != null)
                    {
                        TexureImportInfo tii = importInfo[i];
                        AssetModify.SetTexImportSetting(tii.textureImporter, EditorUserBuildSettings.activeBuildTarget.ToString(), tii.texSize, tii.format);
                        tii.textureImporter.wrapMode = tii.wrapMode;
                        AssetDatabase.ImportAsset(tii.path, ImportAssetOptions.ForceUpdate);
                    }
                }
                EditorUtility.DisplayDialog("Finish", "Combine Finish", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Empty file path", "OK");
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Combine", GUILayout.MaxWidth(150)))
            {
                Combine();
            }
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            for (int i = 0, imax = texs.Length; i < imax; ++i)
            {
                Texture2D tex = texs[i];
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tex != null ? tex.name : "Empty");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ETexChanel c = (ETexChanel)i;

                texs[i] = EditorGUILayout.ObjectField("Chanel" + c, tex, typeof(Texture2D), true, GUILayout.MaxWidth(250)) as Texture2D;
                if (texs[i] != null && namepath == "")
                {
                    namepath = AssetDatabase.GetAssetPath(texs[i]);
                    int index = namepath.LastIndexOf(".");
                    if (index > 0)
                        namepath = namepath.Substring(0, index);
                    namepath += ".png";
                }
                texChanels[i] = (ETexChanel)EditorGUILayout.EnumPopup("通道", texChanels[i]);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            namepath = EditorGUILayout.TextField("Asset Path", namepath);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            width = EditorGUILayout.IntField("Width", width);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            height = EditorGUILayout.IntField("Height", height);
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

        }

        public static void ScaleTexture(Texture src, string despath, int width, int height, string shaderName, RenderTextureFormat renderFormat = RenderTextureFormat.ARGB32)
        {
            Material mat = new Material(Shader.Find(shaderName));
            mat.mainTexture = src;
            RenderTexture rt = new RenderTexture(width, height, 0, renderFormat);
            RenderTexture current = RenderTexture.active;
            Graphics.SetRenderTarget(rt);
            // Set up the simple Matrix
            GL.wireframe = true;
            GL.PushMatrix();
            GL.LoadOrtho();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
            GL.End();
            GL.PopMatrix();

            Texture2D des = new Texture2D(width, height);
            des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            Graphics.SetRenderTarget(current);
            rt.Release();
            GL.wireframe = false;
            GameObject.DestroyImmediate(mat);
            if (File.Exists(despath))
            {
                File.Delete(despath);
                AssetDatabase.Refresh();
            }
            byte[] bytes = des.EncodeToPNG();

            File.WriteAllBytes(despath, bytes);
            AssetDatabase.ImportAsset(despath, ImportAssetOptions.ForceUpdate);
        }
        public static void ScaleTexture(Texture src, string despath, Vector2 tile, Vector2 offset, string shaderName)
        {
            Shader shader = Shader.Find(shaderName);
            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", src);
            mat.SetTextureScale("_MainTex", tile);
            mat.SetTextureOffset("_MainTex", offset);
            RenderTexture rt = RenderTexture.GetTemporary(src.width, src.width, 0, RenderTextureFormat.ARGB32);
            RenderTexture current = RenderTexture.active;
            Graphics.SetRenderTarget(rt);
            // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
            GL.End();
            GL.PopMatrix();
            
            Texture2D des = new Texture2D(src.width, src.height);
            des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            des.Apply();
            Graphics.SetRenderTarget(current);
            GameObject.DestroyImmediate(mat);
            if (File.Exists(despath))
            {
                File.Delete(despath);
                AssetDatabase.Refresh();
            }
            byte[] bytes = des.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(des);
            RenderTexture.ReleaseTemporary(rt);
            File.WriteAllBytes(despath, bytes);
            //AssetDatabase.ImportAsset(despath, ImportAssetOptions.ForceUpdate);
        }
        private static void Reimport(string texPath, Texture2D tex)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(texPath, bytes);
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;
            AssetModify.DefaultCompress(tex, texImporter, texPath, !texPath.Contains("atlas/UI/"));
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
        }
        private static void Split(Texture2D src, TextureImporter textureImporter, int scale, int size, int width, int height, bool horizontal, string targetDir,bool createNew)
        {
            int splitCount = scale;
            string[] splitTexPath = new string[splitCount];
            Texture2D[] splitTex = new Texture2D[splitCount];
            for (int i = 0; i < splitCount; ++i)
            {
                if (createNew)
                {
                    splitTexPath[i] = string.Format("{0}/{1}_{2}.png", targetDir, src.name, i);
                }
                else
                {
                    if (i == 0)
                    {
                        splitTexPath[i] = string.Format("{0}/{1}_{2}2Split.png", targetDir, src.name, horizontal ? "h" : "v");
                    }
                    else
                    {
                        splitTexPath[i] = string.Format("{0}/{1}_{2}2Split_2.png", targetDir, src.name, horizontal ? "h" : "v");
                    }
                }
           
                

                splitTex[i] = new Texture2D(size, size, TextureFormat.RGBA32, false);
            }
            AssetModify.MakeTexReadable(src, textureImporter, true);

            if (horizontal)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index = x / size;
                    Texture2D tex = splitTex[index];
                    for (int y = 0; y < height; ++y)
                    {
                        Color c0 = src.GetPixel(x, y);
                        c0.a = 1;
                        tex.SetPixel(x, y, c0);
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; ++y)
                {
                    int index = y / size;
                    Texture2D tex = splitTex[index];
                    for (int x = 0; x < width; ++x)
                    {
                        Color c0 = src.GetPixel(x, y);
                        c0.a = 1;
                        tex.SetPixel(x, y, c0);
                    }
                }
            }
            AssetModify.MakeTexReadable(src, textureImporter, false);
            for (int i = 0; i < splitCount; ++i)
            {
                Reimport(splitTexPath[i], splitTex[i]);
            }
        }

        private static void Combine(Texture2D src, TextureImporter textureImporter, int size, int width, int height, bool horizontal, string targetDir, bool createNew)
        {
            string combineTexPath = null;
            if(createNew)
                combineTexPath = string.Format("{0}/{1}_0.png", targetDir, src.name);
            else
                combineTexPath = string.Format("{0}/{1}_{2}4Split.png", targetDir, src.name, horizontal ? "h" : "v");
            Texture2D combineTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            AssetModify.MakeTexReadable(src, textureImporter, true);

            if (horizontal)
            {
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        Color c0 = src.GetPixel(x, y);
                        c0.a = 1;
                        if (x >= size)
                        {
                            int newX = x - size;
                            int newY = y + size / 2;
                            combineTex.SetPixel(newX, newY, c0);
                        }
                        else
                        {
                            int newX = x;
                            int newY = y;
                            combineTex.SetPixel(newX, newY, c0);
                        }

                    }
                }
            }
            else
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        Color c0 = src.GetPixel(x, y);
                        c0.a = 1;
                        if (y >= size)
                        {
                            int newX = x + size / 2;
                            int newY = y - size;
                            combineTex.SetPixel(newX, newY, c0);
                        }
                        else
                        {
                            int newX = x;
                            int newY = y;
                            combineTex.SetPixel(newX, newY, c0);
                        }
                    }
                }
            }
            AssetModify.MakeTexReadable(src, textureImporter, false);
            Reimport(combineTexPath, combineTex);
        }
        public static void SplitTexture(Texture2D src, TextureImporter textureImporter, bool compress)
        {
            string path = AssetDatabase.GetAssetPath(src);
            string targetDir = path;
            bool createNew = true;
            if (path.StartsWith("Assets/atlas/UI"))
            {
                targetDir = path.Replace("Assets/", "Assets/Resources/");
                createNew = false;
            }
            int index = targetDir.LastIndexOf("/");
            if (index >= 0)
            {
                targetDir = targetDir.Substring(0, index);
            }

            int width = src.width;
            int height = src.height;
            int scaleW = width / height;
            int scaleH = height / width;
            if (scaleW == 2)
            {
                Split(src, textureImporter, scaleW, height, width, height, true, targetDir, createNew);
            }
            else if (scaleH == 2)
            {
                Split(src, textureImporter, scaleH, width, width, height, false, targetDir, createNew);
            }
            else if (scaleW == 4)
            {
                Combine(src, textureImporter, width / 2, width, height, true, targetDir, createNew);
            }
            else if (scaleH == 4)
            {
                Combine(src, textureImporter, height / 2, width, height, false, targetDir, createNew);
            }
            else
            {
                return;
            }
        }
    }

    //public class MakeEquip : EditorWindow
    //{
    //    //public class ExtraSkinMeshTex
    //    //{
    //    //    public XMeshTexData mtd;
    //    //    public XMeshMultiTexData mmtd;
    //    //    public string name = "";
    //    //    public Texture2D tex;
    //    //}

    //    public UnityEngine.Object model = null;
    //    protected UnityEngine.Object currentModel = null;
    //    protected Vector2 scrollPos = Vector2.zero;
    //    private string srcString = "01";
    //    private string replaceString = "02";
    //    private string modelPath = "";
    //    //private XMeshPartList meshPartList = new XMeshPartList();
    //    protected List<EquipPathInfo> meshParts = new List<EquipPathInfo>();
    //    protected List<ExtraMeshTex> meshList = new List<ExtraMeshTex>();
    //    protected string equipPath = "Assets/Resources/Equipments/";
    //    protected string weaponPath = "Assets/Resources/Equipments/weapon/";

    //    public void Init()
    //    {
    //        AssetModify.s_meshPartLis.Load(true);
    //        model = Selection.activeObject;
    //    }
    //    protected virtual void OnGUI()
    //    {
    //        GUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField(model != null ? model.name : "Empty");
    //        GUILayout.EndHorizontal();

    //        GUILayout.BeginHorizontal();
    //        equipPath = EditorGUILayout.TextField("equip path", equipPath);
    //        GUILayout.EndHorizontal();

    //        GUILayout.BeginHorizontal();
    //        weaponPath = EditorGUILayout.TextField("weapon path", weaponPath);
    //        GUILayout.EndHorizontal();

    //        GUILayout.BeginHorizontal();
    //        model = EditorGUILayout.ObjectField("", model, typeof(UnityEngine.Object), true, GUILayout.MaxWidth(250));
    //        if (currentModel != model)
    //        {
    //            currentModel = model;
    //            //ReplaceEquip.PrepareReplace(currentModel, meshParts, meshList, AssetModify.s_meshPartLis, out modelPath);
    //        }
    //        GUILayout.EndHorizontal();

    //        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
    //        for (int i = 0, imax = meshParts.Count; i < imax; ++i)
    //        {
    //            EquipPathInfo epi = meshParts[i];

    //            GUILayout.BeginHorizontal();
    //            EditorGUILayout.LabelField(epi.name);
    //            GUILayout.EndHorizontal();

    //            GUILayout.BeginHorizontal();
    //            GUILayout.BeginVertical();
    //            epi.newName = EditorGUILayout.TextField("", epi.newName, GUILayout.MaxWidth(300));
    //            GUILayout.EndVertical();

    //            GUILayout.BeginVertical();
    //            epi.newTexPath = EditorGUILayout.TextField(epi.newTexPath, GUILayout.MaxWidth(300));
    //            GUILayout.EndVertical();
    //            GUILayout.BeginVertical();
    //            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
    //            {
    //                epi.newName = "";
    //                epi.newTexPath = "";
    //            }
    //            GUILayout.EndVertical();
    //            GUILayout.EndHorizontal();
    //        }
    //        for (int i = 0, imax = meshList.Count; i < imax; ++i)
    //        {
    //            ExtraMeshTex emt = meshList[i];
    //            GUILayout.BeginHorizontal();
    //            EditorGUILayout.LabelField(emt.mesh != null ? emt.mesh.name : "");
    //            GUILayout.EndHorizontal();

    //            GUILayout.BeginHorizontal();
    //            GUILayout.BeginVertical();
    //            emt.newName = EditorGUILayout.TextField("", emt.newName, GUILayout.MaxWidth(300));
    //            GUILayout.EndVertical();

    //            GUILayout.BeginVertical();
    //            emt.newTexPath = EditorGUILayout.TextField("", emt.newTexPath, GUILayout.MaxWidth(300));
    //            GUILayout.EndVertical();
    //            GUILayout.BeginVertical();

    //            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
    //            {
    //                emt.newTexPath = "";
    //                emt.newName = "";
    //            }
    //            GUILayout.EndVertical();

    //            GUILayout.EndHorizontal();
    //        }


    //        GUILayout.EndScrollView();
    //        srcString = EditorGUILayout.TextField("Src String", srcString);
    //        replaceString = EditorGUILayout.TextField("Replace String", replaceString);
    //        //if (GUILayout.Button("Auto Gen", GUILayout.ExpandWidth(false)))
    //        //{
    //        //    for (int i = 0, imax = meshParts.Count; i < imax; ++i)
    //        //    {
    //        //        EquipPathInfo epi = meshParts[i];
    //        //        epi.newName = epi.name.Replace(srcString, replaceString);
    //        //        epi.newTexPath = epi.texPath.Replace(srcString, replaceString);                    
    //        //    }
    //        //    for (int i = 0, imax = meshList.Count; i < imax; ++i)
    //        //    {
    //        //        ExtraMeshTex emt = meshList[i];
    //        //        if (emt.mesh != null)
    //        //        {
    //        //            emt.name = emt.mesh.name.Replace(srcString, replaceString);
    //        //        }
    //        //        if (emt.srcMat != null)
    //        //        {
    //        //            Texture2D tex = emt.srcMat.mainTexture as Texture2D;
    //        //            if (tex != null)
    //        //            {
    //        //                string texName = tex.name.Replace(srcString, replaceString);
    //        //                emt.tex = AssetDatabase.LoadAssetAtPath(modelPath + texName + ".tga", typeof(Texture2D)) as Texture2D;
    //        //            }
    //        //        }
    //        //    }
    //        //}
    //        if (GUILayout.Button("Export", GUILayout.ExpandWidth(false)))
    //        {
    //            if (model != null)
    //            {
    //                string[] replaceStrings = new string[] { replaceString };
    //                if (ReplaceEquip.MakeReplace(srcString, replaceStrings, meshParts, meshList, AssetModify.s_meshPartLis, modelPath))
    //                {
    //                    AssetModify.SaveEquipInfo();
    //                    AssetDatabase.Refresh();
    //                }                   
    //            }
    //            EditorUtility.DisplayDialog("Finish", "All obj processed finish", "OK");
    //        }
    //        if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
    //        {
    //            this.Close();
    //        }
    //    }
    //}

    public class EquipPreview : EditorWindow
    {
        public class EquipData
        {
            public string name;
            public string path;
        }
        public class ProfessionEquip
        {
            public EquipData[] equipDataList = new EquipData[(int)EPartType.EWeaponEnd];
        }
        public class EquipSuit
        {
            public ProfessionEquip[] professionList = null;
            public string suitName = "";
        }

        public class PartData
        {
            public Mesh mesh;
            public Texture2D tex;
            public string path;
        }
        public class RoleData
        {
            public PartData[] partData = new PartData[(int)EPartType.ECombinePartEnd];
            public GameObject[] mountPartData = new GameObject[4];
            public Texture2D decalData = null;
            public GameObject role = null;
        }

        private PlayStateChange playStateChange = new PlayStateChange();
        private CombineConfig combineConfig = null;
        private List<EquipSuit> equipSuitList = new List<EquipSuit>();
        private List<EquipData>[] declList = null;        
        private DefaultEquip defaultEquip = new DefaultEquip();

        private EquipData[] selectedEquipDataList = new EquipData[(int)EPartType.EWeaponEnd];
        private EquipData selectedDecal = null;
        private RoleData[] roleDataList = null;
        private RoleData tmpRoleData = new RoleData();
        private EquipData[] customEquipDataList = new EquipData[(int)EPartType.EWeaponEnd];
        private string customDir = "";
        private int m_professionIndex = 0;
        private bool makeNew = false;
        private string meshName = "";
        private List<CombineInstance> ciList = new List<CombineInstance>();
        private Vector2 fashionScrollPos = Vector2.zero;
        private Vector2 decalScrollPos = Vector2.zero;
        private Vector2 equipPathScrollPos = Vector2.zero;

        [MenuItem(@"Assets/Tool/Equipment/Preview", false, 0)]
        private static void PreviewEquip()
        {
            EquipPreview window = (EquipPreview)EditorWindow.GetWindow(typeof(EquipPreview), true, "套装预览");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        private int ConvertPart(int pos)
        {
            switch (pos)
            {
                case 0:
                    return (int)EPartType.EHeadgear;
                case 1:
                    return (int)EPartType.EUpperBody;
                case 2:
                    return (int)EPartType.ELowerBody;
                case 3:
                    return (int)EPartType.EGloves;
                case 4:
                    return (int)EPartType.EBoots;
                case 5:
                    return (int)EPartType.EMainWeapon;
                case 6:
                    return (int)EPartType.ESecondaryWeapon;
                case 7:
                    return (int)EPartType.EWings;
                case 8:
                    return (int)EPartType.ETail;
                case 9:
                    return (int)EPartType.EDecal;
                case 10:
                    return (int)EPartType.EFace;
                case 11:
                    return (int)EPartType.EHair;
            }
            return -1;
        }

        
        private string GetEquipName(int partIndex, string path)
        {
            if (partIndex >= 0 && partIndex < combineConfig.EquipPrefix.Length)
            {
                string proPrefix = combineConfig.EquipPrefix[m_professionIndex];
                string partSuffix = combineConfig.PartSuffix[partIndex];

                if (string.IsNullOrEmpty(path))
                {
                    return string.Format("Equipments/Player/{0}{1}", proPrefix, partSuffix);
                }
                else if (path.StartsWith("/"))
                {
                    return string.Format("Equipments/{0}/{1}{2}", path, proPrefix, partSuffix);
                }
                else if (path == "E")
                {
                    return "";
                }
                else
                {
                    return string.Format("Equipments/Player/{0}{1}", proPrefix, path);
                }
            }
            else
            {

                return path;
            }
        }
        private string GetDefaultPath(EPartType part, DefaultEquip.RowData data)
        {
            string partPath = "";
            int partIndex = (int)part;
            switch (part)
            {
                case EPartType.EFace:
                    partPath = GetEquipName(partIndex, data.Face);
                    break;
                case EPartType.EHair:
                    partPath = GetEquipName(partIndex, data.Hair);
                    break;
                case EPartType.EUpperBody:
                    partPath = GetEquipName(partIndex, data.Body);
                    break;
                case EPartType.ELowerBody:
                    partPath = GetEquipName(partIndex, data.Leg);
                    break;
                case EPartType.EGloves:
                    partPath = GetEquipName(partIndex, data.Glove);
                    break;
                case EPartType.EBoots:
                    partPath = GetEquipName(partIndex, data.Boots);
                    break;
                case EPartType.ESecondaryWeapon:
                    partPath = GetEquipName(partIndex, data.SecondWeapon);
                    break;
                case EPartType.EMainWeapon:
                    partPath = GetEquipName(partIndex, data.Weapon);
                    break;
            }
            return partPath;
        }
       
        private void MakePartData(int index, DefaultEquip.RowData data, RoleData roleData, EquipData[] equipDataList)
        {
            EquipData ed = equipDataList[index];

            
            string location = "";
            string srcDir = "";
            if (ed == null || string.IsNullOrEmpty(ed.path))
            {
                location = GetDefaultPath((EPartType)index, data);
                srcDir = "Player";
            }
            else
            {
                location = ed.path;
                int i = location.IndexOf("/");
                srcDir = location.Substring(0, i);
            }

            if (index == (int)EPartType.EMainWeapon)
            {
                GameObject weapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Equipments/" + location + ".prefab");
                if (weapon != null)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(weapon, null);
                    go.name = weapon.name;
                    roleData.mountPartData[0] = go;
                }
                
            }
            else
            {
                byte partType = 0;
                string meshLocation = null;
                string texLocation = null;
                string texpath = location;
                if (AssetModify.s_meshPartLis.GetMeshInfo(location, m_professionIndex, index, srcDir, out partType, ref meshLocation, ref texLocation))
                {
                    if(!string.IsNullOrEmpty(meshLocation))
                    {
                        location = meshLocation;
                    }
                    if (!string.IsNullOrEmpty(texLocation))
                    {
                        texpath = texLocation;
                    }
                }
                else
                {
                    Debug.LogError("mesh not found:" + location);
                    location = GetDefaultPath((EPartType)index, data);
                    texpath = location;
                }
                string meshResPath = "Assets/Resources/" + location + ".asset";
                string texResPath = "Assets/Resources/" + texpath + ".tga";
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshResPath);
                Texture2D tex = null;
                
                if (File.Exists(texResPath))
                {
                    tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texResPath);
                }
                else
                {
                    texResPath = texResPath.Replace("_a", "") + ".tga";
                    tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texResPath);
                }
                PartData pd = roleData.partData[index];
                if (pd == null)
                {
                    pd = new PartData();
                    roleData.partData[index] = pd;
                }
                pd.mesh = mesh;
                pd.tex = tex;
                pd.path = meshResPath;
            }
            
        }
        private void Preview(int x, int y, int z, EquipData[] equipDataList)
        {
            ciList.Clear();

            RoleData roleData = makeNew ? tmpRoleData : roleDataList[m_professionIndex];
            
            DefaultEquip.RowData data = defaultEquip.GetByProfID(m_professionIndex + 1);

            for (int i = 0; i < equipDataList.Length; ++i)
            {
                MakePartData(i, data, roleData, equipDataList);
            }

            Material mat = null;
            PartData bodyPart = roleData.partData[(int)EPartType.EUpperBody];
            bool hasOnepart = false;
            Texture2D alphaTex = null;
            if (bodyPart != null && bodyPart.tex != null && bodyPart.tex.width == 1024)
            {
                hasOnepart = true;
                alphaTex = AssetDatabase.LoadAssetAtPath<Texture2D>(bodyPart.path + "_A.png");
                if(alphaTex!=null)
                {
                    mat = new Material(Shader.Find("Custom/Skin/RimlightBlendCutout"));
                }
                else
                {
                    mat = new Material(Shader.Find("Custom/Skin/RimlightBlend"));
                }
            }
            else
            {
                mat = new Material(Shader.Find("Custom/Skin/RimlightBlend8"));
            }
            ciList.Clear();
            for (int i = 0;i< roleData.partData.Length;++i)
            {
                PartData partData = roleData.partData[i];
                if(partData.mesh!=null)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = partData.mesh;
                    ciList.Add(ci);
                }
            }
            if (ciList.Count > 0)
            {
                GameObject roleGO = null;
                SkinnedMeshRenderer roleSmr = null;
                if (makeNew || roleData.role == null)
                {
                    string skinPrfab = "Prefabs/" + combineConfig.PrefabName[m_professionIndex];
                    string anim = combineConfig.IdleAnimName[m_professionIndex];
                    GameObject prefab = Resources.Load<GameObject>(skinPrfab);
                    roleGO = GameObject.Instantiate<GameObject>(prefab);
                     roleGO.name = prefab.name;
                    CharacterController cc = roleGO.GetComponent<CharacterController>();
                    if (cc != null)
                    {
                        cc.enabled = false;
                    }
                    roleGO.transform.position = new Vector3(x, y, z);

                    Animator ator = roleGO.GetComponent<Animator>();
                    AnimatorOverrideController aoc = new AnimatorOverrideController();
                    aoc.runtimeAnimatorController = ator.runtimeAnimatorController;
                    ator.runtimeAnimatorController = aoc;
                    aoc["Idle"] = Resources.Load<AnimationClip>(anim);                    
                    roleData.role = roleGO;
                }
                else
                {
                    roleGO = roleData.role;
                }

                roleSmr = roleGO.GetComponent<SkinnedMeshRenderer>();
                if (roleSmr.sharedMesh == null)
                    roleSmr.sharedMesh = new Mesh();
                roleGO.SetActive(false);
                roleSmr.sharedMesh.CombineMeshes(ciList.ToArray(), true, false);
                roleSmr.sharedMaterial = mat;
                if (hasOnepart)
                {
                    PartData face = roleData.partData[(int)EPartType.EFace];
                    PartData hair = roleData.partData[(int)EPartType.EHair];
                    PartData helmet = roleData.partData[(int)EPartType.EHeadgear];
                    mat.SetTexture("_Face", face.tex);
                    if (hair.tex == null)
                    {
                        mat.SetTexture("_Hair", helmet.tex);
                    }
                    else
                    {
                        mat.SetTexture("_Hair", hair.tex);
                    }
                    PartData body = roleData.partData[(int)EPartType.EUpperBody];
                    mat.SetTexture("_Body", body.tex);
                    if(alphaTex!=null)
                    {
                        mat.SetTexture("_Alpha", alphaTex);
                    }
                }
                else
                {
                    for (int i = 0; i < roleData.partData.Length; ++i)
                    {
                        PartData partData = roleData.partData[i];
                        mat.SetTexture("_Tex" + i.ToString(), partData.tex);
                    }
                }
      
                roleGO.SetActive(true);

                GameObject weaponGO = roleData.mountPartData[0];
                if (weaponGO != null&&data.WeaponPoint != null && data.WeaponPoint.Length > 0)
                {
                    string weapon = data.WeaponPoint[0];
                    Transform trans = roleGO.transform.Find(weapon);
                    if (trans != null)
                    {
                        weaponGO.transform.parent = trans;
                        weaponGO.transform.localPosition = Vector3.zero;
                        weaponGO.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        public void Init()
        {
            combineConfig = CombineConfig.GetConfig();
            AssetModify.LoadMeshPartInfo();

            XBinaryReader.Init();
            XBinaryReader reader = XBinaryReader.Get();
            TextAsset ta = Resources.Load<TextAsset>(@"Table/DefaultEquip");
            if (ta != null)
            {
                reader.Init(ta);
                defaultEquip.ReadFile(reader);
                XBinaryReader.Return(reader);
            }
            int professionCount = combineConfig.EquipFolderName.Length;
            roleDataList = new RoleData[professionCount];
            declList = new List<EquipData>[professionCount];
            for (int i = 0; i < professionCount; ++i)
            {
                declList[i] = new List<EquipData>();
                roleDataList[i] = new RoleData();
            }

            DirectoryInfo di = new DirectoryInfo("Assets/Resources/Equipments");
            DirectoryInfo[] subDirs = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < subDirs.Length; ++i)
            {
                DirectoryInfo subDir = subDirs[i];
                if (subDir.Name == "decal")
                {
                    FileInfo[] files = subDir.GetFiles("*.tga", SearchOption.TopDirectoryOnly);
                    for (int j = 0; j < files.Length; ++j)
                    {
                        FileInfo file = files[j];
                        string name = file.Name.Replace(".tga", "");
                        int professionIndex = -1;
                        for (int k = 0; k < combineConfig.EquipPrefix.Length; ++k)
                        {
                            string prefix = combineConfig.EquipPrefix[k];
                            if (name.StartsWith(prefix))
                            {
                                professionIndex = k;
                                break;
                            }
                        }
                        if (professionIndex >= 0)
                        {
                            List<EquipData> decals = declList[professionIndex];
                            EquipData ed = new EquipData();
                            ed.name = name;
                            ed.path = "Equipments/decal/" + name;
                            decals.Add(ed);
                        }
                    }
                       
                }
                else
                {
                    EquipSuit es = new EquipSuit();
                    es.suitName = subDir.Name;
                    es.professionList = new ProfessionEquip[professionCount];
                    FileInfo[] files = subDir.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
                    for (int j = 0; j < files.Length; ++j)
                    {
                        FileInfo file = files[j];
                        string name = file.Name.Replace(".asset", "");
                        string dirname = file.Directory.Name;
                        int professionIndex = -1;
                        for (int k = 0; k < combineConfig.EquipPrefix.Length; ++k)
                        {
                            string prefix = combineConfig.EquipPrefix[k];
                            if (name.StartsWith(prefix))
                            {
                                professionIndex = k;
                                break;
                            }
                        }
                        ProfessionEquip pe = null;
                        if (professionIndex >= 0)
                        {
                            pe = es.professionList[professionIndex];
                            if (pe == null)
                            {
                                pe = new ProfessionEquip();
                                es.professionList[professionIndex] = pe;
                            }
                            string tmp = name.Replace(combineConfig.EquipPrefix[professionIndex], "");
                            for (int k = 0; k < combineConfig.PartSuffix.Length; ++k)
                            {
                                string suffix = combineConfig.PartSuffix[k];
                                if (tmp.StartsWith(suffix))
                                {
                                    EquipData ed = pe.equipDataList[k];
                                    if (ed==null)
                                    {
                                        ed = new EquipData();
                                        pe.equipDataList[k] = ed;
                                    }
                                    ed.name = name;
                                    ed.path = "Equipments/" + dirname + "/" + name;
                                    break;
                                }
                            }
                        }

                    }
                    equipSuitList.Add(es);
                }
            }
            di = new DirectoryInfo("Assets/Resources/Prefabs/Equipment");
            FileInfo[] prefabs = di.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < prefabs.Length; ++i)
            {
                FileInfo file = prefabs[i];
                string name = file.Name.Replace(".prefab", "");
            }
            for (int i = 0; i < customEquipDataList.Length; ++i)
            {
                customEquipDataList[i] = new EquipData();
            }
        }

        protected virtual void OnGUI()
        {
            if(playStateChange.Update())
            {
                Init();
                playStateChange.PostUpdate();
            }
            GUILayout.BeginHorizontal();
            if (combineConfig != null)
            {
                int selectIndex = m_professionIndex;
                for (int i = 0; i < combineConfig.EquipFolderName.Length; ++i)
                {
                    if (GUILayout.Button(combineConfig.EquipFolderName[i], GUILayout.MaxWidth(100)))
                    {
                        selectIndex = i;
                        break;
                    }
                }
                if (selectIndex != m_professionIndex)
                {
                    selectedDecal = null;
                    for (int i = 0; i < selectedEquipDataList.Length; ++i)
                    {
                        selectedEquipDataList[i] = null;
                    }
                    m_professionIndex = selectIndex;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
            {
                Preview(m_professionIndex, 0, makeNew ? 0 : 1, selectedEquipDataList);
            }
            makeNew = GUILayout.Toggle(makeNew, "MakeNew", GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
            meshName = GUILayout.TextField(meshName, GUILayout.MaxWidth(200));
            if (GUILayout.Button("Export", GUILayout.MaxWidth(100)))
            {
                RoleData roleData = roleDataList[m_professionIndex];
                if (roleData.role != null)
                {
                    GameObject newGo = GameObject.Instantiate<GameObject>(roleData.role);
                    SkinnedMeshRenderer smr = newGo.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null && smr.sharedMesh != null)
                    {
                        Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(smr.sharedMesh);
                        mesh.name = meshName;
                        Material mat = UnityEngine.Object.Instantiate<Material>(smr.sharedMaterial);
                        mat.name = meshName;
                        string folderName = combineConfig.SkillFolderName[m_professionIndex];
                        AssetModify.CreateOrReplaceAsset<Material>(mat, string.Format("Assets/Creatures/{0}/Materials/{1}.mat", folderName, meshName));
                        MeshUtility.SetMeshCompression(mesh, ModelImporterMeshCompression.High);
                        MeshUtility.Optimize(mesh);
                        mesh.UploadMeshData(true);                       
                        AssetModify.CreateOrReplaceAsset<Mesh>(mesh, string.Format("Assets/Creatures/{0}/{1}.asset", folderName, meshName));
                        smr.sharedMesh = mesh;
                        smr.sharedMaterial = mat;
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MinHeight(400));
            //时装
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(800));
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("时装", GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
            fashionScrollPos = GUILayout.BeginScrollView(fashionScrollPos, false, false);
            for (int i = 0; i < equipSuitList.Count; ++i)
            {
                EquipSuit es = equipSuitList[i];
                ProfessionEquip pe = es.professionList[m_professionIndex];
                if (pe != null)
                {
                    GUILayout.BeginHorizontal();

                    if(GUILayout.Button(es.suitName, GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                    {
                        for (int j = 0; j < pe.equipDataList.Length; ++j)
                        {
                            selectedEquipDataList[j] = pe.equipDataList[j];
                        }
                            
                    }
                    for (int j = 0; j < pe.equipDataList.Length; ++j)
                    {
                        EquipData ed = pe.equipDataList[j];
                        if (ed != null && !string.IsNullOrEmpty(ed.name))
                        {
                            bool selected = selectedEquipDataList[j] == ed;
                            bool afterSelected = GUILayout.Toggle(selected, ed.name, GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
                            if (afterSelected != selected)
                            {
                                selectedEquipDataList[j] = afterSelected ? ed : null;
                            }
                            GUILayout.Space(5);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            //装备
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(100));
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("纹身", GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
            decalScrollPos = GUILayout.BeginScrollView(decalScrollPos, false, false);
            List<EquipData> decals = declList[m_professionIndex];
            for (int i = 0; i < decals.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                EquipData ed = decals[i];
                bool selected = ed == selectedDecal;
                bool afterSelected = GUILayout.Toggle(selected, ed.name, GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
                if (afterSelected != selected)
                {
                    selectedDecal = afterSelected ? ed : null;
                }
                GUILayout.EndHorizontal();
            }
            //List<EquipPart> currentEquipPrefession = m_EquipList[m_professionIndex];
            //for (int i = 0; i < currentEquipPrefession.Count; ++i)
            //{
            //    EquipPart part = currentEquipPrefession[i];
            //    for (int j = 0; j < part.suitName.Count; ++j)
            //    {
            //        GUILayout.BeginHorizontal();
            //        EditorGUILayout.LabelField(part.suitName[j], GUILayout.MaxWidth(200));
            //        EditorGUILayout.LabelField(part.partPath[2], GUILayout.MaxWidth(150));
            //        if (j == 0)
            //        {
            //            if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
            //            {
            //                Preview(m_professionIndex, part, -m_professionIndex, -2, -i);
            //            }
            //        }
            //        GUILayout.EndHorizontal();
            //    }
            //    GUILayout.Space(5);
            //}
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal();
            
            customDir = EditorGUILayout.TextField("Dir", customDir, GUILayout.MaxWidth(250));
            if(GUILayout.Button("Make", GUILayout.MaxWidth(50)))
            {
                string prefix = combineConfig.EquipPrefix[m_professionIndex];
                for (int i = 0; i < customEquipDataList.Length; ++i)
                {
                    EquipData ed = customEquipDataList[i];
                    ed.name = prefix + combineConfig.PartSuffix[i];
                    ed.path = string.Format("Equipments/{0}/{1}", customDir, ed.name);
                }
                Preview(0, 0, 0, customEquipDataList);
            }
            GUILayout.EndHorizontal();
            
            //testFbx = EditorGUILayout.ObjectField(testFbx, typeof(GameObject), true) as GameObject;
            //if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
            //{
            //    EquipPart ep = new EquipPart();
            //    ep.Init("Default");
            //    int prof = MakeTestEquip(ep);
            //    if (prof >= 0)
            //        Preview(prof, ep, 1, 0, 0);
            //}
            GUILayout.EndHorizontal();
            //GUILayout.Space(5);
            //GUILayout.BeginHorizontal();
            //bool find = false;

            //findEquip = EditorGUILayout.TextField(findEquip, GUILayout.MaxWidth(300));
            //if (GUILayout.Button("Find", GUILayout.MaxWidth(100)))
            //{
            //    find = true;
            //}
            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            //equipPathScrollPos = GUILayout.BeginScrollView(equipPathScrollPos, false, false);
            //for (int i = 0; i < meshPathList.Count; ++i)
            //{
            //    EquipPathInfo epi = meshPathList[i];
            //    if (find)
            //    {
            //        string name = epi.name.ToLower();
            //        if (name.Contains(findEquip))
            //        {
            //            float currentFindHeight = i * 18;
            //            if (currentFindHeight > equipPathScrollPos.y)
            //            {
            //                find = false;
            //                equipPathScrollPos.y = currentFindHeight;
            //            }

            //        }
            //    }
            //    GUILayout.BeginHorizontal();
            //    EditorGUILayout.LabelField(epi.name, GUILayout.MaxWidth(300), GUILayout.MaxHeight(20));
            //    EditorGUILayout.LabelField(epi.texPath, GUILayout.MaxWidth(300));
            //    EditorGUILayout.LabelField((epi.type == XMeshPartList.NormalPart ? "normal" : (epi.type == XMeshPartList.CutoutPart ? "CutoutOnepart" : "Onepart")), GUILayout.MaxWidth(150));
            //    GUILayout.EndHorizontal();
            //    //EditorGUILayout.LabelField(string.Format("Name:{0} Tex:{1} Type:{2}", epi.name, epi.texPath, epi.type == 0 ? "normal" : (epi.type == 1 ? "CutoutOnepart" : "Onepart")), GUILayout.MaxWidth(800));
            //}

            //EditorGUILayout.EndScrollView();
            //GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
    //public class CvsTableEditor : EditorWindow
    //{
    //    private UnityEngine.Object table = null;
    //    private string tableName = "";

    //    private List<string> scriptList = new List<string>();
    //    private string[] scriptArray;
    //    private int cvsIndex = -1;
    //    private string cvsName = "";
    //    private Assembly ass = null;
    //    public void Init(UnityEngine.Object obj)
    //    {
    //        if(obj==null)
    //        {
    //            table = null;
    //            tableName = "";
    //            cvsIndex = -1;
    //            return;
    //        }
    //        string path = AssetDatabase.GetAssetPath(obj);
    //        if (path.StartsWith("Assets/Table/") && path.EndsWith(".txt"))
    //        {
    //            table = obj;
    //            tableName = path.Replace("Assets/Table/", "");
    //            tableName = tableName.Replace(".txt", "");
    //            scriptList.Clear();
    //            ass = Assembly.Load("XUtliPoolLib");
    //            if (ass != null)
    //            {
    //                Type[] types = ass.GetExportedTypes();
    //                foreach (Type t in types)
    //                {
    //                    if (t.IsSubclassOf(typeof(CVSReader)))
    //                    {
    //                        string firstWorld = t.Name.Substring(0, 1).ToUpper() + "/";
    //                        scriptList.Add(firstWorld + t.Name);
    //                    }
    //                }
    //                scriptList.Sort();
    //                scriptArray = scriptList.ToArray();
    //            }
    //        }
    //    }

    //    protected virtual void OnGUI()
    //    {
    //        if (table == null)
    //            return;
    //        GUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("Table Init");
    //        if (GUILayout.Button("Generate", GUILayout.MaxWidth(150)))
    //        {
    //            if (ass != null && cvsName != "" && table != null)
    //            {
    //                Type readerType = ass.GetType("XUtliPoolLib." + cvsName, false);
    //                string des = "Assets/Resources/Table/" + tableName + ".bytes";
    //                //AssetModify._SaveBin(table as TextAsset, des, readerType);
    //                TableMap tableMap = XDataIO<TableMap>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Table/Table.xml");
    //                if (tableMap != null)
    //                {
    //                    bool find = false;
    //                    for (int i = 0; i < tableMap.tableScriptMap.Count; ++i)
    //                    {
    //                        TableScriptMap tsm = tableMap.tableScriptMap[i];
    //                        if (tsm.table == tableName)
    //                        {
    //                            tsm.script = cvsName;
    //                            find = true;
    //                            break;
    //                        }
    //                    }
    //                    if (!find)
    //                    {
    //                        TableScriptMap newtsm = new TableScriptMap();
    //                        newtsm.table = tableName;
    //                        newtsm.script = cvsName;
    //                        tableMap.tableScriptMap.Add(newtsm);
    //                    }
    //                    XDataIO<TableMap>.singleton.SerializeData("Assets/Editor/ResImporter/ImporterData/Table/Table.xml", tableMap);
    //                }
    //            }
    //        }
    //        GUILayout.EndHorizontal();
    //        GUILayout.BeginHorizontal();
    //        TextAsset tableObj = EditorGUILayout.ObjectField(table, typeof(TextAsset), true) as TextAsset;
    //        if(tableObj!=table)
    //        {
    //            Init(tableObj);
    //        }
    //        GUILayout.EndHorizontal();
    //        GUILayout.BeginHorizontal();
    //        int newSelect = EditorGUILayout.Popup(cvsIndex, scriptArray, GUILayout.MinWidth(110f));
    //        if (newSelect!= cvsIndex)
    //        {
    //            cvsName = scriptArray[newSelect].Substring(2);
    //            cvsIndex = newSelect;
    //        }
    //        GUILayout.EndHorizontal();            

    //    }
    //}

    public class BytesTableViewEditor : EditorWindow
    {
        [Serializable]
        public class PGCVSField
        {
            public string Name;
            public string Type;
            public string ClientType;
            public string ColNameInExcel;
            public int MakeIndex;
            public bool ClientIndex = true;
            public bool ServerOnly = false;
        }

        [Serializable]
        public class PGCVSStruct
        {
            public string Name;
            public string TableName;
            public bool ServerOnly;

            public List<PGCVSField> Fields = new List<PGCVSField>();
        }
        public class TableInfo
        {
            public List<PGCVSStruct> tables = new List<PGCVSStruct>();
        }
        private TableInfo tableInfo = new TableInfo();
        //private UnityEngine.TextAsset table = null;
        private string tableName = "";
        private string scriptName = "";
        private Type tableType = null;
        private Type tableRowType = null;
        private FieldInfo[] tableRowTypeField = null;
        private CVSReader reader = null;
        private FieldInfo tableListInfo = null;

        private List<string> tableData = new List<string>();
        private Vector2 scrollPos;
        private GUILayoutOption scrollOp0 = null;
        private int pageCount = 50;
        private int totalPage = 0;
        private int currentPage = 0;
        private int gotoPage = 0;
        private string searchKey = "";
        private List<int> findLine = new List<int>();

        public void Init(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                tableName = "";
                return;
            }
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.StartsWith("Assets/Resources/Table/") && path.EndsWith(".bytes"))
            {
                UnityEngine.TextAsset table = obj as TextAsset;
                tableName = path.Replace("Assets/Resources/Table/", "");
                tableName = tableName.Replace(".bytes", "");

                Assembly ass = Assembly.Load("XUtliPoolLib");
                if (ass != null)
                {
                    XmlSerializer serializer = new XmlSerializer(tableInfo.tables.GetType());
                    try
                    {
                        string dataFilePath = @"..\XProject\Shell\cvs.xml";
                        FileStream fs = new FileStream(dataFilePath, FileMode.Open);
                        tableInfo.tables = (List<PGCVSStruct>)serializer.Deserialize(fs);
                        fs.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return;
                    }
                    if (tableInfo != null)
                    {
                        for (int i = 0; i < tableInfo.tables.Count; ++i)
                        {
                            PGCVSStruct tableStruct = tableInfo.tables[i];
                            if(string.IsNullOrEmpty(tableStruct.TableName))
                            {
                                //Debug.LogError("error table name:" + tableStruct.Name);
                            }
                            else
                            {
                                string[] tableNames = tableStruct.TableName.Split('|');
                                for (int j = 0; j < tableNames.Length; ++j)
                                {
                                    if (tableNames[j] == tableName)
                                    {
                                        scriptName = tableStruct.Name;
                                        tableType = ass.GetType("XUtliPoolLib." + scriptName, false);
                                        if (tableType != null)
                                        {
                                            tableRowType = tableType.GetNestedType("RowData");
                                            if (tableRowType != null)
                                            {
                                                tableRowTypeField = tableRowType.GetFields();
                                                tableListInfo = tableType.GetField("Table");
                                                reader = Activator.CreateInstance(tableType) as CVSReader;
                                                if (reader != null)
                                                {
                                                    CVSReader.Init();
                                                    XBinaryReader.Init();
                                                    XBinaryReader read = XBinaryReader.Get();
                                                    read.Init(table);
                                                    //Stream stream = new System.IO.MemoryStream(table.bytes);
                                                    reader.ReadFile(read);
                                                    XBinaryReader.Return(read);
                                                    System.Collections.IList tableList = null;
                                                    System.Collections.IDictionary dicList = null;
                                                    System.Object tableObj = tableListInfo.GetValue(reader);
                                                    tableList = tableObj as System.Collections.IList;
                                                    dicList = tableObj as System.Collections.IDictionary;
                                                    tableData.Clear();
                                                    if ((tableList != null || dicList != null) && tableRowTypeField != null)
                                                    {
                                                        System.Text.StringBuilder sb = new System.Text.StringBuilder();

                                                        string formatStr = "{{{0}:{1}}},";
                                                        string formatStrEnd = "{{{0}:{1}}}";
                                                        string format = "";
                                                        int count = tableList != null ? tableList.Count : dicList.Count;
                                                        var it = dicList != null ? dicList.GetEnumerator() : null;
                                                        for (int x = 0; x < count; ++x)
                                                        {
                                                            System.Object data = null;
                                                            if (tableList != null)
                                                                data = tableList[x];
                                                            else
                                                            {
                                                                it.MoveNext();
                                                                data = it.Value;
                                                            }


                                                            for (int y = 0; y < tableRowTypeField.Length; ++y)
                                                            {
                                                                FieldInfo fi = tableRowTypeField[y];
                                                                System.Object value = null;
                                                                if (data is string)
                                                                {
                                                                    value = data;
                                                                }
                                                                else
                                                                {
                                                                    value = fi.GetValue(data);
                                                                }
                                                                string str = "";
                                                                if (value is Array)
                                                                {
                                                                    Array arr = value as Array;
                                                                    System.Collections.IList lst = value as System.Collections.IList;
                                                                    if (arr != null && lst != null)
                                                                    {
                                                                        for (int a = 0; a < arr.Length; ++a)
                                                                        {
                                                                            str += lst[a].ToString();
                                                                            if (a != arr.Length - 1)
                                                                            {
                                                                                str += "|";
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (value != null)
                                                                    {
                                                                        str = value.ToString();
                                                                    }
                                                                }
                                                                if (y != tableRowTypeField.Length - 1)
                                                                {
                                                                    format = formatStrEnd;
                                                                }
                                                                else
                                                                {
                                                                    format = formatStr;
                                                                }
                                                                sb.Append(string.Format(format, y, str));
                                                            }
                                                            tableData.Add(sb.ToString());
                                                            sb.Length = 0;
                                                        }
                                                        RefreshPage(tableData.Count);
                                                    }
                                                }
                                            }
                                        }
                                        return;
                                    }
                                }
                            }
                           
                            
                        }
                    }
                }
            }
        }
        private void RefreshPage(int count)
        {
            currentPage = 0;
            totalPage = count / pageCount;
            if (tableData.Count % pageCount > 0)
            {
                totalPage++;
            }
        }

        protected virtual void OnGUI()
        {
            if (scrollOp0 == null)
            {
                scrollOp0 = GUILayout.ExpandWidth(true);
            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Table");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Pre Page", GUILayout.Width(80)))
            {
                currentPage--;
                if (currentPage < 0)
                {
                    currentPage = 0;
                }
            }
            if (GUILayout.Button("Next Page", GUILayout.Width(80)))
            {
                currentPage++;
                if (currentPage >= totalPage)
                {
                    currentPage = totalPage;
                }
            }
            int page = EditorGUILayout.IntField(gotoPage, GUILayout.Width(80));
            if (page != currentPage && page > 0 && page <= totalPage)
            {
                gotoPage = page;
            }
            if (GUILayout.Button("GoTo", GUILayout.Width(80)))
            {
                currentPage = gotoPage - 1;
            }
            GUILayout.Label(string.Format("Page:{0}/{1} Line:{2}", currentPage + 1, totalPage, tableData.Count));
            if (GUILayout.Button("Export", GUILayout.Width(80)))
            {
                try
                {
                    using (FileStream fs = new FileStream(string.Format("Assets/Resources/Table/{0}.txt", tableName), FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs);

                        for (int i = 0; i < tableData.Count; ++i)
                        {
                            string col = tableData[i];
                            sw.WriteLine(col);
                        }
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
               
            }

            string key = GUILayout.TextField(searchKey, GUILayout.Width(300));
            if (searchKey != key)
            {
                searchKey = key;
                if (searchKey == "")
                {
                    RefreshPage(tableData.Count);
                }
            }
            if (GUILayout.Button("Search", GUILayout.Width(80)) && searchKey != "")
            {
                findLine.Clear();
                for (int i = 0; i < tableData.Count; ++i)
                {
                    string col = tableData[i];
                    int index = col.IndexOf(searchKey);
                    if (index >= 0)
                    {
                        findLine.Add(i);
                    }
                }
                if (findLine.Count > 0)
                {
                    RefreshPage(findLine.Count);
                }
            }

            GUILayout.EndHorizontal();

            if (tableRowTypeField != null)
            {
                GUILayout.BeginHorizontal();

                scrollPos = GUILayout.BeginScrollView(scrollPos);
                GUILayout.BeginHorizontal();
                for (int i = 0; i < tableRowTypeField.Length; ++i)
                {
                    FieldInfo fi = tableRowTypeField[i];
                    GUILayout.Button(string.Format("{0}:{1}", i, fi.Name));
                }
                GUILayout.EndHorizontal();
                int endCount = (currentPage + 1) * pageCount;
                if (findLine.Count > 0)
                {
                    for (int i = currentPage * pageCount; i < findLine.Count && i < endCount; ++i)
                    {
                        int index = findLine[i];
                        string col = tableData[index];
                        GUILayout.BeginHorizontal();
                        GUILayout.TextField(col);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    for (int i = currentPage * pageCount; i < tableData.Count && i < endCount; ++i)
                    {
                        string col = tableData[i];
                        GUILayout.BeginHorizontal();
                        GUILayout.TextField(col);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();
            }

        }
    }

    //public class MaterialFindEditor : EditorWindow
    //{
    //    //public enum EResType
    //    //{
    //    //    Fx = 0,
    //    //    UI,
    //    //    Equipments,
    //    //    Prefab,
    //    //    Scene
    //    //}
    //    //class MatPrefab : INameCompare
    //    //{
    //    //    public Material mat = null;
    //    //    public List<GameObject> prefabs = new List<GameObject>();
    //    //    public string Name
    //    //    {
    //    //        get
    //    //        {
    //    //            return mat != null ? mat.name : "";
    //    //        }
    //    //    }
    //    //}
    //    //private Dictionary<Shader,List<MatPrefab>> m_Shaders = new Dictionary<Shader, List<MatPrefab>>();
    //    //private List<MatPrefab> m_MaterialPrefabs = null;
    //    //private MatPrefab m_MatPrefab = null;
    //    //private Vector2 shaderScrollPos = Vector2.zero;
    //    //private Vector2 materialScrollPos = Vector2.zero;
    //    //private Vector2 prefabScrollPos = Vector2.zero;
    //    //private Shader m_SelectShader = null;

    //    //private EResType resType = EResType.Fx;
    //    //private NameSort<MatPrefab> matSort = new NameSort<MatPrefab>();
    //    //private Shader replaceShader = null;
    //    //private string[] resPaths = new string[] {
    //    //    "Assets/Resources/Effects",
    //    //    "Assets/Resources/atlas/UI",
    //    //    "Assets/Resources/Equipments",
    //    //    "Assets/Prefabs",
    //    //    "Assets/XScene"};
    //    //private void FindMat(string prefabPath,GameObject prefab, Material mat)
    //    //{
    //    //    if (mat == null)
    //    //    {
    //    //        Debug.LogError("null mat:" + prefabPath);
    //    //    }
    //    //    else
    //    //    {
    //    //        Shader shader = mat.shader;
    //    //        List<MatPrefab> matPrefabs = null;
    //    //        if (!m_Shaders.TryGetValue(shader, out matPrefabs))
    //    //        {
    //    //            matPrefabs = new List<MatPrefab>();
    //    //            m_Shaders.Add(shader, matPrefabs);
    //    //        }
    //    //        foreach(MatPrefab mp in matPrefabs)
    //    //        {
    //    //            if(mp.mat==mat)
    //    //            {
    //    //                if(!mp.prefabs.Contains(prefab))
    //    //                {
    //    //                    mp.prefabs.Add(prefab);
    //    //                }
    //    //                return;
    //    //            }
    //    //        }
    //    //        MatPrefab newMp = new MatPrefab();
    //    //        newMp.mat = mat;
    //    //        newMp.prefabs.Add(prefab);
    //    //        matPrefabs.Add(newMp);
    //    //    }
    //    //}
    //    //private void FindMat(string path)
    //    //{
    //    //    m_Shaders.Clear();
    //    //    m_MaterialPrefabs = null;
    //    //    m_MatPrefab = null;
    //    //    DirectoryInfo di = new DirectoryInfo(path);
    //    //    List<Renderer> renders = new List<Renderer>();            

    //    //    FileInfo[] files = di.GetFiles("*.prefab", SearchOption.AllDirectories);

    //    //    for (int i = 0; i < files.Length; ++i)
    //    //    {
    //    //        FileInfo fi = files[i];
    //    //        string prefabPath = fi.FullName.Replace("\\", "/");
    //    //        int index = prefabPath.IndexOf(path);
    //    //        prefabPath = prefabPath.Substring(index);
    //    //        EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", prefabPath, i, files.Length), path, (float)i / files.Length);

    //    //        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
    //    //        GameObject instance = GameObject.Instantiate(prefab) as GameObject;
    //    //        instance.GetComponentsInChildren<Renderer>(true, renders);
    //    //        //if (!isEquip)
    //    //        {

    //    //            for (int j = 0; j < renders.Count; ++j)
    //    //            {
    //    //                Renderer render = renders[j];
    //    //                if(render is ParticleSystemRenderer)
    //    //                {
    //    //                    ParticleSystemRenderer psr = render as ParticleSystemRenderer;
    //    //                    FindMat(prefabPath, prefab, psr.sharedMaterial);


    //    //                }
    //    //                else
    //    //                {
    //    //                    Material[] mats = render.sharedMaterials;
    //    //                    foreach (Material mat in mats)
    //    //                    {
    //    //                        FindMat(prefabPath, prefab, mat);
    //    //                    }
    //    //                }

    //    //            }
    //    //        }

    //    //        GameObject.DestroyImmediate(instance);                
    //    //    }
    //    //    EditorUtility.ClearProgressBar();
    //    //}


    //    //private void ReplaceShader(Shader des,List<MatPrefab> patPrefab)
    //    //{
    //    //    foreach(MatPrefab mp in patPrefab)
    //    //    {
    //    //        if (mp.mat != null)
    //    //            mp.mat.shader = des;
    //    //    }
    //    //}
    //    //protected virtual void OnGUI()
    //    //{
    //    //    GUILayout.BeginHorizontal();
    //    //    resType = (EResType)EditorGUILayout.EnumPopup("资源目录", resType, GUILayout.MaxWidth(250));
    //    //    GUILayout.EndHorizontal();
    //    //    GUILayout.BeginHorizontal();
    //    //    EditorGUILayout.ObjectField("SelectedShader", m_SelectShader, typeof(Shader), true, GUILayout.MaxWidth(450));
    //    //    if (GUILayout.Button("Scan", GUILayout.MaxWidth(150)))
    //    //    {
    //    //        string resPath = resPaths[(int)resType];
    //    //        FindMat(resPath);
    //    //    }
    //    //    GUILayout.EndHorizontal();
    //    //    GUILayout.BeginHorizontal();

    //    //    GUILayout.BeginVertical();
    //    //    shaderScrollPos = GUILayout.BeginScrollView(shaderScrollPos, false, false,GUILayout.MaxHeight(400));
    //    //    GUILayout.BeginHorizontal();
    //    //    replaceShader = EditorGUILayout.ObjectField("TargetShader", replaceShader, typeof(Shader), true, GUILayout.MaxWidth(450)) as Shader;
    //    //    GUILayout.EndHorizontal();
    //    //    var it = m_Shaders.GetEnumerator();
    //    //    while(it.MoveNext())
    //    //    {
    //    //        GUILayout.BeginHorizontal();
    //    //        Shader shader = it.Current.Key;
    //    //        EditorGUILayout.ObjectField(it.Current.Value.Count.ToString(), shader, typeof(Shader), true, GUILayout.MaxWidth(450));
    //    //        if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
    //    //        {
    //    //            m_SelectShader = shader;
    //    //            m_MaterialPrefabs = it.Current.Value;
    //    //            m_MaterialPrefabs.Sort(matSort);
    //    //            m_MatPrefab = null;
    //    //        }
    //    //        if (GUILayout.Button("Replace", GUILayout.MaxWidth(70)))
    //    //        {                   
    //    //            ReplaceShader(replaceShader, it.Current.Value);
    //    //        }
    //    //        GUILayout.EndHorizontal();
    //    //    }
    //    //    EditorGUILayout.EndScrollView();

    //    //    GUILayout.EndVertical();

    //    //    GUILayout.BeginVertical();
    //    //    materialScrollPos = GUILayout.BeginScrollView(materialScrollPos, false, false, GUILayout.MaxHeight(400));
    //    //    if(m_MaterialPrefabs != null)
    //    //    {
    //    //        for (int i = 0; i < m_MaterialPrefabs.Count; ++i)
    //    //        {
    //    //            GUILayout.BeginHorizontal();
    //    //            MatPrefab mp = m_MaterialPrefabs[i];
    //    //            EditorGUILayout.ObjectField(mp.prefabs.Count.ToString(), mp.mat, typeof(Material), true, GUILayout.MaxWidth(400));
    //    //            if (GUILayout.Button("Prefab", GUILayout.MaxWidth(70)))
    //    //            {
    //    //                m_MatPrefab = mp;
    //    //            }
    //    //            GUILayout.EndHorizontal();
    //    //        }
    //    //    }

    //    //    EditorGUILayout.EndScrollView();
    //    //    GUILayout.EndVertical();
    //    //    GUILayout.EndHorizontal();
    //    //    GUILayout.BeginHorizontal();
    //    //    GUILayout.BeginVertical();
    //    //    prefabScrollPos = GUILayout.BeginScrollView(prefabScrollPos, false, false);
    //    //    if (m_MatPrefab != null)
    //    //    {
    //    //        for (int i = 0; i < m_MatPrefab.prefabs.Count; ++i)
    //    //        {
    //    //            GameObject go = m_MatPrefab.prefabs[i];
    //    //            GUILayout.BeginHorizontal();
    //    //            EditorGUILayout.ObjectField("", go, typeof(GameObject), true, GUILayout.Width(300));
    //    //            GUILayout.EndHorizontal();
    //    //        }
    //    //    }

    //    //    EditorGUILayout.EndScrollView();
    //    //    GUILayout.EndVertical();

    //    //    GUILayout.EndHorizontal();
    //    //}

    //    //public string[] GetMaterialTexturePaths(Material _mat)
    //    //{
    //    //    List<string> results = new List<string>();
    //    //    UnityEngine.Object[] roots = new UnityEngine.Object[] { _mat };
    //    //    UnityEngine.Object[] dependObjs = EditorUtility.CollectDependencies(roots);
    //    //    foreach (UnityEngine.Object dependObj in dependObjs)
    //    //    {
    //    //        if (dependObj.GetType() == typeof(Texture2D))
    //    //        {
    //    //            string texpath = AssetDatabase.GetAssetPath(dependObj.GetInstanceID());
    //    //            results.Add(texpath);
    //    //        }
    //    //    }
    //    //    return results.ToArray();
    //    //}
    //}




    public class AssetFindEditor : EditorWindow
    {
        public enum EResType
        {
            Shader = 0,
            Texture,
            Mesh,
        }
        public enum EResFolder
        {
            Fx = 0,
            UI,
            Equip,
            Prefab,
            Scene,
            SceneIOS,
        }
        interface INameCompare
        {
            string Name { get; }
        }
        class NameSort<T> : IComparer<T> where T : INameCompare
        {
            public int Compare(T x, T y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        interface ISizeCompare
        {
            int Size { get; }
        }
        delegate void OnResGUI();
        delegate void ScanFun(string path);

        private EResType m_ResType = EResType.Shader;
        private EResFolder m_ResFolder = EResFolder.Fx;
        private NameSort<MatPrefab> m_MatPrefabSort = new NameSort<MatPrefab>();
        private OnResGUI[] onResGui = new OnResGUI[12];
        private ScanFun[] onScan = new ScanFun[3];
        private OnResGUI[] onHead = new OnResGUI[3];
        private string[] m_ResPaths = new string[]
        {
            "Assets/Resources/Effects",
            "Assets/Resources/atlas/UI|Assets/Resources/StaticUI|Assets/Resources/UI",
            "Assets/Resources/Equipments",
            "Assets/Resources/Prefabs",
            "Assets/XScene|Assets/Resources/Skyboxes",
            "Assets/XSceneIOS"
        };
        List<MeshFilter> mfList = new List<MeshFilter>();
        List<SkinnedMeshRenderer> smrList = new List<SkinnedMeshRenderer>();
        List<Renderer> renderList = new List<Renderer>();
        List<UITexture> uiTextureList = new List<UITexture>();
        private GUILayoutOption horizontalMaxWidth = null;
        private GUILayoutOption horizontalMinWidth = null;
        private GUILayoutOption horizontalMaxHeigth = null;
        private GUILayoutOption horizontalMinHeigth = null;
        private GUILayoutOption verticalMaxWidth = null;
        private GUILayoutOption verticalMinWidth = null;
        private GUILayoutOption verticalMaxHeigth = null;
        private GUILayoutOption verticalMinHeigth = null;
        private float m_Width = 800;
        private float m_Height = 600;
        class MatPrefab : INameCompare
        {
            public Material mat = null;
            public List<GameObject> prefabs = new List<GameObject>();
            public string Name
            {
                get
                {
                    return mat != null ? mat.name : "";
                }
            }
        }
        private Dictionary<Material, MatPrefab> m_MatPrefabList = new Dictionary<Material, MatPrefab>();

        private Vector2 m_MatScrollPos = Vector2.zero;
        private Vector2 m_PrefabScrollPos = Vector2.zero;
        private List<MatPrefab> m_SelectMatPrefabs = null;
        private List<Material> m_FindMaterial = new List<Material>();
        private MatPrefab m_SelectMatPrefab = null;
        private void OnEnable()
        {
            Init();
        }
        #region Shader

        private Dictionary<Shader, List<MatPrefab>> m_Shaders = new Dictionary<Shader, List<MatPrefab>>();
        private Shader m_ShaderSelectShader = null;
        private Shader m_ShaderReplaceShader = null;

        private Vector2 m_ShaderScrollPos = Vector2.zero;
        #endregion
        #region Texture
        public enum ETexSortType
        {
            Format,
            Size
        }
        public enum ETexPlatform
        {
            Android,
            iPhone
        }
        private class TexInfo : INameCompare
        {
            public Texture tex;
            public string path = "";
            public long sizeAndroid = 0;
            public long sizeiPhone = 0;
            public string androidFormatStr = "";
            public string iPhoneFormatStr = "";
            public bool mipmap = false;
            public List<MatPrefab> matPrefab = new List<MatPrefab>();
            public string Name
            {
                get
                {
                    return tex != null ? tex.name : "";
                }
            }
        }
        private class TexListInfo : ISizeCompare
        {
            public int sizeKey = 0;
            public string formatStr = "";
            public string formatStr2 = "";
            public List<TexInfo> texList = new List<TexInfo>();
            public long size = 0;
            public bool sorted = false;
            public int Size
            {
                get
                {
                    return sizeKey;
                }
            }
        }
        private ETexPlatform m_TexPlatform = ETexPlatform.Android;
        private ETexSortType m_TexSortType = ETexSortType.Format;

        private List<TexListInfo> m_TexFormatAndroidList = new List<TexListInfo>();
        private List<TexListInfo> m_TexFormatiPhoneList = new List<TexListInfo>();
        private List<TexListInfo> m_TexSizeAndroidList = new List<TexListInfo>();
        private List<TexListInfo> m_TexSizeiPhoneList = new List<TexListInfo>();

        private Dictionary<string, TexInfo> m_TexTexInfoMap = new Dictionary<string, TexInfo>();
        private long m_TexTotalAndroidSize = 0;
        private long m_TexTotaliPhoneSize = 0;
        private int m_TexTotalCount = 0;
        private string m_texMapName = "";
        private TexListInfo m_TexSelectTexListInfo = null;
        private TexInfo m_TexSelectTexInfo = null;
        private Vector2 m_TexTypeScrollPos = Vector2.zero;
        private Vector2 m_TexTexScrollPos = Vector2.zero;

        private List<Texture> m_TexFindCache = new List<Texture>();
        private NameSort<TexInfo> m_TexSort = new NameSort<TexInfo>();
        private SizeSort<TexListInfo> m_TexListSort = new SizeSort<TexListInfo>();
        #endregion
        #region Mesh
        public enum EMeshInfoCategory
        {
            Vertex = 0,
            UV2,
            Optimize,
            Readable,
            OptimizeGameObject,
            Fbx,
            Normal,
            Tangents,
        }


        private class MeshInfo : ISizeCompare
        {
            public List<GameObject> PrefabList = new List<GameObject>();
            public Mesh mesh = null;
            public int vertexCount = 0;
            public int triangleCount = 0;
            public bool hasUV2 = false;
            public bool optimize = false;
            public bool readable = false;
            public bool disaplyReadable = false;
            public bool normal = true;
            public bool tangents = false;
            public bool optimizeGameObject = false;
            public bool fbx = false;
            public string meshPath = "";
            public bool hasSkin = false;
            public int Size
            {
                get
                {
                    return vertexCount;
                }
            }

            public void Export(bool readable)
            {
                if (meshPath == "")
                    meshPath = AssetDatabase.GetAssetPath(mesh);
                if (fbx)
                {
                    ModelImporter modelImporter = AssetImporter.GetAtPath(meshPath) as ModelImporter;
                    AssetModify.ExportMeshAvatar(modelImporter, meshPath, null);
                }
                else
                {

                    string data = File.ReadAllText(meshPath);
                    if (readable)
                    {
                        data = data.Replace("m_IsReadable: 0", "m_IsReadable: 1");
                    }
                    else
                    {
                        data = data.Replace("m_IsReadable: 1", "m_IsReadable: 0");
                    }

                    File.WriteAllText(meshPath, data);
                }
            }
        }

        class SizeSort<T> : IComparer<T> where T : ISizeCompare
        {
            public int Compare(T x, T y)
            {
                return x.Size.CompareTo(y.Size);
            }
        }
        private class SizeMeshInfo
        {
            public SizeMeshInfo(string name)
            {
                this.name = name;
            }
            public string name = "";
            public List<MeshInfo> MeshList = new List<MeshInfo>();
            public bool sorted = false;
            public void Clear()
            {
                MeshList.Clear();
                sorted = false;
            }
        }
        private EMeshInfoCategory m_MeshInfoCategory = EMeshInfoCategory.Vertex;
        private List<SizeMeshInfo> m_MeshSizeList = new List<SizeMeshInfo>();
        private SizeMeshInfo m_MeshSelectedSizeMeshInfo = null;
        private MeshInfo m_MeshSelectedMeshInfo = null;
        private int m_MeshTotalCount = 0;

        private Vector2 m_MeshSizeScrollPos = Vector2.zero;
        private Vector2 m_MeshMeshScrollPos = Vector2.zero;
        private Vector2 m_MeshPrefabScrollPos = Vector2.zero;
        private SizeSort<MeshInfo> m_SizeSort = new SizeSort<MeshInfo>();
        private GUIStyle redStyle = null;
        #endregion
        public void Init()
        {
            onResGui[0] = OnShaderInfoList;
            onResGui[1] = OnShaderMatList;
            onResGui[2] = OnPrefabList;
            onResGui[3] = null;

            onResGui[4] = OnTexInfoList;
            onResGui[5] = OnTexTexList;
            onResGui[6] = OnTexMatList;
            onResGui[7] = OnPrefabList;

            onResGui[8] = OnMeshSizeList;
            onResGui[9] = OnMeshInfoList;
            onResGui[10] = OnMeshPrefabList;
            onResGui[11] = null;

            onScan[0] = ScanShader;
            onScan[1] = ScanTexture;
            onScan[2] = ScanMesh;

            onHead[0] = OnShaderHead;
            onHead[1] = OnTexHead;
            onHead[2] = OnMeshHead;

            m_MeshSizeList.Clear();
            m_MeshSizeList.Add(new SizeMeshInfo("0-200"));
            m_MeshSizeList.Add(new SizeMeshInfo("201-500"));
            m_MeshSizeList.Add(new SizeMeshInfo("501-1000"));
            m_MeshSizeList.Add(new SizeMeshInfo("1001-1500"));
            m_MeshSizeList.Add(new SizeMeshInfo("1500-"));

        }
        #region ShaderFun
        void OnShaderHead()
        {
            m_ShaderSelectShader = EditorGUILayout.ObjectField("SelectedShader", m_ShaderSelectShader, typeof(Shader), true, GUILayout.MaxWidth(400)) as Shader;
            if (GUILayout.Button("FindShader", GUILayout.MaxWidth(100)))
            {
                FindShader(m_ShaderSelectShader);
            }
            m_ShaderReplaceShader = EditorGUILayout.ObjectField("TargetShader", m_ShaderReplaceShader, typeof(Shader), true, GUILayout.MaxWidth(400)) as Shader;
            if (GUILayout.Button("Replace", GUILayout.MaxWidth(70)))
            {
                ReplaceShader(m_ShaderReplaceShader, m_SelectMatPrefabs);
            }
           
        }
        void OnShaderInfoList()
        {
            m_ShaderScrollPos = GUILayout.BeginScrollView(m_ShaderScrollPos, false, false, GUILayout.MaxHeight(400));
            if (m_FindMaterial.Count != 0)
            {
                for (int i = 0; i < m_FindMaterial.Count; ++i)
                {
                    Material mat = m_FindMaterial[i];
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("", mat, typeof(Material), true, GUILayout.MaxWidth(450));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                var it = m_Shaders.GetEnumerator();
                while (it.MoveNext())
                {
                    GUILayout.BeginHorizontal();
                    Shader shader = it.Current.Key;
                    EditorGUILayout.ObjectField(it.Current.Value.Count.ToString(), shader, typeof(Shader), true, GUILayout.MaxWidth(450));
                    if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
                    {
                        m_SelectMatPrefabs = it.Current.Value;
                        m_SelectMatPrefab = null;
                        m_ShaderSelectShader = shader;
                    }
                    GUILayout.EndHorizontal();
                }
            }
           
           
            EditorGUILayout.EndScrollView();
        }
        void OnShaderMatList()
        {
            OnMatList(m_ShaderSelectShader != null ? m_ShaderSelectShader.name : "");
        }

        void ScanShader(string path)
        {
            if (m_ResFolder == EResFolder.Scene || m_ResFolder == EResFolder.UI)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; ++i)
                {
                    FileInfo fi = files[i];
                    string matPath = fi.FullName.Replace("\\", "/");
                    int index = matPath.IndexOf(path);
                    matPath = matPath.Substring(index);

                    EditorUtility.DisplayProgressBar(string.Format("ScanSceneMat-{0}/{1}", i, files.Length), matPath, (float)i / files.Length);
                    Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                    MatPrefab mp = null;
                    RecordMatPrefab("", null, mat, out mp);
                    RecordShderMatPrefab("", mat, mp);
                }
                EditorUtility.ClearProgressBar();
            }
        }

        private void ReplaceShader(Shader des, List<MatPrefab> patPrefab)
        {
            foreach (MatPrefab mp in patPrefab)
            {
                if (mp.mat != null)
                    mp.mat.shader = des;
            }
        }

        private void FindShader(Shader shader)
        {
            m_FindMaterial.Clear();
            DirectoryInfo di = new DirectoryInfo("Assets/");
            FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i].FullName;
                path = path.Replace("\\", "/");
                int index = path.IndexOf("Assets/");
                path = path.Substring(index);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null&& shader==mat.shader)
                {
                    m_FindMaterial.Add(mat);
                }
            }
        }
        #endregion
        #region TextureFun
        void OnTexHead()
        {
            m_TexPlatform = (ETexPlatform)EditorGUILayout.EnumPopup("平台", m_TexPlatform, GUILayout.MaxWidth(250));
            m_TexSortType = (ETexSortType)EditorGUILayout.EnumPopup("统计类型", m_TexSortType, GUILayout.MaxWidth(250));
            if (m_TexPlatform == ETexPlatform.Android)
            {
                EditorGUILayout.LabelField(string.Format("Total Count:{0} TotalSize:{1}KB-{2}MB ", m_TexTotalCount, m_TexTotalAndroidSize / 1024, m_TexTotalAndroidSize / 1024 / 1024));
            }
            else
            {
                EditorGUILayout.LabelField(string.Format("Total Count:{0} TotalSize:{1}KB-{2}MB ", m_TexTotalCount, m_TexTotaliPhoneSize / 1024, m_TexTotaliPhoneSize / 1024 / 1024));
            }
        }
        void OnTexInfoList()
        {
            m_TexTypeScrollPos = GUILayout.BeginScrollView(m_TexTypeScrollPos, false, false);

            List<TexListInfo> texList = m_TexPlatform == ETexPlatform.Android ? m_TexFormatAndroidList : m_TexFormatiPhoneList;
            if (m_TexSortType == ETexSortType.Size)
            {
                texList = m_TexPlatform == ETexPlatform.Android ? m_TexSizeAndroidList : m_TexSizeiPhoneList;
            }
            for (int i = 0; i < texList.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                TexListInfo texListInfo = texList[i];
                string name = texListInfo.formatStr;
                string name2 = texListInfo.formatStr2;
                if (string.IsNullOrEmpty(name2))
                {
                    EditorGUILayout.LabelField(string.Format("{0} Count:{1} Size:{2}KB-{3}MB ", name, texListInfo.texList.Count, texListInfo.size / 1024, texListInfo.size / 1024 / 1024), GUILayout.MaxWidth(400));
                }
                else
                {
                    EditorGUILayout.LabelField(string.Format("{0}({4}) Count:{1} Size:{2}KB-{3}MB ", name, texListInfo.texList.Count, texListInfo.size / 1024, texListInfo.size / 1024 / 1024, name2), GUILayout.MaxWidth(400));
                }

                if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
                {
                    m_TexSelectTexListInfo = texListInfo;
                    m_texMapName = name;
                    if (!m_TexSelectTexListInfo.sorted)
                    {
                        m_TexSelectTexListInfo.texList.Sort(m_TexSort);
                        m_TexSelectTexListInfo.sorted = true;
                    }
                    m_TexSelectTexInfo = null;
                    m_SelectMatPrefabs = null;
                    m_SelectMatPrefab = null;
                }

                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        void OnTexTexList()
        {
            if (m_TexSelectTexListInfo != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("Name:{0} Count:{1}", m_texMapName, m_TexSelectTexListInfo.texList.Count));
                if (GUILayout.Button("CompressAll", GUILayout.MaxWidth(150)))
                {
                    CompressTex(m_TexSelectTexListInfo);
                }
                if (GUILayout.Button("DisableMipmapAll", GUILayout.MaxWidth(150)))
                {
                    DisableMipmap(m_TexSelectTexListInfo);
                }
                GUILayout.EndHorizontal();
                m_TexTexScrollPos = GUILayout.BeginScrollView(m_TexTexScrollPos, false, false);
                for (int i = 0; i < m_TexSelectTexListInfo.texList.Count; ++i)
                {
                    TexInfo texInfo = m_TexSelectTexListInfo.texList[i];
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(m_TexPlatform == ETexPlatform.Android ? texInfo.androidFormatStr : texInfo.iPhoneFormatStr, texInfo.tex, typeof(Texture), true);
                    if (GUILayout.Button("Compress", GUILayout.MaxWidth(70)))
                    {
                        Texture2D tex = texInfo.tex as Texture2D;
                        if (tex != null)
                            AssetModify.DefaultCompressTex(tex, texInfo.path, m_ResFolder == EResFolder.Equip, m_ResFolder == EResFolder.Equip);
                    }
                    if (texInfo.mipmap)
                    {
                        if (GUILayout.Button("DisableMipmap", GUILayout.MaxWidth(100)))
                        {
                            Texture2D tex = texInfo.tex as Texture2D;
                            if (tex != null)
                            {
                                AssetModify.EnableMipmap(tex, texInfo.path, false);
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("OpenMipmap", GUILayout.MaxWidth(100)))
                        {
                            Texture2D tex = texInfo.tex as Texture2D;
                            if (tex != null)
                            {
                                AssetModify.EnableMipmap(tex, texInfo.path, true);
                            }
                        }
                    }

                    if (GUILayout.Button(string.Format("Mat:{0}", texInfo.matPrefab.Count), GUILayout.MaxWidth(50)))
                    {
                        m_TexSelectTexInfo = texInfo;
                        m_SelectMatPrefab = null;
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }


        }

        void OnTexMatList()
        {
            if (m_TexSelectTexInfo != null)
            {
                m_SelectMatPrefabs = m_TexSelectTexInfo.matPrefab;
                OnMatList(m_TexSelectTexInfo.Name);
            }

        }

        private void ScanTexture(string path, string ext)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles(ext, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string texPath = fi.FullName.Replace("\\", "/");
                int index = texPath.IndexOf(path);
                texPath = texPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("Scan{0}-{1}/{2}", ext, i, files.Length), path, (float)i / files.Length);
                InnerScanTex(texPath);
            }
            EditorUtility.ClearProgressBar();
        }

        void ScanTexture(string path)
        {
            if (m_ResFolder == EResFolder.Equip || m_ResFolder == EResFolder.UI)
            {
                ScanTexture(path, "*.png");
                ScanTexture(path, "*.tga");
            }
        }
        private void ProcessTexListInfo(string formatStr, string formatStr2, List<TexListInfo> lst, TexInfo ti, long size, int sizeKey)
        {
            TexListInfo find = null;
            for (int i = 0; i < lst.Count; ++i)
            {
                TexListInfo tli = lst[i];
                if (tli.formatStr == formatStr)
                {
                    if (string.IsNullOrEmpty(formatStr2) || formatStr2 == tli.formatStr2)
                    {
                        find = tli;
                        break;
                    }
                }

            }
            if (find == null)
            {
                find = new TexListInfo();
                find.formatStr = formatStr;
                find.formatStr2 = formatStr2;
                find.sizeKey = sizeKey;
                lst.Add(find);
            }
            if (!find.texList.Contains(ti))
            {
                find.texList.Add(ti);
                find.size += size;
            }
        }
        private void RecordTexInfo(Texture tex, string path, TexInfo ti)
        {
            AssetImporter assetImport = AssetImporter.GetAtPath(path);
            TextureImporter texImport = assetImport as TextureImporter;
            if (texImport != null && tex != null)
            {
                ti.mipmap = texImport.mipmapEnabled;
                TextureImporterPlatformSettings tipsAndroid = texImport.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings tipsiPhone = texImport.GetPlatformTextureSettings("iPhone");
                long sizePer32Android = GetSize(tipsAndroid.format);
                long sizePer32iPhone = GetSize(tipsiPhone.format);
                ti.sizeAndroid = tex.width * tex.height * sizePer32Android / 4;
                if (texImport.mipmapEnabled)
                {
                    ti.sizeAndroid = (long)(ti.sizeAndroid * 1.33f);
                }
                string androidFormatStr = tipsAndroid.format.ToString();
                string androidSizeStr = string.Format("{0}x{1}", tex.width, tex.height);



                int iPhoneWidth = tex.width;
                int iPhoneHeigh = tex.height;
                string iPhoneSizeStr2 = "";

                if (tipsiPhone.format == TextureImporterFormat.PVRTC_RGB4 ||
                    tipsiPhone.format == TextureImporterFormat.PVRTC_RGB2 ||
                    tipsiPhone.format == TextureImporterFormat.PVRTC_RGBA2 ||
                    tipsiPhone.format == TextureImporterFormat.PVRTC_RGBA4)
                {

                    iPhoneWidth = tex.width > tex.height ? tex.width : tex.height;
                    iPhoneHeigh = iPhoneWidth;
                    TextureImporterFormat newFormat;
                    long uncompressed = GetUnCompressedSize(tipsiPhone.format, out newFormat);
                    long size = iPhoneWidth * iPhoneHeigh * sizePer32iPhone / 4;
                    long uncompressedsize = tex.width * tex.height * uncompressed / 4;
                    if (size < uncompressedsize)
                    {
                        if (tex.width != tex.height)
                        {
                            if (tipsiPhone.maxTextureSize <= tex.width && tipsiPhone.maxTextureSize <= tex.height)
                            {
                                iPhoneWidth = tipsiPhone.maxTextureSize;
                                iPhoneHeigh = tipsiPhone.maxTextureSize;
                                size = iPhoneWidth * iPhoneHeigh * sizePer32iPhone / 4;
                            }
                            else
                            {
                                iPhoneSizeStr2 = androidSizeStr;
                            }
                        }
                    }
                    else
                    {
                        iPhoneWidth = tex.width;
                        iPhoneHeigh = tex.height;
                        tipsiPhone.format = newFormat;
                    }

                }
                string iPhoneFormatStr = tipsiPhone.format.ToString();
                string iPhoneSizeStr = string.Format("{0}x{1}", iPhoneWidth, iPhoneHeigh);
                ti.sizeiPhone = iPhoneWidth * iPhoneHeigh * sizePer32iPhone / 4;
                if (texImport.mipmapEnabled)
                {
                    ti.sizeiPhone = (long)(ti.sizeiPhone * 1.33f);
                }
                ti.androidFormatStr = androidFormatStr;
                ti.iPhoneFormatStr = iPhoneFormatStr;
                ProcessTexListInfo(androidFormatStr, "", m_TexFormatAndroidList, ti, ti.sizeAndroid, 0);
                ProcessTexListInfo(iPhoneFormatStr, "", m_TexFormatiPhoneList, ti, ti.sizeiPhone, 0);

                ProcessTexListInfo(androidSizeStr, "", m_TexSizeAndroidList, ti, ti.sizeAndroid, tex.width * tex.height);
                ProcessTexListInfo(iPhoneSizeStr, iPhoneSizeStr2, m_TexSizeiPhoneList, ti, ti.sizeiPhone, iPhoneWidth * iPhoneHeigh);

                m_TexTotalAndroidSize += ti.sizeAndroid;
                m_TexTotaliPhoneSize += ti.sizeiPhone;
                m_TexTotalCount++;
            }


        }
        private long GetUnCompressedSize(TextureImporterFormat format, out TextureImporterFormat newFormat)
        {
            long sizePer32 = 1;
            newFormat = format;
            switch (format)
            {
                case TextureImporterFormat.PVRTC_RGB4:
                    newFormat = TextureImporterFormat.RGB24;
                    sizePer32 = 12;
                    break;
                case TextureImporterFormat.PVRTC_RGBA4:
                    newFormat = TextureImporterFormat.RGBA32;
                    sizePer32 = 16;
                    break;
                case TextureImporterFormat.PVRTC_RGB2:
                    newFormat = TextureImporterFormat.RGB16;
                    sizePer32 = 6;
                    break;
                case TextureImporterFormat.PVRTC_RGBA2:
                    newFormat = TextureImporterFormat.RGBA16;
                    sizePer32 = 8;
                    break;
            }
            return sizePer32;
        }

        private long GetSize(TextureImporterFormat format)
        {
            long sizePer32 = 1;
            switch (format)
            {
                case TextureImporterFormat.ETC_RGB4:
                case TextureImporterFormat.PVRTC_RGB4:
                case TextureImporterFormat.PVRTC_RGBA4:
                    sizePer32 = 2;
                    break;
                case TextureImporterFormat.PVRTC_RGB2:
                case TextureImporterFormat.PVRTC_RGBA2:
                    sizePer32 = 1;
                    break;
                case TextureImporterFormat.RGB16:
                    sizePer32 = 6;
                    break;
                case TextureImporterFormat.RGBA16:
                    sizePer32 = 8;
                    break;
                case TextureImporterFormat.RGB24:
                    sizePer32 = 12;
                    break;
                case TextureImporterFormat.RGBA32:
                    sizePer32 = 16;
                    break;
                case TextureImporterFormat.Alpha8:
                    sizePer32 = 4;
                    break;
            }
            return sizePer32;
        }

        private bool ProcessTex(string path, out TexInfo ti)
        {
            if (!m_TexTexInfoMap.TryGetValue(path, out ti))
            {
                ti = new TexInfo();
                ti.path = path;
                m_TexTexInfoMap.Add(path, ti);
                return true;
            }
            return false;
        }

        private void InnerScanTex(string path)
        {
            TexInfo ti = null;
            if (ProcessTex(path, out ti))
            {
                Texture tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
                ti.tex = tex;
                RecordTexInfo(tex, path, ti);
            }
            else
            {
                Debug.LogError(string.Format("null tex:{0}", path));
            }
        }

        private TexInfo InnerScanTex(Texture tex, string prefabPath, string matName)
        {
            if (tex != null)
            {
                TexInfo ti = null;
                string path = AssetDatabase.GetAssetPath(tex);
                if (ProcessTex(path, out ti))
                {
                    ti.tex = tex;
                    RecordTexInfo(tex, path, ti);
                }
                return ti;
            }
            else
            {
                Debug.LogError(string.Format("null tex,mat:{0} prefab:{1}", matName, prefabPath));
                return null;
            }
        }
        private void CompressTex(TexListInfo texListInfo)
        {
            for (int i = 0; i < texListInfo.texList.Count; ++i)
            {
                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "compress tex", i, texListInfo.texList.Count), "", (float)i / texListInfo.texList.Count);
                TexInfo ti = texListInfo.texList[i];
                Texture2D tex = ti.tex as Texture2D;
                if (tex != null)
                {
                    AssetModify.DefaultCompressTex(tex, ti.path, m_ResFolder == EResFolder.Equip, m_ResFolder == EResFolder.Equip);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private void DisableMipmap(TexListInfo texListInfo)
        {
            for (int i = 0; i < texListInfo.texList.Count; ++i)
            {
                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", "compress tex", i, texListInfo.texList.Count), "", (float)i / texListInfo.texList.Count);
                TexInfo ti = texListInfo.texList[i];
                Texture2D tex = ti.tex as Texture2D;
                if (tex != null)
                {
                    AssetModify.EnableMipmap(tex, ti.path, false);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        void ScanUI(GameObject prefab, GameObject instance, string prefabPath)
        {
            instance.GetComponentsInChildren<UITexture>(true, uiTextureList);
            if (uiTextureList.Count > 0)
            {
                for (int i = 0; i < uiTextureList.Count; ++i)
                {
                    UITexture uiTex = uiTextureList[i];
                    if (!string.IsNullOrEmpty(uiTex.texPath))
                    {
                        string path = "Assets/Resources/" + uiTex.texPath + ".png";
                        Texture tex = null;
                        if (!File.Exists(path))
                            Debug.LogError(string.Format("Tex not found:{0} prefab:{1} go:{2}", uiTex.texPath, prefabPath, uiTex.name));
                        else
                        {
                            tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
 
                        }
                       
                        if (tex != null)
                        {
                            int index = uiTex.texPath.LastIndexOf("/");
                            string alphapath = uiTex.texPath;
                            if (index >= 0 && index < uiTex.texPath.Length - 1)
                            {
                                alphapath = uiTex.texPath.Substring(index + 1);
                            }
                            else
                            {
                                Debug.LogError(string.Format("prefab:{0} texpath:{1} gameobject:{2}", prefabPath, uiTex.texPath, uiTex.name));
                            }
                            alphapath = "Assets/Resources/atlas/UI/Alpha/" + alphapath + "_A.png";
                            Texture alphaTex = AssetDatabase.LoadAssetAtPath<Texture>(alphapath);
                            Material uiTexMat = new Material(Shader.Find("Custom/UI/SeparateColorAlpha"));
                            uiTexMat.name = prefab.name;
                            MatPrefab mp = null;
                            RecordMatPrefab(prefabPath, prefab, uiTexMat, out mp);
                            RecordShderMatPrefab(prefabPath, uiTexMat, mp);
                            TexInfo ti = null;

                            if (ProcessTex(path, out ti))
                            {
                                ti.tex = tex;
                                RecordTexInfo(tex, path, ti);
                            }
                            if (ti != null)
                            {
                                if (!ti.matPrefab.Contains(mp))
                                {
                                    ti.matPrefab.Add(mp);
                                }
                            }
                            if (alphaTex != null)
                            {
                                if (ProcessTex(alphapath, out ti))
                                {
                                    ti.tex = alphaTex;
                                    RecordTexInfo(alphaTex, alphapath, ti);
                                }
                                if (ti != null)
                                {
                                    if (!ti.matPrefab.Contains(mp))
                                    {
                                        ti.matPrefab.Add(mp);
                                    }
                                }
                            }

                        }


                    }
                }
            }

            uiTextureList.Clear();
        }
        #endregion
        #region MeshFun
        void OnMeshHead()
        {
            EditorGUILayout.LabelField(string.Format("Total Count:{0}", m_MeshTotalCount));
            if (m_MeshSelectedSizeMeshInfo != null)
            {
                EditorGUILayout.LabelField(string.Format("Mesh Size Type :{0}", m_MeshSelectedSizeMeshInfo.name));
                int buttonState = 0;
                if (GUILayout.Button("Readable", GUILayout.MaxWidth(100)))
                {
                    buttonState = 1;
                }
                if (GUILayout.Button("NotReadable", GUILayout.MaxWidth(100)))
                {
                    buttonState = 2;
                }
                if (buttonState != 0)
                {
                    for (int i = 0; i < m_MeshSelectedSizeMeshInfo.MeshList.Count; ++i)
                    {
                        m_MeshSelectedSizeMeshInfo.MeshList[i].Export(buttonState == 1);
                    }
                }
            }
            m_MeshInfoCategory = (EMeshInfoCategory)EditorGUILayout.EnumPopup("MeshInfo", m_MeshInfoCategory, GUILayout.MaxWidth(250));

        }
        void OnMeshSizeList()
        {
            m_MeshSizeScrollPos = GUILayout.BeginScrollView(m_MeshSizeScrollPos, false, false);
            for (int i = 0; i < m_MeshSizeList.Count; ++i)
            {
                SizeMeshInfo smi = m_MeshSizeList[i];
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("{0} Count:{1}", smi.name, smi.MeshList.Count));
                if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
                {
                    m_MeshSelectedSizeMeshInfo = smi;
                    if (!m_MeshSelectedSizeMeshInfo.sorted)
                    {
                        m_MeshSelectedSizeMeshInfo.MeshList.Sort(m_SizeSort);
                        m_MeshSelectedSizeMeshInfo.sorted = true;
                    }
                    m_MeshSelectedMeshInfo = null;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        void OnMeshInfoList()
        {
            m_MeshMeshScrollPos = GUILayout.BeginScrollView(m_MeshMeshScrollPos, false, false);
            if (m_MeshSelectedSizeMeshInfo != null)
            {
                bool nextRead = false;
                for (int i = 0; i < m_MeshSelectedSizeMeshInfo.MeshList.Count; ++i)
                {
                    bool readWarning = false;
                    if (nextRead)
                    {
                        readWarning = true;
                        nextRead = false;
                    }
                    MeshInfo mi = m_MeshSelectedSizeMeshInfo.MeshList[i];
                    if (i + 1 < m_MeshSelectedSizeMeshInfo.MeshList.Count)
                    {
                        MeshInfo nextMi = m_MeshSelectedSizeMeshInfo.MeshList[i + 1];
                        int vc = Math.Abs(mi.vertexCount - nextMi.vertexCount);
                        int index = Math.Abs(mi.triangleCount - nextMi.triangleCount);
                        if (vc < 1 && index < 1)
                        {
                            readWarning = true;
                            nextRead = true;
                        }
                    }
                    if (mi.mesh != null)
                    {
                        if (m_MeshInfoCategory != EMeshInfoCategory.Vertex)
                        {
                            readWarning = false;
                        }
                        GUILayout.BeginHorizontal();
                        if (redStyle == null)
                        {
                            redStyle = new GUIStyle();
                            redStyle.normal.textColor = Color.red;
                        }
                        string str = "";
                        switch (m_MeshInfoCategory)
                        {
                            case EMeshInfoCategory.Vertex:
                                str = string.Format("v:{0} t:{1}", mi.vertexCount, mi.triangleCount);
                                break;
                            case EMeshInfoCategory.Fbx:
                                str = mi.fbx ? "Fbx" : "";
                                break;
                            case EMeshInfoCategory.UV2:
                                str = mi.hasUV2 ? "UV2" : "";
                                break;
                            case EMeshInfoCategory.Optimize:
                                str = mi.optimize ? "Optimize" : "";
                                break;
                            case EMeshInfoCategory.Readable:
                                if(mi.disaplyReadable)
                                {
                                    str = mi.readable ? "Readable" : "";
                                }
                                else
                                {
                                    str = mi.readable ? "" : "NotReadable";
                                }
                                if(mi.hasSkin)
                                {
                                    if(m_ResFolder== EResFolder.Equip)
                                    {
                                        if(mi.mesh.name.EndsWith("_weapon"))
                                        {
                                            if (mi.readable)
                                            {
                                                str = mi.readable ? "Readable" : "";
                                                readWarning = true;
                                            }
                                        }
                                        else
                                        {
                                            if (mi.readable)
                                            {
                                                str = "";
                                                readWarning = false;
                                            }
                                            else
                                            {
                                                str = "NotReadable";
                                                readWarning = true;
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        if (mi.readable)
                                        {
                                            str = mi.readable ? "Readable" : "";
                                            readWarning = true;
                                        }
                                    }
                                    
                                }
                                else if (mi.disaplyReadable && mi.readable || !mi.disaplyReadable && !mi.readable)
                                {
                                    readWarning = true;
                                }


                                break;
                            case EMeshInfoCategory.OptimizeGameObject:
                                str = mi.optimizeGameObject ? "OptimizeGameObject" : "";
                                break;
                            case EMeshInfoCategory.Normal:
                                str = mi.normal ? "Normal" : "";
                                break;
                            case EMeshInfoCategory.Tangents:
                                str = mi.tangents ? "Tangents" : "";
                                break;
                        }
                        if (readWarning)
                        {
                            EditorGUILayout.LabelField(str, redStyle, GUILayout.MaxWidth(150));
                        }
                        else
                        {
                            EditorGUILayout.LabelField(str, GUILayout.MaxWidth(150));
                        }

                        EditorGUILayout.ObjectField("", mi.mesh, typeof(Mesh), true, GUILayout.MaxWidth(450));
                        if (GUILayout.Button("Prefab", GUILayout.MaxWidth(50)))
                        {
                            m_MeshSelectedMeshInfo = mi;
                        }
                        if (GUILayout.Button("NoRead", GUILayout.MaxWidth(80)))
                        {
                            mi.Export(false);
                        }
                        if (GUILayout.Button("Read", GUILayout.MaxWidth(50)))
                        {
                            mi.Export(true);
                        }
                        GUILayout.EndHorizontal();
                    }

                }
            }
            EditorGUILayout.EndScrollView();
        }
        void OnMeshPrefabList()
        {
            m_MeshPrefabScrollPos = GUILayout.BeginScrollView(m_MeshPrefabScrollPos, false, false);
            if (m_MeshSelectedMeshInfo != null)
            {
                for (int i = 0; i < m_MeshSelectedMeshInfo.PrefabList.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    UnityEngine.Object obj = m_MeshSelectedMeshInfo.PrefabList[i];
                    if (obj != null)
                    {
                        EditorGUILayout.ObjectField("", obj, typeof(UnityEngine.Object), true);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        private void InnerScanMesh(Mesh mesh, GameObject obj,string meshPath)
        {
            int vertexCount = mesh.vertexCount;
            int index = 0;
            bool displayReadable = m_ResFolder != EResFolder.Scene;
            if (vertexCount >= 0 && vertexCount < 200)
            {
                index = 0;
            }
            else if (vertexCount >= 200 && vertexCount < 500)
            {
                index = 1;

            }
            else if (vertexCount >= 500 && vertexCount < 1000)
            {
                index = 2;
            }
            else if (vertexCount >= 1000 && vertexCount < 1500)
            {
                index = 3;
            }
            else
            {
                index = 4;
                if (m_ResFolder == EResFolder.Scene)
                    displayReadable = true;
            }

            MeshInfo findMi = null;
            SizeMeshInfo smi = m_MeshSizeList[index];
            for (int i = 0; i < smi.MeshList.Count; ++i)
            {
                MeshInfo mi = smi.MeshList[i];
                if (mi.mesh == mesh)
                {
                    findMi = mi;
                    break;
                }
            }
            if (findMi == null)
            {
                string path = AssetDatabase.GetAssetPath(mesh);
                ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

                findMi = new MeshInfo();
                findMi.mesh = mesh;
                smi.MeshList.Add(findMi);
                findMi.vertexCount = vertexCount;
                findMi.triangleCount = (int)mesh.GetIndexCount(0) / 3;
                Vector2[] uv2 = mesh.uv2;
                findMi.hasUV2 = uv2 != null && uv2.Length > 0;
                findMi.readable = mesh.isReadable;
                findMi.disaplyReadable = displayReadable;
                findMi.hasSkin = mesh.bindposes.Length > 0;
                Vector3[] normal = mesh.normals;
                findMi.normal = normal != null && normal.Length > 0;
                Vector4[] tangents = mesh.tangents;
                findMi.tangents = tangents != null && tangents.Length > 0;
                findMi.optimize = modelImporter != null ? modelImporter.optimizeMesh : true;
                findMi.optimizeGameObject = modelImporter != null ? modelImporter.optimizeGameObjects : true;
                findMi.fbx = modelImporter != null;
                m_MeshTotalCount++;
            }
            if (obj != null)
            {
                int find = findMi.PrefabList.IndexOf(obj);
                if (find < 0)
                {
                    findMi.PrefabList.Add(obj);
                }
            }
        }
        void ScanMesh(GameObject prefab, GameObject instance)
        {
            instance.GetComponentsInChildren<MeshFilter>(true, mfList);
            for (int j = 0; j < mfList.Count; ++j)
            {
                Mesh mesh = mfList[j].sharedMesh;
                if (mesh != null)
                    InnerScanMesh(mesh, prefab,"");
            }
            mfList.Clear();
            instance.GetComponentsInChildren<SkinnedMeshRenderer>(true, smrList);
            for (int j = 0; j < smrList.Count; ++j)
            {
                Mesh mesh = smrList[j].sharedMesh;
                if (mesh != null)
                    InnerScanMesh(mesh, prefab, "");
            }
            smrList.Clear();
        }
        void ScanMesh(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.asset", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string meshPath = fi.FullName.Replace("\\", "/");
                int index = meshPath.IndexOf(path);
                meshPath = meshPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("Scan Mesh Asset-{0}/{1}", i, files.Length), path, (float)i / files.Length);
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh != null)
                    InnerScanMesh(mesh, null, meshPath);
            }
            EditorUtility.ClearProgressBar();
        }
        #endregion
        #region common
        private void RecordShderMatPrefab(string prefabPath, Material mat, MatPrefab mp)
        {
            Shader shader = mat.shader;
            if (shader != null)
            {
                List<MatPrefab> mpList = null;
                if (!m_Shaders.TryGetValue(shader, out mpList))
                {
                    mpList = new List<MatPrefab>();
                    m_Shaders[shader] = mpList;
                }
                if (mp != null && mpList.IndexOf(mp) < 0)
                {
                    mpList.Add(mp);
                }
            }
            else
            {
                Debug.LogError("null shader:" + prefabPath);
            }
        }

        private bool RecordMatPrefab(string prefabPath, GameObject prefab, Material mat, out MatPrefab mp)
        {
            bool matNotProcessed = false;
            if (!m_MatPrefabList.TryGetValue(mat, out mp))
            {
                mp = new MatPrefab();
                mp.mat = mat;
                m_MatPrefabList.Add(mat, mp);
                matNotProcessed = true;
            }
            if (prefab != null && !mp.prefabs.Contains(prefab))
                mp.prefabs.Add(prefab);
            return matNotProcessed;
        }

        private void InnerScanMat(string prefabPath, GameObject prefab, Material mat)
        {
            if (mat == null)
            {
                Debug.LogError("null mat:" + prefabPath);
            }
            else
            {
                MatPrefab mp = null;
                RecordMatPrefab(prefabPath, prefab, mat, out mp);
                RecordShderMatPrefab(prefabPath, mat, mp);
                AssetModify.GetMatTex(mat, m_TexFindCache);
                for (int i = 0; i < m_TexFindCache.Count; ++i)
                {
                    Texture t = m_TexFindCache[i];
                    TexInfo ti = InnerScanTex(t, prefabPath, mat.name);
                    if (ti != null)
                    {
                        if (!ti.matPrefab.Contains(mp))
                        {
                            ti.matPrefab.Add(mp);
                        }
                    }
                }
                m_TexFindCache.Clear();
            }
        }

        void ScanRender(GameObject prefab, GameObject instance, string prefabPath)
        {
            instance.GetComponentsInChildren<Renderer>(true, renderList);
            for (int i = 0; i < renderList.Count; ++i)
            {
                Renderer render = renderList[i];
                if (render is ParticleSystemRenderer)
                {
                    ParticleSystemRenderer psr = render as ParticleSystemRenderer;
                    InnerScanMat(prefabPath, prefab, psr.sharedMaterial);
                }
                else
                {
                    Material[] mats = render.sharedMaterials;
                    if (mats == null || mats.Length == 0)
                    {
                        Debug.LogError("no mat:" + prefabPath);
                    }
                    else
                    {
                        foreach (Material mat in mats)
                        {
                            InnerScanMat(prefabPath, prefab, mat);
                        }
                    }
                }
            }
            renderList.Clear();
        }
        void Scan(int resType, string paths)
        {
            m_MatPrefabList.Clear();
            m_MatScrollPos = Vector2.zero;
            m_PrefabScrollPos = Vector2.zero;

            m_SelectMatPrefabs = null;
            m_SelectMatPrefab = null;
            //shader
            m_Shaders.Clear();
            m_ShaderSelectShader = null;
            m_ShaderReplaceShader = null;
            m_FindMaterial.Clear();
            //tex
            m_TexFormatAndroidList.Clear();
            m_TexFormatiPhoneList.Clear();
            m_TexSizeAndroidList.Clear();
            m_TexSizeiPhoneList.Clear();
            m_TexTexInfoMap.Clear();
            m_TexTotalAndroidSize = 0;
            m_TexTotaliPhoneSize = 0;
            m_TexTotalCount = 0;
            m_texMapName = "";
            m_TexSelectTexListInfo = null;
            m_TexSelectTexInfo = null;
            //mesh
            for (int i = 0; i < m_MeshSizeList.Count; ++i)
            {
                m_MeshSizeList[i].Clear();
            }
            m_MeshSelectedSizeMeshInfo = null;
            m_MeshSelectedMeshInfo = null;
            m_MeshTotalCount = 0;

            EditorUtility.UnloadUnusedAssetsImmediate();

            string[] pathLst = paths.Split('|');
            foreach (string path in pathLst)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = files = di.GetFiles("*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; ++i)
                {
                    FileInfo fi = files[i];
                    string prefabPath = fi.FullName.Replace("\\", "/");
                    int index = prefabPath.IndexOf(path);
                    prefabPath = prefabPath.Substring(index);
                    EditorUtility.DisplayProgressBar(string.Format("ScanPrefab-{0}/{1}", i, files.Length), path, (float)i / files.Length);

                    GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                    if (prefab != null)
                    {
                        GameObject instance = GameObject.Instantiate<GameObject>(prefab);
                        //mesh
                        ScanMesh(prefab, instance);
                        //render
                        ScanRender(prefab, instance, prefabPath);
                        //ui
                        ScanUI(prefab, instance, prefabPath);
                        GameObject.DestroyImmediate(instance);
                    }
                }
                EditorUtility.ClearProgressBar();
                for (int i = 0; i < onScan.Length; ++i)
                {
                    if (onScan[i] != null)
                    {
                        onScan[i](path);
                    }
                }

            }
            m_TexFormatAndroidList.Sort(m_TexListSort);
            m_TexFormatiPhoneList.Sort(m_TexListSort);
            m_TexSizeAndroidList.Sort(m_TexListSort);
            m_TexSizeiPhoneList.Sort(m_TexListSort);
        }
        #endregion
        #region ui
        void OnMatList(string name)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Name:{0} Count:{1}", name, m_SelectMatPrefabs != null ? m_SelectMatPrefabs.Count : 0));
            GUILayout.EndHorizontal();
            if (m_SelectMatPrefabs != null)
            {
                m_MatScrollPos = GUILayout.BeginScrollView(m_MatScrollPos, false, false);
                for (int i = 0; i < m_SelectMatPrefabs.Count; ++i)
                {
                    MatPrefab mp = m_SelectMatPrefabs[i];
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(mp.Name, mp.mat, typeof(Material), true, GUILayout.MaxWidth(450));
                    if (GUILayout.Button("Prefab", GUILayout.MaxWidth(70)))
                    {
                        m_SelectMatPrefab = mp;
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }


        }

        void OnPrefabList()
        {

            if (m_SelectMatPrefab != null)
            {
                EditorGUILayout.LabelField(string.Format("Name:{0} Count:{1}", m_SelectMatPrefab.Name, m_SelectMatPrefab.prefabs.Count));
                m_PrefabScrollPos = GUILayout.BeginScrollView(m_PrefabScrollPos, false, false);
                for (int i = 0; i < m_SelectMatPrefab.prefabs.Count; ++i)
                {
                    GameObject prefab = m_SelectMatPrefab.prefabs[i];
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(prefab != null ? prefab.name : "", prefab, typeof(GameObject), true, GUILayout.MaxWidth(450));
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

        }
        void OnGUIBlock(OnResGUI onGuiLeft, OnResGUI onGuiRight)
        {
            //box left
            GUILayout.BeginVertical(GUI.skin.box, verticalMaxWidth, verticalMinWidth, verticalMaxHeigth, verticalMinHeigth);
            if (onGuiLeft != null)
            {
                onGuiLeft();
            }
            GUILayout.EndVertical();

            //box right
            GUILayout.BeginVertical(GUI.skin.box, verticalMaxWidth, verticalMinWidth, verticalMaxHeigth, verticalMinHeigth);
            if (onGuiRight != null)
            {
                onGuiRight();
            }
            GUILayout.EndVertical();
        }

        void SizeChange()
        {
            bool change = false;
            if (m_Width != this.position.width)
            {
                m_Width = this.position.width;
                change = true;
            }
            if (m_Height != this.position.height)
            {
                m_Height = this.position.height;
                change = true;
            }
            if (horizontalMaxWidth == null || change)
            {
                horizontalMaxWidth = GUILayout.MaxWidth(m_Width - 8);
            }
            if (horizontalMinWidth == null || change)
            {
                horizontalMinWidth = GUILayout.MinWidth(m_Width - 8);
            }

            if (horizontalMaxHeigth == null || change)
            {
                horizontalMaxHeigth = GUILayout.MaxHeight(m_Height / 2 - 30);
            }
            if (horizontalMinHeigth == null || change)
            {
                horizontalMinHeigth = GUILayout.MinHeight(m_Height / 2 - 30);
            }

            if (verticalMaxWidth == null || change)
            {
                verticalMaxWidth = GUILayout.MaxWidth(m_Width / 2 - 8);
            }
            if (verticalMinWidth == null || change)
            {
                verticalMinWidth = GUILayout.MinWidth(m_Width / 2 - 8);
            }

            if (verticalMaxHeigth == null || change)
            {
                verticalMaxHeigth = GUILayout.MaxHeight(m_Height / 2 - 30);
            }
            if (verticalMinHeigth == null || change)
            {
                verticalMinHeigth = GUILayout.MinHeight(m_Height / 2 - 30);
            }
        }

        void OnGUI()
        {
            SizeChange();
            GUILayout.BeginHorizontal();
            m_ResType = (EResType)EditorGUILayout.EnumPopup("资源统计类型", m_ResType, GUILayout.MaxWidth(250));
            m_ResFolder = (EResFolder)EditorGUILayout.EnumPopup("资源统计目录", m_ResFolder, GUILayout.MaxWidth(250));
            int resType = (int)m_ResType;
            if (GUILayout.Button("Scan", GUILayout.MaxWidth(150)))
            {
                string resFolder = m_ResPaths[(int)m_ResFolder];
                Scan(resType, resFolder);
            }
            GUILayout.EndHorizontal();

            //head line
            GUILayout.BeginHorizontal();
            if (onHead[resType] != null)
            {
                onHead[resType]();
            }
            GUILayout.EndHorizontal();

            //box up
            GUILayout.BeginHorizontal(GUI.skin.box, horizontalMaxWidth, horizontalMinWidth, horizontalMaxHeigth, horizontalMinHeigth);
            OnGUIBlock(onResGui[resType * 4], onResGui[resType * 4 + 1]);
            GUILayout.EndHorizontal();

            //box down
            GUILayout.BeginHorizontal(GUI.skin.box, horizontalMaxWidth, horizontalMinWidth, horizontalMaxHeigth, horizontalMinHeigth);
            OnGUIBlock(onResGui[resType * 4 + 2], onResGui[resType * 4 + 3]);
            GUILayout.EndHorizontal();
        }
        #endregion
    }

    public class TerrainEditor : EditorWindow
    {
        private Terrain terrain;
        private Terrain[] terrains;
        //[MenuItem(@"Assets/TerrainEditor", false, 0)]
        //private static void EditTerrain()
        //{
        //    TerrainEditor window = EditorWindow.GetWindow<TerrainEditor>("TerrainEditor", true);
        //    window.position = new Rect(100, 100, 1200, 800);
        //}
        private void ExportTerrainBlendTex(TerrainData td, List<Texture2D> texs)
        {
            float[,,] map = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
            int layerCount = map.GetLength(2);
            if (layerCount == 0)
                return;

            for (int i = 0; i < layerCount; i += 4)
            {
                texs.Add(new Texture2D(td.alphamapWidth, td.alphamapHeight, TextureFormat.ARGB32, false));
            }
            int maxLayCount = texs.Count * 4;
            float[] colorValue = new float[maxLayCount];
            for (int y = 0; y < td.alphamapHeight; y++)
            {
                for (int x = 0; x < td.alphamapWidth; x++)
                {
                    for (int i = 0; i < colorValue.Length; ++i)
                    {

                        if (i < layerCount)
                        {
                            colorValue[i] = map[y, x, i];
                        }
                        else
                        {
                            colorValue[i] = 0.0f;
                        }
                    }
                    int index = 0;
                    for (int i = 0; i < layerCount; i += 4)
                    {
                        Color c = new Color(colorValue[i], colorValue[i + 1], colorValue[i + 2], colorValue[i + 3]);
                        texs[index++].SetPixel(x, y, c);
                    }

                }
            }
            for (int i = 0; i < texs.Count; ++i)
            {
                if (texs[i] != null)
                    texs[i].Apply();
            }
        }
        private void ExportMesh()
        {
            if(terrain!=null)
            {
                TerrainData td = terrain.terrainData;


                float width = td.size.x;
                float length = td.size.z;

                float heithMapWidthScale = td.heightmapResolution/width;
                float heightMapLengthScale = td.heightmapResolution/length;

                int widthVertexCount = td.alphamapWidth;
                int lengthVertexCount = td.alphamapHeight;
                float widthPerBlock = width / (float)(td.alphamapWidth - 1);
                float lengthPerBlock = length / (float)(td.alphamapHeight - 1);
                float uvPerBlock = 1.0f / (float)(td.alphamapWidth - 1);
                List<Vector3> vertex = new List<Vector3>();
                List<int> index = new List<int>();
                List<Vector2> uv = new List<Vector2>();
                //float minWidth = 30.0f;
                //float minLength = 23.0f;
                //float maxWidth = 138.3f;
                //float maxLength = 130.3f;
                for (int z = 0; z < lengthVertexCount - 1; ++z)
                {
                    for (int x = 0; x < widthVertexCount - 1; ++x)
                    {
                        float currentX = x * widthPerBlock;
                        float currentY = z * lengthPerBlock;
                        //if (currentX >= minWidth && currentX <= maxWidth && currentY >= minLength && currentY <= maxLength)
                        {
                            int vertexIndex = vertex.Count;

                            index.Add(vertexIndex); index.Add(vertexIndex + 2); index.Add(vertexIndex + 1);
                            index.Add(vertexIndex); index.Add(vertexIndex + 3); index.Add(vertexIndex + 2);
                            Vector3 p0 = new Vector3(x * widthPerBlock, 0, z * lengthPerBlock);
                            p0.y = td.GetHeight((int)(p0.x * heithMapWidthScale), (int)(p0.z * heightMapLengthScale));
                            Vector3 p1 = new Vector3((x + 1) * widthPerBlock, 0, z * lengthPerBlock);
                            p1.y = td.GetHeight((int)(p1.x * heithMapWidthScale), (int)(p1.z * heightMapLengthScale));
                            Vector3 p2 = new Vector3((x + 1) * widthPerBlock, 0, (z + 1) * lengthPerBlock);
                            p2.y = td.GetHeight((int)(p2.x * heithMapWidthScale), (int)(p2.z * heightMapLengthScale));
                            Vector3 p3 = new Vector3(x * widthPerBlock, 0, (z + 1) * lengthPerBlock);
                            p3.y = td.GetHeight((int)(p3.x * heithMapWidthScale), (int)(p3.z * heightMapLengthScale));
                            vertex.Add(p0);
                            vertex.Add(p1);
                            vertex.Add(p2);
                            vertex.Add(p3);
                            Vector2 uv0 = new Vector2(x * uvPerBlock, z * uvPerBlock);
                            Vector2 uv1 = new Vector2((x + 1) * uvPerBlock, z * uvPerBlock);
                            Vector2 uv2 = new Vector2((x + 1) * uvPerBlock, (z + 1) * uvPerBlock);
                            Vector2 uv3 = new Vector2(x * uvPerBlock, (z + 1) * uvPerBlock);
                            uv.Add(uv0);
                            uv.Add(uv1);
                            uv.Add(uv2);
                            uv.Add(uv3);
                        } 
                    }
                }
                UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
                Mesh terrainMesh = new Mesh();
                terrainMesh.name = s.name+ "_terrain";
                terrainMesh.vertices = vertex.ToArray();
                terrainMesh.triangles = index.ToArray();
                terrainMesh.uv = uv.ToArray();
                terrainMesh.uv2 = null;
                MeshUtility.SetMeshCompression(terrainMesh, ModelImporterMeshCompression.Medium);
                MeshUtility.Optimize(terrainMesh);
                terrainMesh.UploadMeshData(true);
                string sceneDir = "";
                
                int j = s.path.LastIndexOf("/");
                if(j >= 0)
                {
                    sceneDir = s.path.Substring(0, j);
                }
                string terrainMehsPath = string.Format("{0}/{1}_terrain.asset", sceneDir, s.name);
                AssetModify.CreateOrReplaceAsset<Mesh>(terrainMesh, terrainMehsPath);
                List<Texture2D> texs = new List<Texture2D>();
                ExportTerrainBlendTex(td, texs);
                for (int i = 0; i < texs.Count; ++i)
                {
                    if (texs[i] != null)
                    {
                        byte[] bytes = texs[i].EncodeToPNG();
                        string filePath = string.Format("{0}/{1}_{2}.png", sceneDir, s.name, i);
                        File.WriteAllBytes(filePath, bytes);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }

        }
        //private void DisplayTerrainInfo()
        //{
        //    if (terrains != null)
        //    {
        //        for(int i=0;i< terrains.Length;++i)
        //        {
        //            TerrainData td = terrains[i].terrainData;

        //        }
        //    }

        //}
        protected virtual void OnGUI()
        {
            if(terrain==null)
            {
                terrain = Terrain.activeTerrain;
            }
            if (terrains == null)
            {
                terrains = Terrain.activeTerrains;
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ExportMesh", GUILayout.MaxWidth(100)))
            {
                ExportMesh();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif