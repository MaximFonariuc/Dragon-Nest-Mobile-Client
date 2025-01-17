using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using XUtliPoolLib;

public enum MoveDirection
{
    TowardTarget = 1,
    ReverseTarget = 2,
    SelfFaceDir = 3,
    ReverseFaceDir = 4,
}

public enum SetDestWay
{
    Target = 1,
    BornPos = 2,
    NavPos = 3,
}

public enum AdjustDirection
{
    TargetDir = 1,
    TargetFace = 2,
    SelfFace = 3,
}

public class NavToTarget : Action
{
    public SharedTransform mAIArgTarget;
    public SharedTransform mAIArgNavTarget;
    public SharedVector3 mAIArgNavPos;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.GetValue() == null)
        {
            if (mAIArgNavTarget.GetValue() == null)
            {
                if (mAIArgNavPos.Value == Vector3.zero)
                    return TaskStatus.Failure;
                else
                {
                    if (AIMgrUtil.GetAIMgrInterface().ActionNav(transform.gameObject, mAIArgNavPos.Value))
                        return TaskStatus.Success;
                    else
                        return TaskStatus.Failure;
                }

            }
            else
            {
                if (AIMgrUtil.GetAIMgrInterface().NavToTarget(transform.gameObject, mAIArgNavTarget.Value.gameObject))
                    return TaskStatus.Success;
                else
                    return TaskStatus.Failure;
            }
        }
        else
        {
            if (AIMgrUtil.GetAIMgrInterface().NavToTarget(transform.gameObject, mAIArgTarget.Value.gameObject))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
            
        //return TaskStatus.Success;
    }
}

