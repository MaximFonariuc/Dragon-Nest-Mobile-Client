using System;
using XUtliPoolLib;

public class LuaGameInfo : ILuaGameInfo
{
    private static LuaGameInfo _single = null;
    public static LuaGameInfo single
    {
        get
        {
            if (_single == null)
            {
                _single = new LuaGameInfo();
            }
            return _single;
        }
    }

    public string name { get; set; }
    public uint exp { get; set; }
    public uint maxexp { get; set; }
    public uint level { get; set; }
    public int ppt { get; set; }


    public uint coin { get; set; }
    public uint dia { get; set; }
    public uint energy { get; set; }

    public uint draggon { get; set; }

}

