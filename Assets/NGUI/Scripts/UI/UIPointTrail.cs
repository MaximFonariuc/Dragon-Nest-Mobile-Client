using UnityEngine;
using System.Collections.Generic;



[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI PointTrail")]
public class UIPointTrail: UIWidget
{
    public class KeyPoint
    {
        public Vector3 point;
        public float enqueueTime;
        public float size;
        public bool mainPoint;
    }

    static public UIPointTrail current;
    
    [HideInInspector] [SerializeField] Material mMat = null;
    [HideInInspector][SerializeField] float mPointTime = 0;
    [HideInInspector] [SerializeField] int mMaxSize = 64;

    [System.NonSerialized]
    protected List< Queue<KeyPoint>> mKeyQueues = new List<Queue<KeyPoint>>();

    private Queue<KeyPoint> mCurrentQueue = null;

    protected Rect mUV = new Rect();

    // how many points is sampled on a point queue
    public int mSampleRate = 6;

    // how much time a point can stay
   

    // when mNewQueue = true, new coming point will push to a new queue
    private bool mNewQueue = false;
    private KeyPoint mLastPoint = null;

    protected override void OnInit()
    {
        base.OnInit();

        current = this;

        mNewQueue = true;
        //Vector3 point = new Vector3(10.0f, 0.0f, 0.0f);
        //mKeyQueues.Enqueue(point);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (mKeyQueues.Count > 0)
        {
            foreach (Queue<KeyPoint> _queue in mKeyQueues)
            {
                UpdateQueue(_queue);
            }
            // update key point
            mChanged = true;
        }
    }

    protected void UpdateQueue(Queue<KeyPoint> queue)
    {
        if (queue.Count <= 0) return;

        KeyPoint kp = queue.Peek();

        float t = RealTime.time;

        while ((kp != null) && (t - kp.enqueueTime > mPointTime))
        {
            queue.Dequeue();

            if (queue.Count > 0)
            {
                kp = queue.Peek();
            }
            else
            {
                kp = null;
            }
        }

        int i = 0;
        foreach (KeyPoint _kp in queue)
        {
            _kp.size = (float)(i+1)/(queue.Count);
            i++;
        }
    }

    public void AddPoint(Vector3 pos, float time)
    {
        KeyPoint newKP = new KeyPoint();
        newKP.point = pos;
        newKP.enqueueTime = time;
        newKP.size = 1.0f;
        newKP.mainPoint = true;

        if (mNewQueue == true)
        {
            GetEmptyQueue();
            mNewQueue = false;
        }

        if (mCurrentQueue != null)
        {
            if (mCurrentQueue.Count > 0 && mLastPoint != null)
            {
                KeyPoint kp = mLastPoint;

                float dis = Vector3.Distance(kp.point, newKP.point);

                if (kp != null && dis > 1.0f && dis < 50.0f)
                {
                    KeyPoint additionKp = new KeyPoint();
                    additionKp.point = (kp.point + newKP.point) / 2;
                    additionKp.enqueueTime = (kp.enqueueTime + newKP.enqueueTime) / 2;
                    additionKp.size = (kp.size + newKP.size) / 2;
                    additionKp.mainPoint = false;

                    mCurrentQueue.Enqueue(additionKp);
                }
            }

            mLastPoint = newKP;
            mCurrentQueue.Enqueue(newKP);
        }
    }

    protected void GetEmptyQueue()
    {
        bool bFindEmpty = false;

        foreach (Queue<KeyPoint> _queue in mKeyQueues)
        {
            if (_queue.Count == 0)
            {
                bFindEmpty = true;
                mCurrentQueue = _queue;
            }
        }

        if (!bFindEmpty)
        {
            Queue<KeyPoint> newQueue = new Queue<KeyPoint>();
            mKeyQueues.Add(newQueue);
            mCurrentQueue = newQueue;
        }
    }

    public void SetNewQueue()
    {
        mNewQueue = true;
    }

    public override Material material { get { return mMat; } }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        foreach (Queue<KeyPoint> _queue in mKeyQueues)
        {
            if(_queue.Count <= 0) continue;

           // Queue<KeyPoint> cloneQueue = new Queue<KeyPoint>();

            QueueOnFill(_queue, verts, uvs, cols);
        }
    }

    public void QueueOnFill(Queue<KeyPoint> queue, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        foreach (KeyPoint kp in queue)
        {
            Vector3 center = kp.point;

            UIRoot rt = NGUITools.FindInParents<UIRoot>(cachedTransform);
            center *= rt.GetPixelSizeAdjustment(Screen.height);

            float scale = kp.size;
            
            verts.Add(new Vector3(center.x - scale * mMaxSize / 2, center.y - scale * mMaxSize / 2));
            verts.Add(new Vector3(center.x - scale * mMaxSize / 2, center.y + scale * mMaxSize / 2));
            verts.Add(new Vector3(center.x + scale * mMaxSize / 2, center.y + scale * mMaxSize / 2));
            verts.Add(new Vector3(center.x + scale * mMaxSize / 2, center.y - scale * mMaxSize / 2));

            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(0f, 1f));
            uvs.Add(new Vector2(1f, 1f));
            uvs.Add(new Vector2(1f, 0f));

            Color col = color;

            //if (kp.mainPoint == false) col = Color.red;
            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
        }
    }
}

