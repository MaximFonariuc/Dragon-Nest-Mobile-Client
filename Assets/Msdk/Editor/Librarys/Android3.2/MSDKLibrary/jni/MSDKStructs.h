//
//  MSDKStructs.h
//  MSDKFoundation
//
//  Created by Jason on 14/11/6.
//  Copyright (c) 2014年 Tencent. All rights reserved.
//

#ifndef MSDKFoundation_MSDKStructs_h
#define MSDKFoundation_MSDKStructs_h

#include <string>
#include <vector>
#include <pthread.h>
#include "MSDKEnums.h"

#ifdef ANDROID
#include <jni.h>
#endif

#ifdef ANDROID
#define MSDKEXPORT __attribute__((visibility("default")))
#elif __APPLE__
#define MSDKEXPORT
#endif

typedef struct
{
    std::string key;
    std::string value;
} KVPair;

#ifdef ANDROID
typedef struct
{
    int platform;                 //平台类型
    int flag;                     //操作结果
    std::string desc;             //结果描述（保留）
    std::string open_id;          // qq传递的openid
    std::string wx_card_list;     // card信息
    std::vector<KVPair> extInfo;  //游戏－平台携带的自定义参数手Q专用
} CardRet;

#endif

typedef struct
{
    int type;
    std::string value;
    long long expiration;
} TokenRet;

typedef struct sLoginRet
{
    int flag;          //返回标记，标识成功和失败类型
    std::string desc;  //返回描述
    int platform;      //当前登录的平台
    std::string open_id;
    std::vector<TokenRet> token;
    std::string user_id;  //用户ID，先保留，等待和微信协商
    std::string pf;
    std::string pf_key;
    std::string prajna_ext;
    std::string extra_json;

    sLoginRet() : flag(-1), platform(0) {}
} LoginRet;

typedef struct sCheckULRet
{
    int step;          //检查步骤
    bool success;  // 检测是否成功
    std::string errorInfo; //错误信息
    std::string suggestion; // 建议开发者处理方案
    sCheckULRet() : step(0), success(false), errorInfo(""), suggestion("") {}
} CheckULRet;

typedef void (*CallbackFun)(LoginRet lr);

typedef struct wakeupRet_
{
    int flag;                     //错误码
    int platform;                 //被什么平台唤起
    std::string media_tag_name;   // wx回传得meidaTagName
    std::string open_id;          // qq传递的openid
    std::string desc;             //描述
    std::string lang;             //语言     目前只有微信5.1以上用，手Q不用
    std::string country;          //国家     目前只有微信5.1以上用，手Q不用
    std::string messageExt;       //游戏分享传入自定义字符串，平台拉起游戏不做任何处理返回
                                  //目前只有微信5.1以上用，手Q不用
    std::vector<KVPair> extInfo;  //游戏－平台携带的自定义参数手Q专用

    wakeupRet_() : flag(-1), platform(0){};
} WakeupRet;

typedef struct ShareRet_
{
    int platform;         //平台类型
    int flag;             //操作结果
    std::string desc;     //结果描述（保留）
    std::string extInfo;  //游戏分享是传入的自定义字符串，用来标示分

    ShareRet_() : platform(0), flag(-1){};
} ShareRet;

typedef struct
{
    std::string viewTag;  // Button点击的tag
    _eADType scene;       //暂停位还是退出位
} ADRet;

typedef struct
{
    std::string name;  //地点名称
    std::string addr;  //具体地址
    int distance;      //离此次定位地点的距离
} AddressInfo;

typedef struct
{
    std::string nickName;       //昵称
    std::string openId;         //帐号唯一标示
    std::string gender;         //性别
    std::string pictureSmall;   //小头像
    std::string pictureMiddle;  //中头像
    std::string pictureLarge;   // datouxiang
    std::string provice;        //省份(老版本属性，为了不让外部app改代码，没有放在AddressInfo)
    std::string city;           //城市(老版本属性，为了不让外部app改代码，没有放在AddressInfo)
    std::string unionID;        //用户unionID
    bool isFriend;              //是否好友
    int distance;               //离此次定位地点的距离
    std::string lang;           //语言
    std::string country;        //国家
    std::string gpsCity;        //根据GPS信息获取到的城市
    std::string apple_email;          // Apple 登录提供的 email
    std::string apple_namePrefix;     // Apple 登录 full name 的一部分
    std::string apple_givenName;      // Apple 登录 full name 的一部分
    std::string apple_middleName;     // Apple 登录 full name 的一部分
    std::string apple_familyName;     // Apple 登录 full name 的一部分
    std::string apple_nameSuffix;     // Apple 登录 full name 的一部分
    std::string apple_nickname;       // Apple 登录 full name 的一部分
    
    std::string friendRemark;       //仅针对于wx，权限正常且微信正常返回好友备注时，该字段才存在
} PersonInfo;

