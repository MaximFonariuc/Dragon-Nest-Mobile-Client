using UILib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using XUtliPoolLib;

public abstract class XDataBase
{
    public virtual void Recycle()
    {
    }

    public virtual void Init()
    {
    }
}
public class XDataPool<T> where T : XDataBase, new()
{
    private static Queue<T> _pool = new Queue<T>();

    public static T GetData()
    {
        if (_pool.Count > 0)
        {
            T t = _pool.Dequeue();

            t.Init();
            return t;
        }
        else
        {
            T t = new T();
            t.Init();
            return t;
        }
    }

    public static void Recycle(T data)
    {
        _pool.Enqueue(data);
    }
}

public class XUILabelSymbol : XUIObject, IXUILabelSymbol
{
    HyperLinkClickEventHandler GuildClickHandler;
    HyperLinkClickEventHandler DragonGuildClickHandler;
    HyperLinkClickEventHandler TeamClickHandler;
    HyperLinkClickEventHandler ItemClickHandler;
    HyperLinkClickEventHandler NameClickHandler;
    HyperLinkClickEventHandler PkClickHandler;
    HyperLinkClickEventHandler SpectateHandler;
    HyperLinkClickEventHandler UIClickHandler;
    HyperLinkClickEventHandler DefaultClickHandler;
    LabelSymbolClickEventHandler SymbolClickHandler;

    enum Type
    {
        LST_NONE = -1,
        LST_IMAGE,
        LST_GUILD,
        LST_TEAM,
        LST_ITEM,
        LST_NAME,
        LST_PK,
        LST_UI,
        LST_SPECTATE,
        LST_ANIMATION,
        LST_DRAGON_GUILD
    }

    class StringData : XDataBase
    {
        public Type type { get; set; }
        public int startIndex { get { return m_StartIndex; } }
        public int length { get { return m_Length; } }
        public string str { get { return m_Str; } }

        private int m_StartIndex;
        private int m_Length;
        private string m_Str;

        public bool Set(string s, int start_index, int len)
        {
            m_Str = s;
            m_StartIndex = start_index;
            m_Length = len;

            if (type == Type.LST_NONE)
                return true;
            int paramCount = 1;
            int index = s.IndexOf(_EscapedSeparator, start_index, len);
            while (index != -1)
            {
                ++paramCount;
                index = s.IndexOf(_EscapedSeparator, index + 1, start_index + len - index - 1);
            }
            if (type == Type.LST_IMAGE && paramCount == 2)  // atlas + sprite
                return true;
            if ((type == Type.LST_GUILD || type == Type.LST_PK || type == Type.LST_DRAGON_GUILD) && paramCount == 2)   // name + id
                return true;
            if ((type == Type.LST_TEAM) && paramCount == 3)   // name + teamid + expid
                return true;
            if ((type == Type.LST_UI) && paramCount >= 2)    // name + sysid + [param, param, ...]
                return true;
            if ((type == Type.LST_SPECTATE) && paramCount == 3)//name + liveid + type
                return true;
            if ((type == Type.LST_ITEM || type == Type.LST_NAME) && paramCount == 3)    // name + color + id
                return true;
            if (type == Type.LST_ANIMATION && paramCount == 3)  //atlas + sprite + frame rate
                return true;
            return false;
        }

        public override void Recycle()
        {
            base.Recycle();
            XDataPool<StringData>.Recycle(this);
        }
    }

    class SymbolData : XDataBase
    {
        public Type type;
        public int startIndex;
        public int endIndex;
        public virtual bool OnClick(int hitIndex, XUILabelSymbol labelSymbol) { return false; }
        public static bool IsImage(Type type) { return type == Type.LST_IMAGE; }
        public static bool IsAnimation(Type type) { return type == Type.LST_ANIMATION; }
        public static bool IsHyperLink(Type type) { return type != Type.LST_NONE && type != Type.LST_IMAGE; }
    }

    class ImageSymbolData : SymbolData
    {
        public UISprite sprite;
        public ImageSymbolData()
        {
            type = Type.LST_IMAGE;
        }

