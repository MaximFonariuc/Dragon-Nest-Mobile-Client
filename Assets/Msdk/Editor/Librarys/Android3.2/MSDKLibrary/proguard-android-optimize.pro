-optimizationpasses 5
-dontusemixedcaseclassnames
-dontskipnonpubliclibraryclasses
-dontpreverify
-dontoptimize
-ignorewarning
-verbose
-optimizations !code/simplification/arithmetic,!field/*,!class/merging/*
-keep public class * extends android.app.Activity
-keep public class * extends android.app.Application
-keep public class * extends android.app.Service
-keep public class * extends android.content.BroadcastReceiver
-keep public class * extends android.content.ContentProvider
-keep public class * extends android.app.backup.BackupAgentHelper
-keep public class * extends android.preference.Preference
-keep public class com.android.vending.licensing.ILicensingService
-keep class android.**{*;}
-dontwarn android.**
-keep class android.hardware.fingerprint.FingerprintManager.**{*;}
-keep class android.support.**{*;}
-dontwarn android.support.**
-keepclasseswithmembernames class * {
    native <methods>;
}

-keepclasseswithmembernames class * {
    public <init>(android.content.Context, android.util.AttributeSet);
}

-keepclasseswithmembernames class * {
    public <init>(android.content.Context, android.util.AttributeSet, int);
}
#过滤泛型
-keepattributes Signature 
#保留行号
-keepattributes SourceFile,LineNumberTable
-keepclassmembers enum * {
    public static **[] values();
    public static ** valueOf(java.lang.String);
}

-keep class * implements android.os.Parcelable {
  public static final android.os.Parcelable$Creator *;
}

-keepattributes InnerClasses

-keep class com.tencent.msdk.api.**{*;}

#jni加密
-keep class com.tencent.msdk.a.**{*;}



-keep public class com.tencent.msdk.framework.**{*;}

#-keep class com.tencent.msdk.**{*;}
#-dontwarn com.tencent.msdk.**
-keep class com.tencent.msdk.WeGame { *;}
-keep class com.tencent.msdk.WeGame.** { *;}
#-dontwarn com.tencent.msdk.WeGame.**
-keep class com.tencent.msdk.config.** { *;}
-keep class com.tencent.msdk.NameAuthActivity.** { *;}
-keep class com.tencent.msdk.PermissionActivity.** { *;}
-keep class com.tencent.msdk.SimpleCallback.** { *;}
#c2j反射调用
-keep class com.tencent.msdk.sdkwrapper.MSDKJniHelper.MSDKMethodC2J { *;}
-keep class com.tencent.msdk.lbs.** { *;}

#新代码jni调用
-keep class com.tencent.msdk.sdkwrapper.** { *;}
-keep class com.tencent.msdk.login.LoginExtraInfo {*;}
-keep class com.tencent.msdk.login.LoginInfoManager {*;}
-keep class com.tencent.msdk.WeGameNotifyGame.** { *;}
-keep class com.tencent.msdk.tools.** { *;}
-keep class com.tencent.msdk.ad.** { *;}
-keep class com.tencent.msdk.consts.**{*;}
-keep class com.tencent.msdk.notice.**{*;}
-keep class com.tencent.msdk.realnameauth.**{*;}
-keep class com.tencent.msdk.qq.**{*;}
-keep class com.tencent.msdk.remote.api.**{*;}
-keep class com.tencent.msdk.weixin.**{*;}
#-keep class com.tencent.msdk.myapp.autoupdate.**{*;}
-keep class com.tencent.msdk.stat.**{*;}
-keep class com.tencent.msdk.emweb.**{*;}

#应用宝
-keep class com.tencent.TMSelfUpdateManager {*;}
-keep class com.tencent.msdk.myapp.**{*;}
-keep class com.tencent.mid.**{*;}
-dontwarn com.tencent.mid.**
#stat jar
-keep class com.tencent.stat.**{*;}
-dontwarn com.tencent.stat.**

#灯塔
-keep class com.tencent.beacon.**{*;}
-keep class com.tencent.qmsp.** { *; }
-keep class com.tencent**qimei.** { *;}
-dontwarn com.tencent.beacon.**
#微信sdk jar
-keep class com.tencent.mm.**{*;}
-dontwarn com.tencent.mm.**
-keep class com.tencent.wxop.**{*;}
-dontwarn com.tencent.wxop.**


-keep class com.tencent.apkupdate.**{*;}
-dontwarn com.tencent.apkupdate.**

-keep class com.tencent.tmassistantsdk.**{*;}
-dontwarn com.tencent.tmassistantsdk.**
#http apache mime jar
-keep class org.apache.http.entity.mime.**{*;}
-dontwarn org.apache.http.entity.mime.**

-keep class com.qq.jce.**{*;}
#wup jar
-keep class com.qq.taf.**{*;}
-dontwarn com.qq.**
#opensdk jar
-keep class com.tencent.connect.**{*;}
-keep class com.tencent.map.**{*;}
-keep class com.tencent.open.**{*;}
-keep class com.tencent.qqconnect.**{*;}
-keep class com.tencent.tauth.**{*;}
#灯塔jar
-keep class com.tencent.android.tpush.**{*;}
#jgfiltersdk.jar
-keep class com.jg.** {*;}

-keep class com.tencent.feedback.**{*;}

-keep class common.**{*;}
-keep class exceptionupload.**{*;}
-keep class mqq.**{*;}
-keep class qimei.**{*;}
-keep class strategy.**{*;}
-keep class userinfo.**{*;}
#mid jar
-keep class com.tencent.mid.**{*;}
#应用宝jar
-keep class com.tencent.assistant.sdk.remote.** { *;}
-keep class com.tencent.tmapkupdatesdk.** {*;}
-keep class com.tencent.tmassistant.** {*;}
-keep class com.tencent.tmassistantbase.** { *;}
-keep class com.tencent.tmassistantsdk.** {*;}
-keep class com.tencent.tmdownloader.** {*;}
-keep class com.tencent.tmselfupdatesdk.** { *;}



#Bugly interface
-keep public class com.tencent.bugly.**{*;}
-dontwarn com.tencent.bugly.**

-keep public interface com.tencent.bugly.**{*;}

#Tbs Webview
-keep class com.tencent.msdk.webview.**{*;}
-dontwarn com.tencent.msdk.webview.**
-keep class com.tencent.smtt.**{*;}
-dontwarn com.tencent.smtt.**
-keep class MTT.ThirdAppInfoNew{*;}
-dontwarn MTT.ThirdAppInfoNew
-keep class com.tencent.tbs.** {*;}
-dontwarn com.tencent.tbs.**
#WebviewX
-keep class com.tencent.msdk.webviewx.**{*;}
-dontwarn com.tencent.msdk.webviewx.**
#EmWeb
-keep class com.tencent.msdk.emweb.**{*;}
-dontwarn com.tencent.msdk.emweb.**

#-keep class dalvik.system.**{*;}
#------------------  下方是共性的排除项目         ----------------
# 方法名中含有“JNI”字符的，认定是Java Native Interface方法，自动排除
# 方法名中含有“JRI”字符的，认定是Java Reflection Interface方法，自动排除

-keepclasseswithmembers class * {
    ... *JNI*(...);
}

-keepclasseswithmembernames class * {
	... *JRI*(...);
}

-keep class **JNI* {*;}

-ignorewarning
-keepattributes *Annotation*
-keepattributes Exceptions
-keepattributes InnerClasses
-keepattributes Signature
-keepattributes SourceFile,LineNumberTable
-keep class com.hianalytics.android.**{*;}
-keep class com.huawei.updatesdk.**{*;}
-keep class com.huawei.hms.**{*;}
-keep class com.huawei.android.hms.agent.**{*;}
-keep class com.xiaomi.**{*;}
-keep public class * extends com.xiaomi.mipush.sdk.PushMessageReceiver
-dontwarn com.meizu.cloud.pushsdk.**
-keep class com.meizu.cloud.pushsdk.**{*;}
-keep public class * extends android.app.Service
-keep public class * extends android.content.BroadcastReceiver
-keep class com.tencent.android.tpush.** {*;}
-keep class com.tencent.tpns.baseapi.** {*;}
-keep class com.tencent.tpns.mqttchannel.** {*;}
-keep class com.tencent.tpns.dataacquisition.** {*;}
-dontwarn com.vivo.push.**
-keep class com.vivo.push.**{*; }
-keep class com.vivo.vms.**{*; }
-keep class com.tencent.android.vivopush.VivoPushMessageReceiver{*;}
-keep public class * extends android.app.Service
-keep class com.heytap.mcssdk.** {*;}
-keep class com.heytap.msp.push.** { *;}
-keep class com.pushsdk.** {*;}
-keep class com.meizu.cloud.pushinternal.** {*;}
