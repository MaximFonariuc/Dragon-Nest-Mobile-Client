#if UNITY_EDITOR
using XUtliPoolLib;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

public class XBehaviorTree : MonoBehaviour, IXBehaviorTree
{
    public static Dictionary<string, BehaviorSource> BehaviorCache = new Dictionary<string, BehaviorSource>();

    public bool Deprecated
    {
        get;
        set;
    }

    void Awake()
    {
        _behavior_tree = gameObject.AddComponent<BehaviorTree>();
    }
    public static BehaviorSource DeepClone(BehaviorSource source) //深clone
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, source);
        stream.Position = 0;
        return formatter.Deserialize(stream) as BehaviorSource;
    }

    public void SetManual(bool enable)
    {
        if (enable)
            BehaviorManager.instance.UpdateInterval = UpdateIntervalType.Manual;
    }
    public void TickBehaviorTree()
    {
        if (_behavior_tree != null)
            BehaviorManager.instance.Tick(_behavior_tree);
    }

    public bool SetBehaviorTree(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        string location = "Assets/Behavior Designer/Behavior Data/" + name + ".asset";
        ExternalBehaviorTree ebt = UnityEditor.AssetDatabase.LoadAssetAtPath(location, typeof(ExternalBehaviorTree)) as ExternalBehaviorTree;
        _behavior_tree.ExternalBehavior = ebt;
        _behavior_tree.RestartWhenComplete = true;

        return true;
    }

    public void EnableBehaviorTree(bool enable)
    {
        if (_behavior_tree == null)
            return;

        if (enable)
            _behavior_tree.EnableBehavior();
        else
            _behavior_tree.DisableBehavior();

    }

    public void OnStartSkill(uint skillid)
    {
        //List<XAIIsCastingSkill> castingSkills = _behavior_tree.FindTasks<XAIIsCastingSkill>();

        //for (int i = 0; i < castingSkills.Count; i++)
        //    castingSkills[i].IsCastingSkill = true;

        //List<XAITryCastSkill> trySkills = _behavior_tree.FindTasks<XAITryCastSkill>();
        //for (int i = 0; i < trySkills.Count; i++)
        //    trySkills[i].CastSkillId = skillid;
    }

    public void OnEndSkill(uint skillid)
    {
        //List<XAIIsCastingSkill> castingSkills = _behavior_tree.FindTasks<XAIIsCastingSkill>();

        //for (int i = 0; i < castingSkills.Count; i++)
        //    castingSkills[i].IsCastingSkill = false;
    }

    public void OnSkillHurt()
    {
        //List<XAIIsCastingSkill> isCasting = _behavior_tree.FindTasks<XAIIsCastingSkill>();

        //for (int i = 0; i < isCasting.Count; i++)
        //    isCasting[i].IsHurt = true;
    }

    public float OnGetHeartRate()
    {
        if (_behavior_tree == null)
            return 0;

        SharedFloat innerRate = (SharedFloat)_behavior_tree.GetVariable("heartrate");
        return innerRate.Value;
    }

    public void SetTarget(Transform target)
    {
        SharedTransform innertarget = (SharedTransform)_behavior_tree.GetVariable("target");

        if (innertarget != null)
        {
            innertarget.SetValue(target);
        }
    }

    public void SetNavPoint(Transform navpoint)
    {
        SharedTransform innernav = (SharedTransform)_behavior_tree.GetVariable("navtarget");

        if (innernav != null)
        {
            innernav.SetValue(navpoint);
        }
    }

    public void SetVariable(string name, object value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }

    public void SetIntByName(string name, int value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }
    public void SetFloatByName(string name, float value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }
    public void SetBoolByName(string name, bool value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }
    public void SetVector3ByName(string name, Vector3 value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }
    public void SetTransformByName(string name, Transform value)
    {
        if (_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value);
        }
    }
    public void SetXGameObjectByName(string name, XGameObject value)
    {
        if(_behavior_tree == null)
            return;

        SharedVariable sharedvar = _behavior_tree.GetVariable(name);

        if (sharedvar != null)
        {
            sharedvar.SetValue(value == null ? null : value.Find(""));
        }
    }

    public static AudioClip GetAudioClip(string path)
    {
#if UNITY_EDITOR
        AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
        return clip;
#else
        return null;
#endif
    }

    private BehaviorTree _behavior_tree = null;
}
#endif