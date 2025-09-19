//
//  WXApiObject+Private.h
//  WeChatSDK
//
//  Created by liuyunxuan on 2017/2/6.
//
//

NS_ASSUME_NONNULL_BEGIN

/** @brief 企业微信消息类型
 */
typedef NS_ENUM(NSUInteger, MMWeWorkMsgSubType) {
    MMWeWorkMsgSubTypeChatRecord = 1, //聊天记录
    MMWeWorkMsgSubTypeChatRecordOneByOne = 2, //逐条聊天记录
};

#pragma mark - WXHandleScanResultReq
/** ! @brief 调用微信处理扫码结果
 */
@interface WXHandleScanResultReq : BaseReq
@property (nonatomic, copy, nullable) NSString *scanResult;
@end


#pragma mark - CreateChatRoomReq
/*! @brief 游戏方公会建群请求
 *
 * 第三方程序向微信终端发送游戏方公会建群请求
 */
@interface CreateChatRoomReq : BaseReq
/** 公会id
 * @attention 长度不能超过1024字节
 */
@property (nonatomic, copy) NSString *groupId;

/** 公会名称(群名称)
 * @attention 长度不能超过512字节
 */
@property (nonatomic, copy) NSString *chatRoomName;

/** 玩家名称(群里展示)
 * @attention 长度不能超过512字节
 */
@property (nonatomic, copy) NSString *chatRoomNickName;

/** 第三方程序自定义简单数据，微信终端会回传给第三方程序处理
 * @attention 长度不能超过2k
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - CreateChatRoomResp
/*! @brief 微信终端向第三方程序返回的CreateChatRoomReq处理结果。
 *
 * 第三方程序向微信终端发送CreateChatRoomReq后，微信发送回来的处理结果，该结果用CreateChatRoomResp表示。
 */
@interface CreateChatRoomResp : BaseResp

@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - JoinChatRoomReq
/*! @brief 游戏方公会加群请求
 *
 * 第三方程序向微信终端发送游戏方公会加群请求
 */
@interface JoinChatRoomReq : BaseReq

/** 公会id
 * @attention 长度不能超过1024字节
 */
@property (nonatomic, copy) NSString *groupId;

/** 公会名称(群名称)
 * @attention 长度不能超过512字节
 */
@property (nonatomic, copy) NSString *chatRoomNickName;

/** 第三方程序自定义简单数据，微信终端会回传给第三方程序处理
 * @attention 长度不能超过2k
 */
@property (nonatomic, copy) NSString *extMsg;
@end

#pragma mark - JoinChatRoomResp
/*! @brief 微信终端向第三方程序返回的JoinChatRoomReq处理结果。
 *
 * 第三方程序向微信终端发送JoinChatRoomReq后，微信发送回来的处理结果，该结果用JoinChatRoomResp表示。
 */
@interface JoinChatRoomResp : BaseResp
/** 第三方程序自定义简单数据，微信终端会回传给第三方程序处理
 * @attention 长度不能超过2k
 */
@property (nonatomic, copy) NSString *extMsg;
@end

#pragma mark - WXVideoFileObject
/*! @brief 多媒体消息中包含的文件数据对象
 *
 * @see WXMediaMessage
 */
@interface WXVideoFileObject : NSObject

/*! @brief 返回一个WXVideoFileObject对象，目前只支持分享朋友圈的场景
 *
 * @note 返回的WXVideoFileObject对象是自动释放的
 */
+(WXVideoFileObject *) object;

/** 文件真实数据内容
 * @note 大小不能超过10OM
 */
@property (nonatomic, strong) NSData *videoFileData;

/**视频标识符
 * @note 一般通过PHAsset获取，微信用标识符在本地读取视频
 */
@property (nonatomic, copy) NSString *localIdentifier;

@end

#pragma mark - WXVideoFileObject
/*! @brief 多媒体消息中包含的文件数据对象
 * @see WXMediaMessage
 */
@interface WXGameVideoFileObject : NSObject

