using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace Assets.SDK
{


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public abstract class JoyYouSDKAttribute : System.Attribute
    {
		//private string _current
		public abstract string NAME
		{
			get;
		}
        public int appId { get; private set; }
        public string appKey { get; private set; }
        public string notifyObjName { get; private set; }
        public bool logEnable { get; private set; }
        public int rechargeAmount { get; private set; }
        public bool isLongConnect { get; private set; }
        public bool rechargeEnable { get; private set; }
        public string closeRechargeAlertMsg { get; protected set; }
        public bool isOriLandscapeLeft { get; private set; }
        public bool isOriLandscapeRight { get; private set; }
        public bool isOriPortrait { get; private set; }
        public bool isOriPortraitUpsideDown { get; private set; }
        public virtual void InitSDK()
        {
			Debug.Log("------SDK Init Description Class------ : " + this.GetType().ToString());
            JoyYouNativeInterface.InitSDK(
                this.appId,
                this.appKey,
                this.logEnable,
                this.isLongConnect,
                this.rechargeEnable,
                this.rechargeAmount,
                this.closeRechargeAlertMsg,
                this.notifyObjName,
                this.isOriPortrait,
                this.isOriLandscapeLeft,
                this.isOriLandscapeRight,
                this.isOriPortraitUpsideDown);
        }
        public JoyYouSDKAttribute(
            int appId,
            string appKey,
            string noficationObjectName,
            bool isLongConnect,
            bool rechargeEnable,
            int rechargeAmount,
            string closeRechargeAlertMsg,
            bool isOriPortrait,
            bool isOriLandscapeLeft,
            bool isOriLandscapeRight,
            bool isOriPortraitUpsideDown,
            bool logEnable)
        {
            this.appId = appId;
            this.appKey = appKey;
            this.notifyObjName = noficationObjectName;
            this.logEnable = logEnable;
            this.rechargeAmount = rechargeAmount;
            this.isLongConnect = isLongConnect;
            this.rechargeEnable = rechargeEnable;
            this.closeRechargeAlertMsg = closeRechargeAlertMsg;
            this.isOriLandscapeLeft = isOriLandscapeLeft;
            this.isOriLandscapeRight = isOriLandscapeRight;
            this.isOriPortrait = isOriPortrait;
            this.isOriPortraitUpsideDown = isOriPortraitUpsideDown;
        }

		public bool IsPlatformMatched(string name /*, JoyYouSDKAttribute attr*/)
		{
			bool nameMatched = !string.IsNullOrEmpty(name) && name == this.NAME;
			bool typeMatched = /*attr != null ? attr.GetType() == this.GetType() : */true;
			return nameMatched && typeMatched;
		}

		public class ParamsCollector
		{
			private Dictionary<string, object> paramsDict = new Dictionary<string, object>();
			public ParamsCollector()
			{

			}

			public ParamsCollector AddItemPair(string key, object value)
			{
				this.paramsDict.Add(key, value);
				return this;
			}

			public string GetJsonData()
			{
				string json = "{{\n {0} \n}}";

				StringBuilder sb = new StringBuilder();
				int index = 0;
				string token = "\"";
				string split = ":";

				foreach (KeyValuePair<string, object> pair in paramsDict)
				{
					index++;
					sb.Append(token).Append(pair.Key).Append(token).Append(split);
					if (pair.Value.GetType() == typeof(string))
					{
						sb.Append(token).Append(pair.Value).Append(token);
					}
					else if (pair.Value.GetType() == typeof(int) 
						|| pair.Value.GetType() == typeof(long) 
						|| pair.Value.GetType() == typeof(float) 
						|| pair.Value.GetType() == typeof(double))
					{
						sb.Append(pair.Value);
					}
					if (index < paramsDict.Count)
					{
						sb.Append(",\n");
					}
				}
				json = string.Format(json, sb.ToString());
				//Debug.Log(json.ToString());
				return json;
			}
		}
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InitBaiduStatSDKParamAttribute : System.Attribute
    {
        public bool isUse;
        public bool enableExceptionLog;
        public string channelId;
        public int logStrategy;
        public int logSendInterval;
        public bool logSendWifyOnly;
        public int sessionResumeInterval;
        public string appKey;
        public InitBaiduStatSDKParamAttribute(bool isUse,
                                       bool enableExceptionLog,
                                       string channelId,
                                       int logStrategy,
                                       int logSendInterval,
                                       bool logSendWifyOnly,
                                       int sessionResumeInterval,
                                       string appKey)
        {
            this.isUse = isUse;
            this.enableExceptionLog = enableExceptionLog;
            this.channelId = channelId;
            this.logStrategy = logStrategy;
            this.logSendInterval = logSendInterval;
            this.logSendWifyOnly = logSendWifyOnly;
            this.sessionResumeInterval = sessionResumeInterval;
            this.appKey = appKey;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InitGoogleACTSDKParamAttribute : System.Attribute
    {
        public bool is_use;
        public string my_id;
        public string my_label;
        public string my_value;
        public InitGoogleACTSDKParamAttribute(bool is_use, string my_id, string my_label, string my_value)
        {
            this.is_use = is_use;
            this.my_id = my_id;
            this.my_label = my_label;
            this.my_value = my_value;
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class JoyYouComponentAttribute : System.Attribute
    {
        public bool isDelayInit { get; protected set; }
		public bool isStaticLoad {get; protected set;}

        public virtual void DoInit()
        {

        }

		public JoyYouComponentAttribute()
		{
			this.isStaticLoad = true;
		}

    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed partial class JoyYouSDKPlatformFilterAttribute : System.Attribute
    {
        //public const string PLATFORM_NONE = "__NONE_FOR_TEST__";
        public const string PLATFORM_NAME_NONE = "__NONE__";

        public string PlatformName { get; private set; }
        public Type PlatformSettingsAttributeType { get; private set; }
        public JoyYouSDKPlatformFilterAttribute(string platformName)
        {
            PlatformName = platformName;
        }
    }
   
}