typedef struct RelationRet_
{
    int flag;                         //查询结果flag，0为成功
    std::string desc;                 // 描述
    std::vector<PersonInfo> persons;  //保存好友或个人信息
    std::string extInfo;              //游戏查询是传入的自定义字段，用来标示一次查询
	eRelationRetType type; //区分回调来源 0为个人信息回调 1为同玩好友回调 2为unionID
    RelationRet_():type(eRet_QueryMyInfo){}
} RelationRet;

typedef struct
{
    int flag;                       // 查询结果flag，0为成功
    std::string desc;               // 描述
    double longitude;               // 经度
    double latitude;                // 纬度
} LocationRet;


typedef struct
{
    int ret;                        // 查询结果flag，0为成功
    std::string msg;                // 描述
    std::string country;            // 国家
    bool isQueryByRequestHeader;    // 标志是否请求头查询
} GetCountryFromIPRet;

typedef struct
{
    int ret;                        //查询结果flag, 0为成功
    int country_no;                 //国家编号
    int province_no;                //省份编号
    int city_no;                    //城市编号
    std::string msg;                //描述
    std::string country;            //国家
    std::string province;           //省份
    std::string city;               //城市
    bool isQueryByRequestHeader;    //标志是否请求头查询
} GetIPInfoRet;

typedef struct WXGroupInfo_
{
    std::string openIdList;   //群成员openId,以","分隔
    std::string memberNum;    //群成员数
    std::string chatRoomURL;  //创建（加入）群聊URL
    int status;               // 0表示没有创建或者加群，1表示已经创建群或者加群
    WXGroupInfo_() : status(-1){};
} WXGroupInfo;

typedef struct
{
    std::string groupName;    //群名称
    std::string fingerMemo;   //群的相关简介
    std::string memberNum;    //群成员数
    std::string maxNum;       //该群可容纳的最多成员数
    std::string ownerOpenid;  //群主openid
    std::string unionid;      //与该QQ群绑定的公会ID
    std::string zoneid;       //大区ID
    std::string
        adminOpenids;  //管理员openid。如果管理员有多个的话，用“,”隔开，例如0000000000000000000000002329FBEF,0000000000000000000000002329FAFF
    //群openID
    std::string groupOpenid;  //和游戏公会ID绑定的QQ群的groupOpenid
    //加群用的群key
    std::string groupKey;  //需要添加的QQ群对应的key
    std::string relation;  //用户与群的关系,1群主，2管理员，3普通成员，4非成员
} QQGroupInfo;

typedef struct QQGroup_
{
    std::string groupId;  //群号
    std::string groupName; // 群名称
    QQGroup_():groupId(""),groupName(""){};
}QQGroup;

typedef struct QQGroupInfoV2_
{
    int relation;
    std::string guildId;
    std::string guildName;
    std::vector<QQGroup> qqGroups;
    QQGroupInfoV2_():relation(-1),guildId(""),guildName(""){};
}QQGroupInfoV2;



typedef struct
{
    int flag;          // 0成功
    int errorCode;     //平台返回参数，当flag非0时需关注
    std::string desc;  //错误信息
    int platform;      //平台
    QQGroupInfoV2 mQQGroupInfoV2;
#ifdef __APPLE__
    WXGroupInfo wxGroupInfo;  //微信群信息
    QQGroupInfo qqGroupInfo;  // QQ群信息
#endif

#ifdef ANDROID
    WXGroupInfo mWXGroupInfo;  //微信群信息
    QQGroupInfo mQQGroupInfo;  // QQ群信息
#endif
} GroupRet;

typedef struct {
    int flag;                       //0成功
  
    std::string msgData; 
}WebviewRet;

typedef struct
{
    int flag;          // 0成功
    int errorCode;     //平台返回参数，当flag非0时需关注
    std::string desc;  //错误信息
    int platform;      //平台
} RealNameAuthRet;


