using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using XUtliPoolLib;

public enum AIMsgType
{
    ExtString = 0,
    StringCmd = 1,
}

public enum AIMsgTarget
{
    Ally = 0,
    Role,
    Boss,
    Monster,
    Enemy,
    All,
    LevelMgr,
    GlobalAI,
}

public enum SetEnmityType
{
    HateValue = 1,
    Provoke,
    ProvokeIfNotProvoked,
    Clear
}

public class Shout : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class SetExString : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class ReceiveAIEvent : Action
{
    public bool mAIArgDeprecate;
    public AIMsgType mAIArgMsgType;
    public SharedString mAIArgMsgStr;
    public SharedInt mAIArgTypeId;
    public SharedVector3 mAIArgPos;
    public SharedInt mAIArgSkillTemplateId;
    public SharedInt mAIArgSkillId;
    public SharedFloat mAIArgFloatArg;
    public SharedString mAIArgSenderUID;

    public override TaskStatus OnUpdate()
    {
        string stringCmd = AIMgrUtil.GetAIMgrInterface().ReceiveAIEvent(transform.gameObject, (int)mAIArgMsgType, mAIArgDeprecate);

        if (stringCmd == "")
            return TaskStatus.Failure;
        else
        {
            string[] args = stringCmd.Split(' ');
            mAIArgMsgStr.Value = args[0];
            mAIArgTypeId.Value = int.Parse(args[1]);
#if UNITY_EDITOR
            mAIArgPos.Value = new Vector3(XEditor.XParse.Parse(args[2]), XEditor.XParse.Parse(args[3]), XEditor.XParse.Parse(args[4]));
#endif
            mAIArgSkillId.Value = int.Parse(args[5]);
            mAIArgSenderUID.Value = args[6];
            return TaskStatus.Success;
        }

    }
}

