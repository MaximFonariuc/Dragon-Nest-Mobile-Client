using System;
using UnityEngine;
using Assets.SDK.JoyyouInput;
namespace Assets.SDK
{
	public interface ITencentWX
	{
		/*
		 * 微信分享链接
		 * url:分享的链接地址
		 * title:分享的标题
		 * description:分享的具体描述，只有当分享到指定朋友时会被显示，分享到朋友圈时显示不出来
		 * image:分享后显示的小图片，需要提前准备图片资源供选择，现在暂时填为空""，表示选择默认图片
		 * scene:分享的场景，{0:分享到朋友,1:分享到朋友圈}
		 */
		void WXShareLink(string url, string title, string description, string image, int scene);

		/*
		 * 微信register
		 * @param url:Weixin AppId
		 */
		void WXShareRegister(string appId);
	}

	public interface IShare
	{
		void ShareText(string content);
		void ShareTimeline(string title, string description, string imageURL);
	}

	public enum SHARE_TYPE
	{
		SHARE_WEIXIN,
		SHARE_QQ,
	}

	public interface IShareDirector
	{
		// imageURI格式:
		// 1. 本地文件 file://theImagePath
		// 2. 网络文件 http(s)://theImageURL
		void ShareTimeline(SHARE_TYPE type, string title, string description, string imageURI);
	}

	public interface IBaiduStat
	{
		// eventId需要在网站上创建，其余字符串都自定义但不要为空
		void singleEventLog(string eventId, string eventLabel);

		// 以下2个接口成对调用，标志一个事件的开始和结束，百度后台会计算事件时间
		void eventStart(string eventId, string eventLabel);
		void eventEnd(string eventId, string eventLabel);

		// 事件统计，自己计算事件时间后传入
		void eventWithDurationTime(string eventId, string eventLabel, int duration);

		// 页面统计，成对调用，pageName是自定义的
		void pageStart(string pageName);
		void pageEnd(string pageName);
	}

	public interface I3RDPlatformSDK
	{
		/*
		 * 第三方 sdk 会自动检测更新，检测完毕后会异步调用 IJoyYouCB.VerifyingUpdatePassCallBack(string msg)
		 * 当有界面关闭时，调用 IJoyYouCB.UserViewClosedCallBack(string msg)
		 */

		/*
		 * 打开登录界面
		 * 用户操作完成后，会异步回调 IJoyYouCB.LoginCallBack(string token)
		 */
		void ShowLoginView();

		/*
		 * 打开登录界面
		 * 区分平台
		 */
		void ShowLoginViewWithType(int type);

		/*
		 * 用户登出
		 * 完成后，会异步回调 IJoyYouCB.LogoutCallBack(string msg)
		 */
		void Logout();

		/*
		 * 支付
		 * price 价格，单位为分(RMB) 
		 * billNo 订单号
		 * billTitle 支付界面提示信息
		 * roleId 角色ID
		 * zoneId 分区ID
		 *
		 * 用户操作完成后，会回调 IJoyYouCB.PayCallBack(string msg)
		 */
		/*
		 * 特别说明，在 android 有米(Umipay) 平台中
		 * billTitle 的格式为 "订单描述;等级;角色名" (用半角分号隔开)
		 */
		void Pay(int price, string billNo, string billTitle, string roleId, int zoneId);

		/*
		 * 打开用户中心
		 */
		void ShowCenterView();
	}

	public interface I3RDPlatformSDKEX
	{
		// 显示浮动工具栏
		// x, y 表示屏幕当前方向相对左上角的位置，范围为[0, 1]
		// visible 为 ture 时 显示，否则隐藏
		void ShowFloatToolkit(bool visible, double x, double y);

