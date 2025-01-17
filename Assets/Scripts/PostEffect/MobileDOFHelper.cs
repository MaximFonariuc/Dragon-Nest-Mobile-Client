using System;
using UnityEngine;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace FxProNS
{
    [Serializable]
    public class MobileDOFHelperParams
    {
        public Camera EffectCamera;
        public Transform Target;

        [Range(.01f, 1f)]
        public float FocalLengthMultiplier = .33f;



        [Range(.5f, 2f)]
        public float DOFBlurSize = 1f;

        public float NonTargetFocalDist = 1f;
    }
    public class MobileDOFHelper : Singleton<MobileDOFHelper>, IDisposable
    {
        private static Material _mat;

        public static Material Mat
        {
            get
            {
                if (null == _mat)
                {
                    Shader shader = XUtliPoolLib.ShaderManager.singleton.FindShader("MobileDOFPro", "Hidden/MobileDOFPro");
                    _mat = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                }

                return _mat;
            }
        }


        private MobileDOFHelperParams _p;

        public void SetParams(MobileDOFHelperParams p)
        {
            _p = p;
        }

        public void Init(bool searchForNonDepthmapAlphaObjects)
        {
            _p.FocalLengthMultiplier = Mathf.Clamp(_p.FocalLengthMultiplier, .01f, .99f);
            if (_p.EffectCamera.depthTextureMode != DepthTextureMode.DepthNormals)
                _p.EffectCamera.depthTextureMode = DepthTextureMode.Depth;

            Mat.EnableKeyword("USE_CAMERA_DEPTH_TEXTURE");
            Mat.DisableKeyword("DONT_USE_CAMERA_DEPTH_TEXTURE");
        }

        private void CalculateAndUpdateFocalDist()
        {
            if (null == _p.EffectCamera)
            {
                Debug.LogError("null == p.camera");
                return;
            }

            float focalDist;

            if (null != _p.Target)
            {
                Vector3 targetPosInViewportSpace = _p.EffectCamera.WorldToViewportPoint(_p.Target.position);
                focalDist = targetPosInViewportSpace.z;
                //		float focalDist = (target.position - transform.position).magnitude / camera.farClipPlane;
            }
            else
            {
                focalDist = _p.NonTargetFocalDist;
                //            Debug.Log("focalDist: " + focalDist);
            }

            focalDist /= _p.EffectCamera.farClipPlane;

            //focalDist *= _p.FocalDistMultiplier ;

            Mat.SetFloat("_FocalDist", focalDist);

            //Make sure that focalLength < focalDist
            Mat.SetFloat("_FocalLength", focalDist * _p.FocalLengthMultiplier);
        }


        public void RenderMobileDOFBlur(RenderTexture src, RenderTexture dest)
        {
            //Graphics.Blit( src, dest );

            //if (null == cocTexture)
            //{
            //    Debug.LogError("null == cocTexture");
            //    return;
            //}

            //Mat.SetTexture("_COCTex", cocTexture);

            //		//Apply separable DOF

            //Mat.SetFloat("", _p.FocalLengthMultiplier);
            //Mat.SetFloat("", _p.FocalDistMultiplier);

            CalculateAndUpdateFocalDist();

            RenderTexture tempRt = RenderTextureManager.Instance.RequestRenderTexture(src.width, src.height, src.depth, src.format);

                Mat.SetVector("_SeparableBlurOffsets", new Vector4(_p.DOFBlurSize, 0f, 0f, 0f));
                Graphics.Blit(src, tempRt, Mat, 0);
                Mat.SetVector("_SeparableBlurOffsets", new Vector4(0f, _p.DOFBlurSize, 0f, 0f));
                Graphics.Blit(tempRt, dest, Mat, 0);

                RenderTextureManager.Instance.ReleaseRenderTexture(tempRt);

        }


        public void SetBlurRadius(int radius)
        {
            Shader.DisableKeyword("BLUR_RADIUS_10");
            Shader.DisableKeyword("BLUR_RADIUS_5");
            Shader.DisableKeyword("BLUR_RADIUS_3");
            Shader.DisableKeyword("BLUR_RADIUS_2");
            Shader.DisableKeyword("BLUR_RADIUS_1");

            if (radius != 10 && radius != 5 && radius != 3 && radius != 2 && radius != 1) radius = 5;

            if (radius < 3) radius = 3;

            //Debug.Log( "blur radius: " + radius );

            Shader.EnableKeyword("BLUR_RADIUS_" + radius);
        }





        public void Dispose()
        {
            if (null != Mat)
                Object.DestroyImmediate(Mat);
            if (_p != null && _p.EffectCamera != null)
                _p.EffectCamera.depthTextureMode = DepthTextureMode.None;
            RenderTextureManager.Instance.Dispose();
        }
    }
}