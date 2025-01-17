using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

#region interface about platform call

public enum ApolloVoiceRegion {
    //        REGION_CHINA = 156,
    REGION_CHINA = 1888,
    REGION_TAIWAN = 158,
    REGION_HONGKONG = 344,
    REGION_USA = 840,
    REGION_THAILAND = 764,
    REGION_INDIA = 356,
    REGION_VIETNAM = 704,
    REGION_GERMANY = 276,
    REGION_BRAZIL  = 76,
    REGION_MALAYSIA = 458,
    REGION_KOREA= 410,
};

public enum ApolloVoiceErr 
{    
        APOLLO_VOICE_SUCC           = 0,
        APOLLO_VOICE_NONE           = 1,
        APOLLO_VOICE_UNKNOWN        = 3,
		APOLLO_VOICE_STATE_ERR		= 4,
		APOLLO_VOICE_CREATE_ERR		= 5,
        APOLLO_VOICE_IN_ROOM        = 6,
        APOLLO_VOICE_PATH_NULL      = 7,
        APOLLO_VOICE_PATH_ACCESS    = 8,
        APOLLO_VOICE_UPLOAD         = 9,
        APOLLO_VOICE_DOWNLOAD       = 10,
        APOLLO_VOICE_HTTP_BUSY      = 11,
		APOLLO_VOICE_RECORDING		= 12,
		APOLLO_VOICE_OPENID         = 13,
		APOLLO_VOICE_AUDIENCE       = 14,
        
        APOLLO_VOICE_JOIN_TIMEOUT   = 50,
        APOLLO_VOICE_JOIN_SUCC      = 51,
        APOLLO_VOICE_JOIN_NOTIN     = 52,
        APOLLO_VOICE_JOIN_FAIL      = 53,
        APOLLO_VOICE_JOIN_URL       = 54,
   		
        APOLLO_VOICE_NET_TIMEOUT    = 121,

        APOLLO_VOICE_PERMISSION_MIC = 200,
        APOLLO_VOICE_SPEAKER        = 201,
        APOLLO_VOICE_EMPTY_AUTHKEY = 202,
        APOLLO_VOICE_EMPTY_FILE = 203,
        APOLLO_VOICE_EMPTY_OPENID = 204,
        APOLLO_VOICE_EMPTY_FILE_ID = 205,
        
        APOLLO_VOICE_TVE_CREATE     = 300,
   		APOLLO_VOICE_TVE_NULL		= 301,
        APOLLO_VOICE_TVE_STOP		= 302,
        APOLLO_VOICE_TVE_INIT		= 303,
        APOLLO_VOICE_TVE_START      = 304,
        APOLLO_VOICE_TVE_CREATE_NTFY = 305,
        APOLLO_VOICE_TVE_FILEKEY_NULL = 306,
        APOLLO_VOICE_TVE_BUF_NULL   = 307,
        APOLLO_VOICE_TVE_PLAYSOUND  = 308,
        APOLLO_VOICE_TVE_FORBID     = 309,
		APOLLO_VOICE_CHANGE_MODE    = 310,
        
        APOLLO_VOICE_CDNV_CREATE    = 400,
        APOLLO_VOICE_CDNV_NULL		= 401,
        APOLLO_VOICE_CDNV_QUIT      = 402,
        APOLLO_VOICE_CDNV_CREATE_NTFY = 403,
        APOLLO_VOICE_HTTP_ERROR_DATA = 404,
        APOLLO_VOICE_HTTP_BADPARAM  = 405,

        APOLLO_VOICE_PARAM_NULL = 406,
		APOLLO_VOICE_STOP_PLAY_FILE = 407,
        APOLLO_VOICE_CDNV_URL = 408,

        APOLLO_VOICE_PAUSED = 409,
        APOLLO_VOICE_WRONG_MODE = 410,
        APOLLO_VOICE_RECORD_FILE_FAILED = 411,
        APOLLO_VOICE_CLOSE_MIC = 412,
        APOLLO_VOICE_FILE_OPERATION = 413,
        APOLLO_VOICE_HTTP_INIT = 414,
        APOLLO_VOICE_HTTP_GET = 415,
}

