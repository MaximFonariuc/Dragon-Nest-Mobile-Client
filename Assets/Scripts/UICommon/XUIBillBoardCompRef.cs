using UILib;
using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XUIBillBoardCompRef : MonoBehaviour, IXUIBillBoardCompRef
{
    public XUISpecLabelSymbol _NameSpecLabelSymbol;
    public XUISpecLabelSymbol _GuildSpecLabelSymbol;
    public XUISpecLabelSymbol _DesiSpecLabelSymbol;
    public XUIProgress _BloodBar;
    public XUIProgress _IndureBar;

    public IXUISpecLabelSymbol NameSpecLabelSymbol { get { return _NameSpecLabelSymbol; } }
    public IXUISpecLabelSymbol GuildSpecLabelSymbol { get { return _GuildSpecLabelSymbol; } }
    public IXUISpecLabelSymbol DesiSpecLabelSymbol { get { return _DesiSpecLabelSymbol; } }
    public IXUIProgress BloodBar { get { return _BloodBar; } }
    public IXUIProgress IndureBar { get { return _IndureBar; } }
}
