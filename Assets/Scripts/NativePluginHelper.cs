using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class NativePluginHelper: XUtliPoolLib.INativePlugin
{
    public bool enable = false;
    // Unity editor doesn't unload dlls after 'preview'
#if UNITY_EDITOR
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void InitEngine();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void UpdateEngine();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ReadTable(short tableID, short tableType, byte[] buffer, int length);
    // Make sure that delegate instances are identical to the actual method being called
    InitEngine _InitEngine;
    UpdateEngine _UpdateEngine;
    ReadTable _ReadTable;
    IntPtr? plugin_dll = null;

    public void InitializeHooks()
    {
        if(enable)
        {
            plugin_dll = NativeMethods.LoadLibrary(@"Assets/Plugins/x64/NativeClient.dll");
            if (plugin_dll != IntPtr.Zero)
            {
                IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(plugin_dll.Value, "InitEngine");
                if (pAddressOfFunctionToCall != IntPtr.Zero)
                    _InitEngine = (InitEngine)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(InitEngine));

                pAddressOfFunctionToCall = NativeMethods.GetProcAddress(plugin_dll.Value, "UpdateEngine");
                if (pAddressOfFunctionToCall != IntPtr.Zero)
                    _UpdateEngine = (UpdateEngine)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(UpdateEngine));

                pAddressOfFunctionToCall = NativeMethods.GetProcAddress(plugin_dll.Value, "ReadTable");
                if (pAddressOfFunctionToCall != IntPtr.Zero)
                    _ReadTable = (ReadTable)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(ReadTable));
            }
        }
    }
    public void CloseHooks()
    {
        if (plugin_dll == null)
        {
            return;
        }
        bool result = NativeMethods.FreeLibrary(plugin_dll.Value);

        plugin_dll = null;
    }
    public void UnloadNativePlugin()
    {
    }
#elif UNITY_ANDROID
     //[DllImport("NativeClient")]
     //public delegate void InitEngine();
 
     //[DllImport("NativeClient")]
     //public delegate void UpdateEngine();
 
     public void InitializeHooks(){}
     public void CloseHooks(){}
#else
     //[DllImport("unityplugin.dll")]
     //public delegate void InitEngine();
 
     //[DllImport("unityplugin.dll")]
     //public delegate void UpdateEngine();
 
     public void InitializeHooks(){}
     public void CloseHooks(){}
#endif

    public NativePluginHelper()
    {
        CloseHooks();
    }

    public void Init()
    {
#if UNITY_EDITOR
        if (_InitEngine != null)
        {
            _InitEngine();
        }
#endif
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (_UpdateEngine != null)
        {
            _UpdateEngine();
        }
#endif
    }

    public void InputData(short tableID, short tableType, byte[] buffer, int length)
    {
#if UNITY_EDITOR
        if (_ReadTable != null)
        {
            _ReadTable(tableID, tableType, buffer, length);
        }
#endif
    }
}