using UILib;
using UnityEngine;
using XUtliPoolLib;
using System.Collections.Generic;

public class XUISpecLabelSymbol: XUIObject, IXUISpecLabelSymbol
{
    public XUILabel _Label;
    public XUISprite _Board;
    public XUISprite[] _SpriteList;
    public UISpriteAnimation[] _AnimationList;
    public int BoardWidthOffset;
    public int[] _SpriteMinHeight;
    public int[] _SpriteMaxHeight;
    
    public IXUILabel Label { get { return _Label; } }
    public IXUISprite Board { get { return _Board; } }
    public IXUISprite[] SpriteList { get { return _SpriteList; } }

    public void SetColor(Color color)
    {
        _Label.SetColor(color);
    }

    public Color GetColor()
    {
        return _Label.GetColor();
    }

    public void SetInputText(List<string> sprite)  // atlas null means is a label
    {
        int widthTol = 0;
        int index = 0;
        bool labelUsed = false;
        List<Transform> list = new List<Transform>();
        List<int> widthList = new List<int>();
        for(int i = 0; i < sprite.Count; i++)  // set ui and cal width
        {
            string[] strList = sprite[i].Split('|');
            if (strList.Length != 3)
                continue;
            if (string.IsNullOrEmpty(strList[0]))  //label
            {
                _Label.SetVisible(true);
                _Label.SetText(strList[1]);
                labelUsed = true;
                widthTol += _Label.spriteWidth;
                list.Add(_Label.gameObject.transform);
                widthList.Add(_Label.spriteWidth);
            }
            else
            {
                if (index < _SpriteList.Length && _SpriteList[index] != null)
                {
                    _SpriteList[index].SetVisible(true);
                    _SpriteList[index].SetSprite(strList[1], strList[0]);
                    if (index < _AnimationList.Length && _AnimationList[index] != null)
                    {
                        if (strList[2] == "1")
                        {
                            _AnimationList[index].enabled = true;
                            _AnimationList[index].namePrefix = strList[1];
                            _AnimationList[index].Reset();
                        }
                        else
                        {
                            _AnimationList[index].enabled = false;
                        }
                    }
                    _SpriteList[index].MakePixelPerfect();
                    int height = _SpriteList[index].spriteHeight;
                    int width = _SpriteList[index].spriteWidth;
                    int newHeight = _SpriteList[index].spriteHeight;
                    if (newHeight < _SpriteMinHeight[index])
                        newHeight = _SpriteMinHeight[index];
                    if (newHeight > _SpriteMaxHeight[index])
                        newHeight = _SpriteMaxHeight[index];
                    if (newHeight != height && height > 0)
                    {
                        _SpriteList[index].spriteWidth = width * newHeight / height;
                        _SpriteList[index].spriteHeight = newHeight;
                    }
                    widthTol += _SpriteList[index].spriteWidth;
                    list.Add(_SpriteList[index].gameObject.transform);
                    widthList.Add(_SpriteList[index].spriteWidth);
                }
                index++;
            }
        }
        if(Board.IsVisible())
            Board.spriteWidth = widthTol + BoardWidthOffset;
        _Label.SetVisible(labelUsed);
        for (int i = index; i < _SpriteList.Length; i++)
            SetSpriteVisibleFalse(i);
        int startX = -widthTol / 2;
        float y;
        index = 0;
        for(int i = 0; i < list.Count; i++)
        {
            y = list[i].localPosition.y;
            list[i].localPosition = new Vector3(startX + widthList[i] / 2f, y);
            startX += widthList[i];
        }
    }

    public void SetSpriteVisibleFalse(int index)
    {
        _SpriteList[index].SetVisible(false);
        if (index < _AnimationList.Length && _AnimationList[index] != null)
            _AnimationList[index].enabled = false;
    }

    public void Copy(IXUISpecLabelSymbol other)
    {
        gameObject.SetActive(other.IsVisible());
        _Label.SetVisible(other.IsVisible());
        _Label.SetText(other.Label.GetText());
        _Label.SetColor(other.Label.GetColor());
        _Label.transform.localPosition = other.Label.gameObject.transform.localPosition;
        _Board.SetVisible(other.Board.IsVisible());
        _Board.spriteWidth = other.Board.spriteWidth;
        for(int i = 0; i < _SpriteList.Length; i++)
        {
            _SpriteList[i].SetVisible(other.SpriteList[i].IsVisible());
            _SpriteList[i].SetSprite(other.SpriteList[i].spriteName, other.SpriteList[i].atlasPath, true);
            _SpriteList[i].spriteWidth = other.SpriteList[i].spriteWidth;
            _SpriteList[i].spriteHeight = other.SpriteList[i].spriteHeight;
            _SpriteList[i].transform.localPosition = other.SpriteList[i].transform.localPosition;
        }
    }
}
