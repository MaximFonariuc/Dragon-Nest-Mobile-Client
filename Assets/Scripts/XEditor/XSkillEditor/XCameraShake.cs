#if UNITY_EDITOR
using System;
using UnityEngine;
using XUtliPoolLib;

namespace XEditor
{
	public class XCameraShake
	{
        private GameObject _gameObject = null;
        private Camera _camera = null;

        private float _timeEscaped = 0;
        private float _timeInterval = 0;
        private bool  _shake = false;

        private Vector3 x = Vector3.zero;
        private Vector3 y = Vector3.zero;
        private Vector3 z = Vector3.zero;

        private float _time = 0;
        private float _fovAmp = 0;
        private float _amplitude_x = 0;
        private float _amplitude_y = 0;
        private float _amplitude_z = 0;
        private float _frequency = 0;
        private CameraMotionSpace _coordinate = CameraMotionSpace.World;
        private bool _shakeX = true;
        private bool _shakeY = true;
        private bool _shakeZ = true;

        private bool _random = false;
        private float _fov = 0;

        private int _rfactor = 1;

        public XCameraShake(GameObject go, Camera camera)
        {
            _camera = camera;
            _gameObject = go;
        }

        public bool OnShake(
            float time,
            float fovAmp,
            float amplitude_x,
            float amplitude_y,
            float amplitude_z,
            float frequency,
            CameraMotionSpace coordinate,
            bool shakeX,
            bool shakeY,
            bool shakeZ,
            bool random)
        {
            _time = time;

            _fovAmp = fovAmp;
            _amplitude_x = amplitude_x;
            _amplitude_y = amplitude_y;
            _amplitude_z = amplitude_z;
            _frequency = frequency;
            _coordinate = coordinate;
            _shakeX = shakeX;
            _shakeY = shakeY;
            _shakeZ = shakeZ;

            _random = random;

            _fov = _camera.fieldOfView;

            if (null != _camera)
            {
                _timeEscaped = 0;
                _timeInterval = 0;

                _shake = true;

                switch (_coordinate)
                {
                    case CameraMotionSpace.Camera:
                        {
                            x = _camera.transform.right;
                            y = _camera.transform.up;
                            z = _camera.transform.forward;
                        } break;
                    case CameraMotionSpace.Self:
                        {
                            x = _gameObject.transform.right;
                            y = _gameObject.transform.up;
                            z = _gameObject.transform.forward;
                        } break;
                    case CameraMotionSpace.World:
                        {
                            x = Vector3.right;
                            y = Vector3.up;
                            z = Vector3.forward;
                        } break;
                }
            }

            _rfactor = 1;
            return true;
        }

        public void Update(float fDeltaT)
        {
            if (null != _camera && _shake)
            {
                _timeEscaped += fDeltaT;
                _timeInterval += fDeltaT;

                if (XCommon.singleton.IsGreater(_timeEscaped, _time))
                {
                    StopShake();
                }
                else
                {
                    if (XCommon.singleton.IsGreater(_timeInterval, 1 / _frequency))
                    {
                        _rfactor = -_rfactor;

                        _camera.transform.position += Shake();

                        float fov = UnityEngine.Random.Range(-_fovAmp, _fovAmp);
                        _camera.fieldOfView = _fov + (_random ? fov : _fovAmp * _rfactor);

                        _timeInterval = 0;
                    }
                }
            }
        }

        private void StopShake()
        {
            _timeEscaped = 0;
            _shake = false;

            _camera.fieldOfView = _fov;
        }

        private Vector3 Shake()
        {
            float offsetX = _random ? UnityEngine.Random.Range(-_amplitude_x, _amplitude_x) : _amplitude_x * _rfactor;
            float offsetY = _random ? UnityEngine.Random.Range(-_amplitude_y, _amplitude_y) : _amplitude_y * _rfactor;
            float offsetZ = _random ? UnityEngine.Random.Range(-_amplitude_z, _amplitude_z) : _amplitude_z * _rfactor;

            Vector3 v = Vector3.zero;
            if (_shakeX) v += (x * offsetX);
            if (_shakeY) v += (y * offsetY);
            if (_shakeZ) v += (z * offsetZ);

            return v;
        }
	}
}
#endif