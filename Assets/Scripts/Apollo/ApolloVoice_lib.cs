using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_IPHONE
public class ApolloVoice_lib : IApolloVoice
{
    private static bool bInit = false;

#if UNITY_ANDROID
    private static AndroidJavaClass mApolloVoice = null;
#endif
   
  

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceCreateEngine([MarshalAs(UnmanagedType.LPArray)] string appID, [MarshalAs(UnmanagedType.LPArray)] string openID);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceDestoryEngine();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceJoinRoom([MarshalAs(UnmanagedType.LPArray)] string url1, [MarshalAs(UnmanagedType.LPArray)]string url2,[MarshalAs(UnmanagedType.LPArray)] string url3, Int64 roomId, Int64 roomKey, short memberId, [MarshalAs(UnmanagedType.LPArray)] string openId, int nTimeOut);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetJoinRoomResult();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceQuitRoom(Int64 roomId, short memberId, [MarshalAs(UnmanagedType.LPArray)] byte [] OpenId);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceOpenMic();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceCloseMic();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceOpenSpeaker();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceCloseSpeaker();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoicePause();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceResume();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif  
    private static extern int ApolloVoiceGetMemberState(int[] memberState, int nSize);
  
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetMemberCount(int nCount);
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceStartRecord([MarshalAs(UnmanagedType.LPArray)] string strFullPath, bool bOptim);
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceStopRecord(bool bAutoSend, bool bOptim);
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetFileID(byte[]  filekey, int nSize);  
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetAppReportInfo([MarshalAs(UnmanagedType.LPArray)] string reportInfo);  

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSendRecFile([MarshalAs(UnmanagedType.LPArray)] string strFullPath);  

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceDownloadFile([MarshalAs(UnmanagedType.LPArray)] string strFullPath, [MarshalAs(UnmanagedType.LPArray)] string strFileID, bool bAutoPlay);  
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoicePlayFile([MarshalAs(UnmanagedType.LPArray)] string strFullPath); 
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceForbidMemberVoice(int nMemberId, bool bEnable);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetMode(int nMode);  
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloGetMicLevel(); 

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetSpeakerVolume(int nvol);  
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetPhoneMode(); 
    
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetSpeakerLevel();         
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern uint ApolloVoiceGetOfflineFileSize();       
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern float ApolloVoiceGetOfflineFileTime();


// big room

#if UNITY_IPHONE
[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
private static extern int ApolloVoiceJoinBigRoom([MarshalAs(UnmanagedType.LPArray)] string urls,int role, UInt32 businessID , Int64 roomId, Int64 roomKey, short memberId, [MarshalAs(UnmanagedType.LPArray)] string openId, int nTimeOut);


#if UNITY_IPHONE
[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
private static extern int ApolloVoiceGetJoinBigRoomResult();

#if UNITY_IPHONE
[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
private static extern int ApolloVoiceQuitBigRoom();

#if UNITY_IPHONE
[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
private static extern int ApolloVoiceSetAudience([MarshalAs(UnmanagedType.LPArray)] int [] audience, int count);

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
	[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceSetBGMPath([MarshalAs(UnmanagedType.LPArray)] string path);

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
	[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceStartBGMPlay();

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
	[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceStopBGMPlay();

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
	[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoicePauseBGMPlay();

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
	[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceResumeBGMPlay();

#if UNITY_IPHONE
		[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
		[DllImport("apollo_voice")]
#endif
		private static extern int ApolloVoiceEnableNativeBGMPlay(bool bEnable);
		
#if UNITY_IPHONE
		[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
		[DllImport("apollo_voice")]
#endif
		private static extern int ApolloVoiceSetBGMVol(int nvol);

#if UNITY_IPHONE
		[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
		[DllImport("apollo_voice")]
#endif
		private static extern int ApolloVoiceGetBGMPlayState();


#if UNITY_IPHONE
        [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        [DllImport("apollo_voice")]
#endif
        private static extern int ApolloVoiceSetRegion(int region);  

	public void Init()
    {

        ApolloVoiceLog("apollo voice android sdk init ok!");       

    }

    public int _CreateApolloVoiceEngine(string appID, string openID)
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _CreateApolloVoiceEngine");
		int nRet = ApolloVoiceCreateEngine(appID, openID);
        if (0 == nRet)
        {
            bInit = true;
        }
        
        ApolloVoiceLog("ApolloVoiceC# API: _CreateApolloVoiceEngine nRet=" + nRet);
        return nRet;
    }
    
    public int _DestoryApolloVoiceEngine()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _DestoryApolloVoiceEngine");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}

        bInit = false;
        int nRet = ApolloVoiceDestoryEngine();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _DestoryApolloVoiceEngine Failed nRet=" + nRet);
        return nRet;
    }

    public int _JoinRoom(string url1, string url2, string url3, Int64 roomId, Int64 roomKey, short memberId, string OpenId, int nTimeOut)
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _JoinRoom");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

		/*
        byte[] byteUrl1 = System.Text.Encoding.UTF8.GetBytes(url1);
		byte[] byteUrl2 = System.Text.Encoding.UTF8.GetBytes(url2);
		byte[] byteUrl3 = System.Text.Encoding.UTF8.GetBytes(url3);

		byte[] byteOpenId = System.Text.Encoding.UTF8.GetBytes(OpenId);
		*/
		int nRet = ApolloVoiceJoinRoom(url1, url2, url3, roomId, roomKey, memberId, OpenId, nTimeOut);
        if (0 == nRet)
        {
            //ApolloVoiceDelegate.m_RealtimeFuncCall[0] = 1;
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _JoinRoom Failed nRet=" + nRet);
        return nRet;
    }

	public int _JoinBigRoom(string urls, int role, UInt32 busniessID, Int64 roomId, Int64 roomKey, short memberId, string OpenId, int nTimeOut)
	{
		ApolloVoiceLog("ApolloVoiceC# API: _JoinBigRoom");
		if (!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}

		int nRet = ApolloVoiceJoinBigRoom(urls,role, busniessID, roomId, roomKey, memberId, OpenId, nTimeOut);
		if (0 == nRet)
		{
			return 0;
		}
		ApolloVoiceLog("ApolloVoiceC# API: _JoinBigRoom Failed nRet=" + nRet);
		return nRet;

	}
	
	public int _GetJoinRoomBigResult()
	{
		ApolloVoiceLog("ApolloVoiceC# API: _GetJoinRoomBigResult");
		if (!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		
		return ApolloVoiceGetJoinBigRoomResult();
	}

	public int _QuitBigRoom()
	{
		ApolloVoiceLog("ApolloVoiceC# API: _QuitBigRoom");
		if (!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}	
		
		int nRet = ApolloVoiceQuitBigRoom();		
		return nRet;

	}

    public int _GetJoinRoomResult()
    {
        ApolloVoiceLog("ApolloVoiceC# API: _GetJoinRoomResult");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        return ApolloVoiceGetJoinRoomResult();
    }


    public int _QuitRoom(Int64 roomId, short memberId, String OpenId)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _QuitRoom");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        

        byte[] byteOpenId = System.Text.Encoding.ASCII.GetBytes(OpenId);
        int nRet = ApolloVoiceQuitRoom(roomId, memberId, byteOpenId);
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _QuitRoom Failed nRet=" + nRet);
        
        return nRet;
    }
    
    public int _OpenMic()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _OpenMic");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
	
 				
        int nRet = ApolloVoiceOpenMic();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _OpenMic Failed nRet=" + nRet);
        return nRet;
    }
    
    public int _CloseMic()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _CloseMic");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }



        int nRet = ApolloVoiceCloseMic();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _CloseMic Failed nRet=" + nRet);
        return nRet;
    }
    
     public int _OpenSpeaker()
    {    	
    	ApolloVoiceLog("ApolloVoiceC# API: _OpenSpeaker");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
       
        int nRet = ApolloVoiceOpenSpeaker();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _OpenSpeaker Failed nRet=" + nRet);

        return nRet;
    }
    
     public int _CloseSpeaker()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _ClosenSpeaker");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        int nRet = ApolloVoiceCloseSpeaker();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _CloseSpeaker Failed nRet=" + nRet);
        return nRet;
    }
    
     public int _Resume()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _Resume");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        int nRet = ApolloVoiceResume();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _Resume Failed nRet=" + nRet);
        return nRet;
    }
    
     public int _Pause()
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _Pause");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        int nRet = ApolloVoicePause();
        if (0 == nRet)
        {
            return 0;
        }
        ApolloVoiceLog("ApolloVoiceC# API: _Pause Failed nRet=" + nRet);
        return nRet;
    }

     /*
    Get Member State, the audioengine return format is : memberId +'#' + memberState + '#' + memberId + '#' + memberState
      * memberId and memberState is string format
      */
     public int _GetMemberState(int[] memberState)
     {         
        // ApolloVoiceLog("ApolloVoiceC# API: _GetMemberState");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

         int nSize = memberState.Length;

         //StringBuilder strmemberState = new StringBuilder(nSize * 4);
         //c++ clear the buffer, 
         int nRet = ApolloVoiceGetMemberState(memberState, nSize);
        if( nRet > 0)
        {
            for(int i = 0; i < nRet; ++i)
            {
                ApolloVoiceLog("GetMemberState Result "  +  memberState[i]);
            }
        }


         return nRet;
     }	
     
      public int _StartRecord(string strFullPath)
     {
         ApolloVoiceLog("ApolloVoiceC# API: _StartRecord");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
				
         int nRet = ApolloVoiceStartRecord(strFullPath, false);        

         return nRet;
     }	
     
     public int _StopRecord(bool bAutoSend)
     {
         ApolloVoiceLog("ApolloVoiceC# API: _StopRecord");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

				
         int nRet = ApolloVoiceStopRecord(bAutoSend, false);        

         return nRet;
     }	

#if UNITY_IPHONE
      public int _StartRecord(string strFullPath, bool bOptim)
     {
         ApolloVoiceLog("ApolloVoiceC# API: _StartRecord");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
				
         int nRet = ApolloVoiceStartRecord(strFullPath, bOptim);        

         return nRet;
     }	
     
     public int _StopRecord(bool bAutoSend, bool bOptim)
     {
         ApolloVoiceLog("ApolloVoiceC# API: _StopRecord");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

				
         int nRet = ApolloVoiceStopRecord(bAutoSend, bOptim);        

         return nRet;
     }	
#endif
     
      public int _SetMemberCount(int nCount)
     {
         ApolloVoiceLog("ApolloVoiceC# API: _SetMemberCount");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

				
         int nRet = ApolloVoiceSetMemberCount(nCount);        

         return nRet;
     }	
     
     public int _GetFileID(byte [] fileKey)
     {
     	 	ApolloVoiceLog("ApolloVoiceC# API: _GetFileKey");
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

				int nSize = fileKey.Length;
         int nRet = ApolloVoiceGetFileID(fileKey, nSize); 
         ApolloVoiceLog("fileID buffer size " + nSize + " fileID = " + Encoding.Default.GetString(fileKey));

         return nRet;
    }
    
    public int _SetAppReportInfo(string reportInfo)
    {
         if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }

         int nRet = ApolloVoiceSetAppReportInfo(reportInfo); 
         return nRet;
    }

    public int _SendRecFile(string strFullPath)
    {
     	ApolloVoiceLog("ApolloVoiceC# API: _SendRecFile");
		 if (!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
         //ApolloVoiceDelegate.m_OfflineFuncCall[0] = 1;
    	return ApolloVoiceSendRecFile(strFullPath);
    }
    
    public int _PlayFile(string strFullPath)
    {
    	ApolloVoiceLog("ApolloVoiceC# API: _PlayFile");
       if (!bInit)
       {
           return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
       //ApolloVoiceDelegate.m_OfflineFuncCall[2] = 1;
       return ApolloVoicePlayFile(strFullPath);
    }
    
    public int _DownloadVoiceFile(string strFullPath, string strFileID, bool bAutoPlay)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _DownloadVoiceFile");
       if (!bInit)
       {
           return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
       //ApolloVoiceDelegate.m_OfflineFuncCall[1] = 1;
       return ApolloVoiceDownloadFile(strFullPath, strFileID, bAutoPlay);
    }
    
    public int _ForbidMemberVoice(int nMemberId, bool bEnable)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _ForbidMemberVoice");
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
    	return ApolloVoiceForbidMemberVoice(nMemberId, bEnable);
    }
    
    public int _SetMode(int nMode)
    {
    		ApolloVoiceLog("ApolloVoiceC# API: _SetMode");
       if (!bInit)
       {
           return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
    	return ApolloVoiceSetMode(nMode);
    }
    
    public int _GetMicLevel()
    {    	
       if (!bInit)
       {
           return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
    	return ApolloGetMicLevel();
    }

    public int _SetSpeakerVolume(int nVol)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _SetSpeakerVolume");
       if (!bInit)
       {
           return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
        return ApolloVoiceSetSpeakerVolume(nVol);
    }
    
    public int _GetSpeakerLevel()
    {
    	if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
       }
       
       return ApolloVoiceGetSpeakerLevel();
    }

    public uint _GetOfflineFileSize()
    {
         if(!bInit)
         {
             return 0;
         }
         return (uint)ApolloVoiceGetOfflineFileSize();
    }
    public float _GetOfflineFileTime()
    {
         if(!bInit)
         {
             return 0;
         }
         return ApolloVoiceGetOfflineFileTime();
    }

	public int _SetBGMPath(string path)
	{
		ApolloVoiceLog("ApolloVoiceC# API: _SetBGMPath");
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceSetBGMPath(path);
	}
	public int _StartBGMPlay()
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceStartBGMPlay();
	}
	public int _StopBGMPlay()
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceStopBGMPlay();
	}
	public int _PauseBGMPlay()
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoicePauseBGMPlay();
	}
	public int _ResumeBGMPlay()
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceResumeBGMPlay();
	}
	public int _EnableNativeBGMPlay(bool bEnable)
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceEnableNativeBGMPlay(bEnable);
		
	}
	public int _SetBGMVol(int nvol)
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceSetBGMVol(nvol);
	}

	public int _GetBGMPlayState()
	{
		if(!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}
		return ApolloVoiceGetBGMPlayState();

		
	}
	public int _SetAudience(int []audience)
	{
		return ApolloVoiceSetAudience (audience, audience.Length); 
	}

    public int _SetRegion(ApolloVoiceRegion region)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _SetRegion");
        if (!bInit)
        {
            return 0;
        }

        int nRet = ApolloVoiceSetRegion((int)region);
        return nRet;
    }
    
