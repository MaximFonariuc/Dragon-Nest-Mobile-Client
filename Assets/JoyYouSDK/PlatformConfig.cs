namespace Assets.SDK
{
    /* 
     * PLATFORM_NAME_JOYYOU 				// 欢乐平台		ios&android
     * PLATFORM_NAME_PP 					// PP助手		ios
     * PLATFORM_NAME_ITOOLS 				// iTools		ios
     * PLATFORM_NAME_TONGBU 				// 同步推		ios
     * PLATFORM_NAME_BAIDU91 				// 百度91		ios
     * PLATFORM_NAME_XYSDK 					// XY			ios
     * PLATFORM_NAME_I4SDK 					// 爱思			ios
     * PLATFORM_NAME_DOWNJOY 				// 当乐			ios
     * PLATFORM_NAME_KUAIYONG 				// 快用			ios
     * PLATFORM_NAME_HAIMA 					// 海马			ios
     * PLATFORM_NAME_TENCENT 				// 腾讯			ios
     * PLATFORM_NAME_UC_ANDROID 			// UC			android
     * PLATFORM_NAME_XIAOMI_ANDROID 		// 小米			android
     * PLATFORM_NAME_OPPO_ANDROID 			// OPPO			android
     * PLATFORM_NAME_BAIDU91_ANDROID 		// 百度91		andorid
     * PLATFORM_NAME_DOWNJOY_ANDROID 		// 当乐			android
     * PLATFORM_NAME_360_ANDROID 			// 360			andorid
     * PLATFORM_NAME_M4399_ANDROID 			// 4399			android
     * PLATFORM_NAME_MEIZU_ANDROID 			// 魅族			android
     * PLATFORM_NAME_WANDOUJIA_ANDROID 		// 豌豆荚		android
     * PLATFORM_NAME_MUZHIWAN_ANDROID 		// 拇指玩		android
     * PLATFORM_NAME_TENCENT_ANDROID 		// 腾讯			andorid
     * PLATFORM_NAME_HARDCORE_ANDROID 		// 硬核			android
     * PLATFORM_NAME_M37WAN_ANDROID 		// 37玩			android
     * PLATFORM_NAME_SOGOU_ANDROID 			// 搜狗			android
     * PLATFORM_NAME_ANZHI_ANDROID 			// 安智			android
     * PLATFORM_NAME_VIVO_ANDROID 			// Vivo			android
     * PLATFORM_NAME_PPS_ANDROID 			// PPS			android
     * PLATFORM_NAME_YOUKU_ANDROID 			// 优酷			android
     * PLATFORM_NAME_PPTV_ANDROID 			// PPTV			android
     * PLATFORM_NAME_SDKBUSSD_ANDROID 		// 盛大			android
     * PLATFORM_NAME_PK96_ANDROID 			// PK96			android
     * PLATFORM_NAME_UMIPAY_ANDROID 		// 有米（偶玩）	android
     * PLATFORM_NAME_DYOO_ANDROID 			// 点优			android
     * PLATFORM_NAME_AIPAI_ANDROID 			// 爱拍			android
     * PLATFORM_NAME_SY07073_ANDROID 		// 07073		android
     * PLATFORM_NAME_GIONEE_ANDROID 		// 金立			android
     * PLATFORM_NAME_KAOPU_ANDROID 			// 靠谱			android
     * PLATFORM_NAME_7XZ_ANDROID 			// 七匣子		android
     * PLATFORM_NAME_EFUN_ANDROID 			// 易幻			android
     * 
     * PLATFORM_NAME_KOREAN_ANDROID 		// onestore		android(韩文版)
     * 
     * PLATFORM_NAME_ONESTORE_ANDROID 		// onestore		android(韩文版)
     * 
     */
#if UNITY_STANDALONE
    [JoyYouSDKPlatformFilter(JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT_ANDROID)]
#elif UNITY_ANDROID
    [JoyYouSDKPlatformFilter(JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT_ANDROID)]
#elif UNITY_IOS
    [JoyYouSDKPlatformFilter(JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT)]
#endif
    public partial class JoyYouSDK {}
}