        // return symbolstring
        public string SetSprite(XUILabelSymbol labelSymbol, string str, int startIndex, int length, ref int usedSprite)
        {
            int separate = str.IndexOf(_EscapedSeparator, startIndex, length);
            if (separate != -1)
            {
                string atlasName = str.Substring(startIndex, separate - startIndex);
                string spriteName = str.Substring(separate + 1, length - (separate - startIndex) - 1);
                if (usedSprite >= labelSymbol.SpriteList.Count)
                {
                    sprite = NGUITools.AddChild<UISprite>(labelSymbol.gameObject);
                    //sprite.autoFindPanel = false;
                    sprite.gameObject.AddComponent<XUISprite>();
                    sprite.depth = labelSymbol.m_Label.depth;
                    sprite.gameObject.layer = labelSymbol.gameObject.layer;
                    sprite.pivot = UIWidget.Pivot.Left;
                    labelSymbol.SpriteList.Add(sprite);
                    ++usedSprite;
                    //sprite.panel = m_Label.panel;
                }
                else
                {
                    sprite = labelSymbol.SpriteList[usedSprite++];
                    sprite.gameObject.SetActive(true);
                }

                GameObject obj = XResourceLoaderMgr.singleton.GetSharedResource<GameObject>("atlas/UI/" + atlasName, ".prefab");
                sprite.atlas = obj == null ? null : obj.GetComponent<UIAtlas>();

                sprite.spriteName = spriteName;
                sprite.MakePixelPerfect();
                // 调整图片，不超过上限
                if (labelSymbol.MaxImageHeight > 0 && sprite.height > labelSymbol.MaxImageHeight)
                {
                    int w = sprite.width * labelSymbol.MaxImageHeight / sprite.height;
                    int h = labelSymbol.MaxImageHeight;

                    if ((w & 1) == 1) ++w;
                    if ((h & 1) == 1) ++h;

                    sprite.width = w;
                    sprite.height = h;
                }

                int width = sprite.width;

                return labelSymbol._GetSpaceWithSameWidth(width);
            }
            return null;
        }

        public override void Recycle()
        {
            base.Recycle();
            XDataPool<ImageSymbolData>.Recycle(this);
        }
    }

    class AnimationSymbolData : SymbolData
    {
        public UISprite sprite;
        public UISpriteAnimation animation;
        public AnimationSymbolData()
        {
            type = Type.LST_ANIMATION;
        }

        public string SetSprite(XUILabelSymbol labelSymbol, string str, int startIndex, int length, ref int usedAnimation)
        {
            int separate = str.IndexOf(_EscapedSeparator, startIndex, length);
            int separate2 = str.IndexOf(_EscapedSeparator, separate + 1, length + startIndex - separate);
            if (separate != -1)
            {
                string atlasName = str.Substring(startIndex, separate - startIndex);
                string spriteName = str.Substring(separate + 1, separate2 - separate - 1);
                int frameRate = int.Parse(str.Substring(separate2 + 1, length + startIndex - separate2 - 1));
                if (usedAnimation >= labelSymbol.AnimationList.Count)
                {
                    sprite = NGUITools.AddChild<UISprite>(labelSymbol.gameObject);
                    animation = sprite.gameObject.AddComponent<UISpriteAnimation>();
                    sprite.gameObject.AddComponent<XUISprite>();
                    //sprite.autoFindPanel = false;
                    sprite.depth = labelSymbol.m_Label.depth;
                    sprite.gameObject.layer = labelSymbol.gameObject.layer;
                    sprite.pivot = UIWidget.Pivot.Left;
                    labelSymbol.AnimationList.Add(sprite);
                    ++usedAnimation;
                    //sprite.panel = m_Label.panel;
                }
                else
                {
                    sprite = labelSymbol.AnimationList[usedAnimation++];
                    animation = sprite.gameObject.GetComponent<UISpriteAnimation>();
                    sprite.gameObject.SetActive(true);
                }

                GameObject obj = XResourceLoaderMgr.singleton.GetSharedResource<GameObject>("atlas/UI/" + atlasName, ".prefab");
                sprite.atlas = obj == null ? null : obj.GetComponent<UIAtlas>();

                //sprite.spriteName = spriteName;
                animation.namePrefix = spriteName;
                animation.framesPerSecond = frameRate;
                animation.Reset();
                sprite.MakePixelPerfect();
                // 调整图片，不超过上限
                int targetHeight = 0;

                if (labelSymbol.MaxImageHeight > 0 && sprite.height > labelSymbol.MaxImageHeight)
                    targetHeight = labelSymbol.MaxImageHeight;
                else if (labelSymbol.MinImageHeight > 0 && sprite.height < labelSymbol.MinImageHeight)
                    targetHeight = labelSymbol.MinImageHeight;

                if(targetHeight != 0)
                {
                    int w = sprite.width * targetHeight / sprite.height;
                    int h = targetHeight;

                    if ((w & 1) == 1) ++w;
                    if ((h & 1) == 1) ++h;

                    sprite.width = w;
                    sprite.height = h;
                }

                int width = sprite.width;

                return labelSymbol._GetSpaceWithSameWidth(width);
            }
            return null;
        }

