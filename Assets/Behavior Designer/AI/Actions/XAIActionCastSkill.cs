using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public enum ComboSkillType
{
    StartSkill,
    ComboSkill,
}

public class PhysicalAttack : Action
{
    public SharedTransform mAIArgTarget;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
            return TaskStatus.Failure;

        if (AIMgrUtil.GetAIMgrInterface().TryCastPhysicalSkill(transform.gameObject, mAIArgTarget.Value.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class TryCastQTE : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().CastQTESkill(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class CastDash : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().CastDashSkill(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class CastStartSkill : Action
{
    public SharedTransform mAIArgTarget;
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Failure;
    }
}

public class CastNextSkill : Action
{
    public SharedTransform mAIArgTarget;
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Failure;
    }
}

public class FilterSkill : Action
{
    public SharedTransform mAIArgTarget;
    //public bool mAIArgUseInstall;
    public bool mAIArgUseMP;
    public bool mAIArgUseName;
    public bool mAIArgUseHP;
    public bool mAIArgUseCoolDown;
    public bool mAIArgUseAttackField;
    public bool mAIArgUseCombo;
    public bool mAIArgUseInstall = false;
    public ComboSkillType mAIArgSkillType;
    public string mAIArgSkillName;
    public bool mAIArgDetectAllPlayInAttackField;
    public int mAIArgMaxSkillNum;

    public override TaskStatus OnUpdate()
    {
        XUtliPoolLib.FilterSkillArg skillArg = new XUtliPoolLib.FilterSkillArg();
        skillArg.mAIArgTarget = mAIArgTarget.Value;
        skillArg.mAIArgUseMP = mAIArgUseMP;
        skillArg.mAIArgUseName = mAIArgUseName;
        skillArg.mAIArgUseHP = mAIArgUseHP;
        skillArg.mAIArgUseCoolDown = mAIArgUseCoolDown;
        skillArg.mAIArgUseAttackField = mAIArgUseAttackField;
        skillArg.mAIArgUseCombo = mAIArgUseCombo;
        skillArg.mAIArgUseInstall = mAIArgUseInstall;
        skillArg.mAIArgSkillType = (int)mAIArgSkillType;
        skillArg.mAIArgSkillName = mAIArgSkillName;
        skillArg.mAIArgDetectAllPlayInAttackField = mAIArgDetectAllPlayInAttackField;
        skillArg.mAIArgMaxSkillNum = mAIArgMaxSkillNum;

        if (AIMgrUtil.GetAIMgrInterface().SelectSkill(transform.gameObject, skillArg))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectSkillInOrder : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DoSelectInOrder(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectSkillRandom : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DoSelectRandom(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoCastSkill : Action
{
    public SharedTransform mAIArgTarget;
    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
        {
            if (AIMgrUtil.GetAIMgrInterface().DoCastSkill(transform.gameObject, null))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
        else
        {
            if (AIMgrUtil.GetAIMgrInterface().DoCastSkill(transform.gameObject, mAIArgTarget.Value.gameObject))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}

public class StopCastingSkill : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().StopCastingSkill(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsInSkillAttackField : Action
{
    public SharedString mAIArgUID;
    public SharedInt mAIArgSkillID;
}