		/*
		 * 发送游戏扩展数据
		 *
		 * 注意，android UC 平台中, 用户上传信息时的格式
		 * type := "loginGameRole"
		 * jsonData := {
		 *				"roleId":"string", 		//必填
		 *				"roleName":"string", 	//必填
		 *				"roleLevel":"string", 	//必填
		 *				"zoneId":int, 			//必填
		 *				"zoneName":"string" 	//必填
		 *				}
		 *
		 * 注意，andorid 360 平台中，用户上传信息时的格式
		 * type := "updateScore"
		 * jsonData := {
		 *               "qihooId":"string", 	//必填
		 *               "score":"string" 		//必填
		 *              }
		 *
		 * 注意，android oppo 平台中，用户上传信息时的格式
		 * type := "sometype"
		 * jsonData := "gameId=1&service=rxsn&role=mengbo&grade=guy"
		 *
		 * 注意，android 37玩 平台中，用户上传信息时的格式
		 * type := "loginGameRole"
		 * jsonData := "ServerId=1&ServerName=rexue&RoleId=x1&RoleName=n1&RoleLevel=lv12&Balance=64&PartyName=party&VipLevel=vip9"
		 * 说明：
		 *      Balance: 用户余额（RMB购买的游戏币）
		 *      PartyName: 帮派、公会等，没有则填空字符串
		 *
		 * 注意，android mumayi 平台中，用户上传信息时的格式
		 * type := "MMY_USER"
		 * {
		 * 	 "username":"XXXXX",
		 *   "Zone":"YYY",
		 *   "level":99
		 *  }
		 *
		 * 注意，android 有米(umipay) 平台中，用户上传信息时的格式
		 * type := ""
		 * jsonData := {
		 *		        "ServerId":"00101",
		 *		        "ServerName":"一区",
		 *		        "RoleId":"10001",
		 *		        "RoleName":"骚年",
		 *		        "RoleLevel":"32"
		 *              }
		 *
		 * 注意，android 7K7K 平台中，用户上传信息时的格式
		 * type := "7K7K_USER" 			// 直接传这个值就好
		 * {
		 *   "GameName":"笑傲江湖",
		 *   "RoleId":"R0010",
		 *   "RoleName":"令狐冲",
		 *   "RoleLevel":"99",
		 *   "ZoneId":"192825",
		 *   "ZoneName":"游戏一区-逍遥谷"
		 *  }
		 *
		 * 注意，韩文版 说明
		 * type := null or type := ""
		 * content := null or content := "Binding"
		 * 当 content 为 "Binding" 时，表示弹出用户绑定界面
		 *
		 * 注意，斗鱼 平台说明
		 * type := "GameStart" | "LevelUp"
		 * 当确认用户登录成功后，设置 type 为 "GameStart"，content = {"uid":"xxx", "zoneId":"xxx", "loginTime":"xxx", "roleName":"xxx", "roleLevel":"xxx"}
		 * 当角色升级时，设置 type 为 "LevelUp"，content = {"uid":"xxx", "roleName":"xxx", "roleLevel":"xxx"}
		 * 当领取鱼丸时，设置 type 为 "DrawYuWan"，content = "xxx_uid"
		 * UserViewClosedCallBack 返回 {"STYLE":1, "data":{"drawResult":1/-1003/-1004/-1005}}
		 * 当获取抽奖号码时，设置 type 为 "RedeemCode", content = {"uid":"xxx", "roleName":"xxx"}
		 * UserViewClosedCallBack 返回 {"STYLE":2, "data":{"code":"xxxx"}}
		 *
		 * 注意，草花 平台说明
		 * 1 进入游戏时必须发送数据
		 *    type := "enterGame" content := @"{\"SrvNo\":\"服务器id\", \"SrvName\":\"服务器名称\", \"RoleId\":\"角色id\", \"RoleName\":\"角色名称\"}"
		 * 2 角色升级时必须发送数据
		 *	  type := "levelUp" content := @"{\"RoleName\":\"name\", \"SrvNo\":\"1\", \"Level\":99}"
		 *
		 * 注意，靠谱 平台说明
		 *	  type := "enterGame" content := @"{\"RoleName\":\"name\", \"SrvName\":\"唐齐一中\", \"Level\":99}"
		 *
		 * 注意，使用 盛大 推送服务时，在用户进入游戏后发送数据
		 *    type := "APS_UserInfo" content := @"{\"AreaId\":\"the area id\", \"RoleName\":\"the role name\"}"
		 *
		 * 注意，安智 平台说明
		 * 在用户进入游戏后发送数据
		 * type := "userinfo" content := @"{\"zone\":\"x\",\"level\":10, \"name\":\"zhaozhao\",\"memo\":\"may be nothing\"}"
		 *
		 * 注意，泰国 版本说明
		 * 在角色创建、登录及等级改变时发送数据
		 * type := "userinfo" content := @"{\"roleId\":\"123456\", \"roleName\":\"任逸\", \"roleLevel\":100, \"rolePower\":1000, \"roleVIPLv\":9}"
		 * type := "serverId" content := "srvId" 			// 必须在登录前设置
		 *
		 * 注意，新马 版本说明
		 * type := "gameReady" 			// 游戏完成加载后，进入第一个等待用户操作的界面（通常为选服）
		 * type := "roleInfo" content := @"{\"roleId\":\"角色ID\", \"roleName\":\"角色名\", \"serverId\":\"区服id\", \"roleLevel\":100, \"roleCoin\":1000, \"roleMoney\":1000}"
		 * roleCoin--游戏过程中获得的游戏币（如银两）余额
		 * roleMoney--充值获得的游戏币（如元宝）余额
		 *
		 * 注意，oppo 平台说明
		 * type := "RoleInfo" content := @"{\"roleName\":\"孔太大要搞小点\", \"server\":\"服务器名称or服务器id\", \"roleLevel\":100}"
		 *
		 * 注意，果盘 平台说明
		 * 登录成功以后发送数据
		 * type := "RoleInfo" content := @"{\"roleName\":\"孔太大要搞小点\", \"serverId\":\"服务器id\", \"serverName\":\"服务器名称\", \"roleLevel\":100, \"roleId\":\"角色id\"}"
		 *
		 * 注意，啪啪 平台说明
		 * 创角后发送数据
		 * type := "createRole" & "enterGame"
		 * content := @{\"roleName\":\"xxxxx\"
		 *              \"district\":1 			// 区id, int类型
		 *              \"server\":1 			// 服id, int类型
		 * }
		 *
		 */
		void SendGameExtData(string type, string jsonData);