/*! @brief 返回一个WXGameVideoFileObject对象，目前只支持分享朋友圈的场景
 *
 * @note 返回的WXGameVideoFileObject对象是自动释放的
 */
+(WXGameVideoFileObject *) object;

/** 文件真实数据内容
 * @note 大小不能超过10M，时间不超过10s
 */
@property (nonatomic, strong) NSData *videoFileData;


/** video的url
 * @note
 */
@property (nonatomic, copy) NSString *videoUrl;

/** 缩略图的url
 * @note
 */
@property (nonatomic, copy) NSString *thumbUrl;

@end



#pragma mark - WXOpenTypeWebViewReq
/*! @brief 调用接口打开某个网页，这个接口不可以直接使用，实际的业务需要封装该接口
 *
 *
 */
@interface WXOpenTypeWebViewReq : BaseReq

/** webView业务类型
 * @attention
 */
@property (nonatomic, copy) NSString *wxInternalResptype;

/** webView业务类型
 * @attention
 */
@property (nonatomic, copy) NSString *query;

@end

#pragma mark - WXOpenTypeWebViewResp
/*! @brief 微信终端向第三方程序返回的WXOpenTypeWebViewResp处理结果。
 *
 * 第三方程序向微信终端发送WXOpenTypeWebViewReq后，微信发送回来的处理结果，该结果用WXOpenTypeWebViewResp表示。
 */
@interface WXOpenTypeWebViewResp : BaseResp
/** 第三方程序自定义简单数据，微信终端会回传给第三方程序处理
 * @attention 长度不能超过2k
 */
@property (nonatomic, strong) NSDictionary *infoDic;

@end


#pragma mark - WXEnterpriseCardObject
/*! @brief 企业微信名片包含的数据对象
 *
 * @see WXMediaMessage
 */
@interface WXEnterpriseCardObject : NSObject

/** 返回一个WXEnterpriseCardObject对象
 */
+(WXEnterpriseCardObject *) object;

/** 名片信息内容
 */
@property (nonatomic, copy) NSString *cardInfo;

/** 名片消息类型
 */
@property (nonatomic, assign) NSInteger msgType;

@end


#pragma mark - WXDynamicVideoMiniProgramObject

@interface WXDynamicVideoMiniProgramObject : NSObject

/*! @brief WXDynamicVideoMiniProgramObject对象
 *
 * @note 返回的WXDynamicVideoMiniProgramObject对象是自动释放的
 */
+ (WXDynamicVideoMiniProgramObject *)object;

/** 低版本网页链接
 * @attention 长度不能超过1024字节
 */
@property (nonatomic, copy) NSString *webpageUrl;

/** 小程序username */
@property (nonatomic, copy) NSString *userName;

/** 小程序页面的路径
 * @attention 不填默认拉起小程序首页
 */
@property (nonatomic, copy, nullable) NSString *path;

/** 小程序新版本的预览图
 * @attention 大小不能超过128k
 */
@property (nonatomic, strong, nullable) NSData *hdImageData;

/** 是否使用带 shareTicket 的转发 */
@property (nonatomic, assign) BOOL withShareTicket;

/** 是否禁用转发 */
@property (nonatomic, assign) BOOL disableForward;

/** 分享小程序的版本
 * @attention （正式，开发，体验）
 */
@property (nonatomic, assign) WXMiniProgramType miniProgramType;

/** 视频源
 * @attention 必填
 */
@property (nonatomic, copy) NSString *videoSource;

/** 视频首帧缩略图URL
 * @attention 大小不能超过10K
 */
@property (nonatomic, copy, nullable) NSString *appThumbUrl;
@end


#pragma mark - WXWeWorkObject

@interface WXWeWorkObject : NSObject

/*! @brief WXWeWorkObject对象
 *
 * @note 返回的WXWeWorkObject对象是自动释放的
 */
+ (WXWeWorkObject *)object;

/** 消息数据
 * @attention
 */
@property (nonatomic, strong) NSData *data;

/** 消息类型
 * @attention
 */
@property (nonatomic, assign) int subType;

/** ext信息
 * @note 选填
 */
