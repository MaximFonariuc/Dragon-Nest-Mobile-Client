using UnityEngine;
using System.Collections;
#if (UNITY_IOS || UNITY_ANDROID ) && GAMESIR && !UNITY_EDITOR
using Gamesir;
#endif

using XUtliPoolLib;
public class XGameSirControl : MonoBehaviour,IXGameSirControl
{
	private bool mIsOpen = false;
    public void ShowGameSirDialog()
    {
#if (UNITY_IOS || UNITY_ANDROID ) && GAMESIR && !UNITY_EDITOR
		if(mIsOpen){
			GamesirInput.Instance().OpenConnectDialog();
		}
#endif
    }
    
    public int GetGameSirState()
    {
		#if (UNITY_IOS || UNITY_ANDROID )&& GAMESIR &&  !UNITY_EDITOR
		if(mIsOpen)
        	return GamesirInput.Instance().GetGameSirState();
		else
			return 0;
#else
        return 0;
#endif
    }


    public float GetAxis(string axisName)
    {
		#if (UNITY_IOS || UNITY_ANDROID ) && GAMESIR && !UNITY_EDITOR
		if(mIsOpen)
        	return GamesirInput.Instance().GetAxis(axisName);
		else
			return 0;

#else
        return 0;
#endif
    }

    public bool GetButton(string buttonName)
    {
#if (UNITY_IOS || UNITY_ANDROID )&& GAMESIR &&  !UNITY_EDITOR
		if(mIsOpen)
        	return GamesirInput.Instance().GetButton(buttonName);
		else
			return false;
#else 
        return false;
#endif
    }

	public bool IsOpen
	{
		get{ return mIsOpen;}
	}

	public void Init()
	{

	}

	void Start()
	{
		//GamesirInput.Instance().SetDebug (true);
		#if(UNITY_IOS || UNITY_ANDROID)&& GAMESIR && !UNITY_EDITOR
		GamesirInput.Instance().SetIconLocation(IconLocation.BOTTOM_CENTER);
		GamesirInput.Instance().setHiddenConnectIcon(true);
		GamesirInput.Instance().onStart();
		mIsOpen = true;
		#else
		mIsOpen = false;
		#endif
	}
	
	public void StartSir()
	{
		
		#if(UNITY_IOS || UNITY_ANDROID) && GAMESIR && !UNITY_EDITOR
		if(!IsConnected())
			GamesirInput.Instance().AutoConnectToGCM();
		#endif
	}

	public void StopSir()
	{
#if(UNITY_IOS || UNITY_ANDROID) && GAMESIR && !UNITY_EDITOR
		if(IsConnected())
			GamesirInput.Instance().DisConnectGCM();
#endif
	}

    public bool IsConnected()
    {
		#if (UNITY_IOS || UNITY_ANDROID )&& GAMESIR &&  !UNITY_EDITOR
		return mIsOpen && GetGameSirState() == 3;
#else 
        return false;
#endif
    }

   
    void OnDestroy()
    {
#if (UNITY_IOS || UNITY_ANDROID ) && GAMESIR&&  !UNITY_EDITOR
		if(mIsOpen)
        	GamesirInput.Instance().OnDestory();
#endif
    }

    void OnApplicationQuit()
    {
#if (UNITY_IOS || UNITY_ANDROID )&& GAMESIR &&  !UNITY_EDITOR
		if(mIsOpen)
        	GamesirInput.Instance().OnQuit();
#endif
    }
}
