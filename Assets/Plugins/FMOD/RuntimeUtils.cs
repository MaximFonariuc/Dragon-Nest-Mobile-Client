using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FMODUnity
{
    public class EventNotFoundException : Exception
    {
        public Guid Guid;
        public string Path;
        public EventNotFoundException(string path)
            : base("FMOD Studio event not found '" + path + "'")
        {           
            Path = path;
        }

        public EventNotFoundException(Guid guid)
            : base("FMOD Studio event not found " + guid.ToString("b") + "")
        {
            Guid = guid;
        }
    }
        
    public class BusNotFoundException : Exception
    {
        public string Path;
        public BusNotFoundException(string path)
            : base("FMOD Studio bus not found '" + path + "'")
        {           
            Path = path;
        }
    }

    public class VCANotFoundException : Exception
    {
        public string Path;
        public VCANotFoundException(string path)
            : base("FMOD Studio VCA not found '" + path + "'")
        {
            Path = path;
        }
    }

    public class BankLoadException : Exception
    {
        public string Path;
        public FMOD.RESULT Result;

        public BankLoadException(string path, FMOD.RESULT result)
            : base(String.Format("FMOD Studio could not load bank '{0}' : {1} : {2}", path, result.ToString(), FMOD.Error.String(result)))
        {
            Path = path;
            Result = result;
        }
        public BankLoadException(string path, string error)
            : base(String.Format("FMOD Studio could not load bank '{0}' : {1}", path, error))
        {
            Path = path;
            Result = FMOD.RESULT.ERR_INTERNAL;
        }
    }

    public class SystemNotInitializedException : Exception
    {
        public FMOD.RESULT Result;
        public string Location;

        public SystemNotInitializedException(FMOD.RESULT result, string location)
            : base(String.Format("FMOD Studio initialization failed : {2} : {0} : {1}", result.ToString(), FMOD.Error.String(result), location))
        {
            Result = result;
            Location = location;
        }

        public SystemNotInitializedException(Exception inner)
            : base("FMOD Studio initialization failed", inner)
        {
        }
    }

    public enum EmitterGameEvent
    {
        None,
        ObjectStart,
        ObjectDestroy,
        TriggerEnter,
        TriggerExit,
        TriggerEnter2D,
        TriggerExit2D,
        CollisionEnter,
        CollisionExit,
        CollisionEnter2D,
        CollisionExit2D,
        ObjectEnable,
        ObjectDisable
    }

    public enum LoaderGameEvent
    {
        None,
        ObjectStart,
        ObjectDestroy,
        TriggerEnter,
        TriggerExit,
        TriggerEnter2D,
        TriggerExit2D,
    }
    
    public static class RuntimeUtils
    {
        public const string LogFileName = "fmod.log";

        public static FMOD.VECTOR ToFMODVector(this Vector3 vec)
        {
            FMOD.VECTOR temp;
            temp.x = vec.x;
            temp.y = vec.y;
            temp.z = vec.z;

            return temp;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(this Vector3 pos)
        {
            FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();
            attributes.forward = ToFMODVector(Vector3.forward);
            attributes.up = ToFMODVector(Vector3.up);
            attributes.position = ToFMODVector(pos);

            return attributes;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(this Transform transform)
        {
            FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();
            attributes.forward = transform.forward.ToFMODVector();
            attributes.up = transform.up.ToFMODVector();
            attributes.position = transform.position.ToFMODVector();

            return attributes;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody rigidbody = null)
        {
            FMOD.ATTRIBUTES_3D attributes = transform.To3DAttributes();

            if (rigidbody)
            {
                attributes.velocity = rigidbody.velocity.ToFMODVector();
            }

            return attributes;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody rigidbody = null)
        {
            FMOD.ATTRIBUTES_3D attributes = go.transform.To3DAttributes();

            if (rigidbody)
            {
                attributes.velocity = rigidbody.velocity.ToFMODVector();
            }

            return attributes;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody2D rigidbody)
        {
            FMOD.ATTRIBUTES_3D attributes = transform.To3DAttributes();

            if (rigidbody)
            {
                FMOD.VECTOR vel;
                vel.x = rigidbody.velocity.x;
                vel.y = rigidbody.velocity.y;
                vel.z = 0;
                attributes.velocity = vel;
            }

            return attributes;
        }

        public static FMOD.ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody2D rigidbody)
        {
            FMOD.ATTRIBUTES_3D attributes = go.transform.To3DAttributes();

            if (rigidbody)
            {
                FMOD.VECTOR vel;
                vel.x = rigidbody.velocity.x;
                vel.y = rigidbody.velocity.y;
                vel.z = 0;
                attributes.velocity = vel;
            }

            return attributes;
        }

        // Internal Helper Functions
        internal static FMODPlatform GetCurrentPlatform()
        {
            #if UNITY_EDITOR
            return FMODPlatform.PlayInEditor;
            #elif UNITY_STANDALONE_WIN
            return FMODPlatform.Windows;
            #elif UNITY_STANDALONE_OSX
            return FMODPlatform.Mac;
            #elif UNITY_STANDALONE_LINUX
            return FMODPlatform.Linux;
			#elif UNITY_TVOS
			return FMODPlatform.AppleTV;
            #elif UNITY_IOS
            FMODPlatform result;
            switch (UnityEngine.iOS.Device.generation)
            {
			case UnityEngine.iOS.DeviceGeneration.iPhone5:
			case UnityEngine.iOS.DeviceGeneration.iPhone5C:
			case UnityEngine.iOS.DeviceGeneration.iPhone5S:
			case UnityEngine.iOS.DeviceGeneration.iPadAir1:
			case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPhone6:
			case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
			case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadAir2:
                result = FMODPlatform.MobileHigh;
				break;
            default:
                result = FMODPlatform.MobileLow;
				break;
            }

            UnityEngine.Debug.Log(String.Format("FMOD Studio: Device {0} classed as {1}", SystemInfo.deviceModel, result.ToString()));
            return result;
            #elif UNITY_ANDROID
            FMODPlatform result;
            if (SystemInfo.processorCount <= 2)
            {
                result = FMODPlatform.MobileLow;
            }
            else if (SystemInfo.processorCount >= 8)
            {
                result = FMODPlatform.MobileHigh;
            }
            else
            {
                // check the clock rate on quad core systems            
                string freqinfo = "/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_max_freq";
                try
                {
                    using (global::System.IO.TextReader reader = new global::System.IO.StreamReader(freqinfo))
                    {
                        string line = reader.ReadLine();
                        int khz = Int32.Parse(line) / 1000;
                        if (khz >= 1600)
                        {
                            result = FMODPlatform.MobileHigh;
                        }
                        else
                        {
                            result = FMODPlatform.MobileLow;
                        }
                    }
                }
                catch
                {
                    result = FMODPlatform.MobileLow;
                }
            }
            
            UnityEngine.Debug.Log(String.Format("FMOD Studio: Device {0} classed as {1}", SystemInfo.deviceModel, result.ToString()));
            return result;
            #elif UNITY_WINRT_8_1
            FMODPlatform result;
            if (SystemInfo.processorCount <= 2)
            {
                result = FMODPlatform.MobileLow;
            }
            else
            {
                result = FMODPlatform.MobileHigh;
            }

            UnityEngine.Debug.Log(String.Format("FMOD Studio: Device {0} classed as {1}", SystemInfo.deviceModel, result.ToString()));
            return result;

            #elif UNITY_PS4
            return FMODPlatform.PS4;
            #elif UNITY_XBOXONE
            return FMODPlatform.XboxOne;
            #elif UNITY_PSP2
            return FMODPlatform.PSVita;
            #elif (!UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1) && UNITY_WIIU
            return FMODPlatform.WiiU;
            #elif UNITY_WSA_10_0
            return FMODPlatform.UWP;
            #elif UNITY_SWITCH
            return FMODPlatform.Switch;
            #endif
        }

        const string BankExtension = ".bank";
        internal static string GetBankPath(string bankName)
        {           
            #if UNITY_EDITOR
            // For play in editor use original asset location because streaming asset folder will contain platform specific banks
            string bankFolder = Settings.Instance.SourceBankPath;
			if (Settings.Instance.HasPlatforms)
			{
				bankFolder = global::System.IO.Path.Combine(bankFolder, Settings.Instance.GetBankPlatform(FMODPlatform.PlayInEditor));
			} 
            #elif UNITY_ANDROID
            string bankFolder = null;
            if (System.IO.Path.GetExtension(Application.dataPath) == ".apk")
            {
                bankFolder = "file:///android_asset";
            }
            else
            {
                bankFolder = String.Format("jar:file://{0}!/assets", Application.dataPath);
            }
            #elif UNITY_WINRT_8_1 || UNITY_WSA_10_0
            string bankFolder = "ms-appx:///Data/StreamingAssets";
            #else
            string bankFolder = Application.streamingAssetsPath;
            #endif

            // Special case for Switch, remove / at start if needed.
            #if UNITY_5 && UNITY_SWITCH
            if (bankFolder[0] == '/')
                bankFolder = bankFolder.Substring(1);
            #endif

            if (System.IO.Path.GetExtension(bankName) != BankExtension)
            {
                return String.Format("{0}/{1}.bank", bankFolder, bankName);
            }
            else
            {
                return String.Format("{0}/{1}", bankFolder, bankName);
            }            
        }

        internal static string GetPluginPath(string pluginName)
        {
            #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_XBOXONE || UNITY_WINRT_8_1 || UNITY_WSA_10_0
                string pluginFileName = pluginName + ".dll";
            #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            	string pluginFileName = pluginName + ".bundle";
            #elif UNITY_PS4
                string pluginFileName = pluginName + ".prx";
            #elif UNITY_ANDROID || UNITY_STANDALONE_LINUX
                string pluginFileName = "lib" + pluginName + ".so";
            #endif

            #if UNITY_EDITOR_WIN && UNITY_EDITOR_64
                string pluginFolder = Application.dataPath + "/Plugins/X86_64/";
            #elif UNITY_EDITOR_WIN
                string pluginFolder = Application.dataPath + "/Plugins/X86/";
            #elif UNITY_STANDALONE_WIN || UNITY_PS4 || UNITY_XBOXONE || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_WSA_10_0
                string pluginFolder = Application.dataPath + "/Plugins/";
            #elif UNITY_STANDALONE_LINUX
                string pluginFolder = Application.dataPath + ((IntPtr.Size == 8) ? "/Plugins/x86_64/" : "/Plugins/x86/");
            #elif UNITY_WINRT_8_1
                string pluginFolder = "";
            #elif UNITY_ANDROID
            	var dirInfo = new global::System.IO.DirectoryInfo(Application.persistentDataPath);
            	string packageName = dirInfo.Parent.Name;
                string pluginFolder = "/data/data/" + packageName + "/lib/";
            #else
                string pluginFileName = "";
                string pluginFolder = "";
            #endif

            return pluginFolder + pluginFileName;
        }

        public static void EnforceLibraryOrder()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

			AndroidJavaClass jSystem = new AndroidJavaClass("java.lang.System");
			jSystem.CallStatic("loadLibrary", FMOD.VERSION.dll);
			jSystem.CallStatic("loadLibrary", FMOD.Studio.STUDIO_VERSION.dll);
            
            #endif

            // Call a function in fmod.dll to make sure it's loaded before fmodstudio.dll
            int temp1, temp2;
            FMOD.Memory.GetStats(out temp1, out temp2);

            Guid temp3;
            FMOD.Studio.Util.ParseID("", out temp3);
        }

        #if UNITY_EDITOR
        public static FMODPlatform GetEditorFMODPlatform()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return FMODPlatform.Android;
				#if UNITY_4_6 || UNITY_4_7
                case BuildTarget.iPhone:
				#else
				case BuildTarget.iOS:
				#endif
                    return FMODPlatform.iOS;
                case BuildTarget.PS4:
                    return FMODPlatform.PS4;
                case BuildTarget.PSP2:
                    return FMODPlatform.PSVita;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return FMODPlatform.Linux;
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return FMODPlatform.Mac;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return FMODPlatform.Windows;
                case BuildTarget.XboxOne:
                    return FMODPlatform.XboxOne;
                #if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
                case BuildTarget.WiiU:
                    return FMODPlatform.WiiU;
                #endif
				#if UNITY_4_6 || UNITY_4_7
                case BuildTarget.MetroPlayer:
                #else
                case BuildTarget.WSAPlayer:
                #endif
                #if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
                    if (EditorUserBuildSettings.wsaSDK == WSASDK.UWP)
                    {
                        return FMODPlatform.UWP;
                    }
                #else
                    if (EditorUserBuildSettings.metroSDK == MetroSDK.PhoneSDK81)
                    { 
                        return FMODPlatform.WindowsPhone;
                    }
                #endif
                return FMODPlatform.None;
                #if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
			    case BuildTarget.tvOS:
					return FMODPlatform.AppleTV;
                #endif
                #if UNITY_SWITCH
                case BuildTarget.Switch:
                    return FMODPlatform.Switch;
                #endif
                default:
                    return FMODPlatform.None;
            }
        }
        #endif
    }
}
