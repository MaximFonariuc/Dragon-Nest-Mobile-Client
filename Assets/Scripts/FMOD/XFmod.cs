using UnityEngine;
using System.Collections;
using XUtliPoolLib;
using FMODUnity;
using System.Collections.Generic;

public class XFmod : MonoBehaviour ,IXFmod
{
    private FMODUnity.StudioEventEmitter _emitter = null;
    private FMODUnity.StudioEventEmitter _emitter2 = null;
    private FMODUnity.StudioEventEmitter _emitter3 = null;
    private FMODUnity.StudioEventEmitter _emitter4 = null;
    private FMODUnity.StudioEventEmitter _emitter5 = null;

    private Vector3 _3dPos = Vector3.zero;

    public void Destroy()
    {
        if (_emitter != null)
        {
            _emitter.Stop();
            _emitter = null;
        }

        if (_emitter2 != null)
        {
            _emitter2.Stop();
            _emitter2 = null;
        }

        if (_emitter3 != null)
        {
            _emitter3.Stop();
            _emitter3 = null;
        }

        if (_emitter4 != null)
        {
            _emitter4.Stop();
            _emitter4 = null;
        }
    }

    public bool IsPlaying(AudioChannel channel)
    {
        FMODUnity.StudioEventEmitter e = GetEmitter(channel);

        return e!=null && !e.IsPlaying();
    }

    public void StartEvent(string key, AudioChannel channel = AudioChannel.Action, bool stopPre = true, string para = "", float value = 0)
    {
        FMODUnity.StudioEventEmitter e = GetEmitter(channel);
		if (e == null)
            return;

        if (stopPre) e.Stop();

        if (!string.IsNullOrEmpty(key))
        {
            e.Event = key;
        }
        
        e.CacheEventInstance();

        SetParamValue(channel, para, value);

        //if (_3dPos != Vector3.zero)
        //{
        //    e.Update3DAttributes(_3dPos);
        //    _3dPos = Vector3.zero;
        //}

        e.Play();
    }
    
    public void Play(AudioChannel channel = AudioChannel.Action)
    {
        FMODUnity.StudioEventEmitter e = GetEmitter(channel);

        if (e != null)
            e.Play();
    }

    public void Stop(AudioChannel channel = AudioChannel.Action)
    {
        FMODUnity.StudioEventEmitter e = GetEmitter(channel);

        if (e != null)
            e.Stop();
    }

    public void PlayOneShot(string key, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(key, pos);
    }

    public void Update3DAttributes(Vector3 vec, AudioChannel channel = AudioChannel.Action)
    {
        _3dPos = vec;
    }

    public void SetParamValue(AudioChannel channel, string param, float value)
    {
        if (!string.IsNullOrEmpty(param))
        {
            FMODUnity.StudioEventEmitter e = GetEmitter(channel);
            if(e!=null)
            {
                FMODUnity.ParamRef fmodParam = new ParamRef();
                fmodParam.Name = param;
                fmodParam.Value = value;

                if (e.Params == null)
                {
                    e.Params = new ParamRef[1];
                    e.Params[0].Name = param;
                    e.Params[0].Value = value;
                }
            }
            
        }   
    }

    public FMODUnity.StudioEventEmitter GetEmitter(AudioChannel channel)
    {
#if !DISABLE_FMODE
        switch (channel)
        {
            case AudioChannel.Action:
                {
                    if (_emitter == null)
                    {
                        _emitter = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                        _emitter.StopEvent = EmitterGameEvent.ObjectDestroy;
                    }

                    return _emitter;
                }
            case AudioChannel.Motion:
                {
                    if (_emitter2 == null)
                    {
                        _emitter2 = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                        _emitter2.StopEvent = EmitterGameEvent.ObjectDestroy;
                    }

                    return _emitter2;
                }
            case AudioChannel.Skill:
                {
                    if (_emitter3 == null)
                    {
                        _emitter3 = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                        _emitter3.StopEvent = EmitterGameEvent.ObjectDestroy;
                    }

                    return _emitter3;
                }
            case AudioChannel.Behit:
                {
                    if (_emitter4 == null)
                    {
                        _emitter4 = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                        _emitter4.StopEvent = EmitterGameEvent.ObjectDestroy;
                    }

                    return _emitter4;
                }
            case AudioChannel.SkillCombine:
                {
                    if (_emitter5 == null)
                    {
                        _emitter5 = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                        //_emitter5.StopEvent = EmitterGameEvent.ObjectDestroy;
                    }

                    return _emitter5;
                }

        }

#endif
        return null;
        
    }
    public void Init(GameObject go, Rigidbody rigidbody)
    {
    }
    public bool Deprecated
    {
        get;
        set;
    }
}



public class XRuntimeFmod : IXFmod
{
    private static Queue<RuntimeStudioEventEmitter> emitterQueue = new Queue<RuntimeStudioEventEmitter>();
    private static Queue<XRuntimeFmod> fmodQueue = new Queue<XRuntimeFmod>();

    private RuntimeStudioEventEmitter _emitter = null;
    private RuntimeStudioEventEmitter _emitter2 = null;
    private RuntimeStudioEventEmitter _emitter3 = null;
    private RuntimeStudioEventEmitter _emitter4 = null;
    private RuntimeStudioEventEmitter _emitter5 = null;

