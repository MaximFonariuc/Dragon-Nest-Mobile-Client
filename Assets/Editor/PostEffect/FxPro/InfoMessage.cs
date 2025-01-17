//#define DEBUG_INFO_MESSAGE

//
// A set of helper classes used for displaying an info message (such as an available update).
//

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace FxProNS {
	[InitializeOnLoad]
	public class InfoMessageBootstrap {
		const bool CheckForMessages = true;
		
		public const string PluginName = "FxPro";
		public const string PluginVersion = "2.2";
		
		private const double IntervalBetweenChecks = 86400.0;	//Check for a new version once a day
		//private const double IntervalBetweenChecks = 15.0;	//Check for a new version once a day
		private static double _timeMessageWasShown;
	
		static InfoMessageBootstrap()
		{
			//if (CheckForMessages)
			//	EditorApplication.update += Update;
		}
		
		//private static void Update() {
		//	var deltaTime = EditorApplication.timeSinceStartup - _timeMessageWasShown;
			
		//	if (deltaTime >= IntervalBetweenChecks) {
		//		ServerGateway.ConnectToServer();
		//		_timeMessageWasShown = EditorApplication.timeSinceStartup;
		//	}
		//}
	}
	
	public static class GUIDManager {
		const string GUIDKey = "zebra_guid";
	
		private static void SetGUID() {
			var guid = System.Guid.NewGuid();
			EditorPrefs.SetString( GUIDKey, guid.ToString() );
		}
		
		public static string GetGUID() {
			if ( !EditorPrefs.HasKey(GUIDKey) )
				SetGUID();
			
			return EditorPrefs.GetString( GUIDKey );
		}
	}
	
	public static class ServerGateway {
		public static void ConnectToServer() {
			const string processorURL = "http://unityninjas.com/unity/asset_store/get_message.php";
			
			string guid = GUIDManager.GetGUID();
			
			string getString = processorURL + "?guid=" + guid + "&product_id=" + InfoMessageBootstrap.PluginName +
								"&product_version=" + InfoMessageBootstrap.PluginVersion;
			
			StartWWWRequest( getString, ProcessServerReply, null );
		}
		
		public static void ReportClick(string messageType) {
			const string processorURL = "http://unityninjas.com/unity/asset_store/report_click.php";
			
			string guid = GUIDManager.GetGUID();
			
			string getString = processorURL + "?guid=" + guid + "&product_id=" + InfoMessageBootstrap.PluginName +
								"&message_type=" + messageType;
			
			StartWWWRequest( getString, null, null );
		}
		
		private static void StartWWWRequest(string url, Action<WWW, object> callback, object callbackArgs) {
			var www = new WWW(url);
			
			ContinuationManager.Add(() => www.isDone, () =>
			                        {
				if ( !string.IsNullOrEmpty(www.error) )
					return;
				
				if (null != callback)
					callback.Invoke( www, callbackArgs );
			});
		}
		
		private static void ProcessServerReply(WWW www, object args) {
			if ( www.text.Contains("error") )
	//			return;
				throw new InvalidOperationException("Received a error response from the server:\n" + www.text);
			
			var responseData = ServerResponseData.DeserializeFromXML( www.text );
			
			if (null == responseData)
				return;
	//			throw new InvalidOperationException("Unable to deserialize XML response:\n" + www.text);
			
	//		Debug.Log("Response:\n" +
	//		          "MessageType: " + responseData.MessageType + "\n" +
	//		          "ImageUrl: " + responseData.ImageUrl + "\n" +
	//		          "TargetUrl: " + responseData.TargetUrl + "\n" +
	//		          "TimesToShow: " + responseData.TimesToShow + "\n" +
	//		          "IntervalInMinutes: " + responseData.IntervalInMinutes + "\n");
			MessageDisplayLogic.ResetIfShould( responseData );
			
			if ( !MessageDisplayLogic.CanShowMessage(responseData) )
				return;
			
			LoadTexture( responseData );
		}
		
		private static void ProcessWWWTexture(WWW www, object args) {
			var response = (ServerResponseData)args;
			
			var texture = www.texture;
			
			if (null == texture)
				return;
				
			ShowMessage( texture, response );
		}
		
		private static void ShowMessage( Texture2D texture, ServerResponseData response) {
			MessageDisplayLogic.IncreaseTimesShown( response.MessageName );
			MessageDisplayLogic.SetLastMessageShownDate( response.MessageName );
			
			InfoMessage.ShowMessage( texture, response );
		}
		
		private static void LoadTexture(ServerResponseData response) {
			if ( string.IsNullOrEmpty( response.ImageUrl ) )
				return;
	//			throw new Exception("No ImageUrl provided in response");
			
			StartWWWRequest( response.ImageUrl, ProcessWWWTexture, response );
		}
	}
	
	public static class MessageDisplayLogic {
		public static bool CanShowMessage(ServerResponseData response) {
			if ( !HasEnoughTimePassed( response ) )
				return false;
			
			if ( ExceededMaxTimesToShow( response ) )
				return false;
			
			return true;
		}
		
		public static bool ExceededMaxTimesToShow(ServerResponseData response) {
			var timesMessageWasShownKey = GetTimesMessageWasShownKey(response.MessageName);
			
			EditorPrefsHelper.InitIfUndefined( timesMessageWasShownKey, 0 );
			
			int timesMessageWasShown = GetTimesMessageWasShown( response.MessageName );
			
			return timesMessageWasShown >= response.TimesToShow;
		}
		
		public static bool HasEnoughTimePassed( ServerResponseData response ) {		
			string messageDateKey = GetMessageDateKey( response.MessageName );
			
			string currentDate = System.DateTime.Now.Ticks.ToString();
			
			bool newKey = !EditorPrefs.HasKey( messageDateKey );
			
			EditorPrefsHelper.InitIfUndefined( messageDateKey, currentDate );
			
			long lastTimeMessageShownTicks = long.Parse( EditorPrefs.GetString( messageDateKey ) );
			double minutesPassedSinceLastMessage = System.TimeSpan.FromTicks(System.DateTime.Now.Ticks - lastTimeMessageShownTicks).TotalMinutes;
			
			var timesMessageWasShown = GetTimesMessageWasShown( response.MessageName );
			
			var interval = response.IntervalInMinutes * ( Mathf.Exp(timesMessageWasShown) - 1 );
			
			#if DEBUG_INFO_MESSAGE
			Debug.Log("timesMessageWasShown: " + timesMessageWasShown + "; interval: " +
			          minutesPassedSinceLastMessage + "/" + interval);
			#endif
			
			return newKey || minutesPassedSinceLastMessage >= interval;
		}
		
		public static void ResetIfShould( ServerResponseData response ) {
			if (response.ResetToken < 0 )
				return;
			
			var resetTokenKey = GetResetTokenKeyName( response );
			
			bool shouldReset = !EditorPrefs.HasKey( resetTokenKey ) || (EditorPrefs.GetInt( resetTokenKey ) != response.ResetToken);
			if ( shouldReset ) {
				#if DEBUG_INFO_MESSAGE
				Debug.Log("Reset");
				#endif
				//Reset mesage info
				MessageDisplayLogic.SetTimesMessageWasShown( response.MessageName, 0 );
				MessageDisplayLogic.SetLastMessageShownDate( response.MessageName );
			}
			
			//Update reset token
			EditorPrefs.SetInt( resetTokenKey, response.ResetToken );
		}
		
		private static string GetResetTokenKeyName( ServerResponseData response ) {
			if (response.ResetToken < 0) return null;
		
			return InfoMessageBootstrap.PluginName + "_" + response.MessageName + "_reset_token";
		}
		
		public static string GetTimesMessageWasShownKey(string messageName) {
			return InfoMessageBootstrap.PluginName + "_" + messageName + "_message_shown_times";
		}
		
		public static string GetMessageDateKey(string messageName) {
			return InfoMessageBootstrap.PluginName + "_" + messageName + "_message_date";
		}
		
		public static int GetTimesMessageWasShown(string messageName) {
			var key = GetTimesMessageWasShownKey( messageName );
			
			return EditorPrefs.HasKey(key) ? EditorPrefs.GetInt( key ) : 0;
		}
		
		public static void IncreaseTimesShown( string messageName ) {
			EditorPrefsHelper.IncreaseValue( GetTimesMessageWasShownKey(messageName), 1 );
		}
		
		public static void SetTimesMessageWasShown(string messageName, int times) {
			var key = GetTimesMessageWasShownKey( messageName );
			
			EditorPrefs.SetInt(key, times);
		}
		
		public static void SetLastMessageShownDate(string messageName) {
			string currentDate = System.DateTime.Now.Ticks.ToString();
			EditorPrefs.SetString( GetMessageDateKey( messageName ) , currentDate);
		}
	}
	
	public class InfoMessage : EditorWindow {
		private Texture2D _texture;
		private ServerResponseData _responseData;
		
		public static void ShowMessage(Texture2D texture, ServerResponseData responseData) {
			var rect = MakeCenteredRect(texture.width + 20, texture.height + 20);
		
			var window = EditorWindow.GetWindowWithRect<InfoMessage>(rect, true, InfoMessageBootstrap.PluginName, true);
			
			window.position = rect;
			window._texture = texture;
			window._responseData = responseData;
		}
		
		public static Rect MakeCenteredRect(int width, int height) {
			var resolution = new Vector2( Screen.currentResolution.width, Screen.currentResolution.height );
			
			var rect = new Rect( (resolution.x - width)/2, (resolution.y - height)/2, width, height );
			
			return rect;
		}
		
		void OnGUI() {
			if ( GUILayout.Button(_texture) ) {
				if (null != _responseData && !string.IsNullOrEmpty( _responseData.TargetUrl ) ) {
					Application.OpenURL( _responseData.TargetUrl );
					ServerGateway.ReportClick( _responseData.MessageType );
					
					//Make sure to never show the message again if clicked on it
					MessageDisplayLogic.SetTimesMessageWasShown( _responseData.MessageName, 100000);
				}
				this.Close();
			}
		}
	}
	
	public static class EditorPrefsHelper {
		public static void IncreaseValue(string key, int increaseBy) {
			if ( !EditorPrefs.HasKey( key ) )
				return;
			
			var value = EditorPrefs.GetInt( key );
			
			EditorPrefs.SetInt(key, value + increaseBy);
		}
		
		public static void IncreaseValue(string key, float increaseBy) {
			if ( !EditorPrefs.HasKey( key ) )
				return;
			
			var value = EditorPrefs.GetFloat( key );
			
			EditorPrefs.GetFloat(key, value + increaseBy);
		}
		
		public static void InitIfUndefined(string key, string value) {
			if ( !EditorPrefs.HasKey( key ) )
				EditorPrefs.SetString( key, value );
		}
		
		public static void InitIfUndefined(string key, int value) {
			if ( !EditorPrefs.HasKey( key ) )
				EditorPrefs.SetInt( key, value );
		}
		
		public static void InitIfUndefined(string key, float value) {
			if ( !EditorPrefs.HasKey( key ) )
				EditorPrefs.SetFloat( key, value );
		}
	}
	
	[XmlRoot("ServerResponse")]
	public class ServerResponseData {
		[XmlElement("ExpInterval")]
		public bool ExpInterval;
		
		[XmlElement("ResetToken")]
		public int ResetToken = -1;
	
		[XmlElement("MessageType")]
		public string MessageType;
		
		[XmlElement("MessageName")]
		public string MessageName;
	
		[XmlElement("ImageUrl")]
		public string ImageUrl;
		
		[XmlElement("TargetUrl")]
		public string TargetUrl;
		
		[XmlElement("TimesToShow")]
		public int TimesToShow;
		
		[XmlElement("IntervalInMinutes")]
		public float IntervalInMinutes;
		
		public static ServerResponseData DeserializeFromXML(string input)
		{
			XmlSerializer deserializer = new XmlSerializer( typeof(ServerResponseData) );
			
			ServerResponseData result;
			
			using (TextReader reader = new StringReader(input))
			{
				result = (ServerResponseData)deserializer.Deserialize( reader );
			}
			
			return result;
		}
	}
	
	internal static class ContinuationManager
	{
		private class Job
		{
			public Job(Func<bool> completed, Action continueWith)
			{
				Completed = completed;
				ContinueWith = continueWith;
			}
			public Func<bool> Completed { get; private set; }
			public Action ContinueWith { get; private set; }
		}
		private static readonly List<Job> jobs = new List<Job>();
		public static void Add(Func<bool> completed, Action continueWith)
		{
			if (!jobs.Any()) EditorApplication.update += Update;
			jobs.Add(new Job(completed, continueWith));
		}
		private static void Update()
		{
			for (int i = 0; i >= 0; --i)
			{
				var jobIt = jobs[i];
				if (jobIt.Completed())
				{
					jobIt.ContinueWith();
					jobs.RemoveAt(i);
				}
			}
			if (!jobs.Any()) EditorApplication.update -= Update;
		}
	}
}
