#if !DISABLE_PLUGIN

#if UNITY_IOS  && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Threading;
using QGameUtils;

public class QGameKitiOSBridge : MonoBehaviour {

	private static QGameKitiOSBridge singletonInstance = null;
	public QGameKit.UserAccountDelegate accountDelegate;
	public QGameKit.CommentReceiveDelegate commentDelegate;
	public QGameKit.LogDelegate logDelegate;
	public QGameKit.LiveStatusChangedDelegate liveStatusDelegate;
	public QGameKit.ShareDelegate shareDelegate;
	public QGameKit.ErrorCodeListenerDelegate errorCodeDelegate;

	public static QGameKitiOSBridge Setup()
	{ 
		if(singletonInstance != null)
		{
			return singletonInstance;
		}

		GameObject sdkObject = new GameObject("QGameKitiOSBridge");
		DontDestroyOnLoad(sdkObject);
		singletonInstance = sdkObject.AddComponent<QGameKitiOSBridge>();
		
		return singletonInstance;
	}

	private void DidReceivedGetUserAccountRequest()
	{
		QGameKit.UserAccount account = accountDelegate();
		QGameKit.UpdateUserAccount (account);
	}

	private void DidReceivedComments(string data)
	{
		if (commentDelegate == null) {
			return;
		}

		List<QGameKit.LiveComment> comments = new List<QGameKit.LiveComment>();
		List<object> array = Json.Deserialize(data) as List<object>;

		foreach(var item in array) {
			IDictionary itemDict = (IDictionary)item;
			QGameKit.LiveComment comment = new QGameKit.LiveComment();
			comment.type = (QGameKit.CommentType)(Int32.Parse((string)itemDict["type"]));
			comment.nick = (string)itemDict["nick"];
			comment.content = (string)itemDict["content"];
			comment.timestamp = Int64.Parse((string)itemDict["timestamp"]);
			comments.Add(comment);
		}
		this.commentDelegate(comments);
	}

	private void DidReceivedLog(string log)
	{
		if (logDelegate == null) {
			return;
		}
		logDelegate (log);
	}

	private void DidReceivedLiveStatusChanged(string liveStatus)
	{
		if (liveStatusDelegate == null) {
			return;
		}

		QGameKit.LiveStatus status = (QGameKit.LiveStatus)int.Parse(liveStatus);
		liveStatusDelegate (status);
	}

	private void DidReceivedShareContent(string content)
	{
		if (shareDelegate == null) {
			Debug.Log ("Share Delegate Null");
			return;
		}
			
		QGameKit.ShareContent shareContent = new QGameKit.ShareContent();
		var data = Json.Deserialize(content)  as Dictionary<string,object>;
		shareContent.title = data["title"].ToString();
		shareContent.description = data["description"].ToString();
		shareContent.targetUrl = data["targetUrl"].ToString();
		shareContent.imageUrl = data["imageUrl"].ToString();
		shareDelegate(shareContent);
	}

	private void DidReceivedError(string error)
	{
		if (errorCodeDelegate == null) {
			Debug.Log ("Error Delegate Null");
			return;
		}
	
		var data = Json.Deserialize(error)  as Dictionary<string,string>;
		errorCodeDelegate (int.Parse (data["errorCode"]), data["errorMessage"]);
	}
}
#endif

#endif
