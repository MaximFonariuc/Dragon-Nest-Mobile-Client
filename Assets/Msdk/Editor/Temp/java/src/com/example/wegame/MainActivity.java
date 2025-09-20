package com.example.wegame;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;


import com.tencent.msdk.WeGame;
import com.tencent.msdk.adapter.MsdkActivity;
import com.tencent.msdk.api.LoginRet;
import com.tencent.msdk.api.WGPlatform;
import com.tencent.msdk.consts.CallbackFlag;
import com.tencent.msdk.consts.EPlatform;
import com.tencent.msdk.consts.TokenType;
import com.tencent.msdk.tools.Logger;

/**.
 * 游戏主Activity需要继承MsdkActivity
 */

public class MainActivity extends MsdkActivity {

    private static Activity activity = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        activity = this;
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        activity = null;
    }

    // MSDKDemo Functions
    /**
     * TODO Game 拉起支付示例，游戏需要参考米大师文档自己编写支付相关代码
     * 需要先安装支付示例工程AndroidPaySample(位于MSDKzip包中的Tencent AndroidPayRelease.zip中)
     * 才能拉起支付Demo
     */
    public static void launchPayDemo() {
        Logger.d("called");
        Intent i = new Intent("com.tencent.pay.sdksample.AndroidPaySample");
        LoginRet lr = new LoginRet();
        WGPlatform.WGGetLoginRecord(lr);
        
        // 注意：这里需要判断登录态是否有效
        if (lr.flag != CallbackFlag.eFlag_Succ) {
            if (lr.platform == EPlatform.ePlatform_Weixin.val()) {
                // accesstoken过期，尝试刷新票据
                if (lr.flag == CallbackFlag.eFlag_WX_AccessTokenExpired) {
                    WGPlatform.WGRefreshWXToken();
                    return;
                } else {
                    // 微信登录态失效，引导用户重新登录授权
                    return;
                }
            } else {
                // 手Q登录态失效，引导用户重新登录授权
                return;
            } 
        }
        
        i.putExtra("userId", lr.open_id);
        i.putExtra("offerId", WeGame.getInstance().offerId);
        if (lr.platform == WeGame.WXPLATID) {
            i.putExtra("userKey", lr.getTokenByType(TokenType.eToken_WX_Access));
            i.putExtra("sessionType", "wc_actoken");
            i.putExtra("sessionId", "hy_gameid");
        } else if (lr.platform == WeGame.QQPLATID) {
            i.putExtra("userKey", lr.getTokenByType(TokenType.eToken_QQ_Pay));
            i.putExtra("sessionType", "kp_actoken");
            i.putExtra("sessionId", "openid");
        }

        i.putExtra("pf", WGPlatform.WGGetPf(""));
        i.putExtra("zoneId", "1");
        i.putExtra("pfKey", WGPlatform.WGGetPfKey());
        i.putExtra("acctType", "common");
        i.putExtra("saveValue", "60");
        i.putExtra("msdk", true);
        activity.startActivity(i);
    }
    
    /**
     * 游戏崩溃后会将堆栈信息上报到腾讯组件————灯塔中。这时制造native层崩溃测试异常上报
     */

        // Native异常测试



    
    /**
     * 游戏崩溃后会将堆栈信息上报到腾讯组件————灯塔中。这时制造空指针异常测试异常上报
     */
    public static void nullPointerExceptionTest() {
        // 空指针异常测试
        Logger.d("called");
        Handler mainHandler = new Handler(Looper.getMainLooper());
        mainHandler.post(new Runnable() {
            
            @Override
            public void run() {
                String str = null;
                str.equals("");
            }
            
        });
    }    
}