//-----------------------------------------HTTP Upload/Download-----------------------------
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceSetServiceInfo(int nIP0, int nIP1, int nIP2, int nIP3, int nPort, int nTimeout);

    public int _SetServiceInfo(int nIP0, int nIP1, int nIP2, int nIP3, int nPort, int nTimeout)
    {
    	if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceSetServiceInfo(nIP0, nIP1, nIP2, nIP3, nPort, nTimeout);
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceSetAuthkey([MarshalAs(UnmanagedType.LPArray)] byte[] strAuthkey, int nLength);

    public int _SetAuthkey(byte[] strAuthkey, int nLength)
    {
        if(!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceSetAuthkey(strAuthkey, nLength);
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetDownloadState();

    public int _GetVoiceDownloadState()
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceGetDownloadState();
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetUploadState();

    public int _GetVoiceUploadState()
    {
         if(!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
         return ApolloVoiceGetUploadState();
    }
		
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
    private static extern int ApolloVoiceTestMic();
#endif
	
    public int _TestMic()
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        ApolloVoiceLog("ApolloVoiceC# API: _TestMic");
    	if (!bInit)
       {
             return 0;
       }
       
       return ApolloVoiceTestMic();
#else
		return 0;
#endif
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetPlayFileState();

    public int _GetPlayFileState()
    {
         if(!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
         return ApolloVoiceGetPlayFileState();
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceStopPlayFile();
    public int _StopPlayFile()
    {
         if(!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
         return ApolloVoiceStopPlayFile();
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetSubBID([MarshalAs(UnmanagedType.LPArray)] string cszSubBID, int nLength);

    public int _SetSubBID(string strSubBID)
    {
        if(!bInit)
         {
             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
         }
         return ApolloVoiceSetSubBID(strSubBID, strSubBID.Length);
    }

///////////////////////////////////////////////////////////////////////////////
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceEncodeWAVFileForPESQ([MarshalAs(UnmanagedType.LPArray)] string strSrcFile,[MarshalAs(UnmanagedType.LPArray)] string strDstFile);

    public int _EncodeWAVFile(string strSrcFile, string strDstFile)
    {
        if(!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceEncodeWAVFileForPESQ(strSrcFile, strDstFile);
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceDecodeToWAVEFile([MarshalAs(UnmanagedType.LPArray)] string strSrcFile,[MarshalAs(UnmanagedType.LPArray)] string strDstFile);

    public int _DecodeWAVFile(string strSrcFile, string strDstFile)
    {
        if(!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceDecodeToWAVEFile(strSrcFile, strDstFile);
    }
#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetWAVEFileProcessedState();

    public int _GetWAVEFileProcessedState()
    {
        if(!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceGetWAVEFileProcessedState();
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceEnableLog(bool bEnable);

    public int _EnableLog(bool bEnable)
    {
        if(!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceEnableLog(bEnable);
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceSetCodec(int mode, int codec);
    
    public int _SetCodec(int mode, int codec)
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        return ApolloVoiceSetCodec( mode, codec );
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceDownloadMusicFile([MarshalAs(UnmanagedType.LPArray)] string strUrl, [MarshalAs(UnmanagedType.LPArray)] string strPath, int nTimeout);

    public int _DownMusicFile(string strUrl, string strPath, int nTimeout)
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
    
        return ApolloVoiceDownloadMusicFile(strUrl, strPath, nTimeout);
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceGetDownloadMusicFileState();

    public int _GetDownloadMusicFileState()
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceGetDownloadMusicFileState();
    }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVocieSetAnchorUsed(bool bEnable);
    public int _SetAnchorUsed(bool bEnable)
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVocieSetAnchorUsed(bEnable);
    }

#if UNITY_IPHONE
        [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        [DllImport("apollo_voice")]
#endif
    private static extern float ApolloVoiceGetLostRate();
    public float _GetLostRate()
    {
        return ApolloVoiceGetLostRate();
    }

// #if UNITY_IPHONE
//         [DllImport("__Internal")]
// #elif UNITY_ANDROID || UNITY_STANDALONE_WIN
//         [DllImport("apollo_voice")]
// #endif
//     private static extern int ApolloVoiceEnableCaptureMicrophone(bool bEnable);
//     public int _EnableCaptureMicrophone(bool bEnable)
//     {
//          if (!bInit)
//         {
//             return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
//         }
//         return ApolloVoiceEnableCaptureMicrophone(bEnable);
//     }

#if UNITY_IPHONE
    [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
    [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceQuitDownloadMusicFile();

    public int _QuitDownMusicFile()
    {
        if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }

        return ApolloVoiceQuitDownloadMusicFile();
    }

    [System.Diagnostics.Conditional("DEBUG_VOICE")]
    private void ApolloVoiceLog(string logStr)
    {
        Debug.Log(logStr);
    }


#if UNITY_IPHONE
        [DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        [DllImport("apollo_voice")]
#endif
    private static extern int ApolloVoiceCaptureMicrophone(bool bEnable);
    public int _CaptureMicrophone(bool bEnable)
    {
         if (!bInit)
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceCaptureMicrophone(bEnable);
    }

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceEnableSoftAec(bool bEnable);

	public int _EnableSoftAec(bool bEnable)
	{
		ApolloVoiceLog("ApolloVoiceC# API: _EnableSoftAecIOS");
		if (!bInit)
		{
			return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
		}

		int nRet = ApolloVoiceEnableSoftAec(bEnable);        

		return nRet;
	}	

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceInvoke(uint nCmd, uint nParam1, uint nParam2, [MarshalAs(UnmanagedType.LPArray)] int[] pOutput);

    public int _Invoke(uint nCmd, uint nParam1, uint nParam2, int[] pOutput)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _Invoke");
        if( !bInit )
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceInvoke(nCmd, nParam1, nParam2, pOutput);
    }

#if UNITY_IPHONE
	[DllImport("__Internal")]
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
[DllImport("apollo_voice")]
#endif
	private static extern int ApolloVoiceEnableSpeakerOn(bool bEnable);

    public int _SetSpeakerOn(bool bEnable)
    {
        ApolloVoiceLog("ApolloVoiceC# API: _SetSpeakerOn");
        if( !bInit )
        {
            return (int)ApolloVoiceErr.APOLLO_VOICE_STATE_ERR;
        }
        return ApolloVoiceEnableSpeakerOn(bEnable);
    }

}
#endif
