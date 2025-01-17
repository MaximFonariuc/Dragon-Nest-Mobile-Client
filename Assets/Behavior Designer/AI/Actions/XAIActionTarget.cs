using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindTargetByDistance : Action
{
    public SharedFloat mAIArgDistance;
    public bool mAIArgFilterImmortal;
    public float mAIArgAngle;
    public float mAIArgDelta;
    public int mAIArgTargetType;
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().FindTargetByDistance(transform.gameObject, mAIArgDistance.Value, mAIArgFilterImmortal, mAIArgAngle, mAIArgDelta, mAIArgTargetType))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class FindTargetByHitLevel : Action
{
    //public SharedFloat mAIArgDistance;
    public bool mAIArgFilterImmortal;
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().FindTargetByHitLevel(transform.gameObject, mAIArgFilterImmortal))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class FindTargetByNonImmortal : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().FindTargetByNonImmortal(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class TargetByHatredList : Action
{
    public SharedFloat mAIArgDistance;
    public bool mAIArgFilterImmortal;
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().FindTargetByHartedList(transform.gameObject, mAIArgFilterImmortal))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectByUID : Action
{
    public SharedString mAIArgUID;
}

public class DoSelectRoleByIndex : Action
{
    public SharedInt mAIArgIndex;
}

public class FindNavPath : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().FindNavPath(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;  
    }
}

public class ResetTargets : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().ResetTargets(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectNearest : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DoSelectNearest(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;        
    }
}


public class DoSelectFarthest : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DoSelectFarthest(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectRandomTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DoSelectRandomTarget(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;        
    }
}

public class CalDistance : Action
{
    public SharedTransform mAIArgObject;
    public SharedFloat mAIArgDistance;
    public SharedVector3 mAIArgDestPoint;


    public override TaskStatus OnUpdate()
    {
        //if (mAIArgDistance.Value == null)
        //    return TaskStatus.Failure;

        if (mAIArgObject.Value != null)
        {
            mAIArgDistance.Value = (transform.position - mAIArgObject.Value.position).magnitude;
        }
        else
        {
            mAIArgDistance.Value = (transform.position - mAIArgDestPoint.Value).magnitude;
        }

        
        return TaskStatus.Success;
    }
}

public class SelectMoveTargetById : Action
{
    public SharedTransform mAIArgMoveTarget;
    public int mAIArgObjectId;

    public override TaskStatus OnUpdate()
    {
        Transform moveTarget = AIMgrUtil.GetAIMgrInterface().SelectMoveTargetById(transform.gameObject, mAIArgObjectId);

        if (moveTarget == null)
            return TaskStatus.Failure;
        else
        {
            mAIArgMoveTarget.Value = moveTarget;
            return TaskStatus.Success;
        }
    }
}

public class SelectBuffTarget : Action
{
    //public SharedVector3 mAIArgBuffTarget;
    public SharedTransform mAIArgBuffTarget;

    public override TaskStatus OnUpdate()
    {
        Transform buffTarget = AIMgrUtil.GetAIMgrInterface().SelectBuffTarget(transform.gameObject);

        if (buffTarget == null)
        {
            return TaskStatus.Failure;
        }
        else
        {
            mAIArgBuffTarget.Value = buffTarget;
            return TaskStatus.Success;
        }
    }
}

public class SelectItemTarget : Action
{
    //public SharedVector3 mAIArgBuffTarget;
    public SharedTransform mAIArgItemTarget;

    public override TaskStatus OnUpdate()
    {
        Transform itemTarget = AIMgrUtil.GetAIMgrInterface().SelectItemTarget(transform.gameObject);

        if (itemTarget == null)
            return TaskStatus.Failure;
        else
        {
            mAIArgItemTarget.Value = itemTarget;
            return TaskStatus.Success;
        }
    }
}

public class SelectTargetBySkillCircle : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().SelectTargetBySkillCircle(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;    
    }
}

public class SelectNonHartedList : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class ResetHartedList : Action
{
    public override TaskStatus OnUpdate()
    {
        AIMgrUtil.GetAIMgrInterface().ResetHartedList(transform.gameObject);
        return TaskStatus.Success;
    }
}