public enum ApolloVoiceRole
{
	ANCHOR = 1,
	AUDIENCE = 2,
}

public enum ApolloVoiceMode
{
    REALTIME_VOICE = 0, //实时语音
    OFFLINE_VOICE = 1,  //离线语音
    STT_VOICE = 2,      //离线转文字语音
}

public interface IApolloVoice
{
    #region BasicFunctions

    /**
    * Init Environment.
    * @result: 0 for succ, otherwise failed
    */
    void Init();

    /**
    * Create Apollo Voice Engine.
    * @param appID: The project appID
    * @param openID: the user's openID
    * @result: 0 for succ, otherwise failed
    */
    int _CreateApolloVoiceEngine(string appID, string openID = null);

    /**
    * Destory Apollo Voice Engine.
    * @result: 0 for succ, otherwise failed
    */
    int _DestoryApolloVoiceEngine();

    /**
    * Set Engine work mode.
    * @param nMode: 0 for realtime chat, 1 for offline file record, 2 for STT(Sound To Text).
    * @result: 0 for succ, otherwise failed
    */
    int _SetMode(int nMode);

    /**
    * Pause the voice engine(while App switch to background, you need call this function).
    * @result: 0 for succ, otherwise failed
    */
    int _Pause();

    /**
    * Resume the voice engine(while App switch back to foreground, you need call this function).
    * @result: 0 for succ, otherwise failed
    */
    int _Resume();

    /**
    * Set the codec for VoiceEngine.
    * default: realtime use opus, offline use Speex
    * default codec is good enough, unless you have other requirement, you should never change the codec
    * @param mode: 0 for realtime, 1 for offline
    * @param codec: the codec you need to use in current mode
    * 	QT_CODEC_PCM                    = 4097,
	    QT_CODEC_SPEEX_MONO             = 4098,
	    QT_CODEC_SPEEX_MS               = 4098,
	    QT_CODEC_CELT_0_11_1            = 4099,
	    QT_CODEC_SPEEX_ST               = 4100,
	    QT_CODED_SPEEX_IS               = 4101,
	    QT_CODEC_OPUS_1_1_0             = 4102,
	    QT_CODEC_SILK_1_0_8             = 4103,
	    QT_CODEC_AMRWB                  = 4104,
	    QT_CODEC_AMRWBPLUS              = 4105,
	    QT_CODEC_EAACPLUS               = 4106,
	    QT_CODEC_TEST                   = 4107,
	    QT_CODEC_G718					= 4108,
	    QT_CODEC_AMR_NB                 = 4109,
	    QT_CODEC_MP3                    = 4110,
    * @result: 0 for succ, otherwise failed
    */
    int _SetCodec(int mode, int codec);

    /**
    * Get latest Voice Frame Level captured by Mic.
    * @result: the captured mic volume, the range is 0 - 0x0FFFF
    */
    int _GetMicLevel();

    /**
    * Set the Amplification factor of speaker volume.
    * @param nVol: On Mobile Platform, 0 is Silence, Max is 800 times. On Windows platform, the range is 
    * set speaker's volume directly, the range is 0 - 100
    * @result: 0 for succ, otherwise failed
    */
    int _SetSpeakerVolume(int nVol);

    /**
    * Get the Amplification factor of voice.
    * @result: the enlargement times
    */
    int _GetSpeakerLevel();

    /**
    * Open Or Close VoiceEngine's Log.
    * @param bEnable: true for enable, false for disable
    * @result: 0 for succ, otherwise failed
    */
    int _EnableLog(bool bEnable);

