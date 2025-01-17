using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public sealed class BugtraceAgent2  {
	#if UNITY_4_6
	private static Application.LogCallback s_oldLogCallback;
	public static Application.LogCallback GetCurrentLogCallback(){
        Type t = typeof(Application);
        // Instance Field
        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        // Static Field
        flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        FieldInfo fieldInfo = t.GetField ("s_LogCallback", flag);
        if (fieldInfo != null && fieldInfo.IsPrivate && fieldInfo.IsStatic) {
            object callback = fieldInfo.GetValue (null);
            if (callback != null){
                return (Application.LogCallback)callback;
            }
        }
        return null;
	}
	#endif

	private static bool _isInitialized = false;
	public static void EnableExceptionHandler(){
		if (_isInitialized){
			return;
		}
		RegisterExceptionHandler();
		_isInitialized = true;
	}

	public static void DisableExceptionHandler(){
		if (!_isInitialized){
			return;
		}
		UnregisterExceptionHandler();
		_isInitialized = false;
	}

	private static void RegisterExceptionHandler(){
		#if UNITY_4_6 
		AppDomain.CurrentDomain.UnhandledException += UncaughtExceptionHandler;
		s_oldLogCallback = GetCurrentLogCallback ();
		Application.RegisterLogCallback (LogCallbackHandler);
		#endif

		#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
		Application.logMessageReceived += LogCallbackHandler;
		#endif

	}

	private static void UnregisterExceptionHandler(){
		#if UNITY_4_6
		AppDomain.CurrentDomain.UnhandledException -= UncaughtExceptionHandler;
		Application.RegisterLogCallback (s_oldLogCallback);
		#endif

		#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
		Application.logMessageReceived -= LogCallbackHandler;
		#endif
	}

	private static void LogCallbackHandler(string condition, string stack, LogType type){
		if (type == LogType.Exception) {
			//FileLog.Instance.Log ("<LogCallbackHandler> reason:" + condition);
			//FileLog.Instance.Log ("<LogCallbackHandler> stack:" + stack);
			HandleException (condition, stack);
		}
		#if UNITY_4_6
		if (s_oldLogCallback != null) {
            s_oldLogCallback(condition, stack, type);
		}
		#endif
	}

	private static void UncaughtExceptionHandler(object sender, System.UnhandledExceptionEventArgs args){
		Exception e = (Exception)args.ExceptionObject;
		if (e != null){
			HandleException (e.Message, e.StackTrace);
		}

	}

	#if (UNITY_EDITOR || UNITY_STANDALONE)
	private static void HandleException(string reason, string stack){

	}

	#elif UNITY_ANDROID
    private static void HandleException(string reason, string stack){
        string cmd = "crash-reportcsharpexception|";
        cmd += "cause:" + reason;
        cmd += "stack:" + stack;
        //FileLog.Instance.Log("Android HandleException");
        try{
            AndroidJavaClass agent  = new AndroidJavaClass("com.tencent.tp.TssJavaMethod");
            if (agent != null){
                agent.CallStatic("sendCmd", cmd);
            }
        }catch{
            //FileLog.Instance.Log("Android HandleException catch");
        }
	}

	#elif (UNITY_IOS || UNITY_IPHONE)
	[DllImport("__Internal")]
	private static extern void ReportCSharpException (string reason, string stack);
	private static void HandleException(string reason, string stack){
        ReportCSharpException(reason, stack);
	}

	#endif
} // end of class BugtraceAgent2

public static class TssSdk{

	public enum ESERAILIZETAG{
		TAG_INT = 0x00,
		TAG_TYPE = 0x01,
		TAG_GAME_ID = 0x02,
		TAG_GAME_STATUS = 0x03,
		TAG_ENTRY_ID = 0x04,
		TAG_WORLD_ID = 0x05,
		TAG_STR = 0x40,
		TAG_APPID = 0x41,
		TAG_OPENID = 0x42,
		TAG_ROLEID = 0x43
	}

	public enum ESERIALIZETYPE{
		TYPE_INIT = 0x01,
		TYPE_SETUSERINFO = 0x02,
		TYPE_SETGAMESTATUS = 0x03
	}
	public enum EUINTYPE
	{
		UIN_TYPE_INT = 1, // integer format
		UIN_TYPE_STR = 2  // string format
	}
	
	public enum EAPPIDTYPE
	{
		APP_ID_TYPE_INT = 1, // integer format
		APP_ID_TYPE_STR = 2  // string format
	}
	