        public override void Recycle()
        {
            base.Recycle();
            XDataPool<AnimationSymbolData>.Recycle(this);
        }
    }

    abstract class HyperLinkSymbolData : SymbolData
    {
        public string param;
        public virtual string Set(string wholeStr, int startIndex, int length, int separateIndex) { return null; }

        protected virtual bool OnClick(XUILabelSymbol labelSymbol) { return false; }
        public override bool OnClick(int hitIndex, XUILabelSymbol labelSymbol)
        {
            if (startIndex < hitIndex && endIndex > hitIndex)
            {
                XDebug.singleton.AddLog(param);
                return OnClick(labelSymbol);
            }
            return false;
        }

        protected static string _MakeHyperLinkString(string mainStr, Type type, string color = "61eee6")
        {
            return string.Format(" [{0}][u]{1}[/u][-] ", color, mainStr);
        }

        public static bool CreateHyperLinkSymbolData(StringData input, out SymbolData data, out string symbolString)
        {
            int separate = input.str.IndexOf(_EscapedSeparator, input.startIndex, input.length);

            if (separate == -1)
            {
                data = null;
                symbolString = null;
                return false;
            }

            HyperLinkSymbolData hyperLinkSymbolData = null;
            switch (input.type)
            {
                case Type.LST_GUILD:
                    hyperLinkSymbolData = XDataPool<GuildHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_DRAGON_GUILD:
                    hyperLinkSymbolData = XDataPool<DragonGuildHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_TEAM:
                    hyperLinkSymbolData = XDataPool<TeamHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_ITEM:
                    hyperLinkSymbolData = XDataPool<ItemHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_NAME:
                    hyperLinkSymbolData = XDataPool<NameHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_PK:
                    hyperLinkSymbolData = XDataPool<PkHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_UI:
                    hyperLinkSymbolData = XDataPool<UIHyperLinkSymbolData>.GetData();
                    break;
                case Type.LST_SPECTATE:
                      hyperLinkSymbolData = XDataPool<SpectateHyperLinkSymbolData>.GetData();
                    break;
            }
            symbolString = hyperLinkSymbolData.Set(input.str, input.startIndex, input.length, separate);
            if (symbolString == null)
            {
                data = null;
                return false;
            }
            data = hyperLinkSymbolData;
            return true;
        }
    }
    abstract class NormalHyperLinkSymbolData : HyperLinkSymbolData
    {
        public override string Set(string wholeStr, int startIndex, int length, int separateIndex)
        {
            param = wholeStr.Substring(separateIndex + 1, startIndex + length - separateIndex - 1);
            return _MakeHyperLinkString(wholeStr.Substring(startIndex, separateIndex - startIndex), type);
        }
    }