    /**
    * Test Mic is enable or not.
    * @result: 0 for succ, APOLLO_VOICE_RECORDING for recording now, otherwise failed
    */
    int _TestMic();

//     /**
//      * enable or disable capture microphone 
//      */
//     int _EnableCaptureMicrophone(bool bEnable
    /**
     * 
     */
    int _CaptureMicrophone(bool bEnable);
    #endregion


    #region Functions For RealtimeVoice Only
    /**
     * Join CDN Small Voice Room(Only Support up to 20 members in one room, generally used for 5V5 Scenes,
     * Only Support 5 people talking at the same time).
     * @param url1,url2,url3: CDN address
     * @param roomid: voice room ID, 
     * @param memberid: user'id in voice room
     * @param roomKey: user' roomkey in voice room
     * @param openid: user's login id
     * @param nTimeout: the max join room time
     * @result: 0 represent function executed succ(does not mean join succ), otherwise failed
     */
    int _JoinRoom(string url1, string url2, string url3, Int64 roomId, Int64 roomKey, short memberId, string OpenId, int nTimeOut);

    /**
    * Get JoinRoom Result.
    * @param nMode: 0 for realtime chat, 1 for offline file record, 2 for STT(Sound To Text)
    * @result: 0 for succ, otherwise failed
    */
    int _GetJoinRoomResult();

    /**
    * Quit VoiceRoom.
    * @param roomId: user's roomid
    * @param memberId: user's memberid
    * @param openid: user's login id
    * @result: 0 for succ, otherwise failed
    */
    int _QuitRoom(Int64 roomId, short memberId, string OpenId);

    /**
     * Join CDN Big Voice Room(Used for thousands of people playing in one room at the same time, Still Only Support 5 people talking
     * at the same time).
     * @param urls: CDN address
     * @param role: Players role, 0 for anchor(up to 5 people), 1 for audience(no limits)
     * @param busniessID: the busniessID for this project
     * @param memberid: user'id in voice room
     * @param roomKey: user' roomkey in voice room
     * @param openid: user's login id
     * @param nTimeout: the max join room time
     * @result: 0 represent function executed succ(does not mean join succ), otherwise failed
     */
    int _JoinBigRoom(string urls, int role, UInt32 busniessID, Int64 roomId, Int64 roomKey, short memberId, string OpenId, int nTimeOut);

    /**
    * Get JoinRoom Result.
    * @param nMode: 0 for realtime chat, 1 for offline file record, 2 for STT(Sound To Text)
    * @result: 0 for succ, other failed;
    */
    int _GetJoinRoomBigResult();

    /**
    * Quit Big Voice Room.
    * @result: 0 for succ, otherwise failed
    */
    int _QuitBigRoom();

    /**
    * Open Mic.
    * @result: 0 for succ, otherwise failed
    */
    int _OpenMic();

    /**
    * Close Mic.
    * @result: 0 for succ, otherwise failed
    */
    int _CloseMic();

    /**
    * Open Speaker.
    * @result: 0 for succ, otherwise failed
    */
    int _OpenSpeaker();

    /**
    * Close Speaker.
    * @result: 0 for succ, otherwise failed
    */
    int _CloseSpeaker();

    /*
     * Get member Speaking State.
     * @param memberState: the format is: MemberID(4 Byte) | State (4 Byte)|MemberID (4 Byte)| State(4 Byte)......
     * state : 1 for speaking ,0 for not speaking right now
     * @result: 0 for succ, otherwise failed
     */
    int _GetMemberState(int [] memberState);

    /**
    * Set room member count.
    * @param nCount: the num of member
    * @result: 0 for succ, otherwise failed
    */
    int _SetMemberCount(int nCount);

    #region BGM Play
    /**
    * Set the BGM path.
    * @param path: the BGM path
    * @result: 0 for succ, otherwise failed
    */
    int _SetBGMPath(string path);

    /**
    * Start playing BGM.
    * @result: 0 for succ, otherwise failed
    */
    int _StartBGMPlay();

    /**
    * Stop playing BGM.
    * @result: 0 for succ, otherwise failed
    */
    int _StopBGMPlay();

