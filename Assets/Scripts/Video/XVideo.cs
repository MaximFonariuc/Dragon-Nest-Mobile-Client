using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using XUtliPoolLib;

public class XVideo : MonoBehaviour
{
    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private VideoPlayer _movieTexture = null;
    private AudioSource _audio = null;

    private bool _avoid_serialize_bug = false;
    private bool _is_waiting = false;

    public bool isPlaying { get { return _is_waiting || (_movieTexture != null && _movieTexture.isPlaying); } }

    public void Stop()
    {
        if (_movieTexture.isPlaying)
        {
            _movieTexture.Stop();
            if (_audio != null)
                _audio.Stop();
        }

        _is_waiting = false;

        _movieTexture = null;
        _audio = null;
    }

    public void Play(VideoPlayer movie, AudioSource audio, bool loop)
    {
        _movieTexture = movie;
        if (_movieTexture != null)
        {
            _movieTexture.isLooping = loop;
            _audio = audio;

            _avoid_serialize_bug = false;
            _is_waiting = true;

            StartCoroutine(StartStream());
        }

    }

    protected IEnumerator StartStream()
    {
        if (_movieTexture != null)
        {
            _movieTexture.Stop();
            _movieTexture.Play();

            if (_movieTexture.clip != null)
            {
                /*
                _audio.clip = _movieTexture.clip;
                */
                _audio.Play();
            }

            yield return null;

            _avoid_serialize_bug = true;
        }
    }

    void OnGUI()
    {
        if (_movieTexture != null && _movieTexture.isPlaying && _avoid_serialize_bug)
        {
            _is_waiting = false;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _movieTexture.texture, ScaleMode.ScaleToFit);
        }
    }
#else
    public bool isDownloading { get { return false; } }
    public bool isPlaying { get { return false; } }
#endif
}