public class SendAIEvent : Action
{
    public AIMsgTarget mAIArgMsgTo;
    public AIMsgType mAIArgMsgType;
    public int mAIArgEntityTypeId;
    public string mAIArgMsgStr;
    public SharedVector3 mAIArgPos;
    public float mAIArgDelayTime;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().SendAIEvent(transform.gameObject, (int)mAIArgMsgTo, (int)mAIArgMsgType, mAIArgEntityTypeId, mAIArgMsgStr, mAIArgDelayTime, mAIArgPos.Value))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class CallMonster : Action
{
    public SharedFloat mAIArgDist;
    public SharedFloat mAIArgAngle;
    public int mAIArgMonsterId;
    public SharedInt mAIArgMonsterId2;
    public int mAIArgCopyMonsterId;
    public int mAIArgMaxMonsterNum;
    public float mAIArgLifeTime;
    public float mAIArgDelayTime;
    public SharedVector3 mAIArgPos;
    public int mAIArgBornType; // 0: eventpos, 1: randompos 3: 相对位置
    public Vector3 mAIArgPos1;
    public Vector3 mAIArgPos2;
    public Vector3 mAIArgPos3;
    public Vector3 mAIArgPos4;
    public Vector3 mAIArgFinalPos; // 保底位置
    public bool mAIArgForcePlace; //一定要在指定地点，如果指定地点在地图外，则招怪失败
    public float mAIArgDeltaArg;
    public float mAIArgHPPercent;

    public override TaskStatus OnUpdate()
    {
        CallMonsterData data = new CallMonsterData();
        data.mAIArgDist = mAIArgDist.Value;
        data.mAIArgAngle = mAIArgAngle.Value;
        data.mAIArgMonsterId = mAIArgMonsterId2.Value == 0 ? mAIArgMonsterId : mAIArgMonsterId2.Value;
        data.mAIArgCopyMonsterId = mAIArgCopyMonsterId;
        data.mAIArgLifeTime = mAIArgLifeTime;
        data.mAIArgDelayTime = mAIArgDelayTime;
        data.mAIArgPos = mAIArgPos.Value;
        data.mAIArgBornType = mAIArgBornType;
        data.mAIArgPos1 = mAIArgPos1;
        data.mAIArgPos2 = mAIArgPos2;
        data.mAIArgPos3 = mAIArgPos3;
        data.mAIArgPos4 = mAIArgPos4;
        data.mAIArgFinalPos = mAIArgFinalPos;
        data.mAIArgForcePlace = mAIArgForcePlace;
        data.mAIArgDeltaArg = mAIArgDeltaArg;
        data.mAIArgHPPercent = mAIArgHPPercent;

        if (AIMgrUtil.GetAIMgrInterface().CallMonster(transform.gameObject, data))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class MixMonsterPos : Action
{
    public int mAIArgMixMonsterId0;
    public int mAIArgMixMonsterId1;
    public int mAIArgMixMonsterId2;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class KillMonster : Action
{
    public int mAIArgMonsterId;
    public float mAIArgDelayTime;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class AddBuff : Action
{
    public SharedInt mAIArgMonsterId;
    public SharedInt mAIArgBuffId;
    public SharedInt mAIArgBuffId2;
    public SharedInt mAIArgAddBuffTarget; // 0: monster  1: player
    public SharedInt mAIArgAddBuffWay;  // 0: all 1: random one 2; prof
    public SharedInt mAIArgPlayerProfId; // player proffesion id

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().AddBuff(mAIArgMonsterId.Value, mAIArgBuffId.Value, mAIArgBuffId2.Value))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class RemoveBuff : Action
{
    public int mAIArgMonsterId;
    public int mAIArgBuffId;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class CallScript : Action
{
    public string mAIArgFuncName;
    public float mAIArgDelayTime;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().CallScript(transform.gameObject, mAIArgFuncName, mAIArgDelayTime))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class RunSubTree : Action
{
    public string mAIArgTreeName;

    public override TaskStatus OnUpdate()
    {
        AIMgrUtil.GetAIMgrInterface().RunSubTree(transform.gameObject, mAIArgTreeName);
        return TaskStatus.Success;
    }
}

public class  PlayFx : Action
{
    public string mAIArgFxName;
    public SharedVector3 mAIArgFxPos;
    public float mAIArgDelayTime;

    public override TaskStatus OnUpdate()
    {
        //if (mAIArgFxPos.Value == null)
        //    return TaskStatus.Failure;

        //if (AIMgrUtil.GetAIMgrInterface().PlayFx(transform.gameObject, mAIArgFxName, mAIArgFxPos.Value, mAIArgDelayTime))
        //    return TaskStatus.Success;
        //else
            return TaskStatus.Failure;
    }
}

public class DetectEnemyInRange : Action
{
    public enum FightGroupType
    {
        Opponent = 0,
        Ally
    }

    public int mAIArgRangeType; // 0: circle  1: rect
    public float mAIArgRadius;
    public float mAIArgAngle;
    public float mAIArgLength;
    public float mAIArgWidth;
    public float mAIArgOffsetLength;
    public int mAIArgMonsterType;
    public FightGroupType mAIArgFightGroupType;

    public override TaskStatus OnUpdate()
    {
        DetectEnemyInRangeArg arg;
        arg.mAIArgAngle = mAIArgAngle;
        arg.mAIArgRangeType = mAIArgRangeType;
        arg.mAIArgRadius = mAIArgRadius;
        arg.mAIArgLength = mAIArgLength;
        arg.mAIArgWidth = mAIArgWidth;
        arg.mAIArgOffsetLength = mAIArgOffsetLength;
        arg.mAIArgMonsterType = mAIArgMonsterType;
        arg.mAIArgFightGroupType = XFastEnumIntEqualityComparer<FightGroupType>.ToInt(mAIArgFightGroupType);

        if (AIMgrUtil.GetAIMgrInterface().DetectEnemyInRange(transform.gameObject, ref arg))
            return TaskStatus.Success;
        else
        return TaskStatus.Failure;
    }
}

public class XHashFunc : Action
{
    public string mAIArgInput;
    public SharedInt mAIArgResult;

    public override TaskStatus OnUpdate()
    {
        mAIArgResult.Value = (int)XCommon.singleton.XHash(mAIArgInput);
        return TaskStatus.Success;
    }
}

public class AIDoodad : Action
{
    public SharedInt mAIArgDoodadId;
    public SharedInt mAIArgWaveId;
    public SharedVector3 mAIArgDoodadPos;
    public float mAIArgRandomPos;
    public float mAIArgDelayTime;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().AIDoodad(transform.gameObject, mAIArgDoodadId.Value, mAIArgWaveId.Value, mAIArgDoodadPos.Value, mAIArgRandomPos, mAIArgDelayTime))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class RemoveSceneBuff : Action
{
    public int mAIArgBuffId;


    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class RandomEntityPos : Action
{
    public int mAIArgTemplateId;
    public float mAIArgRadius;
    public Vector3 mAIArgCenterPos;
    public Vector3 mAIArgFinalPos;
    public int mAIArgNearPlayerTemplateId; // 0: no use -1: all player  >0 : prof

    public override TaskStatus OnUpdate()
    {
        return base.OnUpdate();
    }
}

public class SelectPlayerFromList : Action
{
    public int mAIArgSelectType;
    public int mAIArgStartIndex;
    public int mAIArgEndIndex;

    public override TaskStatus OnUpdate()
    {
        return base.OnUpdate();
    }
}

public class GetEntityPos : Action
{
    public int mAIArgIsPlayer;
    public int mAIArgTemplateId;
    public SharedVector3 mAIArgStorePos;

    public override TaskStatus OnUpdate()
    {
        return base.OnUpdate();
    }
}

public class GetEntityCount : Action
{
    ///> TODO
    ///  区分怪还是人，并可以指定TypeID
    public int mAIArgWay;
    public SharedInt mAIArgStoreCount;
}

public class SetEnmity : Action
{
    public SetEnmityType mAIArgOperateType;
    public SharedFloat mAIArgOperateValue;

    public override TaskStatus OnUpdate()
    {
        return base.OnUpdate();
    }
}

public class GetMonsterID : Action
{
    public int mAIArgType;
    public SharedInt mAIArgStoreID;
}

public class GetUID : Action
{
    public SharedString mAIArgStoreID;
}

public class AddGroupLevel : Action
{
    public SharedInt mAIArgFightGroup;
    public int mAIArgLevel;
}

public class NotifyTarget : Action
{

}

public class GetFightGroup : Action
{
    public SharedString mAIArgUID;
    public SharedInt mAIArgStoreFightGroup;
}