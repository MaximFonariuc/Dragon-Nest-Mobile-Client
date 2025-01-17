using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.SDK
{
	public static class JoyYouInterfaceSimulator
	{
		public static string NotificationObjeName { get; set; }

		static JoyYouInterfaceSimulator()
		{
			NotificationObjeName = "";
		}

		public static void ShowLoginView()
		{
			SendObjMsg("LoginCallBack", "Simulating API: \"ShowLoginView()\"");
		}

		public static void ShowLoginViewWithType(int type)
		{
			SendObjMsg("LoginCallBack", "Simulating API: \"ShowLoginViewWithType(int type)\"");
		}

		public static void Logout()
		{
			SendObjMsg("LogoutCallBack", "Simulating API: \"Logout()\"");
		}

		public static void ShowCenterView()
		{
			SendObjMsg("UserCenteredClosedCallBack", "Simulating API: \"ShowCenterView()\"");
		}

		public static void Pay()
		{
			SendObjMsg("PayCallBack", "Simulating API: \"Pay()\"");
		}

		public static void VerifyingUpdatePassCallBack(string msg)
		{
			SendObjMsg("VerifyingUpdatePassCallBack", "Simulating API: \"ShowLoginView\"");
		}

		public static void onRegister()
		{
			UnityEngine.Debug.Log("onRegister");
			SendObjMsg("RegisterCallBack", "Simulating API: \"HLRegister\"");
		}

		public static void SendGameExtData(string type, string jsonData)
		{
			UnityEngine.Debug.Log("SendGameExtData");
		}

		public static bool CheckStatus(string type, string jsonData)
		{
			UnityEngine.Debug.Log("CheckStatus");

			// 默认值
			return true;
		}

		public static string GetSDKConfig(string type, string jsonData)
		{
			//UnityEngine.Debug.Log ("GetSDKConfig");

			// 默认值
			return "Simulator";
		}

		public static void QuitGame(string paramString)
		{
			UnityEngine.Debug.Log("QuitGame");
			SendObjMsg("QuitGame", "Simulating API: \"QuitGame\"");
		}

		public static void getAdvIDFA()
		{
			UnityEngine.Debug.Log("getAdvIDFA");
			SendObjMsg("NotifyIDFA", "IDFA=XXXXXXXXXXXXXXXXX");
		}

		private static void SendObjMsg(string method, string message)
		{
			if (NotificationObjeName != "")
			{
				var obj = UnityEngine.GameObject.Find(NotificationObjeName);
				if (obj != null)
				{
					obj.SendMessage(method, message);
				}
			}
		}

		public static void InitGameRecordItf(string appKey, string _params)
		{
			UnityEngine.Debug.Log("InitGameRecordItf -> appKey : " + appKey);
		}

		public static void GameRecordItf_ShowCtrlBar(bool visible)
		{
			UnityEngine.Debug.Log("GameRecordItf -> ShowCtrlBar : " + visible.ToString());
		}

		public static void GameRecordItf_PauseRecording()
		{
			// if needed
		}

		public static void GameRecordItf_ResumeRecording()
		{
			// if needed
		}

		public static void GameRecordItf_StartRecording()
		{
			// if needed
		}

		public static void GameRecordItf_StopRecording()
		{
			// if needed
		}

		public static void GameRecordItf_ShowCoinWebView()
		{
			// if needed
		}
		
		public static void GameRecordItf_ShowRecordLibraryView()
		{
			// if needed
		}

		public static void GameRecordItf_ShowPlayerClub()
		{
			// if needed
		}
	}
}
