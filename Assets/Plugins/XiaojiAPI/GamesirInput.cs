namespace Gamesir
{
    using System;
    using UnityEngine;
	using System.Collections;
	using System.Runtime.InteropServices;
	using System.Collections.Generic;
	using System.Linq;
    using Gamesir;


    public class GamesirInput
    {
		private static bool debug = false;

		private static GamesirInput _instance;	

		public static GamesirInput Instance()
		{
			if(_instance == null)
			{
				_instance = new GamesirInput();
				#if UNITY_IOS || UNITY_ANDROID
				_instance.preInitXiaoji();

				// 用于维持key的状态
				_instance.keystatus.Add (BTN_A, 0);
				_instance.keystatus.Add (BTN_B, 0);
				_instance.keystatus.Add (BTN_X, 0);
				_instance.keystatus.Add (BTN_Y, 0);
				_instance.keystatus.Add (BTN_L1, 0);
				_instance.keystatus.Add (BTN_R1, 0);
				_instance.keystatus.Add (BTN_L2, 0);
				_instance.keystatus.Add (BTN_R2, 0);
				_instance.keystatus.Add (BTN_THUMBL, 0);
				_instance.keystatus.Add (BTN_THUMBR, 0);
				_instance.keystatus.Add (DPAD_LEFT, 0);
				_instance.keystatus.Add (DPAD_RIGHT, 0);
				_instance.keystatus.Add (DPAD_DOWN, 0);
				_instance.keystatus.Add (DPAD_UP, 0);
				_instance.keystatus.Add (BTN_SELECT, 0);
				_instance.keystatus.Add (BTN_START, 0);
				#endif
			}
			return _instance;
		}


		#if UNITY_IOS
		private void preInitXiaoji()
		{
			GSGamesirSDKInit();
		}

		// 因为IOS SDK 是通过int值来区分不同按键的，所以这里做一次转换
		public Dictionary<String, int> iosKeyMap = new Dictionary<String, int>();

		[DllImport("__Internal")]
		private static extern void GamesirSDKInit();	
		public void GSGamesirSDKInit() { 
			GamesirSDKInit();
			GamePadShowSettingView();

			// map A, B, X, Y, L1, L2, R1, R2, THUMB_L, THUMB_R, SELECT, START
			// 没有THUMB_L, THUMB_R
			iosKeyMap.Add (BTN_A, 0);
			iosKeyMap.Add (BTN_B, 1);
			iosKeyMap.Add (BTN_X, 2);
			iosKeyMap.Add (BTN_Y, 3);
			iosKeyMap.Add (BTN_L1, 4);
			iosKeyMap.Add (BTN_R1, 5);
			iosKeyMap.Add (BTN_L2, 6);
			iosKeyMap.Add (BTN_R2, 7);
			iosKeyMap.Add (BTN_THUMBL, 8);
			iosKeyMap.Add (BTN_THUMBR, 9);
			iosKeyMap.Add (DPAD_LEFT, 10);
			iosKeyMap.Add (DPAD_RIGHT, 11);
			iosKeyMap.Add (DPAD_DOWN, 12);
			iosKeyMap.Add (DPAD_UP, 13);


			iosKeyMap.Add (BTN_SELECT, 14);
			iosKeyMap.Add (BTN_START, 15);
		}

		private bool HandleButton(string buttonName, Gamesir.GamepadNumber index)
		{
			if (!iosKeyMap.ContainsKey(buttonName)) {
				return false;
			}
			int keycode = iosKeyMap [buttonName];
			return GSGamePadbuttonPressed (keycode);
		}

		public float HandleAxis(String keyname, Gamesir.GamepadNumber index) {
			// AXIS_HAT_X, AXIS_HAT_Y, AXIS_X, AXIS_Y, AXIS_Z, AXIS_RZ, AXIS_RTRIGGER, AXIS_LTRIGGER
			if (keyname == AXIS_X) {
				return GSGamePadLeftThumbStickGetxAxis ();
			} else if (keyname == AXIS_Y) {
				return GSGamePadLeftThumbStickGetyAxis ();
			} else if (keyname == AXIS_Z) {
				return GamePadRightThumbStickGetxAxis ();
			} else if (keyname == AXIS_RZ) {
				return GSGamePadRightThumbStickGetyAxis();
			} else if (keyname == AXIS_RTRIGGER) {
				return GSGamePadR2Getz ();
			} else if (keyname == AXIS_LTRIGGER) {
				return GSGamePadL2Getz ();
			} else if (keyname == AXIS_HAT_X) {  // DPAD_LEFT, DPAD_RIGHT
				if (GSGamePadbuttonPressed (iosKeyMap [DPAD_LEFT])) {
					return -1;
				}
				if (GSGamePadbuttonPressed (iosKeyMap [DPAD_RIGHT])) {
					return 1;
				}
			} else if (keyname == AXIS_HAT_Y) {  // DPAD_UP, DPAD_DOWN
				if (GSGamePadbuttonPressed (iosKeyMap [DPAD_UP])) {
					return -1;
				}
				if (GSGamePadbuttonPressed (iosKeyMap [DPAD_DOWN])) {
					return 1;
				}
			}
			return 0.0f;
		}

		[DllImport("__Internal")]
		private static extern void GSGamePadShowSettingView();	
		public void GamePadShowSettingView() { 
			GSGamePadShowSettingView();
		}

		[DllImport("__Internal")]
		private static extern void GSGamePadShowSettingViewAtLocation(int location);
		public void GamePadShowSettingViewAtLocation(int location) { 
			GSGamePadShowSettingViewAtLocation(location);
		}
		[DllImport("__Internal")]
		private static extern void showConnectView();	
		public void ShowConnectView() { 
			showConnectView();
		}
		[DllImport("__Internal")]
		private static extern void hidConnectView();	
		public void HidConnectView() { 
			hidConnectView();
		}

		[DllImport("__Internal")]
		private static extern void hiddenGamesirCoverView();	
		[DllImport("__Internal")]
		private static extern void showGamesirCoverView();	

		public void setHiddenConnectIcon(bool hidden)
		{
			if (Application.isEditor) {
				return;
			}
			if (hidden) {
				hiddenGamesirCoverView ();
			} else {
				showGamesirCoverView();
			}
		}

		[DllImport("__Internal")]
		private static extern  bool IsConnected();
		/* 手柄连接状态
		* 0：断开； 3：gcm(ble)连接上
		*/
		public int GetGameSirState() {
			if (Application.isEditor) {
			return 0;
			}
			if (IsConnected()) {
				return 3;
			}
			return 0;
		}


		// 各种button按下监控
		[DllImport("__Internal")]
		private static extern bool GSGamePadbuttonPressed(int button);
		public bool GamePadbuttonPressed(int button){
			return GSGamePadbuttonPressed(button);
		}

		// 左摇杆x值获取
		[DllImport("__Internal")]
		private static extern float GSGamePadLeftThumbStickGetxAxis();
		public float GamePadLeftThumbStickGetxAxis(){
			return GSGamePadLeftThumbStickGetxAxis();
		}

		// 左摇杆y值获取
		[DllImport("__Internal")]
		private static extern float GSGamePadLeftThumbStickGetyAxis();
		public float GamePadLeftThumbStickGetyAxis(){
			return GSGamePadLeftThumbStickGetyAxis();
		}

		// 右摇杆x值获取
		[DllImport("__Internal")]
		private static extern float GSGamePadRightThumbStickGetxAxis();
		public float GamePadRightThumbStickGetxAxis(){
			return GSGamePadRightThumbStickGetxAxis();
		}

		// 右摇杆y值获取
		[DllImport("__Internal")]
		private static extern float GSGamePadRightThumbStickGetyAxis();
		public float GamePadRightThumbStickGetyAxis(){
			return GSGamePadRightThumbStickGetyAxis();
		}

		// L2 模拟量获取
		[DllImport("__Internal")]
		private static extern float GSGamePadL2Getz();
		public float GamePadL2Getz(){
			return GSGamePadL2Getz();
		}
		// R2 模拟量获取
		[DllImport("__Internal")]
		private static extern float GSGamePadR2Getz();
		public float GamePadR2Getz(){
			return GSGamePadR2Getz();
		}


		// not have effect method
		public void onStart()
		{
		}
		public void SetDebug(bool debug) {
		}

		public void OnDestory()
		{
		}
		public void OnQuit()
		{
		}

		public void SetIconLocation(IconLocation iconLocation) {
			GamePadShowSettingViewAtLocation ( (int)iconLocation);
		}

		[DllImport("__Internal")]
		private static extern float GSGamePadConnectGamePad();
		// 这里要在调用GamesirInput.Instance()之后过几秒才能调用，因为GamesirInput.Instance()会去开启蓝牙功能，这个是一个异步的过程
		// 所以如果马上调用，会提示：[CoreBluetooth] API MISUSE: <private> can only accept this command while in the powered on state
		public void AutoConnectToGCM()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>AutoConnectToGCM()");
			GSGamePadConnectGamePad ();
		}

		[DllImport("__Internal")]
		private static extern float GSGamePadDisConnectGamePad();
		public void DisConnectGCM()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>disConnectGCM()");
			GSGamePadDisConnectGamePad ();
		}

		public bool isGCMConnected() {
			if (Application.isEditor) {
				return false;
			}
			int connected = GetGameSirState ();
			if (connected == 3) {
				return true;
			}
			return false;
		}
		public int GetSPPCodeIndex() {
			return 0;
		}
		public void OpenConnectDialog()
		{
			ShowConnectView ();
		}
		public void CloseConnectDialog()
		{
			HidConnectView ();
		}
		#endif




		// 如果AndroidJavaObject在这里初始化，由于不能在这写进行Application.isEditor的判断，会导致在出错
		// 所以把它放到initialise函数中，但是这样就要求用户在使用之前要先调用一次initialise
		#if UNITY_ANDROID
        private static int mode = 2;
        private static bool[] isGamesirPad = { false, false, false, false };
        private static bool[] isGamesirPadChecked = { false, false, false, false };
        static object lockObject = new object();
        static object lockCurrent = new object();
        private AndroidJavaObject current = null;
		private AndroidJavaObject gamesirUnityInput = null;
		private AndroidJavaObject inputInterceptor = null;
		// 因为Android SDK 是通过int值来区分不同按键的，所以这里做一次转换
		public Dictionary<String, int> androidKeyMap = new Dictionary<String, int>();
		private void preInitXiaoji()
		{
			if (Application.isEditor) {
				return;
			}
			gamesirUnityInput = new AndroidJavaObject ("com.xj.gamesir.sdk.InputStatusManager");
			inputInterceptor = gamesirUnityInput.CallStatic<AndroidJavaObject>("CreateInputInterceptor", new object[] { Current() });
			mode = GetPackageNameSupportMode ();
			LogInfo ("<>initialise() GetPackageNameSupportMode = " + mode);

			// map A, B, X, Y, L1, L2, R1, R2, THUMB_L, THUMB_R, SELECT, START
			androidKeyMap.Add (BTN_A, 1<<6);
			androidKeyMap.Add (BTN_B, 1<<7);
			androidKeyMap.Add (BTN_X, 1<<8);
			androidKeyMap.Add (BTN_Y, 1<<9);
			androidKeyMap.Add (BTN_L1, 1<<10);
			androidKeyMap.Add (BTN_R1, 1<<11);
			androidKeyMap.Add (BTN_L2, 1<<12);
			androidKeyMap.Add (BTN_R2, 1<<13);
			androidKeyMap.Add (BTN_THUMBL, 1<<14);
			androidKeyMap.Add (BTN_THUMBR, 1<<15);
			androidKeyMap.Add (DPAD_LEFT, 1<<2);
			androidKeyMap.Add (DPAD_RIGHT, 1<<3);
			androidKeyMap.Add (DPAD_DOWN, 1<<1);
			androidKeyMap.Add (DPAD_UP, 1<<0);


			androidKeyMap.Add (BTN_SELECT, 1<<4);
			androidKeyMap.Add (BTN_START, 1<<5);
		}

		public void SetIconLocation(IconLocation iconLocation)
		{
			if (Application.isEditor) {
				return;
			}
			inputInterceptor.Call("setIconLocation", (int)iconLocation);
			LogInfo("<>SetIconLocation()");
		}
		public void setHiddenConnectIcon(bool hidden)
		{
			if (Application.isEditor) {
				return;
			}
			if (hidden) {
				inputInterceptor.Call ("setHiddenConnectIcon");
			} else {
				inputInterceptor.Call ("setDisplayConnectIcon");
			}
			LogInfo("<>setHiddenConnectIcon()");
		}
		public void onStart()
		{
			if (Application.isEditor) {
				return;
			}
			gamesirUnityInput.CallStatic("OnStart");
			LogInfo("<>onStart()");
		}

		public void SetDebug(bool debug) {
			GamesirInput.debug = debug;
			gamesirUnityInput.CallStatic("SetDebug", debug);
		}

		public void OnDestory()
		{
			if (Application.isEditor) {
				return;
			}
			gamesirUnityInput.CallStatic("OnDestory");
			LogInfo("<> OnDestory()");
		}
		public void OnQuit()
		{
			if (Application.isEditor) {
				return;
			}
			inputInterceptor.Call("stopFolatWindow", Current());
			DisConnectSpp();
			LogInfo("OnQuit()");
		}



		public void OpenConnectDialog()
		{
			if (Application.isEditor) {
				return;
			}
			inputInterceptor.Call("openConnectDialog");
			LogInfo("<>OpenConnectDialog()");
		}
		public void CloseConnectDialog()
		{
			if (Application.isEditor) {
				return;
			}
			inputInterceptor.Call("closeConnectDialog");
			LogInfo("<>CloseConnectDialog()");
		}
		public void AutoConnectToGCM()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>AutoConnectToGCM()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("autoConnectToBLE", Current());
		}
		public void DisConnectGCM()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>disConnectGCM()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("disConnectBLE", Current());
		}
		public bool isGCMConnected()
		{
			if (Application.isEditor) {
				return false;
			}
			int connected = GetGameSirState ();
			if (connected == 3) {
				return true;
			}
			return false;
		}
		public void AutoConnectToSPP()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>AutoConnectToSPP()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("autoConnectToSPP", Current());
		}

		public void AutoConnectToHID()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>AutoConnectToHID()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("autoConnectToHID", Current());
		}
		public void DisConnectHID()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>DisConnectHID()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("disConnectHID", Current());
		}
		public void DisConnectSpp()
		{
			if (Application.isEditor) {
				return;
			}
			LogInfo("<>DisConnectSpp()");
			new AndroidJavaClass("com.xj.gamesir.sdk.bluetooth.BluetoothInstance").CallStatic<AndroidJavaObject>("getInstance").Call("disConnectSPP", Current());
		}
		//private static bool GetPackageNameSupport()
		//{
		//    return gamesirUnityInput.CallStatic<bool>("getPackageNameSupport");
		//}

		private AndroidJavaObject Current()
		{
			if (Application.isEditor) {
				return null;
			}
			lock (lockCurrent)
			{
				if (current == null)
				{
					current = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
					if (debug)
					{
						Debug.LogError("<> new AndroidJavaClass Current ");
					}
					return current;
				}
				else
				{
					return current;
				}
			}
		}
		private int GetPackageNameSupportMode()
		{
			if (Application.isEditor) {
				return 0;
			}
			return gamesirUnityInput.CallStatic<int> ("getPackageNameSupportMode");
		}
		private bool CheckIsGamesir(int index, object context, string name)
		{
			if (Application.isEditor) {
				return false;
			}
			lock (lockObject)
			{
				if (!isGamesirPadChecked[index % 4])
				{
					isGamesirPad[index % 4] = gamesirUnityInput.CallStatic<bool>("CheckIsGamesir", context, name);
					isGamesirPadChecked[index % 4] = true;
//					LogInfo("<>CheckIsGamesir_1 index = " + (index % 4) + "  padname = " + name + " | Not_updateDataFinished && Not_Checked Invoke JNI Checked" + " isGamesir = " + isGamesirPad[index % 4] /*+ " btnName = " + btnName*/);
					return isGamesirPad[index % 4];
				}
				else
				{
//					LogInfo("<>CheckIsGamesir_2 index = " + (index % 4) + "  padname = " + name + " | Not_updateDataFinished && Has_Invoke JNI Checked " + " isGamesir = " + isGamesirPad[index % 4] /*+ " btnName = " + btnName*/);
					return isGamesirPad[index % 4];
				}
			}
		}
		/* 手柄连接状态
		 * 0：断开；1：hid连接上；2：spp连接上； 3：gcm连接上
		*/
		public int GetGameSirState() {
			if (Application.isEditor) {
				return 0;
			}
			return gamesirUnityInput.CallStatic<int>("GameSirConnected");
		}

		private bool HandleButton(string buttonName, Gamesir.GamepadNumber index)
		{
			if (Application.isEditor) {
				return false;
			}
			// android时通过AXIS_HAT_Y，AXIS_HAT_X
			if (buttonName == DPAD_UP || buttonName == DPAD_DOWN) {
				float hat_y = GetAxis (AXIS_HAT_Y);
				if (buttonName == DPAD_UP && hat_y < -0.5f) {
					return true;
				}
				if (buttonName == DPAD_DOWN && hat_y > 0.5f) {
					return true;
				}
				return false;
			}
			if (buttonName == DPAD_LEFT || buttonName == DPAD_RIGHT) {
				float hat_x = GetAxis (AXIS_HAT_X);
				if (buttonName == DPAD_LEFT && hat_x < -0.5f) {
					return true;
				}
				if (buttonName == DPAD_RIGHT && hat_x > 0.5f) {
					return true;
				}
				return false;
			}

			int keycode = androidKeyMap [buttonName];
			if (0 == mode)
			{
				if (Input.GetJoystickNames().Length > 0)
				{
					try
					{
						// LogInfo("<>HandleButton  Gamesir_BTN_" + buttonName + "_" + index);
						return Input.GetButton("Gamesir_BTN_" + buttonName + "_" + index) || gamesirUnityInput.CallStatic<bool>("GetButton", keycode, (int)index);
					}
					catch // (Exception ex)
					{
						// LogInfo("<>OnGUI GamesirPad not Connected  index = " + ((int)index - 1) + "  buttonName = " + buttonName +"   has exception "+ex);
						return false;
					}
				}
				else
				{
					//return false;
					return gamesirUnityInput.CallStatic<bool>("GetButton", keycode, (int)index);
				}
			}
			else if (1 == mode)
			{
				return false;
			}
			else if (2 == mode)
			{
				if (Input.GetJoystickNames().Length > 0)
				{
					try
					{
						// LogInfo("<>HandleButton  Gamesir_BTN_" + buttonName + "_" + index);
						if (CheckIsGamesir((int)index - 1, Current(), Input.GetJoystickNames()[(int)index - 1]))
						{
							return Input.GetButton("Gamesir_BTN_" + buttonName + "_" + index) || gamesirUnityInput.CallStatic<bool>("GetButton", keycode, (int)index);
						}
						else
						{
							// LogInfo("<>CheckIsGamesir  false");
							return false;
						}
					}
					catch // (Exception ex)
					{
						// LogInfo("<>OnGUI GamesirPad not Connected  index = " + ((int)index - 1) + "  buttonName = " + buttonName +"    "+ ex);
						return false;
					}
				}
				else
				{
					//return false;
					return gamesirUnityInput.CallStatic<bool>("GetButton", keycode, (int)index);
				}

			}
			else
			{
				return false;
			}
		}
		private float HandleAxis(string axisName, Gamesir.GamepadNumber index)
		{
			if (Application.isEditor) {
				return 0;
			}
			if (0 == mode)
			{
				if (Input.GetJoystickNames().Length > 0)
				{
					try
					{
						if (Input.GetAxis("Gamesir_" + axisName + "_" + index) != 0.0f)
						{
							return Input.GetAxis("Gamesir_" + axisName + "_" + index);
						}
						else
						{
							return gamesirUnityInput.CallStatic<float>("GetAxis",  axisName, (int)index);
						}
					}
					catch
					{
						return 0.0f;
					}
				}
				else
				{
					//return 0.0f;
					return gamesirUnityInput.CallStatic<float>("GetAxis", axisName, (int)index);
				}
			}
			else if (1 == mode)
			{
				return 0.0f;
			}
			else if (2 == mode)
			{
				if (Input.GetJoystickNames().Length > 0)
				{
					try
					{
						if (CheckIsGamesir(/*axisName,*/ (int)index - 1, Current(), Input.GetJoystickNames()[(int)index - 1]))
						{
							if (Input.GetAxis("Gamesir_" + axisName + "_" + index) != 0.0f)
							{
								return Input.GetAxis("Gamesir_" + axisName + "_" + index);
							}
							else
							{
								return gamesirUnityInput.CallStatic<float>("GetAxis", axisName, (int)index);
							}
						}
						else
						{
							return 0.0f;
						}
					}
					catch
					{
						return 0.0f;
					}
				}
				else
				{
					//return 0.0f;
					return gamesirUnityInput.CallStatic<float>("GetAxis", axisName, (int)index);
				}
			}
			else
			{
				return 0.0f;
			}
		}

		#endif

		#if UNITY_STANDALONE
		// not have effect method
		public void onStart()
		{
		}

		public void OnDestory()
		{
		}
		public void OnQuit()
		{
		}

		public void SetIconLocation(IconLocation iconLocation) {
		}
		private bool HandleButton(string buttonName, Gamesir.GamepadNumber index)
		{
			if (Input.GetJoystickNames().Length > 0) {
				try {
					LogInfo("HandleButton "+buttonName+" index:"+index);
					return Input.GetButton("Gamesir_BTN_" + buttonName + "_" + index);
				}
				catch//(Exception ex)
				{
					//LogInfo("<>OnGUI GamesirPad not Connected  index = " + ((int)index - 1) + "  buttonName = " + buttonName +"    "+ ex);
					return false;
				}
			}
			return false;
		}

		private float HandleAxis(string axisName, Gamesir.GamepadNumber index)
		{
			float value = 0.0f;
			if (Input.GetJoystickNames ().Length > 0) {
				try {
					if (Input.GetAxis ("Gamesir_" + axisName + "_" + index) != 0.0f) {
						value = Input.GetAxis ("Gamesir_" + axisName + "_" + index);
					}
				} catch {
					value = 0.0f;
				}
			}
			if (value > 0.1f || value < -0.1f) {  // 去掉一些误差
				return value;
			}
			return 0.0f;
		}
		#endif







		private void LogInfo(string logStr)
        {
            if (debug)
            {
                Debug.Log(logStr);
            }
        }
        private void LogError(string logStr)
        {
            if (debug) {
                Debug.LogError(logStr);
            }
        }
        public void EnableLog()
        {
            debug = true;
        }
        public void DisableLog()
        {
            debug = false;
        } 


		// 这里统一输入方式，让它和unity3d的输入系统使用方式相同
		// unity3d是通过 Input.GetButton("Gamesir_BTN_B_First")来取值
		public static String BTN_A = "A";
		public static String BTN_B = "B";
		public static String BTN_X = "X";
		public static String BTN_Y = "Y";
		public static String BTN_L1 = "L1";
		public static String BTN_L2 = "L2";
		public static String BTN_R1 = "R1";
		public static String BTN_R2 = "R2";
		// L2,R2的模拟量
		public static String AXIS_RTRIGGER = "TRIGGERL";
		public static String AXIS_LTRIGGER = "TRIGGERR";
		// dpad
		public static String  DPAD_UP = "DPAD_UP";
		public static String  DPAD_DOWN = "DPAD_DOWN";
		public static String  DPAD_LEFT = "DPAD_LEFT";
		public static String  DPAD_RIGHT = "DPAD_RIGHT";
		// dpad的模拟量
		public static String AXIS_HAT_X = "HAT_X";
		public static String AXIS_HAT_Y = "HAT_Y";

		// 左摇杆
		public static String AXIS_X = "L3D_X";  // in sdk named L3D_X
		public static String AXIS_Y = "L3D_Y";
		public static String BTN_THUMBL = "THUMB_L";
		// 右摇杆
		public static String AXIS_Z = "R3D_Z";  // // in sdk named R3D_Z
		public static String AXIS_RZ = "R3D_RZ";
		public static String BTN_THUMBR = "THUMB_R";

		public static String BTN_SELECT = "SELECT";
		public static String BTN_START = "START";

			

		// A, B, X, Y, L1, L2, R1, R2, THUMB_L, THUMB_R, SELECT, START
		// (DPAD_UP, DPAD_DOWN, DPAD_LEFT, DPAD_RIGHT not use because of AXIS_HAT_X and AXIS_HAT_Y can instead of that)
		public bool GetButton(String keyname) {
			return HandleButton (keyname, Gamesir.GamepadNumber.First);
		}

		// AXIS_HAT_X, AXIS_HAT_Y, AXIS_X, AXIS_Y, AXIS_Z, AXIS_RZ, AXIS_RTRIGGER, AXIS_LTRIGGER
		public float GetAxis(String keyname) {
			return HandleAxis (keyname, Gamesir.GamepadNumber.First);
		}
		#if UNITY_ANDROID
		public bool GetButton(String keyname, Gamesir.GamepadNumber index) {
			return HandleButton (keyname, index);
		}
		public float GetAxis(String keyname, Gamesir.GamepadNumber index) {
			return HandleAxis (keyname, index);
		}
		#endif




		// 临时性支持按键动作，性能可能会差一些
		private Dictionary<String, int> keystatus = new Dictionary<String, int>();
		// 每帧都要调用
		public void OnRefreshKeys() {
			List<String> list = new List<String>();
			list.AddRange(keystatus.Keys);
			foreach (String btn in list) {
				bool pressed = GetButton (btn);
				if (pressed) {
					int prestatus = keystatus[btn];
					if (prestatus >= 0) {
						keystatus[btn] = keystatus[btn] + 1;
					} else {
						keystatus[btn] = 1;
					}
				} else {
					// 要先有按下，才会弹起，所以肯定不会为0
					if (keystatus[btn] == 0) {
						continue;
					}
					int prestatus = keystatus[btn];
					if (prestatus < 0) {
						keystatus[btn] = keystatus[btn] - 1;
					} else {
						keystatus[btn] = -1;
					}
				}
			}
		}

		// 得到按键按下的动作
		public bool GetButtonDown(String button){
			return keystatus [button] == 1;
		} 

		// 得到按键弹起的动作
		public bool GetButtonUp(String button){
			return keystatus [button] == -1;
		} 
    }
}