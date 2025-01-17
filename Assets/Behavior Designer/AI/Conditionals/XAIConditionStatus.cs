using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

internal enum XStateDefine
{
    XState_Idle = 0,
    XState_Move,
    XState_Jump,
    XState_Fall,
    XState_Freeze,
    XState_BeHit,
    XState_Death,
    XState_Charge,
}

public enum XQTEState
{
    None = 0,
    HitBackPresent,
    HitBackStraight,
    HitBackGetup,
    HitFlyPresent,
    HitFlyLand,
    HitFlyStraight,
    HitFlyGetup,
    HitRollPresent,
    HitRollStraight,
    HitRollGetup,
    CanDash = 20,
    DashState,
    ChargeState,
    DashAttackState,
    StayInAir,
    firedash,
    onelight,
    Any = 1000,
}

public class StatusIdle : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsAtState(transform.gameObject, (int)XStateDefine.XState_Idle))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusMove : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsAtState(transform.gameObject, (int)XStateDefine.XState_Move))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusRotate : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsRotate(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusBehit : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsAtState(transform.gameObject, (int)XStateDefine.XState_BeHit))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusDeath : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsAtState(transform.gameObject, (int)XStateDefine.XState_Death))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusFreeze : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsAtState(transform.gameObject, (int)XStateDefine.XState_Freeze))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsCastSkill(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusWoozy : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().IsWoozy(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class IsQTEState : Conditional
{
    public XQTEState mAIArgQTEState;
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().HasQTE(transform.gameObject, (int)mAIArgQTEState))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class TargetQTEState : Conditional
{
    public SharedTransform mAIArgTarget;
    public XQTEState mAIArgQTEState;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
            return TaskStatus.Failure;

        if (AIMgrUtil.GetAIMgrInterface().HasQTE(mAIArgTarget.Value.gameObject, (int)mAIArgQTEState))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

}

public class IsTargetImmortal : Conditional
{
    public SharedTransform mAIArgTarget;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.Value == null)
            return TaskStatus.Failure;
        ulong id = ulong.Parse(mAIArgTarget.Value.gameObject.name);
        if (AIMgrUtil.GetAIMgrInterface().IsTargetImmortal(id))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;        
    }
}