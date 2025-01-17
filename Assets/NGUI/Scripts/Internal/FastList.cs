using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPoolSize
{
    E32 = 0,
    E64,
    E128,
    E256,
    E512,
    E1024,
    E2048,
    E4096,
    E8192,
    ENum
}


public class MemoryPool<T>
{
    public class BufferQueue
    {
        public Queue<T[]> queue = new Queue<T[]>();
        public int defalutCount = 10;
        public int allocCount = 0;
        public int totalCount = 0;
    }
    public MemoryPool()
    {
        BufferQueue bufferQueue = new BufferQueue();
        _pool[0] = bufferQueue;
        bufferQueue.defalutCount = 1024;
        Init(bufferQueue, 32);

        int size = 64;
        int defaultCount = 256;
        int count = (int)EPoolSize.E4096;
        for (int i = 1; i < count; ++i)
        {
            bufferQueue = new BufferQueue();
            _pool[i] = bufferQueue;
            bufferQueue.defalutCount = defaultCount;
            Init(bufferQueue, size);
            size *= 2;
            defaultCount /= 2;
            if (defaultCount < 10)
                defaultCount = 16;
        }

        for (int i = count; i < (int)EPoolSize.ENum; ++i)
        {
            bufferQueue = new BufferQueue();
            _pool[i] = bufferQueue;
            //Init(bufferQueue, size);
            //size *= 2;
        }

    }

    private BufferQueue[] _pool = new BufferQueue[(int)EPoolSize.ENum];

    private void Init(BufferQueue bufferQueue,int size)
    {
        for (int i = 0; i < bufferQueue.defalutCount; ++i)
        {
            T[] buff = new T[size];
            bufferQueue.queue.Enqueue(buff);
        }
    }
    private int GetBuffIndex(int length,ref int size)
    {
        if (length == 0) 
            return -1;
        if (length <= 32)
        {
            size = 32;
            return 0;
        }
        if (length <= 64)
        {
            size = 64;
            return 1;
        }
        if (length <= 128)
        {
            size = 128;
            return 2;
        }
        if (length <= 256)
        {
            size = 256;
            return 3;
        }
        if (length <= 512)
        {
            size = 512;
            return 4;
        }
        if (length <= 1024)
        {
            size = 1024;
            return 5;
        }
        if (length <= 2048)
        {
            size = 2048;
            return 6;
        }
        if (length <= 4096)
        {
            size = 4096;
            return 7;
        }
        if (length <= 8192)
        {
            size = 8192;
            return 8;
        }
        return -1;
    }

    private BufferQueue GetBuffQueue(int length)
    {
        if (length == 0)
            return null;
        if (length == 32)
        {
            return _pool[0];
        }
        if (length == 64)
        {
            return _pool[1];
        }
        if (length == 128)
        {
            return _pool[2];
        }
        if (length == 256)
        {
            return _pool[3];
        }
        if (length == 512)
        {
            return _pool[4];
        }
        if (length == 1024)
        {
            return _pool[5];
        }
        if (length == 2048)
        {
            return _pool[6];
        }
        if (length == 4096)
        {
            return _pool[7];
        }
        if (length == 8192)
        {
            return _pool[8];
        }
        return null;
    }
    public T[] GetBuff(int length)
    {
        int size = 0;
        int index = GetBuffIndex(length, ref size);
        if (index >= 0 && index < _pool.Length)
        {
            BufferQueue bufferQueue = _pool[index];
            if (bufferQueue.queue.Count > 0)
            {
                bufferQueue.allocCount++;
                return bufferQueue.queue.Dequeue();
            }                
            else
            {
                bufferQueue.allocCount++;
                bufferQueue.totalCount++;
                //Debug.LogWarning(string.Format("FastList alloc:{0}", size));
                return new T[size];
            }
        }
        else
        {
            int newLength = Mathf.Max(length << 1, 32);
            if (newLength >= 65000)
            {
                newLength = 64999;
            }
#if UNITY_EDITOR
            Debug.LogWarning(string.Format("FastList alloc large:{0}", newLength));
#endif           

            return new T[newLength];
        }
    }

    public void ReturnBuff(T[] buffer)
    {
        if(buffer!=null)
        {
            BufferQueue bufferQueue = GetBuffQueue(buffer.Length);
            if (bufferQueue != null)
            {
                bufferQueue.allocCount--;
                bufferQueue.queue.Enqueue(buffer);
#if UNITY_EDITOR
                //Debug.LogWarning(string.Format("FastList return buff:{0}", buffer.Length));
#endif 
            }
        }
    }

