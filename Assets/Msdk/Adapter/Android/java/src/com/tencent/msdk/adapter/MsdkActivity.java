package com.tencent.msdk.adapter;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;

import com.tencent.msdk.api.MsdkBaseInfo;
import com.tencent.msdk.api.WGPlatform;
import com.tencent.msdk.api.WGQZonePermissions;
import com.tencent.msdk.consts.EPlatform;
import com.tencent.msdk.tools.Logger;

import com.unity3d.player.UnityPlayerActivity;

/**.
 * 适配Unity3d的MsdkActivity，游戏主Activity需要继承此类
 */

public class MsdkActivity extends UnityPlayerActivity {

    static {
		//加载MSDK so
        System.loadLibrary("MSDKSystem");
        // 加载Unity cpp适配层
        System.loadLibrary("MsdkAdapter");
    }

    private boolean isFirstLogin = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // 初始化MSDK
        /***********************************************************
         *  接入必须要看， baseInfo值因游戏而异，填写请注意以下说明:
         *  	baseInfo值游戏填写错误将导致 QQ、微信的分享，登录失败 ，切记 ！！！
         * 		只接单一平台的游戏请勿随意填写其余平台的信息，否则会导致公告获取失败
         *      offerId 为必填，一般为手QAppId
         ***********************************************************/
        MsdkBaseInfo baseInfo = new MsdkBaseInfo();
        baseInfo.qqAppId = "100703379";
        baseInfo.wxAppId = "wxcde873f99466f74a";
        baseInfo.msdkKey = "5d1467a4d2866771c3b289965db335f4";
        //订阅型测试用offerId
        baseInfo.offerId = "100703379";
        // 自2.7.1a开始游戏可在初始化msdk时动态设置版本号，灯塔和bugly的版本号由msdk统一设置
        // 1、版本号组成 = versionName + versionCode
        // 2、游戏如果不赋值给appVersionName（或者填为""）和appVersionCode(或者填为-1)，
        // msdk默认读取AndroidManifest.xml中android:versionCode="51"及android:versionName="2.7.1"
        // 3、游戏如果在此传入了appVersionName（非空）和appVersionCode（正整数）如下，则灯塔和bugly上获取的版本号为2.7.1.271
        // baseInfo.appVersionName = "2.8.1";
        // baseInfo.appVersionCode = 281;


        // 注意：传入Initialized的activity即this，在游戏运行期间不能被销毁，否则会产生Crash
        WGPlatform.Initialized(this, baseInfo);
        // 设置拉起QQ时候需要用户授权的项
        WGPlatform.WGSetPermission(WGQZonePermissions.eOPEN_ALL);
        /* 使用Cpp回调接口 */

        // 处理游戏被拉起的情况
        // launchActivity的onCreat()和onNewIntent()中必须调用
        // WGPlatform.handleCallback()。否则会造成微信登录无回调
        if (WGPlatform.wakeUpFromHall(this.getIntent())) {
            // 拉起平台为大厅
            Logger.d("LoginPlatform is Hall");
            Logger.d(this.getIntent());
        } else {
            // 拉起平台不是大厅
            Logger.d("LoginPlatform is not Hall");
            Logger.d(this.getIntent());
            WGPlatform.handleCallback(this.getIntent());
        }
        isFirstLogin = true;
    }

    // 游戏需要集成此方法并调用WGPlatform.onRestart()
    @Override
    protected void onRestart() {
        super.onRestart();
        WGPlatform.onRestart();
    }

    // 游戏需要集成此方法并调用WGPlatform.onResume()
    @Override
    protected void onResume() {
        super.onResume();

        //解决onResume卡顿问题
        new Handler().post(new Runnable() {
            public void run() {
                WGPlatform.onResume();
            }
        });

        // 模拟游戏自动登录，这里需要游戏添加加载动画
        // WGLogin是一个异步接口, 传入ePlatform_None则调用本地票据验证票据是否有效
        // 如果从未登录过，则会立即在onLoginNotify中返回flag为eFlag_Local_Invalid，此时应该拉起授权界面
        // 建议在此时机调用WGLogin,它应该在handlecallback之后进行调用。
        if (isFirstLogin) {
            isFirstLogin = false;
            WGPlatform.WGLogin(EPlatform.ePlatform_None);
        }
    }

    // 游戏需要集成此方法并调用WGPlatform.onPause()
    @Override
    protected void onPause() {
        super.onPause();
        WGPlatform.onPause();
    }

    // 游戏需要集成此方法并调用WGPlatform.onStop()
    @Override
    protected void onStop() {
        super.onStop();
        WGPlatform.onStop();
    }

    // 游戏需要集成此方法并调用WGPlatform.onDestory()
    @Override
    protected void onDestroy() {
        super.onDestroy();
        WGPlatform.onDestory(this);
    }

    // 在onActivityResult中需要调用WGPlatform.onActivityResult
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        WGPlatform.onActivityResult(requestCode, resultCode, data);
    }

    // 在onNewIntent中需要调用handleCallback将平台带来的数据交给MSDK处理
    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);

        // 处理游戏被拉起的情况
        // launchActivity的onCreat()和onNewIntent()中必须调用
        // WGPlatform.handleCallback()。否则会造成微信登录无回调
        if (WGPlatform.wakeUpFromHall(intent)) {
            Logger.d("LoginPlatform is Hall");
            Logger.d(intent);
        } else {
            Logger.d("LoginPlatform is not Hall");
            Logger.d(intent);
            WGPlatform.handleCallback(intent);
        }
    }

}
