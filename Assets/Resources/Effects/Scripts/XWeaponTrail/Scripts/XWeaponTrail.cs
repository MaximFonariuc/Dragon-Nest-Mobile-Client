﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XUtliPoolLib;

namespace Xft
{
    public class XWeaponTrail : IWeaponTail
    {
        public class Element
        {
            public Vector3 PointStart;

            public Vector3 PointEnd;

            public Vector3 Pos
            {
                get
                {
                    return (PointStart + PointEnd) / 2f;
                }
            }


            public Element(Vector3 start, Vector3 end)
            {
                PointStart = start;
                PointEnd = end;
            }

            public Element()
            {

            }
        }


        #region public members

        public static string Version = "1.0.1";

        public Transform PointStart;
        public Transform PointEnd;

        public int MaxFrame = 14;
        public int Granularity = 60;
        public float Fps = 60f;

        public Color MyColor = Color.white;
        public Material MyMaterial;
        #endregion



        #region protected members
        protected static Queue<Element> mElementPool = new Queue<Element>();
        protected float mTrailWidth = 0f;
        protected Element mHeadElem = new Element();
        protected List<Element> mSnapshotList = new List<Element>();
        protected Spline mSpline = new Spline();
        protected float mFadeT = 1f;
        protected bool mIsFading = false;
        protected float mFadeTime = 1f;
        protected float mElapsedTime = 0f;
        protected float mFadeElapsedime = 0f;
        protected GameObject mMeshObj;
        protected MeshRenderer mCachedMeshRenderer;
        protected VertexPool mVertexPool;
        protected VertexPool.VertexSegment mVertexSegment;
        protected bool mInited = false;
        private int updateFrame = 0;
        //protected bool FxDestoryed = false;

        #endregion

        #region property
        public float UpdateInterval
        {
            get
            {
                return 1f / Fps;
            }
        }
        public Vector3 CurHeadPos
        {
            get { return (PointStart.position + PointEnd.position) / 2f; }
        }
        public float TrailWidth
        {
            get
            {
                return mTrailWidth;
            }
        }
        #endregion

        #region API
        //you may pre-init the trail to save some performance.
        public void Init()
        {
            if (mInited)
                return;

            mTrailWidth = (PointStart.position - PointEnd.position).magnitude;

            InitMeshObj();

            InitOriginalElements();

            InitSpline();

            mInited = true;
        }

        public override void Activate()
        {

            Init();


            //check if scene has changed, need to recreate the mesh obj.
            if (mMeshObj == null)
            {
                InitMeshObj();
                return;
            }

            gameObject.SetActive(true);
            if (mMeshObj != null)
                mMeshObj.SetActive(true);
            

            mFadeT = 1f;
            mIsFading = false;
            mFadeTime = 1f;
            mFadeElapsedime = 0f;
            mElapsedTime = 0f;
            //mCachedMesh.Clear();
            //reset all elemts to head pos.
            for (int i = 0; i < mSnapshotList.Count; i++)
            {
                mSnapshotList[i].PointStart = PointStart.position;
                mSnapshotList[i].PointEnd = PointEnd.position;
                
                mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
                mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
            }

            //reset vertex too.
            RefreshSpline();
            UpdateVertex();
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
            if (mMeshObj != null)
                mMeshObj.SetActive(false);
        }

        public void StopSmoothly(float fadeTime)
        {
            mIsFading = true;
            mFadeTime = fadeTime;
        }

        #endregion

        #region unity methods
        void Update()
        {

            if (!mInited)
                return;


            //check if scene has changed, need to recreate the mesh obj.
            if (mMeshObj == null)
            {
                InitMeshObj();
                return;
            }

            UpdateHeadElem();


            mElapsedTime += Time.deltaTime;
            if (mElapsedTime < UpdateInterval)
            {
                return;
            }
            mElapsedTime -= UpdateInterval;

            RecordCurElem();

            RefreshSpline();

            UpdateFade();

            UpdateVertex();
            //UpdateIndices();

        }


        void LateUpdate()
        {
            if (!mInited)
                return;


            mVertexPool.LateUpdate();
        }


        void Start()
        {
            Init();
        }

        void OnDrawGizmos()
        {
            if (PointEnd == null || PointStart == null)
            {
                return;
            }


            float dist = (PointStart.position - PointEnd.position).magnitude;

            if (dist < Mathf.Epsilon)
                return;


            Gizmos.color = Color.red;

            Gizmos.DrawSphere(PointStart.position, dist * 0.04f);


            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(PointEnd.position, dist * 0.04f);

        }

        void OnDestroy()
        {
            if (mMeshObj != null)
            {
                Object.Destroy(mMeshObj);
                mMeshObj = null;
            }
        }

        #endregion

        #region local methods

        static Element GetElement()
        {
            if(mElementPool.Count>0)
            {
                return mElementPool.Dequeue();
            }
            return new Element();
        }
        void ReturnElement(Element e)
        {
            mElementPool.Enqueue(e);
        }
        void InitSpline()
        {
            mSpline.Granularity = Granularity;

            mSpline.Clear();

            for (int i = 0; i < MaxFrame; i++)
            {
                mSpline.AddControlPoint(CurHeadPos, PointStart.position - PointEnd.position);
            }
        }

        void RefreshSpline()
        {
            for (int i = 0; i < mSnapshotList.Count; i++)
            {
                mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
                mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
            }

            mSpline.RefreshSpline();
        }