    public void Clear()
    {
        int count = (int)EPoolSize.E4096;
        for (int i = 0; i < count; ++i)
        {
            BufferQueue bufferQueue = _pool[i];
            if (bufferQueue != null)
            {
                while (bufferQueue.queue.Count > bufferQueue.defalutCount)
                {
                    bufferQueue.allocCount--;
                    bufferQueue.queue.Dequeue();
                }
            }
        }

        for (int i = count; i < (int)EPoolSize.ENum; ++i)
        {
            BufferQueue bufferQueue = _pool[i];
            if(bufferQueue!=null)
            {
                while (bufferQueue.queue.Count > 5)
                {
                    bufferQueue.allocCount--;
                    bufferQueue.queue.Dequeue();
                }
            }
 
        }
    }

    public void Print()
    {
#if UNITY_EDITOR
        int size = 32;
        string log = "";
        int count = (int)EPoolSize.ENum;
        for (int i = 0; i < count; ++i)
        {
            BufferQueue bufferQueue = _pool[i];
            if (bufferQueue != null)
                log += string.Format("buff size {0} in queue {1} alloc count:{2} total alloc count:{3}\n", size.ToString(), bufferQueue.queue.Count, bufferQueue.allocCount, bufferQueue.totalCount);
            size *= 2;
        }
        Debug.LogWarning(log);
#endif
    }
}

public class FastList<T> : BetterList<T>
{
    public static MemoryPool<T> ms_Pool = new MemoryPool<T>();

    public static bool UseFastList = true;
    //public override int Count
    //{
    //    get
    //    {
    //        return buffer == null ? 0 : buffer.Length;
    //    }
    //}

    protected override void AllocateMore()
    {
        if(UseFastList)
        {
            int bufferSize = size + 1;
            if (bufferSize >= 65000)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Can not alloc more than 65000 vertex");
#endif     
                return;
            }
            T[] newList = ms_Pool.GetBuff(bufferSize);
            if (buffer != newList)
            {
                ms_Pool.ReturnBuff(buffer);
                Assign(buffer, newList);
                buffer = newList;
            }
        }
        else
        {
            //if (size >= 2048)
            //    Debug.LogWarning(string.Format("FastList alloc large:{0}", size));
            base.AllocateMore();
        }
    }
    protected void AllocateMore(int alloSize)
    {
        if (UseFastList)
        {
            int bufferSize = size + alloSize;
            if (bufferSize >= 65000)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Can not alloc more than 65000 vertex");
#endif
                return;
            }
            
            T[] newList = ms_Pool.GetBuff(bufferSize);
            if (buffer != newList)
            {
                ms_Pool.ReturnBuff(buffer);
                Assign(buffer, newList);
                buffer = newList;
            }
        }
        else
        {
            //if (size >= 2048)
            //    Debug.LogWarning(string.Format("FastList alloc large:{0}", size));
            base.AllocateMore();
        }
    }
    protected virtual void Assign(T[] src, T[] des)
    {
        if (src != null && des != null && size > 0)
        {
            try
            {
                int count = size > des.Length ? des.Length : size;
                Array.Copy(src, 0, des, 0, count);
            }
            catch (Exception)
            {
                XUtliPoolLib.XDebug.singleton.AddErrorLog2("Assign SrcSize:{0} DesSize:{1} DesBuff:{2}", src.Length, size, buffer.Length);
            }            
        }
    }

    protected virtual void TrimNull()
    {

    }

    protected override void Trim()
    {
        if (UseFastList)
        {
            if (size > 0)
            {
                if (size <= buffer.Length / 2)
                {
                    T[] newList = ms_Pool.GetBuff(size);
                    if (buffer != newList)
                    {
                        ms_Pool.ReturnBuff(buffer);
                        Assign(buffer, newList);
                        buffer = newList;
                    }
                }
                else
                {
                    TrimNull();
                }
                
            }
            else
            {
                ms_Pool.ReturnBuff(buffer);
                buffer = null;
            }
        }
        else
        {
            base.Trim();
        }
    }

    public override void Release()
    {
        if (UseFastList)
        {
            size = 0;
            ms_Pool.ReturnBuff(buffer);
            buffer = null;
        }
        else
        {
            base.Release();
        }
    }

    public override void Clear()
    {
        base.Clear();
        if (UseFastList && buffer != null)
        {
            ms_Pool.ReturnBuff(buffer);
            buffer = null;
        }
    }

    public bool CopyFrom(FastList<T> src)
    {
        bool hasExcep = false;
        int newSize = size + src.size;
        if (buffer == null || newSize > buffer.Length)
            AllocateMore(src.size);
        try
        {
            Array.Copy(src.buffer, 0, buffer, size, src.size);
        }
        catch(Exception)
        {
            XUtliPoolLib.XDebug.singleton.AddErrorLog2("SrcSize:{0} DesSize:{1} DesBuff:{2}", src.size, size, buffer.Length);
            hasExcep = true;
        }
        
        size += src.size;
        return hasExcep;
    }
}