public class RotateToTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().RotateToTarget(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DetectEnimyInSight : Action
{
    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().DetectEnimyInSight(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class ActionMove : Action
{
    public SharedVector3 mAIArgMoveDir;
    public SharedVector3 mAIArgMoveDest;
    public SharedFloat mAIArgMoveSpeed;

    public override TaskStatus OnUpdate()
    {
        if ((mAIArgMoveDest.Value - transform.position).magnitude <= 0.01f)
            return TaskStatus.Failure;

        if (mAIArgMoveDir.Value == Vector3.zero)
        {
            mAIArgMoveDir.Value = (mAIArgMoveDest.Value - transform.position).normalized;
            mAIArgMoveDir.Value.Set(mAIArgMoveDir.Value.x, 0, mAIArgMoveDir.Value.z);
        }

        if (mAIArgMoveDest.Value == Vector3.zero)
        {
            mAIArgMoveDest.Value = transform.position + mAIArgMoveDir.Value.normalized * 50;
        }

        if (AIMgrUtil.GetAIMgrInterface().ActionMove(transform.gameObject, mAIArgMoveDir.Value, mAIArgMoveDest.Value, mAIArgMoveSpeed.Value))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}


public class SetDest : Action
{
    public SharedVector3 mAIArgFinalDest;
    public SharedTransform mAIArgTarget;
    public SharedTransform mAIArgNav;
    public SharedVector3 mAIArgBornPos;
    public SharedInt mAIArgTickCount;

    public float mAIArgRandomMax;

    public float mAIArgAdjustAngle;
    public SharedFloat mAIArgAdjustLength;
    public AdjustDirection mAIArgAdjustDir;

    public SetDestWay mAIArgSetDestType;

    private Vector3 _pos_vec;

    public override void OnAwake()
    {
        base.OnAwake();

        _pos_vec = new Vector3(1.0f, 0, 1.0f);
    }

    public override TaskStatus OnUpdate()
    {
        switch (mAIArgSetDestType)
        {
            case SetDestWay.Target:
                if (mAIArgTarget.Value == null)
                    return TaskStatus.Failure;
                mAIArgFinalDest.Value = mAIArgTarget.Value.position;
                break;
            case SetDestWay.BornPos:
                //if (mAIArgBornPos.Value == null)
                //    return TaskStatus.Failure;
                mAIArgFinalDest.Value = mAIArgBornPos.Value;
                break;
            case SetDestWay.NavPos:
                if (mAIArgNav.Value == null)
                    return TaskStatus.Failure;
                mAIArgFinalDest.Value = mAIArgNav.Value.position;
                break;
        }

        if (mAIArgAdjustLength.Value != 0)
        {
            Vector3 vec = Vector3.zero;

            if (mAIArgAdjustDir == AdjustDirection.TargetDir)
                vec = transform.position - mAIArgFinalDest.Value;
            else if (mAIArgAdjustDir == AdjustDirection.TargetFace && mAIArgTarget.Value != null)
                vec = mAIArgTarget.Value.forward.normalized;
            else if (mAIArgAdjustDir == AdjustDirection.SelfFace)
                vec = transform.forward.normalized;

            int randcount = mAIArgTickCount.Value;

            Vector3 adjustDir = (Quaternion.Euler(new Vector3(0, (randcount % 2) * mAIArgAdjustAngle * 2 - mAIArgAdjustAngle, 0)) * vec);
            Vector3 dest = mAIArgFinalDest.Value + adjustDir.normalized * mAIArgAdjustLength.Value;

            if (!AIMgrUtil.GetAIMgrInterface().IsPointInMap(dest))
            {
                for (int i=0; i<18; i++)
                {
                    float angle = mAIArgAdjustAngle + i * 10;
                    adjustDir = (Quaternion.Euler(new Vector3(0, (randcount % 2) * angle * 2 - angle, 0)) * vec);
                    dest = mAIArgFinalDest.Value + adjustDir.normalized * mAIArgAdjustLength.Value;

                    if (AIMgrUtil.GetAIMgrInterface().IsPointInMap(dest))
                        break;

                    angle = mAIArgAdjustAngle - i * 10;
                    adjustDir = (Quaternion.Euler(new Vector3(0, (randcount % 2) * angle * 2 - angle, 0)) * vec);
                    dest = mAIArgFinalDest.Value + adjustDir.normalized * mAIArgAdjustLength.Value;

                    if (AIMgrUtil.GetAIMgrInterface().IsPointInMap(dest))
                        break;
                }
            }
            mAIArgFinalDest.Value = dest;
        }

        if (mAIArgRandomMax > 0)
        {
            _pos_vec.x = (float)XCommon.singleton.RandomFloat(-0.5f, 0.5f);
            _pos_vec.z = (float)XCommon.singleton.RandomFloat(-0.5f, 0.5f);
            mAIArgFinalDest.Value += mAIArgRandomMax * _pos_vec.normalized;
        }

        return TaskStatus.Success;
    }
}

public class MoveStratage : Action
{
    public SharedTransform mAIArgTarget;
    public int mAIArgStratageIndex;
    public SharedVector3 mAIArgFinalDest;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class ActionRotate : Action
{
    public enum RotateType
    {
        Relative = 0,
        Absolute = 1,
    }

    public float mAIArgRotDegree;
    public float mAIArgRotSpeed;
    public RotateType mAIArgRotType;

    public override TaskStatus OnUpdate()
    {
        if (AIMgrUtil.GetAIMgrInterface().ActionRotate(transform.gameObject, mAIArgRotDegree, mAIArgRotSpeed, XFastEnumIntEqualityComparer<RotateType>.ToInt(mAIArgRotType)))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;  
    }
}

public class ConditionCanReach : Action
{
    public SharedInt mAIArgTemplateid;
    public SharedVector3 mAIArgDestPos;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

public class Navigation : Action
{
    public SharedInt mAIArgMoveDir;
    public float mAIArgPatrolPointRadius;
    public bool mAIArgIsGoForward;

    private int oldDir = 1;

    public override TaskStatus OnUpdate()
    {
        int olddir = oldDir;
        oldDir = mAIArgMoveDir.Value;
        if (AIMgrUtil.GetAIMgrInterface().UpdateNavigation(transform.gameObject, oldDir, olddir))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}