	public enum EENTRYID
	{
        ENTRY_ID_QQ         = 1,       // QQ
		ENTRY_ID_QZONE      = 1,	   // QQ
		ENTRY_ID_MM			= 2,	   // WeChat
        ENTRY_ID_WX         = 2,       // 微信
        ENTRT_ID_FACEBOOK   = 3,       // facebook
        ENTRY_ID_TWITTER    = 4,       // twitter
        ENTRY_ID_LINE       = 5,       // line
        ENTRY_ID_WHATSAPP   = 6,       // whatsapp
        ENTRY_ID_OTHERS     = 99,      // 其他平台
	}
	
	public enum EGAMESTATUS
	{
		GAME_STATUS_FRONTEND = 1,  // running in front-end
		GAME_STATUS_BACKEND = 2    // running in back-end
	}
	
	public enum AntiEncryptResult
	{
		ANTI_ENCRYPT_OK = 0,
		ANTI_NOT_NEED_ENCRYPT = 1,
	}
	
	public enum AntiDecryptResult
	{
		ANTI_DECRYPT_OK = 0,
		ANTI_DECRYPT_FAIL = 1,
	}
	
	// sdk anti-data info
	[StructLayout(LayoutKind.Sequential)]
	public class AntiDataInfo
	{
		//[FieldOffset(0)]
		public ushort anti_data_len;
		//[FieldOffset(2)]
		public IntPtr anti_data;
	};
	
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public class EncryptPkgInfo
	{
		[FieldOffset(0)]
		public int cmd_id_;				/* [in] game pkg cmd */
		[FieldOffset(4)]
		public IntPtr game_pkg_;		/* [in] game pkg */
		[FieldOffset(8)]
		public uint game_pkg_len_;		/* [in] the length of game data packets, maximum length less than 65,000 */
		[FieldOffset(12)]
		public IntPtr encrpty_data_;	/* [in/out] assembling encrypted game data package into anti data, memory allocated by the caller, 64k at the maximum */
		[FieldOffset(16)]
		public uint encrypt_data_len_;	/* [in/out] length of anti_data when input, actual length of anti_data when output */
	}
	
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public class DecryptPkgInfo
	{
		[FieldOffset(0)]
		public IntPtr encrypt_data_;		/* [in] anti data received by game client */
		[FieldOffset(4)]
		public uint encrypt_data_len;       /* [in] length of anti data received by game client */
		[FieldOffset(8)]
		public IntPtr game_pkg_;            /* [out] buffer used to store the decrypted game package, space allocated by the caller */
		[FieldOffset(12)]
		public uint game_pkg_len_;          /* [out] input is size of game_pkg_, output is the actual length of decrypted game package */
	}
	
	public static Boolean Is64bit()
	{
		return IntPtr.Size == 8;
	}
	
	public static Boolean Is32bit()
	{
		return IntPtr.Size == 4;
	}
    class OutputUnityBuffer{
            private byte[] data;
            private uint offset;
            private uint count;
            public OutputUnityBuffer(uint length){
                    this.data = new byte[length];
                    this.offset = 0;
                    this.count = length;
            }

            public void write(byte b){
                    if (offset < count) {
                            this.data [offset] = b;
                            this.offset++;
                    }
            }

            public byte[] toByteArray(){
                    return data;
            }

            public uint getLength(){
                    return this.offset;
            }
    }
    class SerializeUnity{
            public static void putLength(OutputUnityBuffer data, uint length){
                    data.write ((byte)(length >> 24));
                    data.write ((byte)(length >> 16));
                    data.write ((byte)(length >> 8));
                    data.write ((byte)(length));
            }

            public static void putInteger(OutputUnityBuffer data, uint value){
                    data.write ((byte)(value >> 24));
                    data.write ((byte)(value >> 16));
                    data.write ((byte)(value >> 8));
                    data.write ((byte)(value));
            }

            public static void putByteArray(OutputUnityBuffer data, byte[] value){
                    int len = value.Length;
                    for (int i = 0; i < len; i++) {
                            data.write (value [i]);
                    }
                    data.write (0);
            }

            public static void setInitInfo(uint gameId){
                    OutputUnityBuffer data = new OutputUnityBuffer (1 + 4 + 1 + 1 + 4 + 4);
                    data.write((byte)ESERAILIZETAG.TAG_TYPE);
                    putLength (data, 1);
                    data.write ((byte)ESERIALIZETYPE.TYPE_INIT);

                    data.write ((byte)ESERAILIZETAG.TAG_GAME_ID);
                    putLength (data, 4);
                    putInteger (data, gameId);

                    tss_unity_str (data.toByteArray (), data.getLength());

            }

