using UnityEngine;
using System.Collections;

public class BattleAttackPress : MonoBehaviour
{

    private GameObject fx;
	// Use this for initialization
	void Start ()
	{
	    fx = transform.parent.Find("glow").gameObject;
	}
	
    void OnPress(bool state)
    {
        if(!state)
        {
            fx.SetActive(false);
        }
    }
}
