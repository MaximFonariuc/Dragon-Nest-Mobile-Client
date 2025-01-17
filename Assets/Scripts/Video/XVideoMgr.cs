using UnityEngine;
using XUtliPoolLib;
using System.Collections;
using System;
using UnityEngine.Video;

public class XVideoMgr : MonoBehaviour, IXVideo
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    public VideoPlayer Movie;
#endif

    private XVideo _video = null;
    private AudioSource _audio = null;

    public bool isPlaying { get { return _video != null && _video.isPlaying; } }

    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        GameObject root = GameObject.Find(@"XGamePoint");

        _video = root.AddComponent<XVideo>();
        _audio = root.AddComponent<AudioSource>();
#endif
    }

    public void Play(bool loop = false)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        {
            _video.Play(Movie, _audio, loop);
        }
#else
        {
            Handheld.PlayFullScreenMovie("CG.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
        }
#endif
    }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    void Update()
    {
        if (_video.isPlaying && Input.GetMouseButtonUp(0))
        {
            _video.Stop();
        }
    }
#endif
    public void Stop()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        {
            if (_video != null) _video.Stop();
        }
#endif
    }

    public bool Deprecated
    {
        get;
        set;
    }
}
