#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using XUtliPoolLib;

using System.IO;
using UnityEditor;

namespace XEditor
{
    public class XParse
    {
        private static bool inited = false;
        public static float Parse(string str)
        {
            if(!inited)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                inited = true;
            }
            float value = 0.0f;
            float.TryParse(str, out value);
            return value;
        }
    }
    public class XGloabelConfLibrary
    {
        private static GlobalTable _table = new GlobalTable();

        public static float Hit_PresentStraight;
        public static float Hit_HardStraight;
        public static float Hit_Offset;
        public static float Hit_Height;

        static string GetValue(string key)
        {
            string ret = "";
            uint k = XCommon.singleton.XHash(key);
            if (_table.Table.TryGetValue(k, out ret))
            {
                return ret;
            }

            return ret;
        }
        static XGloabelConfLibrary()
        {
            XTableReader.ReadFile(@"Table/GlobalConfig", _table);

            Hit_PresentStraight = XParse.Parse(GetValue("PresentStraight"));
            Hit_HardStraight = XParse.Parse(GetValue("HardStraight"));
            Hit_Offset = XParse.Parse(GetValue("Offset"));
            Hit_Height = XParse.Parse(GetValue("Height"));
        }
    }

    public class XQTEStatusLibrary
    {
        private static XQTEStatusTable _table = new XQTEStatusTable();
        public static List<string> NameList = null;
        public static List<string> KeyList = null;

        static XQTEStatusLibrary()
        {
            XTableReader.ReadFile(@"Table/QteStatusList", _table);

            NameList = new List<string>();
            KeyList = new List<string>();

            for (int i = 0; i < _table.Table.Length; ++i)
            {
                XQTEStatusTable.RowData row = _table.Table[i];
                NameList.Add(row.Value + " " + row.Name);
            }

            for (int i = 0; i < (int)KKSG.XSkillSlot.Attack_Max; i++)
                KeyList.Add(((KKSG.XSkillSlot)i).ToString());
        }

        public static int GetStatusValue(int idx)
        {
            if (idx < 0 || idx >= NameList.Count) return 0;

            string[] strs = NameList.ToArray();

            for (int i = 0; i < _table.Table.Length; ++i)
            {
                XQTEStatusTable.RowData row = _table.Table[i];
                if ((row.Value + " " + row.Name) == strs[idx])
                    return (int)row.Value;
            }

            return 0;
        }

        public static int GetStatusIdx(int qte)
        {
            string[] strs = NameList.ToArray();

            string str = null;
            for (int i = 0; i < _table.Table.Length; ++i)
            {
                XQTEStatusTable.RowData row = _table.Table[i];
                if (row.Value == qte)
                {
                    str = (row.Value + " " + row.Name);
                    break;
                }
            }

            if (str != null)
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i] == str) return i;
                }
            }

            return 0;
        }
    }

    public class XSkillListLibrary
    {
        private static SkillList _skilllist = new SkillList();

        static XSkillListLibrary()
        {
            XTableReader.ReadFile(@"Table/SkillList", _skilllist);
        }

        public static SkillList.RowData[] AllList()
        {
            return _skilllist.Table;
        }
    }

    public class XAnimationLibrary
    {
        private static XEntityPresentation _presentations = new XEntityPresentation();

        static XAnimationLibrary()
        {
            XTableReader.ReadFile(@"Table/XEntityPresentation", _presentations);
        }

        public static XEntityPresentation.RowData AssociatedAnimations(uint presentid)
        {
            return _presentations.GetByPresentID(presentid);
        }

        public static GameObject GetDummy(uint presentid)
        {
            XEntityPresentation.RowData raw_data = AssociatedAnimations(presentid);
            if (raw_data == null) return null;

            string prefab = raw_data.Prefab;

            int n = prefab.LastIndexOf("_SkinnedMesh");
            int m = prefab.LastIndexOf("Loading");

            return n < 0 || m > 0?
                AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/" + prefab + ".prefab", typeof(GameObject)) as GameObject :
                AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/" + prefab.Substring(0, n) + ".prefab", typeof(GameObject)) as GameObject;
        }
    }

    public class XStatisticsLibrary
    {
        private static XEntityStatistics _statistics = new XEntityStatistics();

        static XStatisticsLibrary()
        {
            XTableReader.ReadFile(@"Table/XEntityStatistics", _statistics);
        }

        public static XEntityStatistics.RowData AssociatedData(uint id)
        {
            return _statistics.GetByID(id);
        }

        public static GameObject GetDummy(uint id)
        {
            XEntityStatistics.RowData data = AssociatedData(id);
            if (data == null) return null;
            return XAnimationLibrary.GetDummy(data.PresentID);
        }
    }

    public class XSceneLibrary
    {
        private static SceneTable _table = new SceneTable();

        static XSceneLibrary()
        {
            XTableReader.ReadFile(@"Table/SceneList", _table);
        }

        public static SceneTable.RowData AssociatedData(uint id)
        {
            return _table.GetBySceneID((int)id);
        }

        public static string GetDynamicString(string levelConfig)
        {
            for (int i = 0; i < _table.Table.Length; i++)
            {
                if (_table.Table[i].configFile == levelConfig)
                    return _table.Table[i].DynamicScene;
            }

            return "";
        }
    }

    //public class XEquipBoneLibrary
    //{
    //    private static EquipBones _statistics = new EquipBones();

    //    static XEquipBoneLibrary()
    //    {
    //        //XResourceLoaderMgr.singleton.ReadFile(@"Table/EquipBones", _statistics);
    //    }

    //    public static void Read()
    //    {
    //        XResourceLoaderMgr.singleton.ReadFile(@"Table/EquipBones", _statistics);
    //    }

    //    public static EquipBones.RowData AssociatedData(string EquipName)
    //    {
    //        return _statistics.GetByEquipName(EquipName);
    //    }

    //    public static void ChangeData(string EquipName, EquipBones.RowData data)
    //    {
    //        for (int i = 0; i < _statistics.Table.Count; i++)
    //        {
    //            if (_statistics.Table[i].EquipName == EquipName)
    //            {
    //                _statistics.Table[i] = data;
    //                return;
    //            }
    //        }

    //        _statistics.Table.Add(data);
    //    }

    //    public static void WriteToFile()
    //    {
    //        string path = "./Assets/Resources/Table/EquipBones.txt";

    //        using (FileStream writer = new FileStream(path, FileMode.Truncate))
    //        {
    //            StreamWriter sw = new StreamWriter(writer, Encoding.Unicode);

    //            _statistics.Comment = new List<string>();
    //            _statistics.Comment.Add("comment_EquipName");
    //            _statistics.Comment.Add("comment_bones");

    //            _statistics.WriteFile(sw);

    //            sw.Flush();
    //            sw.Close();
    //        }
    //    }
    //}

    [Serializable]
    public class XConfigData
    {
        [SerializeField]
        public string SkillName;
        [SerializeField]
        public float Speed = 2.0f;
        [SerializeField]
        public float RotateSpeed = 12.0f;

        [SerializeField]
        public string SkillClip;
        [SerializeField]
        public string SkillClipName;

        [SerializeField]
        public string Directory = null;
        [SerializeField]
        public int Player = 0;
        [SerializeField]
        public int Dummy = 0;

        [SerializeField]
        public List<XResultDataExtra> Result = new List<XResultDataExtra>();
        [SerializeField]
        public List<XChargeDataExtra> Charge = new List<XChargeDataExtra>();
        [SerializeField]
        public List<XJADataExtra> Ja = new List<XJADataExtra>();
        [SerializeField]
        public List<XCombinedDataExtra> Combined = new List<XCombinedDataExtra>();
        [SerializeField]
        public List<XCameraEffectDataExtra> CameraEffect = new List<XCameraEffectDataExtra>();
        [SerializeField]
        public XLogicalDataExtra Logical = new XLogicalDataExtra();
        [SerializeField]
        public XUltraDataExtra Ultra = new XUltraDataExtra();
        [SerializeField]
        public XCameraPostEffectDataExtra PostEffect = new XCameraPostEffectDataExtra();

        public void Add<T>(T data) where T : XBaseDataExtra
        {
            Type t = typeof(T);

            if (t == typeof(XResultDataExtra)) Result.Add(data as XResultDataExtra);
            else if (t == typeof(XChargeDataExtra)) Charge.Add(data as XChargeDataExtra);
            else if (t == typeof(XJADataExtra)) Ja.Add(data as XJADataExtra);
            else if (t == typeof(XCombinedDataExtra)) Combined.Add(data as XCombinedDataExtra);
            else if (t == typeof(XCameraEffectDataExtra)) CameraEffect.Add(data as XCameraEffectDataExtra);
        }
    }

    [Serializable]
    public class XEditorData
    {
        //for serialized
        [SerializeField]
        public bool XResult_foldout;
        [SerializeField]
        public bool XCharge_foldout;
        [SerializeField]
        public bool XHit_foldout;
        [SerializeField]
        public bool XJA_foldout;
        [SerializeField]
        public bool XScript_foldout;
        [SerializeField]
        public bool XMob_foldout;
        [SerializeField]
        public bool XCastChain_foldout;
        [SerializeField]
        public bool XManipulation_foldout;
        [SerializeField]
        public bool XLogical_foldout;
        [SerializeField]
        public bool XCombined_foldout;
        [SerializeField]
        public bool XUltra_foldout;
        [SerializeField]
        public bool XFx_foldout;
        [SerializeField]
        public bool XAudio_foldout;
        [SerializeField]
        public bool XCameraEffect_foldout;
        [SerializeField]
        public bool XCameraMotion_foldout;
        [SerializeField]
        public bool XCameraPostEffect_foldout;
        [SerializeField]
        public bool XEffect_foldout;
        [SerializeField]
        public bool XHitDummy_foldout;
        [SerializeField]
        public bool XQTEStatus_foldout;
        [SerializeField]
        public bool XWarning_foldout;
        [SerializeField]
        public bool XComboSkills_foldout;

        [SerializeField]
        public bool XAutoSelected;
        [SerializeField]
        public bool XFrameByFrame;
        [SerializeField]
        public bool XAutoJA = false;

        public void ToggleFold<T>(bool b) where T : XBaseData
        {
            Type t = typeof(T);

            if (t == typeof(XResultData)) XResult_foldout = b;
            else if (t == typeof(XChargeData)) XCharge_foldout = b;
            else if (t == typeof(XJAData)) XJA_foldout = b;
            else if (t == typeof(XHitData)) XHit_foldout = b;
            else if (t == typeof(XScriptData)) XScript_foldout = b;
            else if (t == typeof(XLogicalData)) XLogical_foldout = b;
            else if (t == typeof(XFxData)) XFx_foldout = b;
            else if (t == typeof(XAudioData)) XAudio_foldout = b;
            else if (t == typeof(XCameraEffectData)) XCameraEffect_foldout = b;
            else if (t == typeof(XWarningData)) XWarning_foldout = b;
        }
    }

    [Serializable]
    public class XSkillDataExtra
    {
        [SerializeField]
        public AnimationClip SkillClip;
        [SerializeField]
        public float SkillClip_Frame;
        [SerializeField]
        public string ScriptPath;
        [SerializeField]
        public string ScriptFile;
        [SerializeField]
        public GameObject Dummy;

        [SerializeField]
        public List<XResultDataExtraEx> ResultEx = new List<XResultDataExtraEx>();
        [SerializeField]
        public List<XChargeDataExtraEx> ChargeEx = new List<XChargeDataExtraEx>();
        [SerializeField]
        public List<XFxDataExtra> Fx = new List<XFxDataExtra>();
        [SerializeField]
        public List<XManipulationDataExtra> ManipulationEx = new List<XManipulationDataExtra>();
        [SerializeField]
        public List<XWarningDataExtra> Warning = new List<XWarningDataExtra>();
        [SerializeField]
        public List<XAudioDataExtra> Audio = new List<XAudioDataExtra>();
        [SerializeField]
        public List<XMobUnitDataExtra> Mob = new List<XMobUnitDataExtra>();
        [SerializeField]
        public XCameraMotionDataExtra MotionEx = new XCameraMotionDataExtra();
        [SerializeField]
        public XChainCastExtra Chain = new XChainCastExtra();
        [SerializeField]
        public XCameraPostEffectDataExtraEx PostEffectEx = new XCameraPostEffectDataExtraEx();
        [SerializeField]
        public List<XJADataExtraEx> JaEx = new List<XJADataExtraEx>();
        [SerializeField]
        public List<XHitDataExtraEx> HitEx = new List<XHitDataExtraEx>();
        [SerializeField]
        public List<XCombinedDataExtraEx> CombinedEx = new List<XCombinedDataExtraEx>();

        public void Add<T>(T data) where T : XBaseDataExtra
        {
            Type t = typeof(T);

            if (t == typeof(XFxDataExtra)) Fx.Add(data as XFxDataExtra);
            else if (t == typeof(XWarningDataExtra)) Warning.Add(data as XWarningDataExtra);
            else if (t == typeof(XMobUnitDataExtra)) Mob.Add(data as XMobUnitDataExtra);
            else if (t == typeof(XAudioDataExtra)) Audio.Add(data as XAudioDataExtra);
            else if (t == typeof(XJADataExtraEx)) JaEx.Add(data as XJADataExtraEx);
            else if (t == typeof(XManipulationDataExtra)) ManipulationEx.Add(data as XManipulationDataExtra);
            else if (t == typeof(XHitDataExtraEx)) HitEx.Add(data as XHitDataExtraEx);
            else if (t == typeof(XResultDataExtraEx)) ResultEx.Add(data as XResultDataExtraEx);
            else if (t == typeof(XChargeDataExtraEx)) ChargeEx.Add(data as XChargeDataExtraEx);
            else if (t == typeof(XCombinedDataExtraEx)) CombinedEx.Add(data as XCombinedDataExtraEx);
        }
    }

    [Serializable]
    public class XBaseDataExtra { }

    [Serializable]
    public class XResultDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Result_Ratio = 0;
    }

    [Serializable]
    public class XChargeDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Charge_Ratio = 0;
        [SerializeField]
        public float Charge_End_Ratio = 0;
    }

    [Serializable]
    public class XChargeDataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public GameObject Charge_Curve_Prefab_Forward = null;
        [SerializeField]
        public AnimationCurve Charge_Curve_Forward = null;
        [SerializeField]
        public GameObject Charge_Curve_Prefab_Side = null;
        [SerializeField]
        public AnimationCurve Charge_Curve_Side = null;
        [SerializeField]
        public GameObject Charge_Curve_Prefab_Up = null;
        [SerializeField]
        public AnimationCurve Charge_Curve_Up = null;
    }

    [Serializable]
    public class XUltraDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public string ultra_end_Skill_PathWithName = null;
    }

    [Serializable]
    public class XResultDataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public GameObject BulletPrefab = null;
        [SerializeField]
        public GameObject BulletEndFx = null;
        [SerializeField]
        public GameObject BulletHitGroundFx = null;
    }

    [Serializable]
    public class XJADataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public XSkillData Next = null;
        [SerializeField]
        public XSkillData Ja = null;
    }

    [Serializable]
    public class XCombinedDataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public XSkillData Skill = null;
        [SerializeField]
        public AnimationClip Clip = null;
    }

    [Serializable]
    public class XCameraEffectDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XJADataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float JA_Begin_Ratio = 0;
        [SerializeField]
        public float JA_End_Ratio = 0;
        [SerializeField]
        public float JA_Point_Ratio = 0;
        [SerializeField]
        public string JA_Skill_PathWithName = null;
        [SerializeField]
        public string Next_Skill_PathWithName = null;
    }

    [Serializable]
    public class XCombinedDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float From_Ratio = 0;
        [SerializeField]
        public float To_Ratio = 0;
        [SerializeField]
        public string Skill_PathWithName = null;
    }

    [Serializable]
    public class XCameraMotionDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public AnimationClip Motion3D = null;
        [SerializeField]
        public AnimationClip Motion2_5D = null;
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XCameraPostEffectDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public string EffectLocation = null;
    }

    [Serializable]
    public class XMobUnitDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XChainCastExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XCameraPostEffectDataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public UnityEngine.Object Effect = null;
        [SerializeField]
        public UnityEngine.Shader Shader = null;
        [SerializeField]
        public float At_Ratio = 0;
        [SerializeField]
        public float End_Ratio = 0;
        [SerializeField]
        public float Solid_At_Ratio = 0;
        [SerializeField]
        public float Solid_End_Ratio = 0;
    }

    [Serializable]
    public class XQTEDataExtra
    {
        [SerializeField]
        public float QTE_At_Ratio = 0;
        [SerializeField]
        public float QTE_End_Ratio = 0;
    }

    [Serializable]
    public class XLogicalDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float Not_Move_At_Ratio = 0;
        [SerializeField]
        public float Not_Move_End_Ratio = 0;
        [SerializeField]
        public float Not_Dash_At_Ratio = 0;
        [SerializeField]
        public float Not_Dash_End_Ratio = 0;
        [SerializeField]
        public float Rotate_At_Ratio = 0;
        [SerializeField]
        public float Rotate_End_Ratio = 0;
        [SerializeField]
        public List<XQTEDataExtra> QTEDataEx = new List<XQTEDataExtra>();
        [SerializeField]
        public float Cancel_At_Ratio = 0;
        [SerializeField]
        public float Preserved_Ratio = 0;
        [SerializeField]
        public float Preserved_End_Ratio = 0;
        [SerializeField]
        public float ExString_Ratio = 0;
        [SerializeField]
        public float Not_Selected_At_Ratio = 0;
        [SerializeField]
        public float Not_Selected_End_Ratio = 0;
    }

    [Serializable]
    public class XFxDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public GameObject Fx = null;
        [SerializeField]
        public GameObject BindTo = null;
        [SerializeField]
        public float Ratio = 0;
        [SerializeField]
        public float End_Ratio = -1;
    }

    [Serializable]
    public class XManipulationDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public float At_Ratio = 0;
        [SerializeField]
        public float End_Ratio = 0;
        [SerializeField]
        public bool Present = true;
    }

    [Serializable]
    public class XWarningDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public GameObject Fx = null;
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XAudioDataExtra : XBaseDataExtra
    {
        [SerializeField]
        public AudioClip audio = null;
        [SerializeField]
        public float Ratio = 0;
    }

    [Serializable]
    public class XHitDataExtraEx : XBaseDataExtra
    {
        [SerializeField]
        public GameObject Fx = null;
    }
}
#endif