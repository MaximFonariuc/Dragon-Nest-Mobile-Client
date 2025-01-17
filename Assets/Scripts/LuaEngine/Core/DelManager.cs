//
// DelManager.cs
// Created by huailiang.peng on 2016-3-11 18:10:39
//


using UnityEngine;
using System.Collections;
using UILib;


public class DelManager
{
    public delegate void VoidDelegate();

    public delegate void BoolDelegate(bool state);

    public delegate void StringDelegate(string str);

    public delegate void FloatDelegate(float delta);

    public delegate void VectorDelegate(Vector2 delta);

    public delegate void ObjectDelegate(GameObject obj);

    public delegate void KeyCodeDelegate(KeyCode key);

    public delegate void GameObjDelegate(GameObject go);

    public delegate void BytesDelegate(byte[] bytes);

    public delegate void BtnDelegate(XUIButton btn);

    public delegate void SprDelegate(XUISprite spr);

    public static GameObjDelegate onGoClick = null;

    public static ButtonClickEventHandler fButtonDelegate = null;

    public static ButtonClickEventHandler sButtonDelegate = null;

    public static SpriteClickEventHandler sprClickEventHandler = null;

    public static void Clear()
    {
        fButtonDelegate = null;
        sButtonDelegate = null;
        onGoClick = null;
        sprClickEventHandler = null;
    }

}