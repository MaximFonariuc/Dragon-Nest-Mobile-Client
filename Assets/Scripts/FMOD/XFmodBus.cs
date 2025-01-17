using UnityEngine;
using XUtliPoolLib;

public class XFmodBus : MonoBehaviour, IXFmodBus
{
    FMOD.Studio.Bus bus;
    FMOD.Studio.VCA mainVCA;
    FMOD.Studio.VCA bgmVCA;
    FMOD.Studio.VCA sfxVCA;
    FMODUnity.StudioEventEmitter e;

    //public void SetMute(bool mute)
    //{
    //    GetBus();
    //    bus.setMute(mute);
    //}

    public void SetBusVolume(string strBus, float volume)
    {
        FMOD.Studio.Bus bus;
        FMODUnity.RuntimeManager.StudioSystem.getBus(strBus, out bus);

        if (bus != null) bus.setVolume(volume);
    }

    //public void SetFaderLevel(float volume)
    //{
    //    GetBus();
    //    bus.setFaderLevel(volume);
    //}

    //public float GetFaderLevel()
    //{
    //    GetBus();
    //    float f = 0;
    //    bus.getFaderLevel(out f);
    //    return f;
    //}

    //protected void GetBus()
    //{
    //    if (bus == null)
    //        bus = FMOD_StudioSystem.instance.GetBus("bus:/");
    //}

    public void SetMainVolume(float volume)
    {
        if (mainVCA == null)
            FMODUnity.RuntimeManager.StudioSystem.getVCA("vca:/Main Volume Control", out mainVCA);

        if (mainVCA != null) mainVCA.setVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmVCA == null)
            FMODUnity.RuntimeManager.StudioSystem.getVCA("vca:/BGM Volume Control", out bgmVCA);

        if (bgmVCA != null) bgmVCA.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxVCA == null)
            FMODUnity.RuntimeManager.StudioSystem.getVCA("vca:/SFX Volume Control", out sfxVCA);

        if (sfxVCA != null) sfxVCA.setVolume(volume);
    }

    public void PlayOneShot(string key, Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(key, pos);
    }

    public void StartEvent(string key)
    {
        if (e == null)
            e = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();

        e.Event = key;
        e.CacheEventInstance();
        e.Play();
    }

    public void StopEvent()
    {
        if (e == null) return;

        e.Stop();
    }

    public bool Deprecated
    {
        get;
        set;
    }
}
