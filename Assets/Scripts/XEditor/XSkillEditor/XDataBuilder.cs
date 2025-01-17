#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace XEditor
{
    public class XDataBuilder : XSingleton<XDataBuilder>
	{
        public static GameObject hoster = null;
        public static DateTime Time;
        public static string prefixPath = "";

        public void Load(string pathwithname)
        {
            try
            {
                XSkillHoster.Quit = false;
                XConfigData conf = XDataIO<XConfigData>.singleton.DeserializeData(XEditorPath.GetCfgFromSkp(pathwithname));
                GameObject prefab = XAnimationLibrary.GetDummy((uint)conf.Player);

                if (prefab == null)
                {
                    Debug.Log("<color=red>Prefab not found by id: " + conf.Player + "</color>");
                }

                ColdBuild(prefab, conf);

                prefixPath = pathwithname.Substring(0, pathwithname.IndexOf("/SkillPackage"));
                Time = File.GetLastWriteTime(pathwithname);
            }
            catch (Exception e)
            {
                Debug.Log("<color=red>Error occurred during loading config file: " + pathwithname + " with error " + e.Message + "</color>");
            }
        }

        public void HotBuild(XSkillHoster hoster, XConfigData conf)
        {
            hoster.SkillDataExtra.JaEx.Clear();
            if (conf.Ja != null)
            {
                foreach (XJADataExtra ja in conf.Ja)
                {
                    XJADataExtraEx jaex = new XJADataExtraEx();

                    if (ja.Next_Skill_PathWithName != null && ja.Next_Skill_PathWithName.Length > 0)
                    {
                        XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData("Assets/Resources/" + ja.Next_Skill_PathWithName);
                        jaex.Next = skill;
                    }

                    if (ja.JA_Skill_PathWithName != null && ja.JA_Skill_PathWithName.Length > 0)
                    {
                        XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData("Assets/Resources/" + ja.JA_Skill_PathWithName);
                        jaex.Ja = skill;
                    }

                    hoster.SkillDataExtra.JaEx.Add(jaex);
                }
            }
            
            if(hoster.SkillData.TypeToken == 3)
            {
                hoster.SkillDataExtra.CombinedEx.Clear();
                hoster.SkillDataExtra.SkillClip_Frame = 0;

                if (conf.Combined != null)
                {
                    foreach (XCombinedDataExtra combine in conf.Combined)
                    {
                        XCombinedDataExtraEx combineex = new XCombinedDataExtraEx();

                        if (combine.Skill_PathWithName != null && combine.Skill_PathWithName.Length > 0)
                        {
                            XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData("Assets/Resources/" + combine.Skill_PathWithName);
                            combineex.Skill = skill;
                            combineex.Clip = Resources.Load(skill.ClipName, typeof(AnimationClip)) as AnimationClip;

                            hoster.SkillDataExtra.CombinedEx.Add(combineex);
                            hoster.SkillDataExtra.SkillClip_Frame += (combineex.Clip.length / (1.0f / 30.0f));
                        }
                    }
                }
            }
        }

        public void HotBuildEx(XSkillHoster hoster, XConfigData conf)
        {
            XSkillDataExtra edata = hoster.SkillDataExtra;
            XSkillData data = hoster.SkillData;

            edata.ResultEx.Clear();
            edata.ChargeEx.Clear();
            edata.Fx.Clear();
            edata.Audio.Clear();
            edata.HitEx.Clear();
            edata.ManipulationEx.Clear();

            if (data.Result != null)
            {
                foreach (XResultData result in data.Result)
                {
                    XResultDataExtraEx rdee = new XResultDataExtraEx();
                    if (result.LongAttackEffect)
                    {
                        rdee.BulletPrefab = Resources.Load(result.LongAttackData.Prefab) as GameObject;
                        rdee.BulletEndFx = Resources.Load(result.LongAttackData.End_Fx) as GameObject;
                        rdee.BulletHitGroundFx = Resources.Load(result.LongAttackData.HitGround_Fx) as GameObject;
                    }
                    edata.ResultEx.Add(rdee);
                }
            }

            if (data.Charge != null)
            {
                foreach (XChargeData charge in data.Charge)
                {
                    XChargeDataExtraEx cdee = new XChargeDataExtraEx();
                    cdee.Charge_Curve_Prefab_Forward = Resources.Load(charge.Curve_Forward) as GameObject;
                    cdee.Charge_Curve_Forward = cdee.Charge_Curve_Prefab_Forward == null ? null : cdee.Charge_Curve_Prefab_Forward.GetComponent<XCurve>().Curve;

                    cdee.Charge_Curve_Prefab_Side = Resources.Load(charge.Curve_Side) as GameObject;
                    cdee.Charge_Curve_Side = cdee.Charge_Curve_Prefab_Side == null ? null : cdee.Charge_Curve_Prefab_Side.GetComponent<XCurve>().Curve;

                    if (charge.Using_Up)
                    {
                        cdee.Charge_Curve_Prefab_Up = Resources.Load(charge.Curve_Up) as GameObject;
                        cdee.Charge_Curve_Up = cdee.Charge_Curve_Prefab_Up == null ? null : cdee.Charge_Curve_Prefab_Up.GetComponent<XCurve>().Curve;
                    }

                    edata.ChargeEx.Add(cdee);
                }
            }

            if (data.Manipulation != null)
            {
                foreach (XManipulationData manipulation in data.Manipulation)
                {
                    XManipulationDataExtra me = new XManipulationDataExtra();

                    edata.ManipulationEx.Add(me);
                }
            }

            if (data.Hit != null)
            {
                foreach (XHitData hit in data.Hit)
                {
                    XHitDataExtraEx hee = new XHitDataExtraEx();
                    hee.Fx = Resources.Load(hit.Fx) as GameObject;

                    edata.HitEx.Add(hee);
                }
            }

            if (data.Fx != null)
            {
                foreach (XFxData fx in data.Fx)
                {
                    XFxDataExtra fxe = new XFxDataExtra();
                    fxe.Fx = Resources.Load(fx.Fx) as GameObject;
                    if (fx.Bone != null && fx.Bone.Length > 0)
                    {
                        Transform attachPoint = hoster.gameObject.transform.Find(fx.Bone);
                        if (attachPoint != null)
                        {
                            fxe.BindTo = attachPoint.gameObject;
                        }
                        else
                        {
                            int index = fx.Bone.LastIndexOf("/");
                            if (index >= 0)
                            {
                                string bone = fx.Bone.Substring(index + 1);
                                attachPoint = hoster.gameObject.transform.Find(bone);
                                if (attachPoint != null)
                                {
                                    fxe.BindTo = attachPoint.gameObject;
                                }
                            }

                        }
                    }

                    fxe.Ratio = fx.At / data.Time;

                    edata.Fx.Add(fxe);
                }
            }

            if (data.Warning != null)
            {
                foreach (XWarningData warning in data.Warning)
                {
                    XWarningDataExtra we = new XWarningDataExtra();
                    we.Fx = Resources.Load(warning.Fx) as GameObject;
                    we.Ratio = warning.At / data.Time;

                    edata.Warning.Add(we);
                }
            }

            if (data.Mob != null)
            {
                foreach (XMobUnitData mob in data.Mob)
                {
                    XMobUnitDataExtra me = new XMobUnitDataExtra();
                    me.Ratio = mob.At / data.Time;

                    edata.Mob.Add(me);
                }
            }

            if (data.Audio != null)
            {
                foreach (XAudioData au in data.Audio)
                {
                    XAudioDataExtra aue = new XAudioDataExtra();
                    aue.audio = Resources.Load(au.Clip) as AudioClip;
                    aue.Ratio = au.At / data.Time;

                    edata.Audio.Add(aue);
                }
            }

            if (data.CameraMotion != null)
            {
                edata.MotionEx = new XCameraMotionDataExtra();
                edata.MotionEx.Motion3D = Resources.Load(data.CameraMotion.Motion3D, typeof(AnimationClip)) as AnimationClip;
                edata.MotionEx.Motion2_5D = Resources.Load(data.CameraMotion.Motion2_5D, typeof(AnimationClip)) as AnimationClip;
                edata.MotionEx.Ratio = data.CameraMotion.At / data.Time;
            }

            if (data.CameraPostEffect != null)
            {
                edata.PostEffectEx = new XCameraPostEffectDataExtraEx();
                edata.PostEffectEx.Effect = AssetDatabase.LoadAssetAtPath(conf.PostEffect.EffectLocation, typeof(UnityEngine.Object));
                //edata.PostEffectEx.Shader = Resources.Load(data.CameraPostEffect.Shader, typeof(UnityEngine.Shader)) as UnityEngine.Shader;
                edata.PostEffectEx.At_Ratio = data.CameraPostEffect.At / data.Time;
                edata.PostEffectEx.End_Ratio = data.CameraPostEffect.At / data.Time;
            }
        }

        public void ColdBuild(GameObject prefab, XConfigData conf)
        {
            if (hoster != null) GameObject.DestroyImmediate(hoster);

            hoster = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            hoster.transform.localScale = Vector3.one * XAnimationLibrary.AssociatedAnimations((uint)conf.Player).Scale;

            hoster.AddComponent<XSkillHoster>();

            CharacterController cc = hoster.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            UnityEngine.AI.NavMeshAgent agent = hoster.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            XSkillHoster component = hoster.GetComponent<XSkillHoster>();

            string directory = conf.Directory[conf.Directory.Length - 1] == '/' ? conf.Directory.Substring(0, conf.Directory.Length - 1) : conf.Directory;
            string path = XEditorPath.GetPath("SkillPackage" + "/" + directory);

            component.ConfigData = conf;
            component.SkillData = XDataIO<XSkillData>.singleton.DeserializeData(path + conf.SkillName + ".txt");

            component.SkillDataExtra.ScriptPath = path;
            component.SkillDataExtra.ScriptFile = conf.SkillName;

            component.SkillDataExtra.SkillClip = RestoreClip(conf.SkillClip, conf.SkillClipName);

            if (component.SkillData.TypeToken != 3)
            {
                if (component.SkillData.Time == 0)
                    component.SkillData.Time = component.SkillDataExtra.SkillClip.length;
            }

            HotBuild(component, conf);
            HotBuildEx(component, conf);

            EditorGUIUtility.PingObject(hoster);
            Selection.activeObject = hoster;
        }

        public void Update(XSkillHoster hoster)
        {
            string pathwithname = hoster.SkillDataExtra.ScriptPath + hoster.ConfigData.SkillName + ".txt";

            DateTime time = File.GetLastWriteTime(pathwithname);

            if (Time == default(DateTime)) Time = time;

            if (time != Time)
            {
                Time = time;

                if(EditorUtility.DisplayDialog("WARNING!",
                                            "Skill has been Modified outside, Press 'OK' to reload file or 'Ignore' to maintain your change. (Make sure the '.config' file for skill script has been well synchronized)",
                                            "Ok", "Ignore"))
                {
                    hoster.ConfigData = XDataIO<XConfigData>.singleton.DeserializeData(XEditorPath.GetCfgFromSkp(pathwithname));
                    hoster.SkillData = XDataIO<XSkillData>.singleton.DeserializeData(pathwithname);

                    XDataBuilder.singleton.HotBuild(hoster, hoster.ConfigData);
                    XDataBuilder.singleton.HotBuildEx(hoster, hoster.ConfigData);
                }
            }
        }

        private AnimationClip RestoreClip(string path, string name)
        {
            if (path == null || name == null || path == "" || name == "") return null;

            int last = path.LastIndexOf('.');
            string subfix = path.Substring(last, path.Length - last).ToLower();

            if (subfix == ".fbx")
            {
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (UnityEngine.Object obj in objs)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (clip != null && clip.name == name)
                        return clip;
                }
            }
            else if (subfix == ".anim")
            {
				return AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                
            }
            else
                return null;

            return null;
        }
	}
}
#endif