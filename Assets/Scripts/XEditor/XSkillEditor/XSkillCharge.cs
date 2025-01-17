#if UNITY_EDITOR
using System;
using UnityEngine;
using XUtliPoolLib;

namespace XEditor
{
	public class XSkillCharge
	{
        public XSkillCharge(XSkillHoster host, XCurve curve_forward, XCurve curve_side, XCurve curve_up, bool using_up, float time_offset, bool aim, bool standon, bool control)
        {
            if (time_offset >= curve_forward.Curve[curve_forward.Curve.length - 1].time)
            {
                _timeElapsed = 0.1f;
                _span = 0;

                return;
            }

            _using_curve = true;
            _using_up = using_up;
            _aim_target = aim;
            _stand_on = standon;
            _control_towards = control;

            _curve_forward = curve_forward.Curve;
            _curve_side = curve_side.Curve;
            _curve_up = curve_up == null ? null : curve_up.Curve;

            _xcurve_up = curve_up;

            _host = host;

            _span = _curve_forward[_curve_forward.length - 1].time;

            _distance = 0;

            _last_offset_forward = 0;
            _last_offset_side = 0;
            _last_offset_up = 0;

            _timeElapsed = time_offset;

            Prepare();
        }

        public XSkillCharge(XSkillHoster host, float span, float offset, float height, float time_offset, float rotation, bool aim, bool standon, bool control)
        {
            if (time_offset >= span)
            {
                _timeElapsed = 0.1f;
                _span = 0;

                return;
            }

            _using_curve = false;
            _control_towards = control;
            _aim_target = aim;
            _stand_on = standon;

            _span = span;
            _height = height;

            _host = host;

            _distance = offset;
            _timeElapsed = time_offset;

            _rotation_speed = _aim_target ? 0 : rotation;

            Prepare();
        }

        private XSkillHoster _host = null;

        private XCurve _xcurve_up = null;

        private AnimationCurve _curve_forward = null;
        private AnimationCurve _curve_side = null;
        private AnimationCurve _curve_up = null;

        private bool _using_curve = false;
        private bool _using_up = false;
        private bool _aim_target = false;
        private bool _stand_on = true;
        private bool _control_towards = false;

        private float _scale = 1;
        private float _last_offset_forward = 0;
        private float _last_offset_side = 0;
        private float _last_offset_up = 0;

        private float _distance = 0;
        private float _height = 0;
        private float _span = 0;
        private float _height_drop = 0;
        private float _land_time = 0;

        private float _timeElapsed = 0;
        private float _gravity = 0;
        private float _rticalV = 0;

        private float _step_speed = 0;
        private float _rotation_speed = 0;
        private Vector2 _step_dir = Vector2.zero;

        public bool Update(float deltaTime)
        {
            if (XCommon.singleton.IsGreater(_timeElapsed, _span))
                return true;

            float v1 = _rticalV - _gravity * _timeElapsed;
            _timeElapsed += deltaTime;
            float v2 = _rticalV - _gravity * _timeElapsed;

            float dis = (_aim_target && _host.Target != null) ? (_host.Target.gameObject.transform.position - _host.gameObject.transform.position).magnitude : Mathf.Infinity;
            Vector3 forward = (_control_towards && _host.ControlDir.sqrMagnitude > 0) ? _host.ControlDir  : ((_aim_target && _host.Target != null) ? XCommon.singleton.Horizontal(_host.Target.gameObject.transform.position - _host.gameObject.transform.position) : _host.gameObject.transform.forward);

            _step_dir.x = forward.x;
            _step_dir.y = forward.z;
            _step_dir.Normalize();

            Vector2 delta;
            float h = 0;

            if (_using_curve)
            {
                float offset_forward = _curve_forward.Evaluate(_timeElapsed) * _scale;
                float delta_offset_forward = offset_forward - _last_offset_forward;
                _last_offset_forward = offset_forward;

                float offset_side = _curve_side.Evaluate(_timeElapsed) * _scale;
                float delta_offset_side = offset_side - _last_offset_side;
                _last_offset_side = offset_side;

                Vector3 right = XCommon.singleton.Horizontal(Vector3.Cross(_host.gameObject.transform.up, _step_dir));
                delta = delta_offset_forward * _step_dir + delta_offset_side * new Vector2(right.x, right.z);

                if (_using_up)
                {
                    float offset_up = _curve_up.Evaluate(_timeElapsed) * _scale;
                    h = offset_up - _last_offset_up;
                    _last_offset_up = offset_up;
                }
            }
            else
            {
                delta = _step_speed * deltaTime * _step_dir;

                h = (v1 + v2) / 2.0f * deltaTime;

                if (_rotation_speed > 0)
                {
                    _host.gameObject.transform.forward = XCommon.singleton.HorizontalRotateVetor3(_host.gameObject.transform.forward, _rotation_speed * deltaTime);
                }
            }

            h -= _land_time > 0 ? (deltaTime) * (_height_drop / _land_time) : _height_drop;

            if (dis - 0.5f < delta.magnitude) delta.Set(0, 0);
            _host.gameObject.transform.Translate(delta.x, h, delta.y, Space.World);

            return false;
        }

        protected void Prepare()
        {
            Vector3 dir = _host.GetRotateTo();

            _step_dir.x = dir.x;
            _step_dir.y = dir.z;

            _step_dir.Normalize();

            _land_time = 0;

            if (_using_curve)
            {
                if (_using_up)
                {
                    _land_time = _xcurve_up.GetLandValue();
                }
            }
            else
            {
                _step_speed = _distance / _span;

                _rticalV = (_height * 4.0f) / _span;
                _gravity = _rticalV / _span * 2.0f;
            }

            _height_drop = _stand_on ? _host.transform.position.y : 0;
            if (_height_drop < 0) _height_drop = 0;

            _scale = XAnimationLibrary.AssociatedAnimations((uint)_host.ConfigData.Player).Scale;
        }
	}
}
#endif