            public static void setGameStatus(EGAMESTATUS gameStatus){
                    OutputUnityBuffer data = new OutputUnityBuffer (1 + 4 + 1 + 1 + 4 + 4);
                    data.write((byte)ESERAILIZETAG.TAG_TYPE);
                    putLength (data, 1);
                    data.write ((byte)ESERIALIZETYPE.TYPE_SETGAMESTATUS);

                    data.write ((byte)ESERAILIZETAG.TAG_GAME_STATUS);
                    putLength (data, 4);
                    putInteger (data, (uint)gameStatus);

                    tss_unity_str (data.toByteArray (), data.getLength());
            }

            public static void setUserInfoEx(EENTRYID entryId,
                    string uin,
                    string appId,
                    uint worldId,
                    string roleId){

                    byte[] valOpenId = System.Text.Encoding.ASCII.GetBytes (uin);
                    byte[] valAppId = System.Text.Encoding.ASCII.GetBytes (appId);
                    byte[] valRoleId = System.Text.Encoding.ASCII.GetBytes (roleId);
                    uint length = 0;
                    OutputUnityBuffer data = new OutputUnityBuffer (6*1 + 6*4 + 1 + 4 + 4 + (uint)valOpenId.Length + 1 + (uint)valAppId.Length + 1 + (uint)valRoleId.Length + 1);
                    data.write((byte)ESERAILIZETAG.TAG_TYPE);
                    putLength (data, 1);
                    data.write ((byte)ESERIALIZETYPE.TYPE_SETUSERINFO);

                    data.write ((byte)ESERAILIZETAG.TAG_ENTRY_ID);
                    putLength (data, 4);
                    putInteger (data, (uint)entryId);

                    data.write ((byte)ESERAILIZETAG.TAG_OPENID);
                    length = (uint)valOpenId.Length + 1;
                    //Debug.Log ("openid length:" + length);
                    putLength (data, length);
                    putByteArray (data, valOpenId);

                    data.write ((byte)ESERAILIZETAG.TAG_APPID);
                    length = (uint)valAppId.Length + 1;
                    //Debug.Log ("appid length:" + length);
                    putLength (data, length);
                    putByteArray (data, valAppId);

                    data.write ((byte)ESERAILIZETAG.TAG_WORLD_ID);
                    putLength (data, 4);
                    putInteger (data, worldId);

                    data.write ((byte)ESERAILIZETAG.TAG_ROLEID);
                    length = (uint)valRoleId.Length + 1;
                    //Debug.Log ("roleid length:" + length);
                    putLength (data, length);
                    putByteArray (data, valRoleId);

                    //Debug.Log ("data length:" + data.getLength());
                    tss_unity_str (data.toByteArray (), data.getLength());
            }
    }

    private static bool isEnable(string name)
    {
#if (UNITY_IOS || UNITY_IPHONE)
        byte[] keyName = System.Text.Encoding.ASCII.GetBytes (name);
        int ret = tss_unity_is_enable(keyName, keyName.Length);
        return (ret != 0);
#endif

#if UNITY_ANDROID
        try
        {
            AndroidJavaClass agent = new AndroidJavaClass("com.tencent.tp.TssJavaMethod");
            if (agent != null)
            {
                string cmd = "is_enabled2:" + name;
                // FileLog.Instance.Log("[tsssdk]cmd:" + cmd);
                int ret = agent.CallStatic<int>("sendCmdEx", cmd);
                return (ret != 0);
            }
        }
        catch(Exception)
        {
            // Debug.LogException(e);
            // FileLog.Instance.Log("isEnable exception catch");
        }
#endif
        return false;
    }
	
	/// <summary>
	/// Tsses the sdk init.
	/// </summary>
	/// <param name='gameId'>
	/// game id provided by sdk provider
	/// </param>
	public static void TssSdkInit(uint gameId)
	{
		SerializeUnity.setInitInfo (gameId);
		tss_enable_get_report_data();
		tss_log_str(TssSdkVersion.GetSdkVersion());
		tss_log_str(TssSdtVersion.GetSdtVersion());
        if (isEnable("bugtrace"))
        {
            // Debug.Log("Enable Exception Handler");
            BugtraceAgent2.EnableExceptionHandler();
        }
	}
	/// <summary>
	/// Tsses the sdk set game status.
	/// </summary>
	/// <param name='gameStatus'>
	/// back-end or front-end
	/// </param>
	public static void TssSdkSetGameStatus(EGAMESTATUS gameStatus)
	{
		SerializeUnity.setGameStatus(gameStatus);
	}
	
	public static void TssSdkSetUserInfo(EENTRYID entryId,
	                                     string uin,
	                                     string appId)
	{
		TssSdkSetUserInfoEx(entryId, uin, appId, 0, "0");
	}
	
