using UnityEngine;
using System.Collections;
using XUtliPoolLib;

[ExecuteInEditMode]
public class ControlParticle : MonoBehaviour, IControlParticle
{

    public int renderQueue = 3500;
    public bool runOnlyOnce = false;
    public bool autoFindWidget = false;
    public UIWidget widget;
    public int renderQueueOffset = 0;

    [System.NonSerialized]
    UIPanel panel;
    [System.NonSerialized]
    Material matCache;
    [System.NonSerialized]
    private Renderer renderCache;
    void Awake()
    {
        renderCache = GetComponent<Renderer>();
        if (Application.isPlaying && renderCache != null)
        {
            matCache = GetComponent<Renderer>().material;
            if (matCache != null)
            {
                matCache.renderQueue = 3500;//ui defalut
            }
        }
    }
    void Start()
    {
        RefreshRenderQueue(false);
    }
    public void SetWidget(GameObject go)
    {
        if (go != null)
        {
            UIWidget tWidget = go.GetComponent<UIWidget>();
            if (tWidget != null)
            {
                widget = tWidget;
            }
        }
    }
    public void RefreshRenderQueue(bool resetWidget)
    {
        if (autoFindWidget|| resetWidget)
            widget = null;
        if (autoFindWidget && widget == null)
        {
            Transform _parent = transform;
            while (_parent != null)
            {
                UIWidget _widget = _parent.GetComponent<UIWidget>();
                if (_widget != null)
                {
                    widget = _widget;
                    break;
                }
                _parent = _parent.parent;
            }
        }
    }
    [ContextMenu("test")]
    void Test()
    {
        print("widget:" + (widget == null));
        if (widget != null)
        {
            print(widget.name+" panel: "+(widget.panel==null));
            if(widget.panel!=null)
            {
                print("usemerge:" + widget.panel.useMerge+" dc count:"+widget.panel.drawCalls.Count);
            }
        }
    }

    void InnerUpdate(Material mat)
    {
        if (mat != null)
        {
            if (widget != null && widget.drawCall != null)
            {
                renderQueue = widget.drawCall.renderQueue+renderQueueOffset;
            }
            else if (widget != null && widget.panel != null && widget.panel.useMerge  && widget.panel.drawCalls.Count > 0)
            {
                renderQueue = widget.panel.drawCalls[0].renderQueue+renderQueueOffset;
            }
            if (mat.renderQueue != renderQueue)
            {
                mat.renderQueue = renderQueue + renderQueueOffset;
                renderQueue = mat.renderQueue;
            }
            UIPanel newPanel = widget != null ? widget.panel : null;
            if (newPanel != null && panel != newPanel)
            {
                panel = newPanel;
                if (panel.root != null && panel.clipping != UIDrawCall.Clipping.None)
                {
                    Vector4 clipRange = panel.finalClipRegion;
                    Vector3 panelPos = panel.root.transform.InverseTransformPoint(panel.transform.position);
                    clipRange.x += panelPos.x;
                    clipRange.y += panelPos.y;
                    float left = (clipRange.x - clipRange.z * 0.5f) / 568;
                    float right = (clipRange.x + clipRange.z * 0.5f) / 568;
                    float top = (clipRange.y + clipRange.w * 0.5f) / 320;
                    float down = (clipRange.y - clipRange.w * 0.5f) / 320;
                    mat.SetVector("_ClipRange0", new Vector4(left, down, right, top));
                }
                else
                {
                    mat.SetVector("_ClipRange0", new Vector4(-1, -1, 1, 1));
                }
            }
        }        
    }

    void Update()
    {
        if (renderCache != null)
        {
            if (Application.isPlaying)
            {
                InnerUpdate(matCache);
            }
            else
            {
                InnerUpdate(renderCache.sharedMaterial);
            }
        }
        if (runOnlyOnce && Application.isPlaying)
        {
            this.enabled = false;
        }
    }

    //public void SetHierarchy(GameObject go)
    //{
        
    //}

    //public void SetWidget(GameObject go)
    //{
    //    if(go != null)
    //    {
    //        UIWidget tWidget = go.GetComponent<UIWidget>();
    //        if (tWidget != null)
    //        {
    //            widget = tWidget;
    //        }
    //    }
    //}

}
