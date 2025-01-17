#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XEditor
{
    internal class XGesture : XSingleton<XGesture>
    {
        public enum SwypeDirectionType
        {
            Left, Right
        }

        private bool _one = false;
        private bool _double = false;
        private bool _bswype = false;

        private float _last_swype_at = 0;

        private float _start_at = 0;
        //private float _swype_start_at = 0;

        private Vector2 _start = Vector2.zero;
        private Vector2 _swype_start = Vector2.zero;

        private Vector2 _end = Vector2.zero;
        private XTouchItem _touch;

        private Vector3 _swypedir = Vector3.zero;
        private Vector3 _touchpos = Vector3.zero;

        private float _last_touch_at = 0;

        private SwypeDirectionType _swype_type;

        public bool Gestured
        {
            get { return _bswype || _one || _double; }
        }

        public bool OneTouch
        {
            get { return _one; }
        }

        public bool DoubleTouch
        {
            get { return _double; }
        }

        public bool Swype
        {
            get { return _bswype; }
        }

        public float LastSwypeAt
        {
            get { return _last_swype_at; }
        }

        public Vector3 SwypeDirection
        {
            get { return -_swypedir; }
        }

        public SwypeDirectionType SwypeType
        {
            get { return _swype_type; }
        }

        public Vector3 TouchPosition
        {
            get { return _touchpos; }
        }

        public void Update()
        {
            XTouch.singleton.Update();
            _touch = XTouch.singleton.GetTouch();

            if (_touch != null)
            {
                if (_touch.Phase == TouchPhase.Began)
                {
                    _start = _touch.Position;
                    _swype_start = _start;

                    _start_at = Time.time;
                    //_swype_start_at = _start_at;
                }

                _bswype = SwypeUpdate();
                _one = OneUpdate();
                _double = DoubleUpdate();
            }
            else 
            {
                Clear();
            }
        }

        private void Clear()
        {
            _one = false;
            _double = false;
            _bswype = false;
        }

        private bool OneUpdate()
        {
            TouchPhase phase = _touch.Phase;

            if ((phase == TouchPhase.Ended ||
                 phase == TouchPhase.Canceled))
            {
                Vector2 delta = _touch.Position - _start;

                float dist = Mathf.Sqrt(Mathf.Pow(delta.x, 2) + Mathf.Pow(delta.y, 2));
                float duration = Time.time - _start_at;

                if (dist < 5 && duration < 0.2f / Time.timeScale)
                {
                    _last_touch_at = Time.time;
                    _touchpos = _touch.Position;

                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        private bool DoubleUpdate()
        {
            if (_touch.Phase == TouchPhase.Began)
            {
                float deltaT = Time.time - _last_touch_at;
                if (XCommon.singleton.IsLess(deltaT, 0.5f / Time.timeScale))
                {
                    Vector2 delta;
                    delta.x = _start.x - TouchPosition.x;
                    delta.y = _start.y - TouchPosition.y;

                    float dist = Mathf.Sqrt(Mathf.Pow(delta.x, 2) + Mathf.Pow(delta.y, 2));

                    if (dist < 50.0f)
                    {
                        _touchpos = _touch.Position;
                        _one = false;

                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        private bool SwypeUpdate()
        {
            TouchPhase phase = _touch.Phase;

            if (phase == TouchPhase.Moved)
            {
                _end = _touch.Position;
                Vector2 delta = _end - _swype_start;

                float endAt = Time.time;

                float dist = Mathf.Sqrt(Mathf.Pow(delta.x, 2) + Mathf.Pow(delta.y, 2));
                //float angle = Mathf.Atan(delta.y / delta.x) * (180.0f / Mathf.PI);
                //float duration = endAt - _swype_start_at;
                //float speed = dist / duration;

                if (dist > 50.0f)
                {
                    _swype_type = XCommon.singleton.IsGreater(_swype_start.x, Screen.width * 0.5f) ? XGesture.SwypeDirectionType.Right : XGesture.SwypeDirectionType.Left;

                    _swype_start = _end;
                    //_swype_start_at = Time.time;

                    _swypedir.x = delta.x;
                    _swypedir.y = 0;
                    _swypedir.z = delta.y;

                    _swypedir.Normalize();
                    _touchpos = _end;

                    _last_swype_at = endAt; // _swype_start_at;

                    return true;
                }
            }

            return false;
        }
    }
}
#endif