		/*
		 * 带布尔返回值的扩展功能接口
		 * 用于检测QQ、微信等应用是否安装
		 */
		bool CheckStatus(string type, string jsonData);

		/*
		 * 带字符串返回值的扩展功能接口
		 * 用于获取渠道相关的参数
		 */
		string GetSDKConfig(string type, string jsonData);

		void QuitGame(string paramString);
		// void RequestExitGame();
		// void QueryUserAgeInfo(string uid);
		
        /*
         * uid := 平台的用户ID
         * IsQuery := true表示实名查询，false表示实名注册
         */
        void RequestRealUserRegister(string uid, bool IsQuery);
	}

	public interface IHuanlePlatform : I3RDPlatformSDK, I3RDPlatformSDKEX
	{
		// 欢乐平台 原生注册 接口，调用完毕后会异步回调
		// IJoyYouCB.RegisterCallBack(string result)
		void HLRegister(string username, string password, string email);

		// 欢乐平台 原生登录 接口，调用完毕后会异步回调
		// IJoyYouCB.LoginCallBack(string token)
		void HLLogin(string username, string password);

		// 欢乐平台 原生注销 接口，调用完毕后会异步回调
		// IJoyYouCB.LogoutCallBack(string msg)
		void HLLogout();

		void HLResendAppstoreReceiptDataForRole(string roleId);
	}

	public enum ADV_SIZE
	{
		_SIZE_320x48,
		_SIZE_320x250,
		_SIZE_728X90,
		_SIZE_468X60,
		_SIZE_120X600,
		_SIZE_320x50,
	}

	public interface IAdvertisement
	{
		void CreateBanner(int x, int y, int width, int height, ADV_SIZE size, string content);
		void BannerRefresh(int second);
		void RemoveBanner();
		void getAdvIDFA();
	}

	public interface IStatisticalData
	{
		// 统计接口
		void setLogEnable(bool bEnable);

		// cpa part
		/* userid 建议30字符以内 */
		void initAppCPA(string appId, string channelId);
		void onRegister(string userId);
		void onLogin(string userId);

		/* 目前支持的货币种类有:人民币 CNY, 港币 HKD, 台币 TWD, 美元 USD, 欧元 EUR, 英镑 GBP, 日元 JPY */
		void onPay(string userId, string orderId, int amount,/*费用*/ string currency/*币种*/);

		// game part
		void initStatisticalGame(string appId, string partnerId);

		// void onGameResume(Activity page);
		// void onGamePause(Activity page);

		// 注册后调用
		// 判断是否单机游戏
		bool isStandaloneGame();
		void setStandaloneGame(bool isSG);

		// 初始化角色信息（accountId为服务器为角色分配的uuid）
		void initAccount(string accountId);

		// 设置角色账户类型
		void setAccountType(GameAccountType type);

		// 设置账户&角色名(注册名)
		void setAccountName(string name);

		// 角色等级改变时调用
		void setAccountLevel(int level);

		// 设置角色区服
		void setAccountGameServer(string gameServer);

		// 设置性别
		void setAccountGender(GameGender gender);

		// 设置年龄
		void setAccountAge(int age);

