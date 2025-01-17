#if UNITY_EDITOR
using System;
using UnityEngine;
using XUtliPoolLib;

namespace XEditor
{
	internal class XCutSceneCamera
	{
        private GameObject _cameraObject = null;

        private GameObject _dummyObject = null;
        private Transform _dummyCamera = null;
        private Transform _cameraTransform = null;

        private Animator _ator = null;
        public AnimatorOverrideController _overrideController = new AnimatorOverrideController();

        private UnityEngine.Camera _camera = null;

        private bool _root_pos_inited = false;

        private Vector3 _root_pos = Vector3.zero;

        private Quaternion _idle_root_rotation = Quaternion.identity;
        private Vector3 _dummyCamera_pos = Vector3.zero;

        private CameraMotionSpace _effect_axis = CameraMotionSpace.World;
        private XCameraMotionData _motion = new XCameraMotionData();

        public XActor Target = null;

        private string _trigger = null;

        public UnityEngine.Camera UnityCamera
        {
            get { return _camera; }
        }

        public Transform CameraTrans
        {
            get { return _cameraTransform; }
        }

        public Animator CameraAnimator
        {
            get { return _ator; }
        }

        public Vector3 Position
        {
            get { return _cameraTransform.position; }
        }

        public Quaternion Rotaton
        {
            get { return _cameraTransform.rotation; }
        }

        public bool Initialize()
        {
            _cameraObject = GameObject.Find(@"Main Camera");

            if (null != _cameraObject)
            {
                _camera = _cameraObject.GetComponent<UnityEngine.Camera>();
                _cameraTransform = _cameraObject.transform;

                XResourceLoaderMgr.SafeDestroy(ref _dummyObject);
                _dummyObject = XResourceLoaderMgr.singleton.CreateFromPrefab("Prefabs/DummyCamera") as GameObject;
                _dummyObject.name = "Dummy Camera";

                _dummyCamera = _dummyObject.transform.GetChild(0);
                _ator = _dummyObject.GetComponent<Animator>();

                _overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
                _ator.runtimeAnimatorController = _overrideController;

                _root_pos_inited = false;
            }

            return true;
        }

        public void OverrideAnimClip(string motion, string clipname)
        {
            //get override clip
            AnimationClip animClip = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clipname, ".anim");
            OverrideAnimClip(motion, animClip);
        }

        public void OverrideAnimClip(string motion, AnimationClip clip)
        {
            //override
            if (clip != null && _overrideController[motion] != clip) _overrideController[motion] = clip;
        }

        public void PostUpdate(float fDeltaT)
        {
            if (!_root_pos_inited)
            {
                _root_pos = _dummyCamera.position;
                _root_pos_inited = true;

                _idle_root_rotation = _motion.Follow_Position ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            }

            InnerUpdateEx();
            TriggerEffect();
        }

        private void InnerPosition()
        {
            _dummyCamera_pos = _idle_root_rotation * (_dummyCamera.position - _dummyObject.transform.position) + _dummyObject.transform.position;
        }

        private void InnerUpdateEx()
        {
            InnerPosition();

            Vector3 v_self_p = Target == null ? Vector3.zero : Target.Actor.transform.position;

            Vector3 forward = Vector3.Cross(_dummyCamera.forward, _dummyCamera.up);
            Quaternion q = Quaternion.LookRotation(forward, _dummyCamera.up);

            Vector3 delta = (_dummyCamera_pos - _root_pos);
            Vector3 target_pos = _root_pos + (_motion.Follow_Position ? v_self_p : Vector3.zero);

            _cameraTransform.rotation = _idle_root_rotation * q;

            switch (_effect_axis)
            {
                case CameraMotionSpace.World:
                    {
                        target_pos += delta;
                    } break;
            }

            _cameraTransform.position = target_pos;
        }

        public void Effect(XCameraMotionData motion)
        {
            //must be called from UPDATE pass
            AnimationClip clip = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(motion.Motion, ".anim");

            if (clip != null)
            {
                _trigger = "ToEffect";
                if (_overrideController["CameraEffect"] != clip) _overrideController["CameraEffect"] = clip;

                _motion.Follow_Position = motion.Follow_Position;
                _motion.Coordinate = motion.Coordinate;
                _motion.AutoSync_At_Begin = motion.AutoSync_At_Begin;
                _motion.LookAt_Target = motion.LookAt_Target;

                _motion.Motion = motion.Motion;
            }
        }

        private void TriggerEffect()
        {
            if (_trigger != null && !_ator.IsInTransition(0))
            {
                _effect_axis = _motion.Coordinate;

                _ator.SetTrigger(_trigger);
                _root_pos_inited = false;

                _trigger = null;
            }
        }
    }
}
#endif