using System.Collections.Generic;
using UILib;
using UnityEngine;

public class XUIComboBox : XUIObject, IXUIComboBox
{
    Dictionary<int, string> items = new Dictionary<int, string>(); 
    Dictionary<int, GameObject> itemObjects = new Dictionary<int, GameObject>();

    public void ModuleInit()
    {
        itemtpl = transform.Find("Difficulty/DropList/ItemTpl").GetComponent<UISprite>();
        itemtpl.gameObject.SetActive(false);

        droplist = transform.Find("Difficulty/DropList");
        droplist.gameObject.SetActive(false);

        selecttext = transform.Find("Difficulty/SelectedText").GetComponent<UILabel>();

        Transform t = transform.Find("Difficulty/DropList/Close");

        if (t != null) close = t.GetComponent<UIPlayTween>();

        count = 0;
    }

    public void AddItem(string text, int value)
    {
        GameObject newItem = Instantiate(itemtpl.gameObject) as GameObject;
        newItem.SetActive(true);
        newItem.name = value.ToString();

        newItem.transform.parent = droplist;
        newItem.transform.localPosition = new Vector3(0, -count * itemtpl.height);
        newItem.transform.localScale = Vector3.one;

        count ++;

        UILabel lb = newItem.transform.Find("ItemText").GetComponent<UILabel>();
        lb.text = text;

        items.Add(value, text);
        itemObjects.Add(value, newItem);

        XUISprite sp = newItem.GetComponent<XUISprite>();
        sp.ID = (ulong)value;
        sp.RegisterSpriteClickEventHandler(OnItemSelect);
    }

    public GameObject GetItem(int value)
    {
        GameObject go = null;
        itemObjects.TryGetValue(value, out go);
        return go;
    }

    public void ClearItems()
    {
        foreach (KeyValuePair<int, GameObject> pair in itemObjects)
        {
            Destroy(pair.Value);
        }
        items.Clear();
        itemObjects.Clear();
        count = 0;
    }

    protected void OnItemSelect(IXUISprite sp)
    {
        if (_callback != null)
            _callback((int) sp.ID);

        if (close != null) close.Play(true);

        selecttext.text = items[(int) sp.ID];
    }

    public bool SelectItem(int value, bool withCallback)
    {
        string _text = null;
        if(items.TryGetValue(value, out _text))
        {
            selecttext.text = _text;
            if(withCallback && null != _callback)
            {
                _callback(value);
            }
            return true;
        }
        return false;
    }

    public void RegisterSpriteClickEventHandler(ComboboxClickEventHandler eventHandler)
    {
        _callback = eventHandler;
    }

    public void ResetState()
    {
        if(items.Count == 0)
        {
            selecttext.text = "";
        }

        droplist.gameObject.SetActive(false);

        foreach (KeyValuePair<int, string> pair in items)
        {
            selecttext.text = pair.Value;

            if (_callback != null) _callback(pair.Key);

            break;
        }
    }


    private UISprite itemtpl = null;
    private int count = 0;
    private Transform droplist = null;
    private UIPlayTween close = null;
    private UILabel selecttext = null;
    private ComboboxClickEventHandler _callback = null;
}
