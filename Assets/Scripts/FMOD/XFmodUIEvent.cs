using UnityEngine;
using System.Collections;

public class XFmodUIEvent : MonoBehaviour {

    static private GameObject _ui_audio = null;

    public string Name = "";
    public float Delay = 0;
    private float _start_time;
	void Start () {
        if (_ui_audio == null)
        {
            if (GameObject.Find("UIRoot") != null)
                _ui_audio = GameObject.Find("UIRoot").gameObject;
            else
                _ui_audio = GameObject.Find("UIRoot(Clone)").gameObject;
        }
        _start_time = UnityEngine.Time.time;
	}
	
	void FixedUpdate () {

        if (UnityEngine.Time.time - _start_time > Delay)
        {
            XFmod iFmod;
            if (_ui_audio != null)
            {
                iFmod = GetFmodComponent(_ui_audio);
                iFmod.PlayOneShot("event:/" + Name, Vector3.zero);
            }

            Destroy(this);
        }
	}

    public XFmod GetFmodComponent(GameObject go)
    {
        XFmod iFmod = go.GetComponent<XFmod>();

        if (iFmod == null)
            iFmod = go.AddComponent<XFmod>();
        
        return iFmod;
    }
}
