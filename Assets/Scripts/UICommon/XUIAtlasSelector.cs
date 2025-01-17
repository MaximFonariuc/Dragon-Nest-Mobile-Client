using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteInEditMode]
public class XUIAtlasSelector : MonoBehaviour {

    [Serializable]
    public class AtlasInfo
    {
        [SerializeField]
        public string atlasName;
        [SerializeField]
        public int startDepth = 0;
        [SerializeField]
        public List<UIWidget> list = new List<UIWidget>();
    }

    [SerializeField]
    public Dictionary<string, AtlasInfo> atlasInfos; 
    [SerializeField]
    public int widgetSize = 0;
         
	// Use this for initialization
	void Start () {
        Setup();
    }
    [ContextMenu("Execute")]
    public void Excute()
    {
        Setup();
    }

    private void Setup()
    {
        if (atlasInfos == null) atlasInfos = new Dictionary<string, AtlasInfo>();
        UISprite[] uiSprites = transform.GetComponentsInChildren<UISprite>();
        widgetSize = uiSprites.Length;
        UISprite widget;
        string nullAtlas = "NullAtlas";
        int i, length;
        for (i = 0,length = uiSprites.Length; i < length; i++)
        {
            widget = uiSprites[i];
            if (widget.atlas != null)
            {
                Insert(atlasInfos, widget.atlas.name, widget);
            }
            else
            {
                Insert(atlasInfos, nullAtlas, widget);
            }
        }
        string fontName = "Font";
        UILabel[] fonts = transform.GetComponentsInChildren<UILabel>();
        widgetSize += fonts.Length;
        for (i = 0, length = fonts.Length; i < length; i++)
        {
            if(fonts[i].bitmapFont != null)
            {
                Insert(atlasInfos, "bitmapFont:" + fontName, fonts[i]);
            }
            else
            {
                Insert(atlasInfos, fontName, fonts[i]);
            }
           
        }
        foreach (KeyValuePair<string, AtlasInfo> info in atlasInfos)
        {
            info.Value.list.Sort(SortCompare);
            if (info.Value.list.Count > 0)
            {
                info.Value.startDepth = info.Value.list[0].depth;
            }
        }
    }

    private int SortCompare(UIWidget widget1, UIWidget widget2 )
    {
        return widget1.depth - widget2.depth;
    }

    private void Insert(Dictionary<string ,AtlasInfo> infos ,string atlasName , UIWidget widget)
    {
        AtlasInfo info;
        if(!infos.TryGetValue(atlasName,out info))
        {
            info = new AtlasInfo();
            info.atlasName = atlasName;
            infos.Add(atlasName,info);
        }
        info.list.Add(widget);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
