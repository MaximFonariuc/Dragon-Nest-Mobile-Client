using UnityEngine;
using BehaviorDesigner.Runtime;
using XUtliPoolLib;

public class CreateTree : MonoBehaviour
{
    public ExternalBehaviorTree behaviorTree;

    void Start()
    {
        BehaviorTree bt = transform.gameObject.AddComponent<BehaviorTree>();
        bt.ExternalBehavior = behaviorTree;
        bt.StartWhenEnabled = false;
    }
}

public class AIMgrUtil
{
    private static IXAIGeneralMgr _ai_general_mgr = null;

    public static IXAIGeneralMgr GetAIMgrInterface()
    {
        if (_ai_general_mgr == null || _ai_general_mgr.Deprecated)
            _ai_general_mgr = XInterfaceMgr.singleton.GetInterface<IXAIGeneralMgr>(XCommon.singleton.XHash("XAIGeneralMgr"));

        return _ai_general_mgr;
    }

}