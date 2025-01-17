#if !DISABLE_PLUGIN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class QGameKit {

	public enum CaptureType {
		AudioCapture = 1 << 0,       //采集麦克风声音
		AudioApolloVoice = 1 << 1,   //Apollo Voice组件提供声音
		AudioCustom = 1 << 2,        //自定义声音数据
		VideoCapture = 1 << 3,       //SDK录制视频数据
		VideoCustom = 1 << 4,        //自定义视频数据
	}

	public enum LoginPlatform {
		Guest,
		QQ,
		WeChat,
	}
	
	public enum LiveStatus {
		Unknown,		// 未知状态（在获取状态过程中发生了错误）
		Uninitialized,	// 尚未初始化
		Prepared,		// 已准备好
		LiveStarting,	// 直播开启中
		LiveStarted,	// 直播已开始
		LivePaused,		// 直播已暂停
		LiveResume,     // 暂停恢复中
		LiveStopping,	// 直播结束中
		LiveStopped,	// 直播已结束
		Error			// 直播过程出错
	}

	public enum CommentType {
		Normal,			// 普通消息
		System,			// 系统消息
		Anchor,			// 主播消息
		RoomManager,	// 房管消息
		LiveManager,	// 直播管理员消息
		SuperManager,	// 超管消息
		Edit,			// 小编消息
		Gift,			// 礼物消息
		Welcome,		// 用户进入消息
		Vip,			// VIP 用户消息
		LoginVisitor	// 登录用户消息
	}

	public enum Environment {
		Release,	//正式环境
		Debug		//测试环境
	}

    public enum WebViewStatus
    {
        Open,
        Closed,
        SmallWindow,
        FullScreen
    }

    // 用户登录信息
	public class UserAccount {
		public LoginPlatform platform;
		public string appId; // AppID：手Q或微信分享的AppID，必填
		public string id;	// openId：手Q或微信登录的Open ID，必填
		public string token;	// accessToken：必填

		public string phoneNum;	// 可选: Phone number
		public long expires;	// 可选: expires timestamp(ms)
	}

	// 直播评论信息
	public class LiveComment {
		public CommentType type;	// 评论类型
		public string nick;	// 用户昵称
		public string content;	// 评论内容
		public long timestamp;	// 评论的产生时间
	}

	public class ShareContent {
		public string fopenId;	// 可选：若指定则直接发起后端分享，否则拉起手Q/微信分享
		public string title;	// 分享标题
		public string description;	// 分享描述
		public string targetUrl;	// 分享链接
		public string imageUrl;		// 分享消息缩略图 URL
	}


	public delegate UserAccount UserAccountDelegate();	// 用户信息委托：SDK 需要用户登录态时会回调委托，游戏应返回有效的用户信息
	public delegate void CommentReceiveDelegate(List<LiveComment> comments);	// 直播评论委托：直播过程中SDK会定时调用委托并将评论传入
	public delegate void LogDelegate(string log);	// SDK日志落入游戏日志记录体系 Delegate，可选择使用
	public delegate void LiveStatusChangedDelegate(LiveStatus newState);	// 直播状态变化委托，SDK通过它将直播状态变化通知游戏
	public delegate void ShareDelegate(ShareContent shareContent);	// 分享委托，SDK通过委托调用游戏分享能力
    public delegate void ErrorCodeListenerDelegate(int errorCode, string errorMessage); // 直播过程中的错误监听委托
    public delegate void WebViewStatusChangedDelegate(WebViewStatus status); //直播大厅在游戏中时，状态回调

    public static LiveStatus liveStatus = LiveStatus.Uninitialized;

	#if !UNITY_EDITOR
	#if UNITY_ANDROID
	private static QGameKitAndroidBridge	QGameKitObj;	// Android Native 桥接对象
	#elif UNITY_IPHONE
	private static QGameKitiOSBridge    QGameKitObj;    // iOS Native 桥接对象iOS 的桥接对象
	#endif
	#endif
	// 初始化SDK，状态流转：Uninitialized -> Prepared，直播过程中出现任何错误会导致状态流转到 Error
	// 参数 gameId 为企鹅电竞为游戏分配的唯一标识，accountDelegate 为用户信息回调，SDK会调用该委托获取游戏的用户登录态信息

	public static bool Setup(string gameId, string wnsAppId, CaptureType captureType,  UserAccountDelegate accountDelegate, Environment env) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj = QGameKitAndroidBridge.Setup(gameId, wnsAppId, captureType, accountDelegate, env);
		if (null == QGameKitObj)
		{
			Debug.LogError("QGameKitObj init failed!");
			return false;
		}
		#elif UNITY_IPHONE && !UNITY_EDITOR
		QGameKitObj =  QGameKitiOSBridge.Setup ();
		QGameKitObj.accountDelegate = accountDelegate;
		_QGameSetup (gameId, wnsAppId, (int)captureType, (int)env);
		#endif
		
		UserAccount account = accountDelegate();
		UpdateUserAccount(account);

		liveStatus = LiveStatus.Prepared;
		return true;
	}

	// 反初始化，状态流转：Setup -> Uninitialized
	public static void TearDown() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.TearDown();
		#endif
		liveStatus = LiveStatus.Uninitialized;
	}

	// 使用之前已配置的信息重新 Setup，状态流转：Error/LiveStopped -> Setup
	public static bool Reset() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		return QGameKitObj.Reset();
		#endif

		#if UNITY_IOS && !UNITY_EDITOR
		return _QGameReset();
		#endif
		liveStatus = LiveStatus.Prepared;
		return false;
	}

	// 游戏每帧刷新时调用通知 SDK 采集画面
	public static void FrameUpdated() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.CaptureFrame();
		#endif
	}

	// 进入直播大厅
	public static void EnterLiveHall() {
		#if UNITY_IPHONE && !UNITY_EDITOR
		_QGameEnterLiveHall ();
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.EnterLiveHall ();
		#endif
	}

	public static void DisabledLiveHall() {
		#if UNITY_IPHONE && !UNITY_EDITOR
		_QGameDisabledLiveHall ();
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		#endif
	}

    // 进入直播大厅
	public static void EnterLiveHallInGame() {
		#if UNITY_IPHONE && !UNITY_EDITOR
		_QGameEnterLiveHall ();
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.EnterLiveHallInGame ();
		#endif
	}

	// 开始直播，状态流转：Setup -> LiveStarted
	public static bool StartLiveBroadcast(string title, string description) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (QGameKitObj.StartLiveBroadcast(title, description))
		{
			return true;
		} else {
			Debug.LogError("StartLiveBroadcast failed!");
			return false;
		}
		#endif

		#if UNITY_IPHONE && !UNITY_EDITOR
		return _QGameStartLiveBroadcast (title, description);
		#endif

		return false;
	}

	// 结束直播，状态流转：LiveStarted -> LiveStopped
	public static bool StopLiveBroadcast() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (QGameKitObj.StopLiveBroadcast())
		{
			return true;
		} else {
			Debug.LogError("StopLiveBroadcast failed!");
			return false;
		}
		#endif

		#if UNITY_IPHONE && !UNITY_EDITOR
		return _QGameStopLiveBroadcast ();
		#endif

		return false;
	}

	// 暂定直播，状态流转：LiveStarted -> LivePaused
	public static void PauseLiveBroadcast() {
		Debug.LogError("PauseLiveBroadcast unsupported yet!");
	}

	// 恢复直播，状态流转：LivePaused -> LiveStarted
	public static void ResumeLiveBroadcast() {
		Debug.LogError("ResumeLiveBroadcast unsupported yet!");
	}	

	//分享直播
	public static void ShareLiveBroadcast() {
		#if UNITY_ANDROID && !UNITY_EDITOR
			QGameKitObj.ShareLiveBroadcast();
		#elif UNITY_IOS && !UNITY_EDITOR
			_QGameShareLiveBroadcast();
		#endif
	}

	// 获取当前直播状态
	public static LiveStatus GetLiveBroadcastStatus() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		return QGameKitObj.GetLiveBroadcastStatus();
		#endif

		#if UNITY_IOS && !UNITY_EDITOR
		return (LiveStatus)_QGameGetLiveBroadcastStatus ();
		#endif 
		return LiveStatus.LiveStopped;
	}

	// 获取当前直播状态
	public static int GetErrorCode() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		return QGameKitObj.GetErrorCode();
		#elif UNITY_IOS && !UNITY_EDITOR
		return _QGameGetErrorCode();
		#endif
		return 0;
	}	

	// 主动更新用户信息（游戏里用户注销重新登录后主动调用）
	public static void UpdateUserAccount(UserAccount account) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.UpdateUserAccount(account);
		#elif UNITY_IOS && !UNITY_EDITOR
		_QGameUpdateUserAccount(account.appId, account.id, account.token, (int)account.platform);
		#endif
	}
	
	// 切换帐号时通知调用
	public static void UpdateUserAccount() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		QGameKitObj.UpdateUserAccount();
		#endif
	}

	// 往当前自己的直播发送评论
	public static void SendComment(LiveComment comment) {
		Debug.LogError("SendComment unsupported yet!");
	}

	// 设置直播评论 Delegate
	public static void SetCommentReceiveDelegate(CommentReceiveDelegate commentDelegate) {
         #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
                Debug.LogError("QGameKitObj is null!");
                return ;
            }
            QGameKitObj.SetCommentReceiveDelegete(commentDelegate);
        # elif UNITY_IPHONE
		QGameKitObj.commentDelegate = commentDelegate;
        #endif
        #endif
	}

	// 设置日志委托，让SDK的日志有机会跟随游戏的日志落地
	public static void SetLogDelegate(LogDelegate logDelegate) {
        #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
                Debug.LogError("QGameKitObj is null!");
                return ;
            }
            QGameKitObj.SetLogDelegate(logDelegate);
        # elif UNITY_IPHONE
		QGameKitObj.logDelegate = logDelegate;
        #endif
        #endif
        
    }

	// 设置分享委托，让 SDK 能够使用游戏的分享能力
	public static void SetShareDelegate(ShareDelegate shareDelegate) {
        #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
                Debug.LogError("QGameKitObj is null!");
                return ;
            }
            QGameKitObj.SetShareDelegate(shareDelegate);
        #elif UNITY_IPHONE
		QGameKitObj.shareDelegate = shareDelegate;
		_QGameSetShareDelegateEnabled(shareDelegate != null ? true : false);
        #endif
        #endif
	}

	// 设置弹幕面板是否启用，默认不启用
	public static void SetDanmakuEnabled(bool enabled) {
		#if !UNITY_EDITOR
		#if UNITY_ANDROID
			if (QGameKitObj == null)
            {
                Debug.LogError("QGameKitObj is null!");
                return ;
            }
            QGameKitObj.SetDanmakuEnabled(enabled);
		#elif UNITY_IPHONE
		_QGameSetDanmakuEnabled(enabled);
		#endif
		#endif
	}
	
	//Android弹幕面板会遮挡分享 这里添加显示隐藏的接口
	public static void ShowDanmaku() {
		#if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return;
            }
            QGameKitObj.ShowDanmaku();
        #endif
	}
	
	public static void HideDanmaku() {
		#if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return;
            }
            QGameKitObj.HideDanmaku();
        #endif
	}

	// 游戏通知 SDK 发起分享的结果，result = 0 为分享成功否则为失败
	public static void SetShareResult(ShareContent content, int result) {
	}

	// 设置直播状态 Delegate，如果游戏需要实时获得直播状态请注册 delegate
	public static void SetLiveStatusDelegate(LiveStatusChangedDelegate liveStatusDelegate) {
         #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
                Debug.LogError("QGameKitObj is null!");
                return ;
            }
            QGameKitObj.SetLiveStatusChangedDelegate(liveStatusDelegate);
        # elif UNITY_IPHONE
		QGameKitObj.liveStatusDelegate = liveStatusDelegate;
        #endif
        #endif
	}

    public static void SetErrorCodeDelegate(ErrorCodeListenerDelegate errorCodeDelegate) {
        #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.SetErrorCodeDelegate(errorCodeDelegate);
        #elif UNITY_IPHONE
		QGameKitObj.errorCodeDelegate = errorCodeDelegate;
        #endif
        #endif
    }

    public static void SetWebViewStatusChangedDelegate(WebViewStatusChangedDelegate webviewDelegate)
    {
        #if !UNITY_EDITOR
        #if UNITY_ANDROID
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.SetWebViewStatusChangedDelegate(webviewDelegate);
        #elif UNITY_IPHONE
        #endif
        #endif
    }
	public static bool IsLiveBroadcastSupported()
    {
       #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return false;
            }
            return QGameKitObj.IsLiveBroadcastSupported();
        #endif
        return true;
    }

    public static string GetVersionName()
    {
         #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return null;
            }
            return QGameKitObj.GetVersionName();
        #endif
        return null;
    }

    public static bool ShowCamera()
    {
         #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return false;
            }
           return QGameKitObj.ShowCamera();
        #endif
        return false;
    }

    public static void HideCamera()
    {
         #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.HideCamera();
        #endif
        
    }

    public static void DoOnResume()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.DoOnResume();
        #endif
    }

    public static void DoOnPause()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.DoOnPause();
        #endif
    }

    public static void DoOnDestroy()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return ;
            }
            QGameKitObj.DoOnDestroy();
        #endif
    }

    public static bool DoOnBackPressed()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return false;
            }
            return QGameKitObj.DoOnBackPressed();
        #endif
        return false;
    }

    public static bool IsSupportCamera()
    {
         #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return false;
            }
            return QGameKitObj.IsSupportCamera();
        #endif
        return false;
    }

    public static bool IsSupportLiveHall()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (QGameKitObj == null)
            {
               Debug.LogError("QGameKitObj is null!");
               return false;
            }
            return QGameKitObj.IsSupportLiveHall();
        #endif
        return true;
    }

