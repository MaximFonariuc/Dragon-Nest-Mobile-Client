using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace Assets.SDK
{
	public static class ExtendFunctions
	{
		public static T GetCustomAttribute<T>(this Type obj, bool isInhert) where T : System.Attribute
		{
			object [] attrs = obj.GetCustomAttributes(typeof(T), isInhert);
			T retObj = null;
			if (attrs != null)
			{
				if (attrs.Length > 1)
				{
					throw new AmbiguousMatchException("类型" + obj.ToString() + "的属性个数匹配多余一个");
				}
				if (attrs.Length == 1)
				{
					retObj = attrs[0] as T;
				}
			}
			return retObj;
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this Type obj, bool isInhert) where T : System.Attribute
		{
			T[] objs = (T[])obj.GetCustomAttributes(typeof(T), isInhert);
			return objs;
		}
	}

    public static class SDKParams
    {
        public static string Get3rdPlatformName(Type t)
        {
            string platformName = "";

			JoyYouSDKPlatformFilterAttribute[] attrs = (JoyYouSDKPlatformFilterAttribute[])t.GetCustomAttributes(typeof(JoyYouSDKPlatformFilterAttribute), false);
 			/*
			foreach (var attr in t.GetCustomAttributes(false))
            {
                JoyYouSDKPlatformFilterAttribute jySDKAttr = attr as JoyYouSDKPlatformFilterAttribute;
                if (jySDKAttr != null)
                {
                    platformName = jySDKAttr.PlatformName;
                    break;
                }
            }
            */
			if (attrs != null && attrs.Length > 0)
			{
				platformName = attrs[0].PlatformName;
			}
            return platformName;
        }

        public static JoyYouComponentAttribute[] GetJoyYouComponentAttribute(Type t)
        {
			/*
            List<JoyYouComponentAttribute> list = new List<JoyYouComponentAttribute>();
            foreach (var attr in t.GetCustomAttributes(false))
            {
                JoyYouComponentAttribute theAttr = attr as JoyYouComponentAttribute;
                if (theAttr != null)
                {
                    list.Add(theAttr);
                }
            }
            return list.ToArray();
            */
			return (JoyYouComponentAttribute[])t.GetCustomAttributes(typeof(JoyYouComponentAttribute), false);
        }

        private static void InitAdvertisementSDK(Type t)
        {
			/*
            foreach (var attr in t.GetCustomAttributes(false))
            {
                JoyYouSDKAttribute jySDKAttr = attr as JoyYouSDKAttribute;
                if (jySDKAttr != null)
                {
                    if ((jySDKAttr as InitAdvertisementAttribute) != null)
                    {
                        jySDKAttr.InitSDK();
                    }
                }
            }
            */
			InitAdvertisementAttribute[] attrs = (InitAdvertisementAttribute[])t.GetCustomAttributes(typeof(InitAdvertisementAttribute), false);
			if(attrs != null && attrs.Length > 0)
			{
				foreach (InitAdvertisementAttribute attr in attrs)
				{
					attr.InitSDK();
				}
			}
        }

		private static bool Platform_Cast<T>(bool tag, JoyYouSDKAttribute attr) where T : class
		{
			return tag && (attr as T) != null;
		}

		private static void Init3rdPlatformSDK(Type t)
		{
			bool sdk_init_flag = false;

			string platformName = Get3rdPlatformName(t);
			// Debug.Log("Current platformName --> " + platformName);

			foreach (var attr in t.GetCustomAttributes(false))
			{
				JoyYouSDKAttribute jySDKAttr = attr as JoyYouSDKAttribute;
				if (jySDKAttr != null)
				{
					// Debug.Log("Current jySDKAttr.NAME --> " + jySDKAttr.NAME);

                    if (jySDKAttr.NAME == platformName)
                    {
                        Debug.Log("SDK initialised begin --> " + platformName);

                        jySDKAttr.InitSDK();

                        sdk_init_flag = true;

                        break;
                    }
                }
            }

            var pi = t.GetField("isInitialised", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            pi.SetValue(t, sdk_init_flag);
            if (!sdk_init_flag)
            {
                Debug.LogWarning("SDK initialised failed!");
            }
            else
            {
                Debug.Log("SDK initialised succeed --> " + platformName);
            }
        }

        private static void InitComponent(Type t)
        {
            JoyYouComponentAttribute[] components = GetJoyYouComponentAttribute(t);
            foreach (JoyYouComponentAttribute comp in components)
            {
                if (comp != null)
                {
                    if (!comp.isDelayInit)
                    {
                        comp.DoInit();
                    }
                    if ((comp as InitRecNowComponentAttribute) != null)
                    {
                        var pIGameRecInitFlag = t.GetField("isSupported_IGameRecord", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                        pIGameRecInitFlag.SetValue(t, true);
                        Debug.Log("RecNow Plugin Initialised.");

                        //var IGameRecord_doInitMethod = t.GetField("IGameRecord_DelayInit", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                        var AddEventObs = t.GetMethod("AddEventObs");
                        AddEventObs.Invoke(null, new object[] { new IGameRecord_DelayInit(comp.DoInit) });
                    }
					else if ((comp as InitJoymeComponentAttribute) != null)
					{
						var pIGameRecInitFlag = t.GetField("isSupported_IGameRecord", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                        pIGameRecInitFlag.SetValue(t, true);
                        Debug.Log("Joyme Plugin Initialised.");

                        //var IGameRecord_doInitMethod = t.GetField("IGameRecord_DelayInit", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                        var AddEventObs = t.GetMethod("AddEventObs");
                        AddEventObs.Invoke(null, new object[] { new IGameRecord_DelayInit(comp.DoInit) });
					}
                }
            }
        }

        public static void Parse(Type t)
        {
            InitAdvertisementSDK(t);

            Init3rdPlatformSDK(t);

            InitComponent(t);
        }
    }
}
