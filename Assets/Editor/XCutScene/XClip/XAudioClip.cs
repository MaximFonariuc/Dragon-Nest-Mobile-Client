using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

using XUtliPoolLib;

namespace XEditor
{
	public class XAudioClip : XClip
	{
        private XAudioDataClip _data = new XAudioDataClip();

        private int _bind_idx = 0;
        private string _bind_prefab = "None";

        public XAudioClip(float timeline)
            :base(timeline)
        {
            CutSceneClip.Type = XClipType.Audio;
        }

        public XAudioClip(XCutSceneClip data)
            : base(data)
        {

        }

        public override void OnTextColor()
        {
            _textStyle.normal.textColor = Color.cyan;
        }

        public override string Name
        {
            get { return _data.Clip != null ? _data.Clip : "null"; }
        }

        public override XCutSceneClip CutSceneClip
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value as XAudioDataClip;
            }
        }

        public override void Flush()
        {
            _bind_idx = _data.BindIdx + 1;
            _bind_prefab = _bind_idx <= 0 ? "None" : XCutSceneWindow.ActorList[_bind_idx];
        }

        public override void Dump()
        {
            if (_bind_idx <= 0)
            {
                _data.BindIdx = -1;
            }
            else
                _data.BindIdx = _bind_idx - 1;
        }

        protected override void OnInnerGUI(XCutSceneData data)
        {
            _bind_idx = XCutSceneWindow.ActorList.FindIndex(FindActor);

            _data.Clip = EditorGUILayout.TextField("Clip Name", _data.Clip);
            _bind_idx = EditorGUILayout.Popup("Bind To", _bind_idx, XCutSceneWindow.ActorList.ToArray());
            EditorGUILayout.Space();

            _data.Channel = (AudioChannel)EditorGUILayout.EnumPopup("Channel", _data.Channel);

            if (_bind_idx > 0)
            {
                _bind_prefab = XCutSceneWindow.ActorList[_bind_idx];
            }
            else
            {
                _bind_prefab = "None";
            }
            EditorGUILayout.Space();
        }

        // Explicit predicate delegate. 
        private bool FindActor(string prefab)
        {
            return (prefab == _bind_prefab);
        }
    }
}