typedef struct
{
#ifdef __APPLE__
    unsigned char  *ios_imageData;
    int ios_imageDataLen;
#endif
    
#ifdef ANDROID
    unsigned char *android_imagePath;
#endif
} ImageParams;

typedef struct
{
#ifdef __APPLE__
    unsigned char  *ios_videoData;
    int ios_videoDataLen;
    unsigned char  *ios_videoLocalIdentifier;
#endif
    
#ifdef ANDROID
    unsigned char *android_videoPath;
#endif
} VideoParams;




typedef struct
{
    eMSDK_SCREENDIR screenDir;  // 0：横竖屏   1：竖屏 2：横屏
    std::string picPath;        //图片本地路径
    //    ePicType type;         //图片类型
    std::string hashValue;  //图片hash值
} PicInfo;

typedef struct
{
    std::string msg_id;             //公告id
    std::string open_id;            //用户open_id
    std::string msg_url;            //公告跳转链接
    eMSG_NOTICETYPE msg_type;       //公告类型，eMSG_NOTICETYPE
    std::string msg_scene;          //公告展示的场景，管理端后台配置
    std::string start_time;         //公告有效期开始时间
    std::string end_time;           //公告有效期结束时间
    eMSG_CONTENTTYPE content_type;  //公告内容类型，eMSG_CONTENTTYPE
    //网页公告特殊字段
    std::string content_url;  //网页公告URL
    //图片公告特殊字段
    std::vector<PicInfo> picArray;  //图片数组
    //文本公告特殊字段
    std::string msg_title;    //公告标题
    std::string msg_content;  //公告内容
    std::string msg_custom; //公告自定义参数
#ifdef __APPLE__
    int msg_order;  //公告优先级，越大优先级越高，MSDK2.8.0版本新增
#endif

#ifdef ANDROID
    std::string msg_order;  //优先级，数字越大，优先级越高
#endif
} NoticeInfo;

#ifdef __APPLE__
typedef struct
{
    std::string fireDate;          //本地推送触发的时间,格式yyyy-MM-dd HH:mm:ss
    std::string alertBody;         //推送的内容
    int badge;                     //角标的数字
    std::string alertAction;       //替换弹框的按钮文字内容（默认为"启动"）
    std::vector<KVPair> userInfo;  //自定义参数，可以用来标识推送和增加附加信息
    std::string userInfoKey;       //本地推送在前台推送的标识Key
    std::string userInfoValue;     //本地推送在前台推送的标识Key对应的值
} LocalMessage;
#endif

#ifdef ANDROID
struct LocalMessage
{
    LocalMessage()
        : type(1), action_type(-1), icon_type(-1), lights(-1), ring(-1), vibrate(-1), style_id(-1), builderId(-1){};
    int type;
    int action_type;
    int icon_type;
    int lights;
    int ring;
    int vibrate;
    int style_id;
    long builderId;
    std::string content;
    std::string custom_content;
    std::string activity;
    std::string packageDownloadUrl;
    std::string packageName;
    std::string icon_res;
    std::string date;
    std::string hour;
    std::string intent;
    std::string min;
    std::string title;
    std::string url;
    std::string ring_raw;
    std::string small_icon;
};
#endif

typedef struct
{
    std::string name;         //姓名
    eIDType identityType;     //证件类型
    std::string identityNum;  //证件号码
    int province;             //省份
    std::string city;         //城市
} RealNameAuthInfo;

typedef struct GameGuild_
{
    // 工会id
    const char* guildId;
    // 工会名称
    const char* guildName;
    //公会会长的openId
    const char* leaderOpenId;
    //公会会长的roleid
    const char* leaderRoleId;
    //会长区服信息,会长可能转让给非本区服的人，与公会区服一样时可不填
    const char* leaderZoneId;
    // 区id
    const char* zoneId;
    //（小区）区服id，可以不填写，暂时无用
    const char* partition;
    // 角色id
    const char* roleId;
    const char* roleName;
    //用户区服ID，王者的会长可能转让给非本区服的人，所以公会区服不一定是用户区服。与公会区服一样时可不填
    const char* userZoneId;
    //新增修改群名片功能，全不填为不修改群名片；任一有内容为需要修改；样式规则：【YYYY】ZZZZ,ZZZZ指用户的游戏内昵称
    const char* userLabel;
    //用户昵称
    const char* nickName;
    //0公会(或不填),1队伍，2赛事
    const char* type;
    //测试环境使用：游戏大区ID，理论上只有1:QQ,2:微信，但是测试环境有很多虚拟的
    const char* areaId;
    GameGuild_() : guildId(NULL),guildName(NULL),leaderOpenId(NULL),leaderRoleId(NULL),leaderZoneId(NULL),zoneId(NULL),partition
        (NULL),roleId(NULL),roleName(NULL),userZoneId(NULL),userLabel(NULL),nickName(NULL),type("0"),areaId("1"){};
} GameGuild;


