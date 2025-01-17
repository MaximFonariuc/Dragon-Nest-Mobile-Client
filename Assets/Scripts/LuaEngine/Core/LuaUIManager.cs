using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;

public struct LuaNode
{
    public uint id;
    public string name;
    public GameObject go;
    public LuaDlg dlg;
    public List<string> childs;
};



public class LuaUIManager : ILuaUIManager
{
    private Dictionary<uint, LuaNode> m_stask = new Dictionary<uint, LuaNode>();

    private static LuaUIManager _single;
    public static LuaUIManager Instance
    {
        get
        {
            if (_single == null) _single = new LuaUIManager();
            return _single;
        }
    }


    public bool IsUIShowed()
    {
        bool show = false;
        foreach (var item in m_stask)
        {
            if (item.Value.go != null && item.Value.go.activeInHierarchy)
            {
                show = true;
                break;
            }
        }
        return show;
    }


    public bool Load(string name)
    {
        uint id = XCommon.singleton.XHash(name);
        if (!Find(id))
        {
            GameObject root = UICamera.mainCamera.gameObject;
            GameObject go = XResourceLoaderMgr.singleton.CreateFromPrefab(name) as GameObject;
            go.transform.parent = UICamera.mainCamera.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.layer = root.layer;
            go.name = name.Substring(name.LastIndexOf('/') + 1);
            LuaNode node = AttachLuaDlg(go, name, id);
            if (!m_stask.ContainsKey(id))
            {
                m_stask.Add(id, node);
                node.dlg.OnShow();
            }
            return true;
        }
        else
        {
            m_stask[id].go.SetActive(true);
            m_stask[id].dlg.OnShow();
        }
        return false;
    }

    private LuaNode AttachLuaDlg(GameObject go,string name,uint id)
    {
        LuaNode node = new LuaNode();
        LuaDlg luadlg = go.AddComponent<LuaDlg>();
        node.dlg = luadlg;
        node.go = go;
        node.name = name;
        node.id = id;
        return node;
    }

    private GameObject SetupChild(Transform parent, string child)
    {
        uint id = XCommon.singleton.XHash(child);
        bool exist = Find(id);
        GameObject go = exist ? m_stask[id].go : XResourceLoaderMgr.singleton.CreateFromPrefab(child) as GameObject;
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        if (!exist)
        {
            go.name = child.Substring(child.LastIndexOf('/') + 1);
            LuaNode node = AttachLuaDlg(go, child, id);
            if (!m_stask.ContainsKey(id))
            {
                m_stask.Add(id, node);
                node.dlg.OnShow();
            }
        }
        else
        {
            m_stask[id].go.SetActive(true);
            m_stask[id].dlg.OnShow();
        }
        return go;
    }


    public GameObject AttachHandler(string root, string child)
    {
        uint root_id = XCommon.singleton.XHash(root);
        if (m_stask.ContainsKey(root_id))
        {
            var childs = m_stask[root_id].childs;
            if (childs == null) childs = new List<string>();
            if (!childs.Contains(child)) childs.Add(child);
            GameObject go = m_stask[root_id].go;
            if (go != null)
            {
                Transform t = go.transform.Find("Bg/Handler");
                if (t != null)
                {
                    return SetupChild(t, child);
                }
            }
            else
            {
                XDebug.singleton.AddErrorLog("cache task go is nil");
            }
        }
        else
        {
            XDebug.singleton.AddErrorLog("There is no such root is stack ", root, " child: ", child);
        }
        return null;
    }

    public void AttachHandlers(string root, params string[] childs)
    {
        for (int i = 0; i < childs.Length; i++)
        {
            AttachHandler(root, childs[i]);
        }
    }


    public void DestroyChilds(string root)
    {
        uint root_id = XCommon.singleton.XHash(root);
        DestroyChilds(root_id);
    }


    private void DestroyChilds(uint root)
    {
        if (m_stask.ContainsKey(root))
        {
            var childs = m_stask[root].childs;
            if (childs != null)
            {
                for (int i = 0; i < childs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(childs[i])) Destroy(childs[i]);
                }
            }
        }
    }

    /// <summary>
    /// 遍历所有 效率较低
    /// </summary>
    public void DetchHandler(string child)
    {
        foreach (var item in m_stask)
        {
            if (item.Value.childs != null)
            {
                item.Value.childs.RemoveAll(x => x == child);
            }
        }
    }

    public void DetchHandler(string root, string child)
    {
        uint root_id = XCommon.singleton.XHash(root);
        if (m_stask.ContainsKey(root_id))
        {
            var childs = m_stask[root_id].childs;
            if (childs != null)
            {
                childs.RemoveAll(x => x == child);
            }
        }
    }


    public bool Hide(string name)
    {
        uint id = XCommon.singleton.XHash(name);
        return IDHide(id);
    }


    public GameObject GetDlgObj(string name)
    {
        uint code = XCommon.singleton.XHash(name);
        if (m_stask.ContainsKey(code))
        {
            return m_stask[code].go;
        }
        return null;
    }

    public bool IDHide(uint id)
    {
        if (m_stask.Count > 0 && Find(id))
        {
            LuaNode node = m_stask[id];
            if (node.go != null)
                node.go.SetActive(false);
            node.dlg.OnHide();
            return true;
        }
        return true;
    }


    public bool Destroy(string name)
    {
        uint id = XCommon.singleton.XHash(name);
        return IDDestroy(id);
    }

    public bool IDDestroy(uint id)
    {
        if (m_stask.Count > 0 && Find(id))
        {
            LuaNode node = m_stask[id];
            //先删子节点 再删自身
            DestroyChilds(node.name);
            MonoBehaviour.Destroy(node.go);
            if (m_stask.ContainsKey(id)) m_stask.Remove(id);
            return true;
        }
        return false;
    }

    private void DestroyWithoutChild(uint id)
    {
        if (m_stask.Count > 0 && Find(id))
        {
            LuaNode node = m_stask[id];
            MonoBehaviour.Destroy(node.go);
        }
    }

    public void Clear()
    {
        List<uint> list = new List<uint>(m_stask.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            DestroyWithoutChild(list[i]);
        }
        m_stask.Clear();
    }



    private bool Find(uint id)
    {
        if (m_stask.ContainsKey(id))
        {
            if (m_stask[id].go != null)
            {
                return true;
            }
            else
            {
                XDebug.singleton.AddGreenLog("remove id: "+id);
                m_stask.Remove(id);
                return false;
            }
        }
        return false;
    }


}
