using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using XUtliPoolLib;

public class ValueHP : Conditional
{
    public int mAIArgMaxPercent;
    public int mAIArgMinPercent;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsHPValue(transform, mAIArgMinPercent, mAIArgMaxPercent))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class ValueMP : Conditional
{
    public int mAIArgMaxPercent;
    public int mAIArgMinPercent;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsMPValue(transform, mAIArgMinPercent, mAIArgMaxPercent))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class ValueFP : Conditional
{
    public int mAIArgMaxFP;
    public int mAIArgMinFP;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class ValueTarget : Conditional
{
    public SharedTransform mAIArgTarget; 

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsValid(mAIArgTarget.Value))
            return TaskStatus.Success;
        else
        {
            AIMgrUtil.GetAIMgrInterface().ClearTarget(transform);
            return TaskStatus.Failure;
        }
    }
}

public class ValueDistance : Conditional
{
    public SharedTransform mAIArgTarget;
    public SharedFloat mAIArgMaxDistance;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
            return TaskStatus.Failure;

        if ((transform.position - mAIArgTarget.Value.position).magnitude <= mAIArgMaxDistance.Value)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsOppoCastingSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsOppoCastingSkill(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsHurtOppo : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsHurtOppo(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsFixedInCd : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsFixedInCd(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsWander : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsWander(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsCastingSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsCastSkill(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsFighting : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsFighting(transform))
            return TaskStatus.Success;
        else
        {
            AIMgrUtil.GetAIMgrInterface().ClearTarget(transform);
            return TaskStatus.Failure;
        }
    }
}

public class IsSkillChoosed : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsSkillChoosed(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class ConditionDist : Conditional
{
    public SharedTransform mAIArgTarget;
    public SharedFloat mAIArgUpper;
    public SharedFloat mAIArgLower;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
            return TaskStatus.Failure;

        float dist = (mAIArgTarget.Value.position - transform.position).magnitude;

        if (dist >= mAIArgLower.Value && dist <= mAIArgUpper.Value)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class ConditionMonsterNum : Conditional
{
    public SharedInt mAIArgNum;
    public int mAIArgMonsterId;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class ConditionPlayerNum : Conditional
{

    public int mAIArgPlayerBaseProf;
    public int mAIArgPlayerDetailProf;
    public int mAIArgWay;
    public Vector3 mAIArgCenter;
    public float mAIArgRadius;
    public SharedInt mAIArgNum;

    public override TaskStatus OnUpdate()
    {
        int playerprof = AIMgrUtil.GetAIMgrInterface().GetPlayerProf();

        if (mAIArgPlayerBaseProf == 0 && mAIArgPlayerDetailProf == 0)
            mAIArgNum.Value = 1;
        else
        {
            if (mAIArgPlayerBaseProf != 0 && playerprof % 10 == mAIArgPlayerBaseProf)
                mAIArgNum.Value = 1;

            if (mAIArgPlayerDetailProf != 0 && playerprof == mAIArgPlayerDetailProf)
                mAIArgNum.Value = 1;
        }

        return TaskStatus.Success;
    }
}