    abstract class ColorHyperLinkSymbolData : HyperLinkSymbolData
    {
        public override string Set(string wholeStr, int startIndex, int length, int separateIndex)
        {
            int separate2 = wholeStr.IndexOf(_EscapedSeparator, separateIndex + 1, startIndex + length - separateIndex - 1);
            if (separate2 == -1)
            {
                XDebug.singleton.AddErrorLog("The second separator is missing: ", wholeStr);
                return null;
            }
            string color = wholeStr.Substring(separateIndex + 1, separate2 - separateIndex - 1);
            string normalStr = wholeStr.Substring(startIndex, separateIndex - startIndex);

            s_TempSB.Length = 0;
            s_TempSB.Append(wholeStr, separate2 + 1, startIndex + length - separate2 - 1);
            s_TempSB.Append(_EscapedSeparator);
            s_TempSB.Append(normalStr);
            param = s_TempSB.ToString();
            return _MakeHyperLinkString(normalStr, type, color);
        }
    }

    class GuildHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
        public GuildHyperLinkSymbolData()
        {
            type = Type.LST_GUILD;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.GuildClickHandler != null)
            {
                labelSymbol.GuildClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<GuildHyperLinkSymbolData>.Recycle(this);
        }
    }

    class DragonGuildHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
         public DragonGuildHyperLinkSymbolData()
        {
            type = Type.LST_DRAGON_GUILD;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.DragonGuildClickHandler != null)
            {
                labelSymbol.DragonGuildClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<DragonGuildHyperLinkSymbolData>.Recycle(this);
        }
    }
    class TeamHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
        public TeamHyperLinkSymbolData()
        {
            type = Type.LST_TEAM;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.TeamClickHandler != null)
            {
                labelSymbol.TeamClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<TeamHyperLinkSymbolData>.Recycle(this);
        }
    }
    class ItemHyperLinkSymbolData : ColorHyperLinkSymbolData
    {
        public ItemHyperLinkSymbolData()
        {
            type = Type.LST_ITEM;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.ItemClickHandler != null)
            {
                labelSymbol.ItemClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<ItemHyperLinkSymbolData>.Recycle(this);
        }
    }
    class NameHyperLinkSymbolData : ColorHyperLinkSymbolData
    {
        public NameHyperLinkSymbolData()
        {
            type = Type.LST_NAME;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.NameClickHandler != null)
            {
                labelSymbol.NameClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<NameHyperLinkSymbolData>.Recycle(this);
        }
    }
    class PkHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
        public PkHyperLinkSymbolData()
        {
            type = Type.LST_PK;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.PkClickHandler != null)
            {
                labelSymbol.PkClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<PkHyperLinkSymbolData>.Recycle(this);
        }
    }
    class UIHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
        public UIHyperLinkSymbolData()
        {
            type = Type.LST_UI;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.UIClickHandler != null)
            {
                labelSymbol.UIClickHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<UIHyperLinkSymbolData>.Recycle(this);
        }
    }
    class SpectateHyperLinkSymbolData : NormalHyperLinkSymbolData
    {
        public SpectateHyperLinkSymbolData()
        {
            type = Type.LST_SPECTATE;
        }
        protected override bool OnClick(XUILabelSymbol labelSymbol)
        {
            if (labelSymbol.SpectateHandler != null)
            {
                labelSymbol.SpectateHandler(param);
                return true;
            }
            return false;
        }
        public override void Recycle()
        {
            base.Recycle();
            XDataPool<SpectateHyperLinkSymbolData>.Recycle(this);
        }
    }


    protected UILabel m_Label;
    protected BoxCollider m_collider;

    public int MaxImageHeight = 0;
    public int MinImageHeight = 0;
    public int ImageHeightAdjustment = 0;
    public XUISprite BoardSprite;
    public int MinBoardWidth;
    public int MaxBoardWidth;
    public int BoardHeightOffset = 0;
    public int BoardWidthOffset = 0;
    public bool bAutoDisableBoxCollider = false;

    StringBuilder m_OutputSB = new StringBuilder();
    //StringBuilder m_ResSB = new StringBuilder();

    static StringBuilder s_TempSB = new StringBuilder();

    List<StringData> m_StringData = new List<StringData>();
    List<SymbolData> m_Symbols = new List<SymbolData>();

    List<UISprite> m_SpriteList = new List<UISprite>();
    List<UISprite> m_AnimationList = new List<UISprite>();
    public List<UISprite> SpriteList { get { return m_SpriteList; } }
    public List<UISprite> AnimationList { get { return m_AnimationList; } }

    bool m_bBoxColliderCreator = false;

    private string _InputText;
    public string InputText
    {
        set
        {
            _InputText = value;
            if (_InputText == null)
                _InputText = string.Empty;
            Parse();
            SetBoard();
        }
    }

    public IXUISprite IBoardSprite
    {
        get
        {
            return BoardSprite as IXUISprite;
        }
    }

    public void SetBoard()
    {
        //Vector3 size = m_Label.CalculateBounds().size;
        //XDebug.singleton.AddLog("SetBoard");
        Vector2 textSize = NGUIText.CalculatePrintedSize(m_Label.text);
        textSize.x += BoardWidthOffset;

        if (BoardSprite != null)
        {
            if (MinBoardWidth != 0 && MaxBoardWidth != 0 && MaxBoardWidth >= MinBoardWidth)
            {
                if (textSize.x <= MinBoardWidth)
                    BoardSprite.spriteWidth = MinBoardWidth;
                else if (textSize.x > MaxBoardWidth)
                    BoardSprite.spriteWidth = MaxBoardWidth;
                else
                    BoardSprite.spriteWidth = (int)textSize.x;
            }
            else
                BoardSprite.spriteWidth = (int)textSize.x;

            BoardSprite.spriteHeight = (int)textSize.y + 20 + BoardHeightOffset;
        }
    }


    protected override void OnAwake()
    {
        base.OnAwake();
        m_Label = GetComponent<UILabel>();
        m_Label.ProcessText();
        m_collider = m_Label.DefaultBoxCollider;
        if (null == m_Label)
        {
            Debug.LogError("null == m_Label");
        }
        if (m_collider != null)
        {
            UIEventListener listener = UIEventListener.Get(this.gameObject);
            listener.onClick -= OnSymbolClick;
            listener.onClick += OnSymbolClick;
        }
    }

    void _CheckAttachments()
    {
        for (int i = m_SpriteList.Count - 1; i >= 0; --i)
        {
            if (m_SpriteList[i] == null)
            {
                XDebug.singleton.AddErrorLog(string.Format("m_SpriteList[{0}] == null, while m_SpriteList.Count = {1}", i, m_SpriteList.Count));
                m_SpriteList.RemoveAt(i);
            }
        }
        for (int i = m_AnimationList.Count - 1; i >= 0; --i)
        {
            if (m_AnimationList[i] == null)
            {
                XDebug.singleton.AddErrorLog(string.Format("m_AnimationList[{0}] == null, while m_AnimationList.Count = {1}", i, m_AnimationList.Count));
                m_AnimationList.RemoveAt(i);
            }
        }
    }


    public void UpdateDepth(int depth)
    {
        for(int i=0;i<m_SpriteList.Count;i++)
        {
            if(m_SpriteList[i]!=null) m_SpriteList[i].depth = depth;
        }
        for(int i =0;i<m_AnimationList.Count;i++)
        {
            if (m_AnimationList[i] != null) m_AnimationList[i].depth = depth;
        }
    }

    void OnSymbolClick(GameObject go)
    {
        bool bClicked = false;
        do
        {
            if (m_Symbols == null || m_Symbols.Count == 0)
                break;
            int index = m_Label.GetCharacterIndexAtPosition(UICamera.lastHit.point);
            if (index < 0)
                break;

            for (int i = 0; i < m_Symbols.Count; ++i)
            {
                SymbolData data = m_Symbols[i];
                bClicked |= data.OnClick(index, this);
            }
        }
        while (false);

        if (!bClicked && DefaultClickHandler != null)
        {
            DefaultClickHandler(null);
        }

        if (!bClicked && SymbolClickHandler != null)
            SymbolClickHandler(this);

    }

    static BetterList<Vector3> verts = new BetterList<Vector3>();
    static BetterList<int> indices = new BetterList<int>();
    public void Parse()
    {
        m_OutputSB.Length = 0;
        _Separate();
        _Parse();

        if (bAutoDisableBoxCollider && m_collider != null)
            m_collider.enabled = false;

        if (m_Symbols.Count <= 0)
            return;

        verts.Clear();
        indices.Clear();
        NGUIText.PrintCharacterPositions(m_Label.processedText, verts, indices);

        if (verts.size <= 0)
            return;
        m_Label.ApplyOffset(verts, 0);
        int j = 0;

        for (int i = 0; i < m_Symbols.Count; ++i)
        {
            SymbolData data = m_Symbols[i];
            if (SymbolData.IsImage(data.type) || SymbolData.IsAnimation(data.type))
            {
                UISprite sprite = null;
                if (SymbolData.IsImage(data.type))
                {
                    ImageSymbolData imageData = data as ImageSymbolData;
                    sprite = imageData.sprite;
                }
                else
                {
                    AnimationSymbolData animationData = data as AnimationSymbolData;
                    sprite = animationData.sprite;
                }

                for (; j < indices.size; ++j)
                {
                    if (indices[j] == data.startIndex)
                    {
                        sprite.transform.localPosition = verts[j] + new Vector3(0, ImageHeightAdjustment);
                        break;
                    }
                }
                if (j == indices.size && sprite != null)
                {
                    sprite.gameObject.SetActive(false);
                }
            }
            else if (SymbolData.IsHyperLink(data.type))
            {
                if (m_collider == null)
                {
                    m_collider = NGUITools.AddWidgetCollider(gameObject, m_Label, false);
                    UIEventListener listener = UIEventListener.Get(this.gameObject);
                    listener.onClick -= OnSymbolClick;
                    listener.onClick += OnSymbolClick;
                    m_bBoxColliderCreator = true;
                }
                else
                {
                    m_collider.enabled = true;
                    if (m_bBoxColliderCreator)
                    {
                        NGUITools.UpdateWidgetCollider(m_Label, m_collider, false);
                    }
                }
            }
        }

    }

    int _FindClosingBracket(string s, int startIndex)
    {
        int length = s.Length;
        int stack = 0;
        for (int i = startIndex; i < length; ++i)
        {
            if (s[i] == _EscapedLeftBracket)
                ++stack;
            else if (s[i] == _EscapedRightBracket)
            {
                if (--stack < 0)
                    return i;
            }
        }
        return -1;
    }

    private void _Parse()
    {
        for (int i = 0; i < m_Symbols.Count; ++i)
        {
            m_Symbols[i].Recycle();
        }
        m_Symbols.Clear();

        m_Label.text = "";
        m_Label.ProcessText();
        m_Label.UpdateDefaultPrintedSize();
        if (_UniSpaceWidth <= 0.0f)
        {
            //_UniSpaceWidth = NGUIText.CalculatePrintedSize(new string(_UniSpace, 1), 1000).x;
            _UniSpaceWidth = NGUIText.CalculatePrintedSize(" ", 1000).x;
            if (_UniSpaceWidth <= 0.0f)
            {
                XDebug.singleton.AddErrorLog("_SpaceWidth = ", _UniSpaceWidth.ToString(), " gameobject = ", gameObject.ToString(), "; Content: ", _InputText);
            }
        }

        _CheckAttachments();
        string res = "";
        int lineStartIndex = 0;
        int usedSprite = 0;
        int usedAnimation = 0;
        int lineWidth = m_Label.width;
        if (m_Label.overflowMethod == UILabel.Overflow.ResizeFreely)
            lineWidth = 10000;
        string symbolStr = null;
        SymbolData symbolData = null;
        StringData data;
        for (int ii = 0; ii < m_StringData.Count; ++ii)
        {
            // 已超出文本框的范围，直接退出
            if (lineStartIndex != 0 && lineStartIndex >= res.Length)
                break;

            data = m_StringData[ii];
            if (SymbolData.IsImage(data.type))
            {
                ImageSymbolData imageData = XDataPool<ImageSymbolData>.GetData();
                symbolStr = imageData.SetSprite(this, data.str, data.startIndex, data.length, ref usedSprite);
                if (symbolStr == null)
                {
                    imageData.Recycle();
                    continue;
                }
                else
                {
                    symbolData = imageData;
                }
            }
            else if(SymbolData.IsAnimation(data.type))
            {
                AnimationSymbolData animationData = XDataPool<AnimationSymbolData>.GetData();
                symbolStr = animationData.SetSprite(this, data.str, data.startIndex, data.length, ref usedAnimation);
                if (symbolStr == null)
                {
                    animationData.Recycle();
                    continue;
                }
                else
                {
                    symbolData = animationData;
                }
            }
            // hyper links
            else if (SymbolData.IsHyperLink(data.type))
            {
                HyperLinkSymbolData.CreateHyperLinkSymbolData(data, out symbolData, out symbolStr);
            }

            if (symbolStr != null)
            {
                s_TempSB.Length = 0;
                s_TempSB.Append(res, lineStartIndex, res.Length - lineStartIndex);
                s_TempSB.Append(symbolStr);

                int textWidth = Mathf.CeilToInt(
                    NGUIText.CalculatePrintedSize(
                    s_TempSB.ToString(),
                    lineWidth + 1000).x);

                if (textWidth > lineWidth)
                {
                    symbolData.startIndex = res.Length + 1;
                    lineStartIndex = res.Length + 1;
                    s_TempSB.Length = 0;
                    s_TempSB.Append(res);
                    s_TempSB.Append('\n');
                    s_TempSB.Append(symbolStr);
                    res = s_TempSB.ToString();
                }
                else
                {
                    symbolData.startIndex = res.Length;
                    s_TempSB.Length = 0;
                    s_TempSB.Append(res);
                    s_TempSB.Append(symbolStr);
                    res = s_TempSB.ToString();
                }

                symbolData.endIndex = symbolData.startIndex + symbolStr.Length;

                m_Symbols.Add(symbolData);
                //m_Label.text = res;
                //res = m_Label.processedText;
                NGUIText.WrapText(res, out res, false);
                symbolStr = null;
                continue;
            }
            //m_Label.text = res + data.str;
            //res = m_Label.processedText;
            s_TempSB.Length = 0;
            s_TempSB.Append(res);
            s_TempSB.Append(data.str, data.startIndex, data.length);
            NGUIText.WrapText(s_TempSB.ToString(), out res, false);
            lineStartIndex = res.LastIndexOf('\n');
            if (lineStartIndex == -1)
                lineStartIndex = 0;
            else
                ++lineStartIndex;

        }
        for (; usedSprite < m_SpriteList.Count; ++usedSprite)
        {
            m_SpriteList[usedSprite].gameObject.SetActive(false);
        }

        for (; usedAnimation < m_AnimationList.Count; ++usedAnimation )
        {
            m_AnimationList[usedAnimation].gameObject.SetActive(false);
        }

            m_Label.text = res;
    }

    private void _Separate()
    {
        for (int i = 0; i < m_StringData.Count; ++i)
        {
            m_StringData[i].Recycle();
        }
        m_StringData.Clear();

        int lastPos = 0;
        string s = string.IsNullOrEmpty(_InputText)?"":_InputText;
        int count = s.Length;
        Type type = Type.LST_NONE;
        StringData data = null;
        for (int i = 0; i < count; ++i)
        {
            if (_InputText[i] == _EscapedLeftBracket)
            {
                int closingBracket = _FindClosingBracket(s, i + 1);
                if (closingBracket != -1 && closingBracket - i > 3)
                {
                    int startIndex = i + 1;
                    type = _GetType(_InputText, startIndex);
                }
                if (type != Type.LST_NONE)
                {
                    data = XDataPool<StringData>.GetData();
                    data.type = type;
                    if (data.Set(s, i + 4, closingBracket - i - 4))
                    {
                        if (i > lastPos)
                        {
                            StringData noneData = XDataPool<StringData>.GetData();
                            noneData.type = Type.LST_NONE;
                            noneData.Set(s, lastPos, i - lastPos);
                            m_StringData.Add(noneData);
                        }
                        m_StringData.Add(data);
                        lastPos = closingBracket + 1;
                        i = closingBracket;
                    }
                    else
                    {
                        data.Recycle();
                    }
                    type = Type.LST_NONE;
                }
            }
        }
        if (lastPos < count)
        {
            StringData noneData = XDataPool<StringData>.GetData();
            noneData.type = Type.LST_NONE;
            noneData.Set(s, lastPos, count - lastPos);
            m_StringData.Add(noneData);
        }
    }

    Type _GetType(string s, int startIndex)
    {
        if (strcmp(s, "im=", startIndex) == 0)
            return Type.LST_IMAGE;
        if (strcmp(s, "gd=", startIndex) == 0)
            return Type.LST_GUILD;
        if (strcmp(s, "dg=", startIndex) == 0)
            return Type.LST_DRAGON_GUILD;
        if (strcmp(s, "tm=", startIndex) == 0)
            return Type.LST_TEAM;
        if (strcmp(s, "it=", startIndex) == 0)
            return Type.LST_ITEM;
        if (strcmp(s, "nm=", startIndex) == 0)
            return Type.LST_NAME;
        if (strcmp(s, "pk=", startIndex) == 0)
            return Type.LST_PK;
        if (strcmp(s, "ui=", startIndex) == 0)
            return Type.LST_UI;
        if (strcmp(s, "sp=", startIndex) == 0)
            return Type.LST_SPECTATE;
        if (strcmp(s, "an=", startIndex) == 0)
            return Type.LST_ANIMATION;
        return Type.LST_NONE;
    }
    static int strcmp(string left, string right, int leftStartIndex = 0, int rightStartIndex = 0)
    {
        int i;
        int j;
        for (i = leftStartIndex, j = rightStartIndex; i < left.Length && j < right.Length; ++i, ++j)
        {
            if (left[i] == right[j])
                continue;
            if (left[i] < right[j])
                return -1;
            return 1;
        }
        //if (i == left.Length && j == right.Length)
        return 0;
        //if (i == left.Length)
        //    return -1;
        //return 1;
    }

    string _GetSpaceWithSameWidth(int width)
    {
        if(_UniSpaceWidth <= 0.0f)
            _UniSpaceWidth = 5f;

        if (width < _UniSpaceWidth)
            return " ";
        else if (width < 2 * _UniSpaceWidth)
            return "  ";
        else
        {
            s_TempSB.Length = 0;
            int count = Mathf.CeilToInt((width - 2 * _UniSpaceWidth) / _UniSpaceWidth);
            s_TempSB.Append(" ");
            s_TempSB.Append(_UniSpace, count);
            s_TempSB.Append(" ");
            return s_TempSB.ToString();
        }
    }

    static char _UniSpace = ' ';//'\u2009';
    //static char _cSeparator = '|';
    static char _EscapedSeparator = (char)31;
    //static char _cLeftBracket = '[';
    static char _EscapedLeftBracket = (char)2;
    //static char _cRightBracket = ']';
    static char _EscapedRightBracket = (char)3;

    float _UniSpaceWidth = -0.1f;
    //float _SpaceWidth = -0.1f;

    public void RegisterTeamEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        TeamClickHandler = eventHandler;
    }
    public void RegisterGuildEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        GuildClickHandler = eventHandler;
    }
    public void RegisterDragonGuildEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        DragonGuildClickHandler = eventHandler;
    }
    public void RegisterItemEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        ItemClickHandler = eventHandler;
    }
    public void RegisterNameEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        NameClickHandler = eventHandler;
    }
    public void RegisterPkEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        PkClickHandler = eventHandler;
    }
    public void RegisterUIEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        UIClickHandler = eventHandler;
    }

    public void RegisterSpectateEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        SpectateHandler = eventHandler;
    }

    public void RegisterDefaultEventHandler(HyperLinkClickEventHandler eventHandler)
    {
        DefaultClickHandler = eventHandler;
    }

    public void RegisterSymbolClickHandler(LabelSymbolClickEventHandler eventHandler)
    {
        SymbolClickHandler = eventHandler;
    }
}