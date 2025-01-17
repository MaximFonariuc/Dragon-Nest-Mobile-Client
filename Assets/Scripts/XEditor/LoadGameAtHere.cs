#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class LoadGameAtHere : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        if(null ==  XInterfaceMgr.singleton.GetInterface<IEntrance>(0))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
#endif