    /**
    * Pause playing BGM.
    * @result: 0 for succ, otherwise failed
    */
    int _PauseBGMPlay();

    /**
    * Resume playing BGM.
    * @result: 0 for succ, otherwise failed
    */
    int _ResumeBGMPlay();
    
    /*
    * get BGM play state.
    * @ param: pState:0 for playing ,
    *                 1 for stop
    */
    
    int _GetBGMPlayState();

    /**
    * You can hear the bgm in your own device.
    * @param bEnable: default is true, and you can not set false currently
    * @result: 0 for succ, otherwise failed
    */
    int _EnableNativeBGMPlay(bool bEnable);
    
    /** 
    * Set BGM Playing volume.
    * @param nvol:which want set volume of BGM Playing.
    * @result: 0 for succ, otherwise failed
    */
    int _SetBGMVol(int nvol);
    
    /**
     * Download music file
     */
    int _DownMusicFile(string strUrl, string strPath, int nTimeout);

    /**
     * get music file download state
     * result: reference ApolloVoiceErr
     */
    int _GetDownloadMusicFileState();

    /**
     * quit download music file
     */ 
    int _QuitDownMusicFile();
    #endregion

    #endregion

    #region Functions For Offline Voice Only
    /**
    * Start record.
    * @param strFullPath: the place to restore your local record file 
    * @result: 0 for succ, otherwise failed
    */
    int _StartRecord(string strFullPath);
#if UNITY_IPHONE
    /**
    * Start record.
    * @param strFullPath: the place to restore your local record file 
    * @param bOptim: record audio do not enter voip mode, may record BGM
    * @result: 0 for succ, otherwise failed
    */
    int _StartRecord(string strFullPath, bool bOptim);

    /**
    * Stop record.
    * @param bAutoSend: decide whether auto send local file to server
    * @result: 0 for succ, otherwise failed
    */
    int _StopRecord(bool bAutoSend, bool bOptim);
#endif

    /**
    * Stop record.
    * @param bAutoSend: decide whether auto send local file to server
    * @result: 0 for succ, otherwise failed
    */
    int _StopRecord(bool bAutoSend);

    /**
    * Get last file ID.
    * @param fileKey: the latest upload file key
    * @result: 0 for succ, otherwise failed
    */
    int _GetFileID(byte [] fileKey);

    /**
    * send rec file.
    * @param strFullPath: local record file stored place
    * @result: 0 for succ, otherwise failed
    */
    int _SendRecFile(string strFullPath);

    /**
    * Set app report info.
    * @param reportInfo: qos report info,call after SetMode
    * @result: 0 for succ, otherwise failed
    */
    int _SetAppReportInfo(string reportInfo);

    /**
    * play rec file.
    * @param strFullPath: local record file stored place
    * @result: 0 for succ, otherwise failed
    */
    int _PlayFile(string strFullPath);

    /**
    * download voice file.
    * @param strFullPath: local record file stored place
    * @param strFileID: latest uploaded file id
    * @param bAutoPlay: Currently unimplemented! It should be false
    * @result: 0 for succ, otherwise failed
    */
    int _DownloadVoiceFile(string strFullPath, string strFileID, bool bAutoPlay);

    /**
    * forbid one's voice.
    * @param nMemberId: the member id
    * @param bEnable: true for disable one's speaking, false for enable
    * @result: 0 for succ, otherwise failed
    */
    int _ForbidMemberVoice(int nMemberId, bool bEnable);

    /**
    * Set Offline Voice Server info.
    * @param nIP0, nIP1, nIP2, nIP3: the OfflineVoice server url, you can get all of it from game server
    * @param nPort: ip port
    * @param nTimeout: the unit is ms
    * @result: 0 for succ, otherwise failed
    */
    int _SetServiceInfo(int nIP0, int nIP1, int nIP2, int nIP3, int nPort, int nTimeout);