public sealed class FastListV3 : FastList<Vector3>
{

    private static Vector3 vec3 = new Vector3();
    //protected override void Assign(Vector3[] src, Vector3[] des)
    //{
    //    if (src != null && des != null && size > 0)
    //    {
    //        Array.Copy(src, 0, des, 0, src.Length);
    //    }
    //}


    protected override void TrimNull()
    {
        for (int i = size; i < buffer.Length; ++i)
        {
            buffer[i].x = 10000;
            buffer[i].y = 10000;
            buffer[i].z = 10000;
        }
    }
    
    public void Add(float x, float y, float z)
    {
        if (UseFastList)
        {
            if (buffer == null || size == buffer.Length)
                AllocateMore();
            int index = size++;
            if (index < buffer.Length)
            {
                buffer[index].x = x;
                buffer[index].y = y;
                buffer[index].z = z;
            }
        }
        else
        {
            vec3.x = x;
            vec3.y = y;
            vec3.z = z;
            Add(vec3);
        }
    }


    public override void Add(Vector3 v)
    {
        if (UseFastList)
        {
            if (buffer == null || size == buffer.Length)
                AllocateMore();
            int index = size++;
            if (index < buffer.Length)
            {
                buffer[index] = v;
            }
        }
        else
        {
            Add(v);
        }
    }
}

public sealed class FastListV2 : FastList<Vector2>
{
    private static Vector2 vec2 = new Vector2();

    //protected override void Assign(Vector2[] src, Vector2[] des)
    //{
    //    if (src != null && des != null && size > 0)
    //    {
    //        Array.Copy(src, 0, des, 0, src.Length);
    //    }
    //}

    protected override void TrimNull()
    {
        for (int i = size; i < buffer.Length; ++i)
        {
            buffer[i].x = -10;
            buffer[i].y = -10;
        }
    }

    public void Add(float x, float y)
    {
        if (UseFastList)
        {
            if (buffer == null || size == buffer.Length)
                AllocateMore();
            int index = size++;
            if (index < buffer.Length)
            {
                buffer[index].x = x;
                buffer[index].y = y;
            }
        }
        else
        {
            vec2.x = x;
            vec2.y = y;
            Add(vec2);
        }
    }

}

public sealed class FastListColor32 : FastList<Color32>
{
    private static Color32 color32 = new Color32();
    //protected override void Assign(Color32[] src, Color32[] des)
    //{
    //    if (src != null && des != null && size > 0)
    //    {
    //        //for (int i = 0; i < src.Length && i < size; ++i)
    //        //{
    //        //    des[i] = src[i];
    //        //}
    //        try
    //        {
    //            int count = size > des.Length ? des.Length : size;
    //            Array.Copy(src, 0, des, 0, count);
    //        }
    //        catch (Exception)
    //        {
    //            XUtliPoolLib.XDebug.singleton.AddErrorLog2("SrcSize:{0} DesSize:{1} DesBuff:{2}", src.Length, size, buffer.Length);
    //        }
    //    }
    //}

    //protected override void TrimNull()
    //{
    //    //for (int i = 0; i < buffer.Length; ++i)
    //    //{
    //    //    buffer[i].x = -10;
    //    //    buffer[i].y = -10;
    //    //}
    //}

    public void Add(byte r, byte g, byte b, byte a)
    {
        if (UseFastList)
        {
            if (buffer == null || size == buffer.Length)
                AllocateMore();
            int index = size++;
            buffer[index].r = r;
            buffer[index].g = g;
            buffer[index].b = b;
            buffer[index].a = a;
        }
        else
        {
            //if (buffer == null || size == buffer.Length)
            //    AllocateMore();
            //int index = size++;
            //buffer[index].r = r;
            //buffer[index].g = g;
            //buffer[index].r = b;
            //buffer[index].g = a;

            color32.r = r;
            color32.g = g;
            color32.b = b;
            color32.a = a;
            Add(color32);
        }
    }
}