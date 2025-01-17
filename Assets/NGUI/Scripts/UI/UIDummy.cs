using UnityEngine;
using System.Collections;
using UILib;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Dummy")]
public sealed class UIDummy : UIWidget, UILib.IUIDummy
{
    //public bool CalcRenderQueue { get { return calcRenderQueue; } }
    public int RenderQueue { get { return drawCall != null ? drawCall.renderQueue : -1; } }

    public UILib.RefreshRenderQueueCb RefreshRenderQueue { get; set; }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {

    }

    public void LateUpdate()
    {
        if (RefreshRenderQueue != null && RenderQueue > 0)
        {
            RefreshRenderQueue(RenderQueue + 1);
        }
    }
    public void Reset()
    {
    }

    public IXUIPanel GetPanel()
    {
        return XUIPanel.GetPanel(panel);
    }
}