    /**
    * Set Authkey info.
    * @param strAuthkey: the authkey
    * @param nLength: the length of authkey
    * @result: 0 for succ, otherwise failed
    */
    int _SetAuthkey(byte[] strAuthkey, int nLength);

    /**
    * Get the State of _DownloadVoiceFile
    * you should loop it while the result is APOLLO_VOICE_HTTP_BUSY or APOLLO_VOICE_SUCC.
    * @result: 0 for succ,   
    * APOLLO_VOICE_HTTP_BUSY is still download
    * Otherwise is failed
    */
    int _GetVoiceDownloadState();

    /**
    * Get the State of _SendRecFile
    * you should loop it while the result is APOLLO_VOICE_HTTP_BUSY or APOLLO_VOICE_SUCC.
    * @result: 0 for succ,   
    * APOLLO_VOICE_HTTP_BUSY is still download
    * Otherwise is failed
    */
    int _GetVoiceUploadState();

    /**
    * Get Latest Recording File Time.
    * @result: The file time length
    */
    float _GetOfflineFileTime();

    /**
    * Get Latest Recording File Size.
    * @result: The file Size
    */
    uint _GetOfflineFileSize();

    /**
    * Decide whether is playing file now.
    * @result:  0 is idle or play finished, 1 is playing
    */
    int _GetPlayFileState();

    /**
    * Stop playing file immediately.
    * @result: 0 for succ, otherwise failed
    */
    int _StopPlayFile();

    /**
    * Only selected audience can receive voice(used for small room).
    * @param audience: A member id list who can receive voice
    * @result: 0 for succ, otherwise failed
    */
    int _SetAudience(int []audience);

    /**
    * Unused Now.
    * @result: 0 for succ, otherwise failed
    */
    int _SetSubBID(string strSubBID);

    /**
    * Query Lost Rate for Voice Package
    * @result: rate
    */
    float _GetLostRate();

//#if UNITY_STANDALONE_WIN
    int _SetAnchorUsed(bool bEnable);
//#endif
	/**
     * enable or disable software aec mode;
     * default: software aec is disabled;
     */
	int _EnableSoftAec(bool bEnable);

    /**
    * Set which region to deploy
    */
    int _SetRegion(ApolloVoiceRegion region);
    #endregion

    #region Reserved function

    /*
     * reserved function for special function
     * @param: nCmd command for invoke
     * @param: nParam1 command's parameter
     * @param: nParam2 command's parameter
     * @param: pOutput command's buffer for output
     * @return S_OK;
     */
    int _Invoke(uint nCmd, uint nParam1, uint nParam2, int[] pOutput);


	int _SetSpeakerOn(bool bEnable);
    #endregion
}
#endregion


class CApolloVoiceSys
{
    private IApolloVoice m_CallApolloVoiceSDK = null;

    public IApolloVoice CallApolloVoiceSDK
    {
        get
        {            
            return m_CallApolloVoiceSDK;
        }
    }

    public void SysInitial()
    {
#if UNITY_STANDALONE_OSX
		m_CallApolloVoiceSDK = null;
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        m_CallApolloVoiceSDK = new ApolloVoice_lib();
#elif UNITY_IPHONE
        m_CallApolloVoiceSDK = new ApolloVoice_lib();
#elif UNITY_ANDROID
        m_CallApolloVoiceSDK = new ApolloVoice_lib();
#endif
        if (null == m_CallApolloVoiceSDK)
        {
            Debug.Log("apollo voice sdk init error!");
            return;
        }

        Debug.Log("apollo voice sdk init!");
		m_CallApolloVoiceSDK.Init();
//        InitVoiceDelegate();
    }

//     private void InitVoiceDelegate()
//     {
//         GameObject VoiceDelegate = new GameObject("VoiceDelegate");
//         VoiceDelegate.AddComponent<ApolloVoiceDelegate>();
//         ApolloVoiceDelegate.SetVoiceEngine(m_CallApolloVoiceSDK);
// }
}

