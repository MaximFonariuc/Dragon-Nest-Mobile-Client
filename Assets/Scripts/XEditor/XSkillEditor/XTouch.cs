#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using XUtliPoolLib;
using UnityEngine;

namespace XEditor
{
    internal class XTouch : XSingleton<XTouch>
    {
        private bool _bEditorMode = false;
        private bool _bHasTouch = false;
        private Vector3 _lastMousePosition = Vector3.zero;

        private XTouchItem _touch = new XTouchItem();

        public XTouch()
        {
            _bEditorMode = (Application.platform == RuntimePlatform.WindowsEditor ||
                            Application.platform == RuntimePlatform.OSXEditor);
        }

        public XTouchItem GetTouch()
        {
            if (_bHasTouch) 
                return _touch;
            else 
                return null;
        }

        public void Update()
        {
            _bHasTouch = false;

            if (_bEditorMode)
            {
                if (Input.GetMouseButton(0))
                {
                    _touch.faketouch.fingerId = 10;
                    _touch.faketouch.position = Input.mousePosition;
                    _touch.faketouch.deltaTime = Time.deltaTime;
                    _touch.faketouch.deltaPosition = Input.mousePosition - _lastMousePosition;
                    _touch.faketouch.phase = (Input.GetMouseButtonDown(0) ? TouchPhase.Began :
                                        (_touch.faketouch.deltaPosition.sqrMagnitude > 1.0f ? TouchPhase.Moved : TouchPhase.Stationary));
                    _touch.faketouch.tapCount = 1;

                    _touch.Fake = true;

                    HandleTouch();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _touch.faketouch.fingerId = 10;
                    _touch.faketouch.position = Input.mousePosition;
                    _touch.faketouch.deltaTime = Time.deltaTime;
                    _touch.faketouch.deltaPosition = Input.mousePosition - _lastMousePosition;
                    _touch.faketouch.phase = TouchPhase.Ended;
                    _touch.faketouch.tapCount = 1;

                    _touch.Fake = true;

                    HandleTouch();
                }
            }
            else 
            {
                if (Input.touchCount > 0)
                {
                    _touch.Fake = false;
                    _touch.touch = Input.GetTouch(0);

                    HandleTouch();
                }
            }
        }

        private void HandleTouch()
        {
            _bHasTouch = true;
        }
    }

    internal class XTouchItem
    {
        public bool Fake { get; set; }
        public Touch touch;
        public XFakeTouch faketouch = new XFakeTouch();

        public Vector2 DeltaPosition
        {
            get { return Fake ? faketouch.deltaPosition : touch.deltaPosition; }
        }
        public float DeltaTime
        {
            get { return Fake ? faketouch.deltaTime : touch.deltaTime; }
        }
        public int FingerId
        {
            get { return Fake ? faketouch.fingerId : touch.fingerId; }
        }
        public TouchPhase Phase
        {
            get { return Fake ? faketouch.phase : touch.phase; }
        }
        public Vector2 Position
        {
            get { return Fake ? faketouch.position : touch.position; }
        }
        public Vector2 RawPosition
        {
            get { return Fake ? faketouch.rawPosition : touch.rawPosition; }
        }
        public int TapCount
        {
            get { return Fake ? faketouch.tapCount : touch.tapCount; }
        }
    }

    internal struct XFakeTouch
    {
        public Vector2 deltaPosition { get; set; }
        public float deltaTime { get; set; }
        public int fingerId { get; set; }
        public TouchPhase phase { get; set; }
        public Vector2 position { get; set; }
        public Vector2 rawPosition { get; set; }
        public int tapCount { get; set; }
    }
}
#endif