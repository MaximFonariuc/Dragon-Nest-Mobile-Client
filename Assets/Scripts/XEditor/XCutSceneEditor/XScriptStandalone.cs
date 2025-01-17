#if UNITY_EDITOR
using UnityEngine;
using XEditor;
using XUtliPoolLib;
using System.Collections.Generic;
using System.Collections;

public class XScriptStandalone : MonoBehaviour 
{
    private XCutSceneCamera _cut_scene_camera = null;
    private List<XActor> _actors = new List<XActor>();

    private uint _token = 0;

    [SerializeField]
    public XCutSceneData _cut_scene_data = null;

    public GameObject _ui_root;

	// Use this for initialization
	void Start () 
    {
        _cut_scene_camera = new XCutSceneCamera();
        _cut_scene_camera.Initialize();

        XCameraMotionData m = new XCameraMotionData();
        m.AutoSync_At_Begin = false;
        m.Coordinate = CameraMotionSpace.World;
        m.Follow_Position = _cut_scene_data.GeneralShow;
        m.LookAt_Target = false;
        m.At = 0;
        m.Motion = _cut_scene_data.CameraClip;

        _cut_scene_camera.Effect(m);
        _cut_scene_camera.UnityCamera.fieldOfView = _cut_scene_data.FieldOfView;

        XCutSceneUI.singleton.Init();
        XCutSceneUI.singleton.SetText("");

        if (!_cut_scene_data.GeneralShow)
        {
            foreach (XActorDataClip clip in _cut_scene_data.Actors)
            {
                XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clip.Clip, ".anim");
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f - 0.016f, BeOnStage, clip);
            }

            foreach (XPlayerDataClip clip in _cut_scene_data.Player)
            {
                XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clip.Clip1, ".anim");
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f - 0.016f, BePlayerOnStage, clip);
            }

            foreach (XFxDataClip clip in _cut_scene_data.Fxs)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f, Fx, clip);
            }
        }

        foreach (XAudioDataClip clip in _cut_scene_data.Audios)
        {
            XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f, Audio, clip);
        }

        if (_cut_scene_data.AutoEnd) XTimerMgr.singleton.SetTimer((_cut_scene_data.TotalFrame - 30) / 30.0f, EndShow, null);

        if (_cut_scene_data.Mourningborder)
        {
            XCutSceneUI.singleton.SetVisible(true);

            foreach (XSubTitleDataClip clip in _cut_scene_data.SubTitle)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f, SubTitle, clip);
            }

            foreach (XSlashDataClip clip in _cut_scene_data.Slash)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / 30.0f, Slash, clip);
            }
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        XTimerMgr.singleton.Update(Time.deltaTime);

        foreach (XActor actor in _actors)
            actor.Update(Time.deltaTime);
	}

    void BePlayerOnStage(object o)
    {
        XPlayerDataClip clip = o as XPlayerDataClip;
        _actors.Add(new XActor(clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip1));
    }

    void BeOnStage(object o)
    {
        XActor target = null;

        XActorDataClip clip = o as XActorDataClip;
        if(clip.bUsingID)
            target = new XActor((uint)clip.StatisticsID, clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip);
        else
            target = new XActor(clip.Prefab, clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip);

        _actors.Add(target);
    }

    void Fx(object o)
    {
        XFxDataClip clip = o as XFxDataClip;

        Transform transform = (clip.BindIdx < 0) ? null : _actors[clip.BindIdx].Actor.transform;
        if (clip.Bone != null && clip.Bone.Length > 0)
            transform = transform.Find(clip.Bone);
        else
            transform = null;

        XFx fx = XFxMgr.singleton.CreateFx(clip.Fx);

        fx.DelayDestroy = clip.Destroy_Delay;
        if (transform != null)
            fx.Play(transform,Vector3.zero, clip.Scale * Vector3.one, 1, clip.Follow);
        else
            fx.Play(new Vector3(clip.AppearX, clip.AppearY, clip.AppearZ), XCommon.singleton.FloatToQuaternion(clip.Face),Vector3.one);
    }

    void Audio(object o)
    {
        XAudioDataClip clip = o as XAudioDataClip;

        if (clip.BindIdx < 0) return;

        //AudioClip audio = XResourceLoaderMgr.singleton.GetSharedResource<AudioClip>(clip.Clip);
        //AudioSource source = _actors[clip.BindIdx].GetAudioSourceByChannel(clip.Channel);

        XFmod fmod = _actors[clip.BindIdx].Actor.GetComponent<XFmod>();
        if(fmod == null)
            fmod = _actors[clip.BindIdx].Actor.AddComponent<XFmod>();

        fmod.StartEvent("event:/" + clip.Clip, clip.Channel);
        //source.Stop();

        //source.clip = audio;
        //source.volume = clip.Volume;
        //source.loop = clip.Loop;
        //source.Play();
    }

    void SubTitle(object o)
    {
        XSubTitleDataClip clip = o as XSubTitleDataClip;

        XCutSceneUI.singleton.SetText(clip.Context);
        XTimerMgr.singleton.KillTimer(_token);

        _token = XTimerMgr.singleton.SetTimer(clip.Duration / 30.0f, EndSubTitle, null);
    }

    void Slash(object o)
    {
        XSlashDataClip clip = o as XSlashDataClip;

        XCutSceneUI.singleton.SetIntroText(true, clip.Name, clip.Discription, clip.AnchorX, clip.AnchorY);

        XTimerMgr.singleton.SetTimer(clip.Duration, EndSlash, null);
    }

    void EndShow(object o)
    {
        XTimerMgr.singleton.KillTimer(_token);
    }

    void EndSlash(object o)
    {
        XCutSceneUI.singleton.SetIntroText(false, "", "", 0, 0);
    }

    void EndSubTitle(object o)
    {
        XCutSceneUI.singleton.SetText("");
    }

    void LateUpdate()
    {
        _cut_scene_camera.PostUpdate(Time.deltaTime);
    }
}
#endif