        void UpdateVertex()
        {

            VertexPool pool = mVertexSegment.Pool;


            for (int i = 0; i < Granularity; i++)
            {
                int baseIdx = mVertexSegment.VertStart + i * 3;

                float uvSegment = (float)i / Granularity;


                float fadeT = uvSegment * mFadeT;

                Vector2 uvCoord = Vector2.zero;

                Vector3 pos = mSpline.InterpolateByLen(fadeT);

                //Debug.DrawRay(pos, Vector3.up, Color.red);

                Vector3 up = mSpline.InterpolateNormalByLen(fadeT);
                Vector3 pos0 = pos + (up.normalized * mTrailWidth * 0.5f);
                Vector3 pos1 = pos - (up.normalized * mTrailWidth * 0.5f);

                // pos0
                pool.Vertices[baseIdx] = pos0;
                pool.Colors[baseIdx] = MyColor;
                uvCoord.x = 0f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx] = uvCoord;

                //pos
                pool.Vertices[baseIdx + 1] = pos;
                pool.Colors[baseIdx + 1] = MyColor;
                uvCoord.x = 0.5f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx + 1] = uvCoord;

                //pos1
                pool.Vertices[baseIdx + 2] = pos1;
                pool.Colors[baseIdx + 2] = MyColor;
                uvCoord.x = 1f;
                uvCoord.y = uvSegment;
                pool.UVs[baseIdx + 2] = uvCoord;
            }

            mVertexSegment.Pool.UVChanged = true;
            mVertexSegment.Pool.VertChanged = true;
            mVertexSegment.Pool.ColorChanged = true;

        }

        void UpdateIndices()
        {

            VertexPool pool = mVertexSegment.Pool;

            for (int i = 0; i < Granularity - 1; i++)
            {
                int baseIdx = mVertexSegment.VertStart + i * 3;
                int nextBaseIdx = mVertexSegment.VertStart + (i + 1) * 3;

                int iidx = mVertexSegment.IndexStart + i * 12;

                //triangle left
                pool.Indices[iidx + 0] = nextBaseIdx;
                pool.Indices[iidx + 1] = nextBaseIdx + 1;
                pool.Indices[iidx + 2] = baseIdx;
                pool.Indices[iidx + 3] = nextBaseIdx + 1;
                pool.Indices[iidx + 4] = baseIdx + 1;
                pool.Indices[iidx + 5] = baseIdx;


                //triangle right
                pool.Indices[iidx + 6] = nextBaseIdx + 1;
                pool.Indices[iidx + 7] = nextBaseIdx + 2;
                pool.Indices[iidx + 8] = baseIdx + 1;
                pool.Indices[iidx + 9] = nextBaseIdx + 2;
                pool.Indices[iidx + 10] = baseIdx + 2;
                pool.Indices[iidx + 11] = baseIdx + 1;

            }

            pool.IndiceChanged = true;
        }

        void UpdateHeadElem()
        {
            //if (PointStart.position.y > 95 || PointEnd.position.y > 95)
            //{
            //    if (mSnapshotList.Count > 3)
            //    {
            //        mCachedMeshRenderer.enabled = false;
            //    }
            //    return;
            //}

            mCachedMeshRenderer.enabled = true;
            mSnapshotList[0].PointStart = PointStart.position;
            mSnapshotList[0].PointEnd = PointEnd.position;
        }


        void UpdateFade()
        {
            if (!mIsFading)
                return;

            mFadeElapsedime += Time.deltaTime;

            float t = mFadeElapsedime / mFadeTime;

            mFadeT = 1f - t;

            if (mFadeT < 0f)
            {
                Deactivate();
            }
        }

        void RecordCurElem()
        {
            //TODO: use element pool to avoid gc alloc.
            Element elem = GetElement();
            elem.PointStart = PointStart.position;
            elem.PointEnd = PointEnd.position;

            //if (PointStart.position.y > 95 || PointEnd.position.y > 95)
            //{
            //    return;
            //}

            if (mSnapshotList.Count < MaxFrame)
            {
                mSnapshotList.Insert(1, elem);
            }
            else
            {
                if(mSnapshotList.Count>0)
                {
                    Element remove = mSnapshotList[mSnapshotList.Count - 1];
                    mSnapshotList.RemoveAt(mSnapshotList.Count - 1);
                    ReturnElement(remove);
                }
                
                mSnapshotList.Insert(1, elem);
            }

        }

        void InitOriginalElements()
        {
            for (int i = 0; i < mSnapshotList.Count; ++i)
            {
                ReturnElement(mSnapshotList[i]);
            }
            mSnapshotList.Clear();
            //at least add 2 original elements
            Element elem0 = GetElement();
            elem0.PointStart = PointStart.position;
            elem0.PointEnd = PointEnd.position;
            Element elem1 = GetElement();
            elem1.PointStart = PointStart.position;
            elem1.PointEnd = PointEnd.position;
            mSnapshotList.Add(elem0);
            mSnapshotList.Add(elem1);
        }



        void InitMeshObj()
        {
            //create a new mesh obj
            mMeshObj = new GameObject("_XWeaponTrailMesh: " + gameObject.name);
            mMeshObj.layer = gameObject.layer;
            mMeshObj.SetActive(true);
            MeshFilter mf = mMeshObj.AddComponent<MeshFilter>();
            MeshRenderer mr = mMeshObj.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.GetComponent<Renderer>().sharedMaterial = MyMaterial;
            mf.sharedMesh = new Mesh();
            mCachedMeshRenderer = mr;
            
            //init vertexpool
            mVertexPool = new VertexPool(mf.sharedMesh, MyMaterial);
            mVertexSegment = mVertexPool.GetVertices(Granularity * 3, (Granularity - 1) * 12);


            UpdateIndices();

        }

        #endregion


    }

}