	public static void TssSdkSetUserInfoEx(EENTRYID entryId,
	                                       string uin,
	                                       string appId,
	                                       uint worldId,
	                                       string roleId
	                                       )
	{
				
		if (roleId == null) {
			roleId = "0";
		}
		SerializeUnity.setUserInfoEx (entryId, uin, appId, worldId, roleId);

	}
	

	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	private static extern void tss_log_str(string sdk_version);
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	private static extern void tss_sdk_rcv_anti_data(IntPtr info);
	public static void TssSdkRcvAntiData(byte[] data, ushort length)
	{
		IntPtr pv = Marshal.AllocHGlobal (2 + IntPtr.Size);
		if (pv != IntPtr.Zero) 
		{
			Marshal.WriteInt16 (pv,0,(short)length);
			//Marshal.WriteIntPtr (pv,2,(IntPtr)data);
			IntPtr p = Marshal.AllocHGlobal(data.Length);
			if (p != IntPtr.Zero)
			{
				Marshal.Copy (data,0,p, data.Length);
				Marshal.WriteIntPtr (pv,2,p);
				tss_sdk_rcv_anti_data (pv);
				Marshal.FreeHGlobal(p);
			}
			
			Marshal.FreeHGlobal(pv);
		}
	}
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	private static extern AntiEncryptResult tss_sdk_encryptpacket(EncryptPkgInfo info);
	public static AntiEncryptResult TssSdkEncrypt(/*[in]*/int cmd_id, /*[in]*/byte[] src, /*[in]*/uint src_len,
	                                              /*[out]*/ref byte[] tar, /*[out]*/ref uint tar_len) 
	{
		AntiEncryptResult ret = AntiEncryptResult.ANTI_NOT_NEED_ENCRYPT;
		GCHandle src_handle = GCHandle.Alloc(src, GCHandleType.Pinned);
		GCHandle tar_handle = GCHandle.Alloc(tar, GCHandleType.Pinned);
		if (src_handle.IsAllocated && tar_handle.IsAllocated) 
		{
			EncryptPkgInfo info = new EncryptPkgInfo();
			info.cmd_id_ = cmd_id;
			info.game_pkg_ = src_handle.AddrOfPinnedObject();
			info.game_pkg_len_ = src_len;
			info.encrpty_data_ = tar_handle.AddrOfPinnedObject();
			info.encrypt_data_len_ = tar_len;
			ret = tss_sdk_encryptpacket(info);
			tar_len = info.encrypt_data_len_;
		}
		if (src_handle.IsAllocated) src_handle.Free();
		if (tar_handle.IsAllocated) tar_handle.Free();
		return ret;
	}
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	private static extern AntiDecryptResult tss_sdk_decryptpacket(DecryptPkgInfo info);
	public static AntiDecryptResult TssSdkDecrypt(/*[in]*/byte[] src, /*[in]*/uint src_len,
	                                              /*[out]*/ref byte[] tar, /*[out]*/ref uint tar_len) 
	{
		AntiDecryptResult ret = AntiDecryptResult.ANTI_DECRYPT_FAIL;
		GCHandle src_handle = GCHandle.Alloc(src, GCHandleType.Pinned);
		GCHandle tar_handle = GCHandle.Alloc(tar, GCHandleType.Pinned);
		if (src_handle.IsAllocated && tar_handle.IsAllocated) 
		{
			DecryptPkgInfo info = new DecryptPkgInfo();
			info.encrypt_data_ = src_handle.AddrOfPinnedObject();
			info.encrypt_data_len = src_len;
			info.game_pkg_ = tar_handle.AddrOfPinnedObject();
			info.game_pkg_len_ = tar_len;
			ret = tss_sdk_decryptpacket(info);
			tar_len = info.game_pkg_len_;
		}
		if (src_handle.IsAllocated) src_handle.Free();
		if (tar_handle.IsAllocated) tar_handle.Free();
		return ret;
	}
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	private static extern void tss_enable_get_report_data();
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	public static extern IntPtr tss_get_report_data();
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	public static extern void tss_del_report_data(IntPtr info);

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe")]
	#endif
	public static extern void tss_unity_str(byte[] data, UInt32 len);

#if (UNITY_IOS || UNITY_IPHONE)
    [DllImport("__Internal")]
    public static extern int tss_unity_is_enable(byte[] data, int len);
#endif


}

class TssSdkVersion 
{
	private const string  cs_sdk_version = "C# SDK ver: 2.6.1(2016/09/30)";
	public static string GetSdkVersion()
	{
		return cs_sdk_version;	
	}
}

