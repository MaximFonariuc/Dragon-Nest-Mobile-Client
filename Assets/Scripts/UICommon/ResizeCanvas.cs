using System;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class ResizeCanvas : MonoBehaviour {

	// Use this for initialization
	public void Awake () {
        
	}

    public void Start()
    {
        UIRoot rt = NGUITools.FindInParents<UIRoot>(gameObject);

        UISprite sp = GetComponent<UISprite>();

        sp.height = Math.Min(rt.base_ui_height, rt.manualHeight);
    }
	
	
}