#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern void _QGameSetup(string gameId, string wnsAppId, int captureType, int env);

	[DllImport ("__Internal")]
	private static extern void _QGameTearDown();

	[DllImport ("__Internal")]
	private static extern bool _QGameReset();

	[DllImport ("__Internal")]
	private static extern void _QGameEnterLiveHall();

	[DllImport ("__Internal")]
	private static extern bool _QGameStartLiveBroadcast(string title, string description);

	[DllImport ("__Internal")]
	private static extern bool _QGameStopLiveBroadcast();

	[DllImport ("__Internal")]
	private static extern void _QGamePauseLiveBroadcast();

	[DllImport ("__Internal")]
	private static extern void _QGameResumeLiveBroadcast();

	[DllImport ("__Internal")]
	private static extern void  _QGameUpdateUserAccount(string appId, string openId, string accessToken, int loginType);

	[DllImport ("__Internal")]
	private static extern int  _QGameGetErrorCode();

	[DllImport ("__Internal")]
	private static extern int  _QGameGetLiveBroadcastStatus();

	[DllImport ("__Internal")]
	private static extern void  _QGameShareLiveBroadcast();

	[DllImport ("__Internal")]
	private static extern void _QGameDisabledLiveHall();

	[DllImport ("__Internal")]
	private static extern void _QGameSetShareDelegateEnabled(bool enabled);

	[DllImport ("__Internal")]
	private static extern void _QGameSetDanmakuEnabled(bool enabled);
#endif
}


#endif