@property (nonatomic, copy, nullable) NSString *extInfo;

@end


#pragma mark - WXChannelOpenProfileReq
//打开视频号profile页

@interface WXChannelOpenProfileReq : BaseReq

+ (WXChannelOpenProfileReq *)object;

/**用户名
 * @note 必填，长度不能超过1K
 */
@property (nonatomic, copy) NSString *userName;

@end

@interface WXChannelOpenProfileResp : BaseResp

/** 业务返回数据
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - WXChannelOpenLiveReq
//打开视频号直播

@interface WXChannelOpenLiveReq : BaseReq

+ (WXChannelOpenLiveReq *)object;

/**feedID
 * @note 必填，长度不能超过1K
 */
@property (nonatomic, copy) NSString *feedID;

/**nonceID
 * @note 必填，长度不能超过1K
 */
@property (nonatomic, copy) NSString *nonceID;

@end

@interface WXChannelOpenLiveResp : BaseResp

/** 业务返回数据
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - WXChannelOpenFeedReq
//打开视频号FEED页

@interface WXChannelOpenFeedReq : BaseReq

+ (WXChannelOpenFeedReq *)object;

/**feedID
 * @note 必填，长度不能超过1K
 */
@property (nonatomic, copy) NSString *feedID;

/**nonceID
 * @note 必填，长度不能超过1K
 */
@property (nonatomic, copy) NSString *nonceID;

/**notGetReleatedList
 * @note 选填，显示/不显示相似feed，默认显示
 */
@property (nonatomic, assign) BOOL notGetReleatedList;


@end

@interface WXChannelOpenFeedResp : BaseResp

/** 业务返回数据
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - WXChannelBaseJumpInfo

@interface WXChannelBaseJumpInfo : NSObject

/** 跳转文案 */
@property (nonatomic, copy) NSString *wording;

/** 拓展字段 */
@property (nonatomic, strong, nullable) NSString *extData;

@end


#pragma mark - WXChannelJumpMiniProgramInfo

@interface WXChannelJumpMiniProgramInfo : WXChannelBaseJumpInfo

/** 小程序username */
@property (nonatomic, copy) NSString *userName;

/** 小程序页面的路径
 * @attention 不填默认拉起小程序首页
 */
@property (nonatomic, strong, nullable) NSString *path;

@end

#pragma mark - WXChannelJumpUrlInfo

@interface WXChannelJumpUrlInfo : WXChannelBaseJumpInfo

/** 视频号跳转Url */
@property (nonatomic, copy) NSString *url;

@end


#pragma mark - WXChannelShareVideoReq
/*! @brief WXChannelShareVideoReq对象，用于将视频分享到视频号
 *
 */
@interface WXChannelShareVideoReq : BaseReq

+ (WXChannelShareVideoReq *)object;

/**视频标识符
 * @note 必填，一般通过PHAsset获取，在微信视频号中利用标识符在本地读取视频
 */
@property (nonatomic, copy) NSString *localIdentifier;

/**拓展字段
 */
@property (nonatomic, strong, nullable) NSData *extData;

/**跳转数据
 */
@property (nonatomic, strong, nullable) WXChannelBaseJumpInfo *jumpInfo;

@end

@interface WXChannelShareVideoResp : BaseResp

/** 业务返回数据
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end

#pragma mark - WXChannelOpenEvent
@interface WXChannelOpenEventReq : BaseReq

+ (WXChannelOpenEventReq *)object;

/** 活动创建者的username
 @note 必填，长度最大为10240
 */
@property (nonatomic, copy) NSString *userName;


/** 活动的id
 @note 必填，长度最大为10240
 */
@property (nonatomic, copy) NSString *eventId;

/** extInfo
 @note 选填，扩展字段
 */
@property (nonatomic, copy, nullable) NSString *extInfo;

@end

@interface WXChannelOpenEventResp : BaseResp

/** 业务返回数据
 */
@property (nonatomic, copy, nullable) NSString *extMsg;

@end


NS_ASSUME_NONNULL_END