		// 玩家充值
		void accountPay(			// 若数据发送失败，重传与否待定
		      string messageId		/* 64字符以内，游戏自定。该条消息的ID，由开发者自行定义，唯一标识您发来的一条消息。如果接口对某条消息反馈错误代码，可帮助您找出发送数据的问题，并在修改后对该条消息进行重发 */
			, string status			/* 充值状态。标识这是一次充值请求，还是已经成功完成支付。玩家请求充值时填写request；确认玩家完成支付时填写success；收入数据只按success的数据统计 */
			, string accountID		/* 64字符内，玩家的账户ID，与客户端调用SDK的setAccount需传入玩家的accountId类似 */
			, string orderID		/* 200字符内, 订单ID，唯一标识一次交易。注意：1 如果您同时在客户端和服务器都有发送收入数据，请确保两边发送的orderID定义方式是一致的，否则可能造成数据多算；2 同orderID的充值成功如多次收到，只记录最早一条成功的数据，重复数据不计数 */
			, double currencyAmount	/* 充值金额 */
			, string currencyType	// 充值货币类型，请使用ISO 4217中规定的3位字母代码标记货币类型。参考：人民币CNY；美元USD；欧元EUR。如果传入了IOS标准中不存在的类型，则会自动转成CNY（人民币）类型
			, double virtualCurrencyAmount // 充值获得的虚拟币额度
			, long chargeTime		// 玩家充值发生的时间(13位的毫秒），若不想使用，传入 -1
			, string iapID			// 64字符内，玩家购买的充值包类型，若不想使用，传入 ""
			, string paymentType	// 16字符内，支付方式。如：支付宝、苹果iap、银联支付、爱贝支付聚合等，若不想使用，传入 ""
			, string gameServer		// 16字符内，玩家充值的区服
			, string gameVersion	// 充值时游戏客户端的版本号
			, int level				// 玩家充值时的等级，若不想使用，传入 -1
			, string mission		// 玩家充值时所在的关卡或任务，若不想使用，传入""
		);

		// 跟踪游戏消费点
		// 记录付费点
		void onAccountPurchase(
			String item						// 某个消费点的编号，最多32字符
			, int itemNumber				// 消费数量 
			, double priceInVirtualCurrency	// 虚拟币单价 
		);

		// 消耗物品或服务等 
		void onAccountUse(
			String item						// 参数定义同上
			, int itemNumber
		);

		// 任务统计
		void onAccountMissionBegin(String missionId);
		void onAccountMissionCompleted(String missionId);
		void onAccountMissionFailed(String missionId, String cause);

		// 跟踪获赠的虚拟币
		// 赠予虚拟币
		void onAccountCurrencyReward(double virtualCurrencyAmount, String reason);
	}

	public interface IDeviceInfomation
	{
		string GetMACAddress();
		JoyyouInput.Joystick GetJoystick();
		void ProcessJoystick(string message);
	}

	public enum GameAccountType
	{
		 ANONYMOUS,
		 REGISTERED,
		 SINA_WEIBO,
		 QQ,
		 QQ_WEIBO,
		 ND91,
		 TYPE1,TYPE2,TYPE3,TYPE4,TYPE5,TYPE6,TYPE7,TYPE8,TYPE9,TYPE10,
	};
	
	public enum GameGender
	{
		UNKNOW,
		MALE,
		FEMALE,
	}

	public enum UserWindowCode
	{
		Login,
	}

	public interface IGameRecord
	{
		/*
		 * 开始录像
		 */
		void StartRecording();

		/*
		 * 停止录像
		 */
		void StopRecording();

		/*
		 * 暂停录像
		 */
		void PauseRecording();

		/*
		 * 继续录像
		 */
		void ResumeRecording();

		/*
		 * 显示/关闭工具栏
		 */
		void ShowControlBar(bool visible);

		/*
	     * 显示自带的视频管理界面
	     */
		void ShowVideoStore();

		/*
		 * 进入玩家俱乐部
		 */
		void ShowPlayerClub();

		/*
		 * 进入福利中心
		 */
		void ShowWelfareCenter();
	}

	public interface IJoyYouCB
	{
		void RegisterCallBack(string result);
		void LoginCallBack(string token);
		void LogoutCallBack(string msg);
		void UserViewClosedCallBack(string msg);
		void PayCallBack(string msg);
		void VerifyingUpdatePassCallBack(string msg);
		void ShareContentCallBack(string msg);
		void NotifyIDFA (string msg);

        /*
         *  "is_adult"       已成年
         *  "not_adult"      未成年
         *  "not_register"   未实名注册
         *  "register_over"  注册结束
         */
        void RealUserRegisterCallBack(string msg);

		void NotifyJoystick(string msg);
	}
}