class MSDKEXPORT WXMessageButton
{
   public:
    WXMessageButton(std::string aName);
    virtual ~WXMessageButton();
    virtual std::string parserToJsonString() = 0;

   protected:
    //    eWXButtonType type; // 按钮类型
    std::string name;  // 按钮名称
};

class MSDKEXPORT ButtonApp : public WXMessageButton
{
   public:
    ButtonApp(std::string aName, std::string aMessageExt);
    ~ButtonApp();
    std::string parserToJsonString();

   protected:
    std::string messageExt;  // 附加自定义信息，通过按钮拉起应用时会带回游戏
};

class MSDKEXPORT ButtonWebview : public WXMessageButton
{
   public:
    ButtonWebview(std::string aName, std::string aWebViewUrl);
    ~ButtonWebview();
    std::string parserToJsonString();

   protected:
    std::string webViewUrl;  // 点击按钮后要跳转的页面
};

class MSDKEXPORT MSDKEXPORT ButtonRankView : public WXMessageButton
{
   public:
    ButtonRankView(std::string aName, std::string aTitle, std::string aButtonName, std::string aMessageExt);
    ~ButtonRankView();
    std::string parserToJsonString();

   protected:
    std::string title;               // 排行榜名称
    std::string rankViewButtonName;  // 排行榜中按钮的名称
    std::string messageExt;          // 附加自定义信息，通过排行榜中按钮拉起应用时会带回游戏
};

class MSDKEXPORT WXMessageTypeInfo
{
   public:
    WXMessageTypeInfo(std::string aPictureUrl);
    virtual ~WXMessageTypeInfo();
    virtual std::string parserToJsonString(eWXMessageType &type) = 0;

   protected:
    std::string pictureUrl;  // 在消息中心的消息图标Url（图片消息中，此链接则为图片URL）
};

class MSDKEXPORT TypeInfoImage : public WXMessageTypeInfo
{
   public:
    TypeInfoImage(std::string aPictureUrl, int aHeight, int aWidth);
    virtual ~TypeInfoImage();
    virtual std::string parserToJsonString(eWXMessageType &type);

   protected:
    int height;  // 图片高度
    int width;   // 图片宽度
};

class MSDKEXPORT TypeInfoVideo : public TypeInfoImage
{
   public:
    TypeInfoVideo(std::string aPictureUrl, int aHeight, int aWidth, std::string aMediaUrl);
    ~TypeInfoVideo();
    std::string parserToJsonString(eWXMessageType &type);

   protected:
    std::string mediaUrl;  // 相比图片消息，链接消息多此mediaUrl表示视频URL
};

class MSDKEXPORT TypeInfoLink : public WXMessageTypeInfo
{
   public:
    TypeInfoLink(std::string aPictureUrl, std::string aTargetUrl);
    std::string parserToJsonString(eWXMessageType &type);

   protected:
    std::string targetUrl;  // 链接消息的目标URL，点击消息拉起此链接
};

class MSDKEXPORT TypeInfoText : public WXMessageTypeInfo
{
   public:
    TypeInfoText();
    std::string parserToJsonString(eWXMessageType &type);
};

#ifdef ANDROID
static inline jstring StringFromStdString(JNIEnv *env, char const *str)
{
    return env->NewStringUTF(str);
}

static inline void printDumpReferenceTables(JNIEnv *env)
{
    jclass vm_class = env->FindClass("dalvik/system/VMDebug");
    jmethodID dump_mid = env->GetStaticMethodID(vm_class, "dumpReferenceTables", "()V");
    env->CallStaticVoidMethod(vm_class, dump_mid);
    env->DeleteLocalRef(vm_class);
}

#endif

#endif