    private Vector3 _3dPos = Vector3.zero;
    public GameObject cachedGo;
    public Rigidbody cachedRigidBody;
    public static void Clear()
    {
        emitterQueue.Clear();
        fmodQueue.Clear();
    }
    public static XRuntimeFmod GetFMOD()
    {
        if (fmodQueue.Count > 0)
        {
            return fmodQueue.Dequeue();
        }
        return new XRuntimeFmod();
    }

    public static void ReturnFMOD(XRuntimeFmod fmod)
    {
        fmodQueue.Enqueue(fmod);
    }
    private static RuntimeStudioEventEmitter GetEmitter()
    {
        if (emitterQueue.Count > 0)
        {
            RuntimeStudioEventEmitter e = emitterQueue.Dequeue();
            e.Reset();
            return e;
        }
        return new RuntimeStudioEventEmitter();
    }

    private static void ReturnEmitter(RuntimeStudioEventEmitter e)
    {
        emitterQueue.Enqueue(e);
    }
    public void Init(GameObject go, Rigidbody rigidbody)
    {
        cachedGo = go;
        cachedRigidBody = rigidbody;
        //if (_emitter != null)
        //{
        //    _emitter.Set3DPos(cachedGo, cachedRigidBody);
        //}

        //if (_emitter2 != null)
        //{
        //    _emitter2.Set3DPos(cachedGo, cachedRigidBody);
        //}

        //if (_emitter3 != null)
        //{
        //    _emitter3.Set3DPos(cachedGo, cachedRigidBody);
        //}

        //if (_emitter4 != null)
        //{
        //    _emitter4.Set3DPos(cachedGo, cachedRigidBody);
        //}

        //if (_emitter5 != null)
        //{
        //    _emitter5.Set3DPos(cachedGo, cachedRigidBody);
        //}
    }

    public void Destroy()
    {
        if (_emitter != null)
        {
            _emitter.Stop();
            ReturnEmitter(_emitter);
            _emitter = null;
        }

        if (_emitter2 != null)
        {
            _emitter2.Stop();
            ReturnEmitter(_emitter2);
            _emitter2 = null;
        }

        if (_emitter3 != null)
        {
            _emitter3.Stop();
            ReturnEmitter(_emitter3);
            _emitter3 = null;
        }

        if (_emitter4 != null)
        {
            _emitter4.Stop();
            ReturnEmitter(_emitter4);
            _emitter4 = null;
        }

        if (_emitter5 != null)
        {
            _emitter5.Stop();
            ReturnEmitter(_emitter5);
            _emitter5 = null;
        }
    }

    public bool IsPlaying(AudioChannel channel)
    {
        RuntimeStudioEventEmitter e = GetEmitter(channel);

        return e != null && !e.IsPlaying();
    }

    public void StartEvent(string key, AudioChannel channel = AudioChannel.Action, bool stopPre = true, string para = "", float value = 0)
    {
        RuntimeStudioEventEmitter e = GetEmitter(channel);
        if (e == null)
            return;
        if (stopPre) e.Stop();

        if (!string.IsNullOrEmpty(key))
        {
            e.Event = key;
        }

        e.CacheEventInstance();

        SetParamValue(e, para, value);

        if (_3dPos != Vector3.zero)
        {
            e.Update3DAttributes(_3dPos);
            _3dPos = Vector3.zero;
        }

        e.Play(cachedGo, cachedRigidBody);
    }

    public void Play(AudioChannel channel = AudioChannel.Action)
    {
        RuntimeStudioEventEmitter e = GetEmitter(channel);

        if (e != null)
            e.Play(cachedGo, cachedRigidBody);
    }

    public void Stop(AudioChannel channel = AudioChannel.Action)
    {
        RuntimeStudioEventEmitter e = GetEmitter(channel);

        if (e != null)
            e.Stop();
    }

    public void PlayOneShot(string key, Vector3 pos)
    {
#if !DISABLE_FMODE
        RuntimeManager.PlayOneShot(key, pos);
#endif
    }

    public void Update3DAttributes(Vector3 vec, AudioChannel channel = AudioChannel.Action)
    {
        _3dPos = vec;
    }

    public void SetParamValue(RuntimeStudioEventEmitter e, string param, float value)
    {
        if (!string.IsNullOrEmpty(param))
        {
            FMODUnity.ParamRef fmodParam = new ParamRef();
            fmodParam.Name = param;
            fmodParam.Value = value;

            if (e.Params == null)
            {
                e.Params = new List<ParamRef>();
                e.Params.Add(fmodParam);
            }
        }
    }

    private RuntimeStudioEventEmitter GetEmitter(AudioChannel channel)
    {
#if !DISABLE_FMODE
        switch (channel)
        {
            case AudioChannel.Action:
                {
                    if (_emitter == null)
                    {
                        _emitter = GetEmitter();
                    }

                    return _emitter;
                }
            case AudioChannel.Motion:
                {
                    if (_emitter2 == null)
                    {
                        _emitter2 = GetEmitter();
                    }

                    return _emitter2;
                }
            case AudioChannel.Skill:
                {
                    if (_emitter3 == null)
                    {
                        _emitter3 = GetEmitter();
                    }

                    return _emitter3;
                }
            case AudioChannel.Behit:
                {
                    if (_emitter4 == null)
                    {
                        _emitter4 = GetEmitter();
                    }

                    return _emitter4;
                }
            case AudioChannel.SkillCombine:
                {
                    if (_emitter5 == null)
                    {
                        _emitter5 = GetEmitter();
                    }

                    return _emitter5;
                }
        }
#endif
        return null;

    }

    public bool Deprecated
    {
        get;
        set;
    }
}


