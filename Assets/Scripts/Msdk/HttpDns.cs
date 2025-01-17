using UnityEngine;
using System;
using System.Runtime.InteropServices;
using XUtliPoolLib;

namespace com.tencent.httpdns
{
    public class HttpDns
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    	private static AndroidJavaObject m_dnsJo;
		private static AndroidJavaClass sGSDKPlatformClass;
        private static bool inited = false;

		// 初始化HttpDns
		public static void Init()
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if (jc == null)
			{
				return;
			}
			
			AndroidJavaObject joactivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			if (joactivity == null)
			{
				return;
			}

			AndroidJavaObject context = joactivity.Call<AndroidJavaObject>("getApplicationContext");

			AndroidJavaObject joDnsClass = new AndroidJavaObject("com.tencent.msdk.dns.MSDKDnsResolver");
			XDebug.singleton.AddLog("WGGetHostByName ========== " + joDnsClass);

			if (joDnsClass == null)
			{
				return;
			}

			m_dnsJo = joDnsClass.CallStatic<AndroidJavaObject>("getInstance");
			XDebug.singleton.AddLog("WGGetHostByName ========== " + m_dnsJo);

			if (m_dnsJo == null)
			{
				return;
			}

			m_dnsJo.Call("init", context);

            inited = true;
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
		private static extern string WGGetHostByName(string url);

		[DllImport("__Internal")]
		private static extern void WGGetHostByNameAsync(string url);
#endif

        // 解析域名
		public static string GetHostByName(string url)
		{
			string originalUrl = url;
			if (originalUrl == null || originalUrl.Equals(""))
			{
				return null;
			}
			XDebug.singleton.AddLog("originalUrl: " + originalUrl);

			bool isUrl = true;
			if (!originalUrl.Contains("://"))
			{
				isUrl = false;

				// 临时协议头，返回解析结果时注意去除
				originalUrl = "http://" + originalUrl;
			}

			Uri uri = new Uri(originalUrl);
			string domain = uri.Host;
			XDebug.singleton.AddLog("originalDomain: " + domain);

			

			
			string convertedUrl = originalUrl;

#if UNITY_EDITOR
            // Nothing to do
#endif

#if UNITY_IOS && !UNITY_EDITOR && HTTP_DNS
            string ips = string.Empty;
            string convertedDomain = domain;
			ips = WGGetHostByName(domain);
			XDebug.singleton.AddLog("convertedDomainArray: " + ips);

			if (ips != null && !ips.Equals(""))
			{
				// 注意，务必去除尾部的换行符
				ips = ips.TrimEnd((char[])"\n\r".ToCharArray());

				string[] ipArray = ips.Split(new char[] {';'});
				if (ipArray != null && ipArray.Length > 1)
				{
					Debug.Log("ipv4: " + ipArray[0] + " && " + "ipv6: " + ipArray[1]);

					if (!ipArray[1].Equals("0"))
					{
						// ipv6地址需加方框号[]进行处理
						convertedDomain = "[" + ipArray[1] + "]";
					}
					else
					{
						convertedDomain = ipArray[0];
					}
				}
			}
			XDebug.singleton.AddLog("convertedDomain: " + convertedDomain);

			convertedUrl = originalUrl.Replace(domain, convertedDomain);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR && HTTP_DNS
            if (!inited) Init();
            if (!inited) return url;
            string ips = string.Empty;
            string convertedDomain = domain;
            ips = m_dnsJo.Call<string>("getAddrByName", domain);
			XDebug.singleton.AddLog("convertedDomainArray: " + ips);

			if (ips != null && !ips.Equals(""))
			{
				ips = ips.TrimEnd((char[])"\n\r".ToCharArray());

				string[] ipArray = ips.Split(new char[] {';'});
				convertedDomain = ipArray[0];
			}
			XDebug.singleton.AddLog("convertedDomain: " + convertedDomain);

			convertedUrl = originalUrl.Replace(domain, convertedDomain);
#endif

            if (!isUrl)
			{
				// 去除临时添加的协议头
				convertedUrl = convertedUrl.Replace("http://", "");
			}
			XDebug.singleton.AddLog("convertedUrl: " + convertedUrl);

			return convertedUrl;
        }
	}
}
