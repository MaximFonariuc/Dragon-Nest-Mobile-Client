﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
    public class StudioEventEmitter : MonoBehaviour
    {
        [EventRef]
        public String Event = "";
        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;
        public String CollisionTag = "";
        public bool AllowFadeout = true;
        public bool TriggerOnce = false;
        public bool Preload = false;
        public ParamRef[] Params = new ParamRef[0];
        public bool OverrideAttenuation = false;
        public float OverrideMinDistance = -1.0f;
        public float OverrideMaxDistance = -1.0f;

        
        private FMOD.Studio.EventDescription eventDescription = null;
        private FMOD.Studio.EventInstance instance = null;
        private bool hasTriggered = false;
        private bool isQuitting = false;

        void Start() 
        {
            RuntimeUtils.EnforceLibraryOrder();
            if (Preload)
            {
                Lookup();
                eventDescription.loadSampleData();
                RuntimeManager.StudioSystem.update();
                FMOD.Studio.LOADING_STATE loadingState;
                eventDescription.getSampleLoadingState(out loadingState);
                while(loadingState == FMOD.Studio.LOADING_STATE.LOADING)
                {
                    System.Threading.Thread.Sleep(1);
                    eventDescription.getSampleLoadingState(out loadingState);
                }
            }
            HandleGameEvent(EmitterGameEvent.ObjectStart);
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(EmitterGameEvent.ObjectDestroy);
                if (instance != null && instance.isValid())
                {
                    RuntimeManager.DetachInstanceFromGameObject(instance);
                }

                if (Preload)
                {
                    eventDescription.unloadSampleData();
                }
            }
        }

        void OnEnable()
        {
            HandleGameEvent(EmitterGameEvent.ObjectEnable);
        }

        void OnDisable()
        {
            HandleGameEvent(EmitterGameEvent.ObjectDisable);
        }

        void OnTriggerEnter(Collider other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerEnter);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerExit);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerEnter2D);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (String.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(EmitterGameEvent.TriggerExit2D);
            }
        }

        void OnCollisionEnter()
        {
            HandleGameEvent(EmitterGameEvent.CollisionEnter);
        }

        void OnCollisionExit()
        {
            HandleGameEvent(EmitterGameEvent.CollisionExit);
        }

        void OnCollisionEnter2D()
        {
            HandleGameEvent(EmitterGameEvent.CollisionEnter2D);
        }

        void OnCollisionExit2D()
        {
            HandleGameEvent(EmitterGameEvent.CollisionExit2D);
        }

        void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                Play();
            }
            if (StopEvent == gameEvent)
            {
                Stop();
            }
        }

        void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(Event);
        }

        public void Play()
        {
            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (String.IsNullOrEmpty(Event))
            {
                return;
            }

            if (eventDescription == null)
            {
                Lookup();
            }

            bool isOneshot = false;
            if (!Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
            {
                eventDescription.isOneshot(out isOneshot);
            }
            bool is3D;
            eventDescription.is3D(out is3D);

            if (instance != null && !instance.isValid())
            {
                instance = null;
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance != null)
            {
                instance.release();
                instance = null;
            }

            if (instance == null)
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var rigidBody = GetComponent<Rigidbody>();
                    var rigidBody2D = GetComponent<Rigidbody2D>();
                    var transform = GetComponent<Transform>();
                    if (rigidBody)
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                }
            }

            foreach(var param in Params)
            {
                instance.setParameterValue(param.Name, param.Value);
            }

            if (is3D && OverrideAttenuation)
            {
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
            }

            instance.start();

            hasTriggered = true;

        }

        public void Stop()
        {
            if (instance != null)
            {
                instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
                instance = null;
            }
        }
        
        public void SetParameter(string name, float value)
        {
            if (instance != null)
            {
                instance.setParameterValue(name, value);
            }
        }
        
        public bool IsPlaying()
        {
            if (instance != null && instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
            }
            return false;
        }

        public void Update3DAttributes(Vector3 pos)
        {
            if (instance != null)
            {
                FMOD.ATTRIBUTES_3D attr = pos.To3DAttributes();
                instance.set3DAttributes(attr);
            }
            
        }

        public void CacheEventInstance()
        {
            if (String.IsNullOrEmpty(Event))
            {
                return;
            }

            Lookup();
        }
    }

    public class RuntimeStudioEventEmitter
    {
        public String Event = "";

        public List<ParamRef> Params = null;
        public Vector3 Pos = Vector3.zero;

        private FMOD.Studio.EventDescription eventDescription = null;
        private FMOD.Studio.EventInstance instance = null;
        private bool hasTriggered = false;
        public static bool isQuitting = false;

        public void Reset()
        {
            Event = "";
            Params = null;
            Pos = Vector3.zero;

            eventDescription = null;
            instance = null;
            hasTriggered = false;
            isQuitting = false;
        }

        public void Set3DPos(GameObject go, Rigidbody rigidbody)
        {
            if (instance != null)
            {
                instance.set3DAttributes(RuntimeUtils.To3DAttributes(go, rigidbody));
                //RuntimeManager.AttachInstanceToGameObject(instance, go.transform, rigidbody);
            }
        }

        public void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
        }

        void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(Event);
        }

        public void Play(GameObject go, Rigidbody rigidbody)
        {
            if (String.IsNullOrEmpty(Event))
            {
                return;
            }

            if (eventDescription == null)
            {
                Lookup();
            }

            bool isOneshot = false;
            if (!Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
            {
                eventDescription.isOneshot(out isOneshot);
            }
            bool is3D;
            eventDescription.is3D(out is3D);

            if (instance != null && !instance.isValid())
            {
                instance = null;
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance != null)
            {
                instance.release();
                instance = null;
            }

            if (instance == null)
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D && go != null)
                {
                    Set3DPos(go, rigidbody);
                }

                if (is3D && Pos != Vector3.zero)
                {
                    FMOD.ATTRIBUTES_3D attr = Pos.To3DAttributes();
                    instance.set3DAttributes(attr);
                }

                if (Params != null)
                {
                    for (int i = 0; i < Params.Count; ++i)
                    {
                        instance.setParameterValue(Params[i].Name, Params[i].Value);
                    }
                }
            }

            instance.start();

            hasTriggered = true;

        }

        public void Stop()
        {
            if (instance != null)
            {
                //instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
                instance = null;
            }
        }

        //public void SetParameter(string name, float value)
        //{
        //    if (instance != null)
        //    {
        //        instance.setParameterValue(name, value);
        //    }
        //}

        public bool IsPlaying()
        {
            if (instance != null && instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
            }
            return false;
        }

        public void Update3DAttributes(Vector3 pos)
        {
            Pos = pos;
        }

        public void CacheEventInstance()
        {
            if (String.IsNullOrEmpty(Event))
            {
                return;
            }

            Lookup();
        }
    }
}
