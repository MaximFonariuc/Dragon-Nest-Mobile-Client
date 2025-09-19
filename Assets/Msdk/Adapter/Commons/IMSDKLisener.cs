using UnityEngine;
using System.Collections;
using Msdk;

namespace Msdk
{
    public interface IMSDKLisener
    {
        // ****** platformOberser ******
        void OnLoginNotify(string jsonRet);             // 登录回调
        void OnChannelPermissionAuthNotify(string jsonRet); //视频号授权回调
        void OnShareNotify(string jsonRet);         // 分享回调
        void OnWakeupNotify(string jsonRet);            // 第三方唤醒游戏的回调
        void OnRelationNotify(string jsonRet);          // 查询关系健的回调
        void OnLocationNotify(string jsonRet);          // 查询附近的人的回调
        void OnLocationGotNotify(string jsonRet);       // 查询自己位置信息的回调
        void OnFeedbackNotify(string jsonString);		// 反馈上报的回调
        string OnCrashExtMessageNotify();               // Crash时上报额外信息---字符串
        byte[] OnCrashExtDataNotify();                  // Crash时上报额外信息---字节数据

        // ****** 广告回调 ******
        void OnADNotify(string jsonRet);                // 广告展示回调


        // ****** 加绑群回调 ******
        void OnQueryGroupInfoNotify(string jsonRet);    // 查询群信息的回调
        void OnJoinWXGroupNotify(string jsonRet);       // 加入微信群信息的回调
        void OnCreateWXGroupNotify(string jsonRet);		// 创建微信群信息的回调
        //void OnJoinQQGroupNotify(string jsonRet);       // 绑定QQ群信息的回调
        void OnQueryWXGroupStatusNotify(string jsonRet);// 查询微信群状态信息回调
        // #if UNITY_ANDROID // 加绑群接口双端通用，应该不区分 Android/ios


        //void OnBindGroupNotify(string jsonRet);			// 绑定QQ群的回调
        void OnUnbindGroupNotify(string jsonRet);       // 解绑QQ群的回调
                                                        //void OnQueryGroupKeyNotify(string jsonRet);	// 查询QQ群Key的回调

        // ****** 加绑群v2回调 ******
        void OnCreateGroupV2Notify(string jsonRet);

        void OnJoinGroupV2Notify(string jsonRet);

        void OnQueryGroupInfoV2Notify(string jsonRet);

        void OnUnbindGroupV2Notify(string jsonRet);

        void OnGetGroupCodeV2Notify(string jsonRet);

        void OnQueryBindGuildV2Notify(string jsonRet);

        void OnBindExistGroupV2Notify(string jsonRet);

        void OnGetGroupListV2Notify(string jsonRet);

        void OnRemindGuildLeaderV2Notify(string jsonRet);

        #if UNITY_ANDROID
        void OnAddWXCardNotify(string jsonRet);         // 微信插入卡卷的回调

        void OnADBackPressedNotify(string jsonRet);     // 广告界面按返回键时的回调
	    // ****** 应用宝回调 ******
	    /**
	     * 检查应用宝后台是否有更新包的回调, 如果有更新包，则返回新包大小、增量包大小, 游戏结合此接口返回结果和自身业务场景确定是否弹窗提示用户更新
	     * @param newApkSize apk文件大小(全量包)
	     * @param newFeature 新特性说明
	     * @param patchSize 省流量升级包大小
	     * @param status TMSelfUpdateUpdateInfo(WGPublicDefine.h中有定义), 游戏根据此值来确定是否查询成功
	     * @param updateDownloadUrl 更新包的下载链接
	     * @param updateMethod  游戏根据此值确定有更新包的有无和类型
	     */
	    void OnCheckNeedUpdateInfo(string jsonString);

	    /**
	     * 此回调分为两种情况, 拉起应用宝更新和在游戏内更新
	     * 	 1. 拉起应用宝更新时, 此回调表示应用在应用宝下载页面的的下载进度
	     * 	 2. 游戏内更新时, 此回调表示下载游戏包的进度
 	     * @param receiveDataLen 已经接收的数据长度
 	     * @param totalDataLen 全部需要接收的数据长度（如果无法获取目标文件的总长度，此参数返回 －1）
	     */
	    void OnDownloadAppProgressChanged(string jsonString);

	    /**
	     * @param state 下载状态 TMAssistantDownloadTaskState(WGPublicDefine.cs中有定义)
	     * @param errorCode TMAssistantDownloadErrorCode(WGPublicDefine.cs中有定义)
	     * @param errorMsg 错误信息
	     */
	    void OnDownloadAppStateChanged(string jsonString);

	    /**
	     * 拉起应用宝省流量更新(WGStartSaveUpdate(true))，当没有安装应用宝时，会先下载应用宝, 此为下载应用宝包的进度回调
	     * (可选, 游戏可以自行确定是否需要显示下载应用宝的进度, 应用宝下载完成以后会自动拉起系统安装界面)
	     * @param url 当前任务的url
	     * @param receiveDataLen 已经接收的数据长度
	     * @param totalDataLen 全部需要接收的数据长度（如果无法获取目标文件的总长度，此参数返回 －1）
	     */
	    void OnDownloadYYBProgressChanged(string jsonString);

	    /**
	     * 拉起应用宝省流量更新(WGStartSaveUpdate(true))，当没有安装应用宝时，会先下载应用宝, 此为下载应用宝包的状态太变化回调
	     * (可选, 游戏可以自行确定是否需要显示下载应用宝的状态, 应用宝下载完成以后会自动拉起系统安装界面)
	     * @param url 指定任务的url
	     * @param state 下载状态: 取自 TMAssistantDownloadTaskState.DownloadSDKTaskState_*
	     * @param errorCode 错误码
	     * @param errorMsg 错误描述，有可能为null
	     */
	    void OnDownloadYYBStateChanged(string jsonString);

        #endif
        /**
         * js调用的回调
         * @param params 网页js接口传递过来的参数
         */
        void OnJsCallback(string jsParams);

        /**
         * 浏览器打开界面的进度
         * @param url 打开的url
         * @param progress 打开url的进度，范围[0,1]
         */
        void OnWebProgressChanged(string jsonString);

        /**
         * 浏览器关闭的回调
         */
        void OnWebClose(string jsonRet);

        // 根据IP获取用户国籍信息
        void OnLocationGotCountryFromIPNotify(string jsonRet);

        //根据IP获取用户国家及省市信息
        void OnLocationGetIPInfoNotify(string jsonRet);

        #if UNITY_IPHONE || UNITY_IOS
        
        /**
        * tpns点击消息回调，对应xgPushDidReceiveNotificationResponse:withCompletionHandler:
        * @param userInfo，系统response中携带的userInfo
        */
        void OnReceiveNotificationResponseNotify(string userInfo);

        #endif


    //#endif
    }

}