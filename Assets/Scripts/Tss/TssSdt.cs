//TssSdt.cs
//          $ create_at: 2014/4/23 author:saite
//          $ log: 2014/10/22 fix TssSdtFloat, TssSdtDouble got NaN error problem
//          $ log: 2014/12/22 modify operator++, operator-- default imp on null obj
//
//support types:
//  byte -- ok
//  short -- ok
//  ushort -- ok
//  int -- ok
//  uint -- ok
//  long -- ok
//  ulong -- ok
//  float -- ok
//  double -- ok

//#define __VIEW_DESTRUCTOR__                   //close this marco for release version 
//#define __ENABLE_UNIT_TEST__                    //close this marco for release version
//#define __WIN_MEM_STAT__                        //close this marco for release version
//#define __DEBUG_ON_UNITY3D_ANDROID__            //close this marco for release version

#define __TSS_DATATYPE_TEMPLATE__               //this marco should always be opened
#define __TSS_DATATYPE_GENERATE__               //this marco should always be opened
#define __TSS_DATETYPE_TEST_TEMPLATE__          //this marco should always be opened
#define __TSS_DATETYPE_TEST_GENERATE__          //this marco should always be opened


using System;

#if __WIN_MEM_STAT__
    using System.Runtime.InteropServices;
#endif


public class TssSdtDataTypeFactory
{
    private static byte m_byte_xor_key;
    private static short m_short_xor_key;
    private static ushort m_ushort_xor_key;
    private static int m_int_xor_key;
    private static uint m_uint_xor_key;
    private static long m_long_xor_key;
    private static ulong m_ulong_xor_key;
    
    public static byte GetByteXORKey()
    {
        if (m_byte_xor_key == 0)
        {
            Random rand = new Random();
            m_byte_xor_key = (byte)rand.Next(0, 0xff);
        }
        //m_byte_xor_key++;
        return m_byte_xor_key;
    }

    public static void SetByteXORKey(byte v)
    {
        m_byte_xor_key = v;
    }

    public static short GetShortXORKey()
    {
        if (m_short_xor_key == 0)
        {
            Random rand = new Random();
            m_short_xor_key = (short)rand.Next(0, 0xffff);
        }
        //m_short_xor_key++;
        return m_short_xor_key;
    }
    public static ushort GetUshortXORKey()
    {
        if (m_ushort_xor_key == 0)
        {
            Random rand = new Random();
            m_ushort_xor_key = (ushort)rand.Next(0, 0xffff);
        }
        //m_ushort_xor_key++;
        return m_ushort_xor_key;
    }
    public static int GetIntXORKey()
    {
        if (m_int_xor_key == 0)
        {
            Random rand = new Random();
            m_int_xor_key = rand.Next(0, 0xffff);
        }
        //m_int_xor_key++;
        return m_int_xor_key;
    }
    public static uint GetUintXORKey()
    {
        if (m_uint_xor_key == 0)
        {
            Random rand = new Random();
            m_uint_xor_key = (uint)rand.Next(0, 0xffff);
        }
        //m_uint_xor_key++;
        return m_uint_xor_key;
    }
    public static long GetLongXORKey()
    {
        if (m_long_xor_key == 0)
        {
            Random rand = new Random();
            m_long_xor_key = (long)rand.Next(0, 0xffff); 
        }
        //m_long_xor_key++;
        return m_long_xor_key;
    }
    public static ulong GetUlongXORKey()
    {
        if (m_ulong_xor_key == 0)
        {
            Random rand = new Random();
            m_ulong_xor_key = (ulong)rand.Next(0, 0xffff);
        }
        //m_ulong_xor_key++;
        return m_ulong_xor_key;
    }

    public static int GetRandomValueIndex()
    {
        return m_int_xor_key;
    }

    public static int GetValueArraySize()
    {
        return 3;
    }

    public static uint GetFloatEncValue(float v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key;
        }
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static float GetFloatDecValue(uint v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key;
        }
        return BitConverter.ToSingle(bytes, 0);
    }

    public static ulong GetDoubleEncValue(double v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key;
        }
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static double GetDoubleDecValue(ulong v, byte key)
    {
        byte[] bytes = BitConverter.GetBytes(v);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key;
        }
        return BitConverter.ToDouble(bytes, 0);
    }

#if (__VIEW_DESTRUCTOR__)
    private static int m_destruct_cnt;
    public static void LogDestructCnt()
    {
        System.Console.WriteLine("~TssSdtIntSlot():{0}", m_destruct_cnt++);
    }
#endif
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtSlotBase
{
    ~TssSdtSlotBase()
    {
        TssSdtDataTypeFactory.LogDestructCnt();
    }
}
#endif

#if (__TSS_DATATYPE_TEMPLATE__)     //do NOT edit this line

#if (__VIEW_DESTRUCTOR__)
public class TssSdtIntSlot : TssSdtSlotBase
#else
public class TssSdtIntSlot
#endif
{
    private int[] m_value;
    private int m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtIntSlot NewSlot(TssSdtIntSlot slot)
    {
        CollectSlot(slot);
        TssSdtIntSlot new_slot = new TssSdtIntSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtIntSlot slot)
    {
    }
    public TssSdtIntSlot()
    {
        m_value = new int[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(int v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetIntXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		m_value[index] = v ^ m_xor_key;
        m_index = index;
    }
    public int GetValue()
    {
        int v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtInt
{
    private TssSdtIntSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtInt NewTssSdtInt()
    {
        TssSdtInt obj = new TssSdtInt();
        obj.m_slot = TssSdtIntSlot.NewSlot(null);
        return obj;
    }
    private int GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtIntSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(int v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtIntSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }
    public static implicit operator int(TssSdtInt v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtInt(int v)
    {
        TssSdtInt obj = new TssSdtInt();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtInt a, TssSdtInt b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtInt a, TssSdtInt b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtInt operator++(TssSdtInt v)
    {
        TssSdtInt obj = new TssSdtInt();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            int new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtInt operator--(TssSdtInt v)
    {
        TssSdtInt obj = new TssSdtInt();
        if (v == null)
        {
            obj.SetValue(-1);
        }
        else
        {
            int new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}
#endif      //__TSS_DATATYPE_TEMPLATE__    //do NOT edit this line

#if (__TSS_DATATYPE_GENERATE__)            //do NOT edit this line, do NOT write codes between __TSS_DATATYPE_GENERATE__

#if (__VIEW_DESTRUCTOR__)
public class TssSdtUintSlot : TssSdtSlotBase
#else
public class TssSdtUintSlot
#endif
{
    private uint[] m_value;
    private uint m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtUintSlot NewSlot(TssSdtUintSlot slot)
    {
        CollectSlot(slot);
        TssSdtUintSlot new_slot = new TssSdtUintSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtUintSlot slot)
    {
    }
    public TssSdtUintSlot()
    {
        m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(uint v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetUintXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		m_value[index] = v ^ m_xor_key;
		m_index = index;
    }
    public uint GetValue()
    {
        uint v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtUint
{
    private TssSdtUintSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtUint NewTssSdtUint()
    {
        TssSdtUint obj = new TssSdtUint();
        obj.m_slot = TssSdtUintSlot.NewSlot(null);
        return obj;
    }
    private uint GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUintSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(uint v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUintSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator uint(TssSdtUint v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtUint(uint v)
    {
        TssSdtUint obj = new TssSdtUint();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtUint a, TssSdtUint b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtUint a, TssSdtUint b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtUint operator++(TssSdtUint v)
    {
        TssSdtUint obj = new TssSdtUint();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            uint new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtUint operator--(TssSdtUint v)
    {
        TssSdtUint obj = new TssSdtUint();
        if (v == null)
        {
            uint new_v = 0;
            new_v -= 1;
            obj.SetValue(new_v);
        }
        else
        {
            uint new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtLongSlot : TssSdtSlotBase
#else
public class TssSdtLongSlot
#endif
{
    private long[] m_value;
    private long m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtLongSlot NewSlot(TssSdtLongSlot slot)
    {
        CollectSlot(slot);
        TssSdtLongSlot new_slot = new TssSdtLongSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtLongSlot slot)
    {
    }
    public TssSdtLongSlot()
    {
        m_value = new long[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(long v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetLongXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		m_value[index] = v ^ m_xor_key;
		m_index = index;
    }
    public long GetValue()
    {
        long v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtLong
{
    private TssSdtLongSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtLong NewTssSdtLong()
    {
        TssSdtLong obj = new TssSdtLong();
        obj.m_slot = TssSdtLongSlot.NewSlot(null);
        return obj;
    }
    private long GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtLongSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(long v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtLongSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator long(TssSdtLong v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtLong(long v)
    {
        TssSdtLong obj = new TssSdtLong();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtLong a, TssSdtLong b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtLong a, TssSdtLong b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtLong operator++(TssSdtLong v)
    {
        TssSdtLong obj = new TssSdtLong();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            long new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtLong operator--(TssSdtLong v)
    {
        TssSdtLong obj = new TssSdtLong();
        if (v == null)
        {
            obj.SetValue(-1);
        }
        else
        {
            long new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtUlongSlot : TssSdtSlotBase
#else
public class TssSdtUlongSlot
#endif
{
    private ulong[] m_value;
    private ulong m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtUlongSlot NewSlot(TssSdtUlongSlot slot)
    {
        CollectSlot(slot);
        TssSdtUlongSlot new_slot = new TssSdtUlongSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtUlongSlot slot)
    {
    }
    public TssSdtUlongSlot()
    {
        m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(ulong v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetUlongXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		m_value[index] = v ^ m_xor_key;
		m_index = index;
    }
    public ulong GetValue()
    {
        ulong v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtUlong
{
    private TssSdtUlongSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtUlong NewTssSdtUlong()
    {
        TssSdtUlong obj = new TssSdtUlong();
        obj.m_slot = TssSdtUlongSlot.NewSlot(null);
        return obj;
    }
    private ulong GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUlongSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(ulong v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUlongSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator ulong(TssSdtUlong v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtUlong(ulong v)
    {
        TssSdtUlong obj = new TssSdtUlong();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtUlong a, TssSdtUlong b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtUlong a, TssSdtUlong b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtUlong operator++(TssSdtUlong v)
    {
        TssSdtUlong obj = new TssSdtUlong();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            ulong new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtUlong operator--(TssSdtUlong v)
    {
        TssSdtUlong obj = new TssSdtUlong();
        if (v == null)
        {
            ulong new_v = 0;
            new_v -= 1;
            obj.SetValue(new_v);
        }
        else
        {
            ulong new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtShortSlot : TssSdtSlotBase
#else
public class TssSdtShortSlot
#endif
{
    private short[] m_value;
    private short m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtShortSlot NewSlot(TssSdtShortSlot slot)
    {
        CollectSlot(slot);
        TssSdtShortSlot new_slot = new TssSdtShortSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtShortSlot slot)
    {
    }
    public TssSdtShortSlot()
    {
        m_value = new short[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(short v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetShortXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		short enc_v = v;
		enc_v ^= m_xor_key;
		m_value[index] = enc_v;
		m_index = index;
    }
    public short GetValue()
    {
        short v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtShort
{
    private TssSdtShortSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtShort NewTssSdtShort()
    {
        TssSdtShort obj = new TssSdtShort();
        obj.m_slot = TssSdtShortSlot.NewSlot(null);
        return obj;
    }
    private short GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtShortSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(short v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtShortSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator short(TssSdtShort v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtShort(short v)
    {
        TssSdtShort obj = new TssSdtShort();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtShort a, TssSdtShort b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtShort a, TssSdtShort b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtShort operator++(TssSdtShort v)
    {
        TssSdtShort obj = new TssSdtShort();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            short new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtShort operator--(TssSdtShort v)
    {
        TssSdtShort obj = new TssSdtShort();
        if (v == null)
        {
            obj.SetValue(-1);
        }
        else
        {
            short new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtUshortSlot : TssSdtSlotBase
#else
public class TssSdtUshortSlot
#endif
{
    private ushort[] m_value;
    private ushort m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtUshortSlot NewSlot(TssSdtUshortSlot slot)
    {
        CollectSlot(slot);
        TssSdtUshortSlot new_slot = new TssSdtUshortSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtUshortSlot slot)
    {
    }
    public TssSdtUshortSlot()
    {
        m_value = new ushort[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(ushort v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetUshortXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		ushort enc_v = v;
		enc_v ^= m_xor_key;
		m_value[index] = enc_v;
		m_index = index;
    }
    public ushort GetValue()
    {
        ushort v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtUshort
{
    private TssSdtUshortSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtUshort NewTssSdtUshort()
    {
        TssSdtUshort obj = new TssSdtUshort();
        obj.m_slot = TssSdtUshortSlot.NewSlot(null);
        return obj;
    }
    private ushort GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUshortSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(ushort v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtUshortSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator ushort(TssSdtUshort v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtUshort(ushort v)
    {
        TssSdtUshort obj = new TssSdtUshort();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtUshort a, TssSdtUshort b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtUshort a, TssSdtUshort b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtUshort operator++(TssSdtUshort v)
    {
        TssSdtUshort obj = new TssSdtUshort();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            ushort new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtUshort operator--(TssSdtUshort v)
    {
        TssSdtUshort obj = new TssSdtUshort();
        if (v == null)
        {
            ushort new_v = 0;
            new_v -= 1;
            obj.SetValue(new_v);
        }
        else
        {
            ushort new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtByteSlot : TssSdtSlotBase
#else
public class TssSdtByteSlot
#endif
{
    private byte[] m_value;
    private byte m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtByteSlot NewSlot(TssSdtByteSlot slot)
    {
        CollectSlot(slot);
        TssSdtByteSlot new_slot = new TssSdtByteSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtByteSlot slot)
    {
    }
    public TssSdtByteSlot()
    {
        m_value = new byte[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(byte v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		byte enc_v = v;
		enc_v ^= m_xor_key;
		m_value[index] = enc_v;
		m_index = index;
    }
    public byte GetValue()
    {
        byte v = m_value[m_index];
        v ^= m_xor_key;
        return v;
    }
}

public class TssSdtByte
{
    private TssSdtByteSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtByte NewTssSdtByte()
    {
        TssSdtByte obj = new TssSdtByte();
        obj.m_slot = TssSdtByteSlot.NewSlot(null);
        return obj;
    }
    private byte GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtByteSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(byte v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtByteSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator byte(TssSdtByte v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtByte(byte v)
    {
        TssSdtByte obj = new TssSdtByte();
        obj.SetValue(v);
        return obj;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtByte operator++(TssSdtByte v)
    {
        TssSdtByte obj = new TssSdtByte();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            byte new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtByte a, TssSdtByte b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtByte a, TssSdtByte b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtByte operator--(TssSdtByte v)
    {
        TssSdtByte obj = new TssSdtByte();
        if (v == null)
        {
            byte new_v = 0;
            new_v -= 1;
            obj.SetValue(new_v);
        }
        else
        {
            byte new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}
#endif      //__TSS_DATATYPE_GENERATE__    //do NOT edit this line, do NOT write codes between __TSS_DATATYPE_GENERATE__

#if (__VIEW_DESTRUCTOR__)
public class TssSdtFloatSlot : TssStdSlotBase
#else
public class TssSdtFloatSlot
#endif
{
    private uint[] m_value;
    private byte m_xor_key;
    private int m_index;
	
    //reserver for custom memory pool imp
    public static TssSdtFloatSlot NewSlot(TssSdtFloatSlot slot)
    {
        CollectSlot(slot);
        TssSdtFloatSlot new_slot = new TssSdtFloatSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtFloatSlot slot)
    {
    }
    public TssSdtFloatSlot()
    {
        m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(float v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
		m_value[index] = TssSdtDataTypeFactory.GetFloatEncValue(v, m_xor_key);
		m_index = index;
    }
    public float GetValue()
    {
        uint v = m_value[m_index];
        float dec_v = TssSdtDataTypeFactory.GetFloatDecValue(v, m_xor_key);
        return dec_v;
    }
}

public class TssSdtFloat
{
    private TssSdtFloatSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtFloat NewTssSdtFloat()
    {
        TssSdtFloat obj = new TssSdtFloat();
        obj.m_slot = TssSdtFloatSlot.NewSlot(null);
        return obj;
    }
	
    private float GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtFloatSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(float v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtFloatSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator float(TssSdtFloat v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtFloat(float v)
    {
        TssSdtFloat obj = new TssSdtFloat();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtFloat a, TssSdtFloat b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtFloat a, TssSdtFloat b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtFloat operator++(TssSdtFloat v)
    {
        TssSdtFloat obj = new TssSdtFloat();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            float new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtFloat operator--(TssSdtFloat v)
    {
        TssSdtFloat obj = new TssSdtFloat();
        if (v == null)
        {
            obj.SetValue(-1);
        }
        else
        {
            float new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

#if (__VIEW_DESTRUCTOR__)
public class TssSdtDoubleSlot : TssStdSlotBase
#else
public class TssSdtDoubleSlot
#endif
{
    private ulong[] m_value;
    private byte m_xor_key;
    private int m_index;

    //reserver for custom memory pool imp
    public static TssSdtDoubleSlot NewSlot(TssSdtDoubleSlot slot)
    {
        CollectSlot(slot);
        TssSdtDoubleSlot new_slot = new TssSdtDoubleSlot();
        return new_slot;
    }
    //reserver for custom memory pool imp
    private static void CollectSlot(TssSdtDoubleSlot slot)
    {
    }
    public TssSdtDoubleSlot()
    {
        m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
        m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % m_value.Length;
    }

    public void SetValue(double v)
    {
        m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int index = m_index + 1;
		if (index == m_value.Length)
		{
			index = 0;
		}
        m_value[index] = TssSdtDataTypeFactory.GetDoubleEncValue(v, m_xor_key);
		m_index = index;
    }
    public double GetValue()
    {
        ulong v = m_value[m_index];
        double dec_v = TssSdtDataTypeFactory.GetDoubleDecValue(v, m_xor_key);
        return dec_v;
    }
}

public class TssSdtDouble
{
    private TssSdtDoubleSlot m_slot;

    //reserver for custom memory pool imp
    public static TssSdtDouble NewTssSdtDouble()
    {
        TssSdtDouble obj = new TssSdtDouble();
        obj.m_slot = TssSdtDoubleSlot.NewSlot(null);
        return obj;
    }
    private double GetValue()
    {
        if (m_slot == null)
        {
            m_slot = TssSdtDoubleSlot.NewSlot(null);
        }
        return m_slot.GetValue();
    }
    private void SetValue(double v)
    {
        if (m_slot == null)
        {
            m_slot = TssSdtDoubleSlot.NewSlot(null);
        }
        m_slot.SetValue(v);
    }

    public static implicit operator double(TssSdtDouble v)
    {
        if (v == null)
        {
            return 0;
        }
        return v.GetValue();
    }
    public static implicit operator TssSdtDouble(double v)
    {
        TssSdtDouble obj = new TssSdtDouble();
        obj.SetValue(v);
        return obj;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return GetValue().GetHashCode();
    }
    public static bool operator ==(TssSdtDouble a, TssSdtDouble b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return true;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() == b.GetValue();
        }
        return false;
    }
    public static bool operator !=(TssSdtDouble a, TssSdtDouble b)
    {
        if (Object.Equals(a, null) && Object.Equals(b, null))
        {
            return false;
        }
        if (!Object.Equals(a, null) && !Object.Equals(b, null))
        {
            return a.GetValue() != b.GetValue();
        }
        return true;
    }
    //compile err in Unity3D if we don't override operator++
    public static TssSdtDouble operator++(TssSdtDouble v)
    {
        TssSdtDouble obj = new TssSdtDouble();
        if (v == null)
        {
            obj.SetValue(1);
        }
        else
        {
            double new_v = v.GetValue();
            new_v += 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    //compile err in Unity3D if we don't override operator--
    public static TssSdtDouble operator--(TssSdtDouble v)
    {
        TssSdtDouble obj = new TssSdtDouble();
        if (v == null)
        {
            obj.SetValue(-1);
        }
        else
        {
            double new_v = v.GetValue();
            new_v -= 1;
            obj.SetValue(new_v);
        }
        return obj;
    }
    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

class TssSdtVersion 
{
	private const string  cs_sdt_version = "C# SDT ver: 1.8.5(2015/05/13)";
	public static string GetSdtVersion()
	{
		return cs_sdt_version;	
	}
}

#if __ENABLE_UNIT_TEST__
public class TssSdtUnitTest
{
    private int m_raw_unassigned_variable;
    private TssSdtInt m_enc_unassigned_variable;

    public void RunTest()
    {
        Assert(true, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //Assert(false, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        
        TestDataConvert();

        TestNullOp();
        TestEqualOp();
        TestTssSdtInt();
        TestTssSdtUint();
        TestTssSdtLong();
        TestTssSdtUlong();
        TestTssSdtShort();
        TestTssSdtUshort();
        TestTssSdtByte();
        TestTssSdtFloat();
        TestTssSdtDouble();

        TestTssSdtFloatXOREnc();
        TestTssSdtDoubleXOREnc();

        //TestTssSdtIntPerformace(0, 10); 
        //TestTssSdtIntPerformace(0, 10000 * 1);      //use 480K RAM        -- 30x slow on win, 60x slow on android
        //TestTssSdtIntPerformace(0, 10000 * 10);     //use 2.804M RAM      -- 30x slow on win, 60x slow on android
        //TestTssSdtIntPerformace(0, 10000 * 100);    //use 2.901M RAM      -- 30x slow on win, 60x slow on android
        //TestTssSdtIntPerformace(0, 10000 * 1000);   //use 2.85M RAM       -- 30x slow on win, 60x slow on android

        //TestTssSdtFloatPerformace(0, 10000 * 1);      //use 630K RAM      -- 30x slow on win, 60x slow on android
        //TestTssSdtFloatPerformace(0, 10000 * 10);       //use 3.37M RAM
        //TestTssSdtFloatPerformace(0, 10000 * 100);        //use 3.09M RAM
        //TestTssSdtFloatPerformace(0, 10000 * 1000);       //use 3.25M RAM   -- 30x slow on win, 60x slow on android

        //TestTssSdtDoublePerformace(0, 10000 * 1);       //use 1.8M RAM        -- 35x slow on win, 60x slow on android
        //TestTssSdtDoublePerformace(0, 10000 * 10);        //use 2.5M RAM
        //TestTssSdtDoublePerformace(0, 10000 * 100);         //use 2.9M RAM
        //TestTssSdtDoublePerformace(0, 10000 * 1000);        //use 2.34 RAM
        //TestTssSdtDoublePerformace(0, 10000 * 10000);

        Printf("done.");
    }

    private void TestTssSdtFloatXOREnc()
    {
        for (int i = -100; i < 100; i++)
        { 
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                float raw_f = i;
                TssSdtFloat enc_f = raw_f;
                Assert(raw_f == enc_f, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                float raw_f = i * 10;
                TssSdtFloat enc_f = raw_f;
                Assert(raw_f == enc_f, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                float raw_f = i * 100;
                TssSdtFloat enc_f = raw_f;
                Assert(raw_f == enc_f, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                float raw_f = i * 1000;
                TssSdtFloat enc_f = raw_f;
                Assert(raw_f == enc_f, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
        }
    }

    private void TestTssSdtDoubleXOREnc()
    {
        for (int i = -100; i < 100; i++)
        {
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                double raw_d = i;
                TssSdtDouble enc_d = raw_d;
                Assert(raw_d == enc_d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                double raw_d = i * 10;
                TssSdtDouble enc_d = raw_d;
                Assert(raw_d == enc_d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                double raw_d = i * 100;
                TssSdtDouble enc_d = raw_d;
                Assert(raw_d == enc_d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
            for (byte j = 0; j < 0xff; j++)
            {
                TssSdtDataTypeFactory.SetByteXORKey(j);
                double raw_d = i * 1000;
                TssSdtDouble enc_d = raw_d;
                Assert(raw_d == enc_d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
            }
        }
    }

    private void TestByteEqualOp()
    {
        TssSdtByte a = 10;
        TssSdtByte b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestShortEqualOp()
    {
        TssSdtShort a = 10;
        TssSdtShort b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestUshortEqualOp()
    {
        TssSdtUshort a = 10;
        TssSdtUshort b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestIntEqualOp()
    {
        TssSdtInt a = 10;
        TssSdtInt b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestUintEqualOp()
    {
        TssSdtUint a = 10;
        TssSdtUint b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestLongEqualOp()
    {
        TssSdtLong a = 10;
        TssSdtLong b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestUlongEqualOp()
    {
        TssSdtUlong a = 10;
        TssSdtUlong b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestFloatEqualOp()
    {
        TssSdtFloat a = 10;
        TssSdtFloat b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }
    private void TestDoubleEqualOp()
    {
        TssSdtDouble a = 10;
        TssSdtDouble b = 10;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 11;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a < b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        b = 9;
        Assert(a != b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(a > b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestEqualOp()
    {
        TestByteEqualOp();
        TestShortEqualOp();
        TestUshortEqualOp();
        TestIntEqualOp();
        TestUintEqualOp();
        TestLongEqualOp();
        TestUlongEqualOp();
        TestFloatEqualOp();
        TestDoubleEqualOp();
    }

    private void TestNullOp()
    {
        TestByteNullOp();
        TestShortNullOp();
        TestUshortNullOp();
        TestIntNullOp();
        TestUintNullOp();
        TestLongNullOp();
        TestUlongNullOp();
    }

    private void TestByteNullOp()
    {
        byte a = 0;
        TssSdtByte b = null;

        byte c = 0;
        TssSdtByte d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestShortNullOp()
    {
        short a = 0;
        TssSdtShort b = null;

        short c = 0;
        TssSdtShort d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestUshortNullOp()
    {
        ushort a = 0;
        TssSdtUshort b = null;

        ushort c = 0;
        TssSdtUshort d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestIntNullOp()
    {
        int a = 0;
        TssSdtInt b = null;

        int c = 0;
        TssSdtInt d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestUintNullOp()
    {
        uint a = 0;
        TssSdtUint b = null;

        uint c = 0;
        TssSdtUint d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestLongNullOp()
    {
        long a = 0;
        TssSdtLong b = null;

        long c = 0;
        TssSdtLong d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

    private void TestUlongNullOp()
    {
        ulong a = 0;
        TssSdtUlong b = null;

        ulong c = 0;
        TssSdtUlong d = null;

        a++;
        b++;
        Assert(a == b, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        c--;
        d--;
        Assert(c == d, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
    }

#if __WIN_MEM_STAT__
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public uint dwTotalPhys;
        public uint dwAvailPhys;
        public uint dwTotalPageFile;
        public uint dwAvailPageFile;
        public uint dwTotalVirtual;
        public uint dwAvailVirtual;
    }
    [DllImport("kernel32")]
    public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
    public uint GetAvailPhys()
    {
        MEMORY_INFO MemInfo;
        MemInfo = new MEMORY_INFO();
        GlobalMemoryStatus(ref MemInfo);
        return MemInfo.dwAvailPhys;
    }
#else
    public uint GetAvailPhys()
    {
        return 0;
    }
#endif
    private void Printf(string str)
    {
#if (__DEBUG_ON_UNITY3D_ANDROID__)
        FileLog.Log(str);
#else
        System.Console.WriteLine(str);
#endif
    }
    private void PrintMem(uint mem)
    {
        if (mem < 1024) Printf(string.Format("mem:{0}B", mem));
        else if (mem < 1024 * 1024) Printf(string.Format("mem:{0}K", mem / 1024.0));
        else Printf(string.Format("mem:{0}M", mem / 1024.0 / 1024.0));
    }
    private void Assert(bool result, int line)
    {
        if (!result)
        {
            Printf(String.Format("assert err, line:{0}", line));
        }
    }
	private void TestDataConvert()
	{
		int raw_a = 0;
		//raw_a += 1.3f;				//err
		raw_a += 44;
		
		TssSdtInt enc_a = 0;
		//enc_a += 1.3f;
		enc_a += 44;
	}

#if (__TSS_DATETYPE_TEST_TEMPLATE__) 
    //test for operator override
    private void TestTssSdtInt()
    {
        TssSdtInt enc_a = 0;
        int raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = enc_a >> 1;
        raw_a = raw_a >> 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = enc_a << 1;
        raw_a = raw_a << 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = ~enc_a;
        raw_a = ~raw_a;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = ~enc_a;
        enc_a = ~enc_a;
        //^
        enc_a = enc_a ^ 0x3ff3f;
        raw_a = raw_a ^ 0x3ff3f;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtInt ok.");
    }

    private void TestTssSdtIntPerformace(int i, int cnt)
    { 
        //test raw int
        int raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        int j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw int
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc int
        TssSdtInt enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw int
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
#endif      //__TSS_DATETYPE_TEST_TEMPLATE__

#if (__TSS_DATETYPE_TEST_GENERATE__)            //do NOT edit this line, do NOT write codes between __TSS_DATETYPE_TEST_GENERATE__
    //test for operator override
    private void TestTssSdtUint()
    {
        TssSdtUint enc_a = 0;
        uint raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = enc_a >> 1;
        raw_a = raw_a >> 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = enc_a << 1;
        raw_a = raw_a << 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = ~enc_a;
        raw_a = ~raw_a;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = ~enc_a;
        enc_a = ~enc_a;
        //^
        enc_a = enc_a ^ 0x3ff3f;
        raw_a = raw_a ^ 0x3ff3f;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtUint ok.");
    }

    private void TestTssSdtUintPerformace(uint i, uint cnt)
    { 
        //test raw uint
        uint raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        uint j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw uint
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc uint
        TssSdtUint enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw uint
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
    //test for operator override
    private void TestTssSdtLong()
    {
        TssSdtLong enc_a = 0;
        long raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = enc_a >> 1;
        raw_a = raw_a >> 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = enc_a << 1;
        raw_a = raw_a << 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = ~enc_a;
        raw_a = ~raw_a;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = ~enc_a;
        enc_a = ~enc_a;
        //^
        enc_a = enc_a ^ 0x3ff3f;
        raw_a = raw_a ^ 0x3ff3f;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtLong ok.");
    }

    private void TestTssSdtLongPerformace(long i, long cnt)
    { 
        //test raw long
        long raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        long j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw long
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc long
        TssSdtLong enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw long
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
    //test for operator override
    private void TestTssSdtUlong()
    {
        TssSdtUlong enc_a = 0;
        ulong raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = enc_a >> 1;
        raw_a = raw_a >> 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = enc_a << 1;
        raw_a = raw_a << 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = ~enc_a;
        raw_a = ~raw_a;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = ~enc_a;
        enc_a = ~enc_a;
        //^
        enc_a = enc_a ^ 0x3ff3f;
        raw_a = raw_a ^ 0x3ff3f;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtUlong ok.");
    }

    private void TestTssSdtUlongPerformace(ulong i, ulong cnt)
    { 
        //test raw ulong
        ulong raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        ulong j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw ulong
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc ulong
        TssSdtUlong enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw ulong
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
#endif      //__TSS_DATETYPE_TEST_GENERATE__    //do NOT edit this line, do NOT write codes between __TSS_DATETYPE_TEST_GENERATE__
    //test for operator override
    private void TestTssSdtDouble()
    {
        TssSdtDouble enc_a = 0;
        double raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtDouble ok.");
    }

    private void TestTssSdtDoublePerformace(double i, double cnt)
    { 
        //test raw double
        double raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        double j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw double
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc double
        TssSdtDouble enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw double
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
    //test for operator override
    private void TestTssSdtFloat()
    {
        TssSdtFloat enc_a = 0;
        float raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = enc_a + 1;
        raw_a = raw_a + 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = enc_a - 3;
        raw_a = raw_a - 3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = enc_a * 5;
        raw_a = raw_a * 5;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = enc_a / 7;
        raw_a = raw_a / 7;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtFloat ok.");
    }

    private void TestTssSdtFloatPerformace(float i, float cnt)
    { 
        //test raw float
        float raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        float j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw float
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc float
        TssSdtFloat enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw float
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
    //test for operator override
    private void TestTssSdtByte()
    {
        TssSdtByte enc_a = 0;
        byte raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = (TssSdtByte)(enc_a + 1);
        raw_a = (byte)(raw_a + 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = (TssSdtByte)(enc_a - 3);
        raw_a = (byte)(raw_a - 3);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = (TssSdtByte)(enc_a * 5);
        raw_a = (byte)(raw_a * 5);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 0xf3;
        raw_a = 0xf3;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = (TssSdtByte)(enc_a / 7);
        raw_a = (byte)(raw_a / 7);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = (TssSdtByte)(enc_a >> 1);
        raw_a = (byte)(raw_a >> 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = (TssSdtByte)(enc_a << 1);
        raw_a = (byte)(raw_a << 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = (TssSdtByte)(10 * 4 + 3 % 4);
        raw_a = (byte)(10 * 4 + 3 % 4);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = (TssSdtByte)(~enc_a);
        raw_a = (byte)~raw_a;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = (TssSdtByte)~enc_a;
        enc_a = (byte)~enc_a;
        //^
        enc_a = (TssSdtByte)(enc_a ^ 0xcc);
        raw_a = (byte)(raw_a ^ 0xcc);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtByte ok.");
    }

    private void TestTssSdtBytePerformace(byte i, byte cnt)
    { 
        //test raw byte
        byte raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        byte j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw byte
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc byte
        TssSdtByte enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw byte
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }
    //test for operator override
    private void TestTssSdtUshort()
    {
        TssSdtUshort enc_a = 0;
        ushort raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = (TssSdtUshort)(enc_a + 1);
        raw_a = (ushort)(raw_a + 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = (TssSdtUshort)(enc_a - 3);
        raw_a = (ushort)(raw_a - 3);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = (TssSdtUshort)(enc_a * 5);
        raw_a = (ushort)(raw_a * 5);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = (TssSdtUshort)(enc_a / 7);
        raw_a = (ushort)(raw_a / 7);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = (TssSdtUshort)(enc_a >> 1);
        raw_a = (ushort)(raw_a >> 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = (TssSdtUshort)(enc_a << 1);
        raw_a = (ushort)(raw_a << 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = (TssSdtUshort)(~enc_a);
        raw_a = (ushort)(~raw_a);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = (TssSdtUshort)(~enc_a);
        enc_a = (ushort)~enc_a;
        //^
        enc_a = (TssSdtUshort)(enc_a ^ 0x3ff3f);
        raw_a = (ushort)(raw_a ^ 0x3ff3f);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtUshort ok.");
    }

    private void TestTssSdtUshortPerformace(ushort i, ushort cnt)
    { 
        //test raw ushort
        ushort raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        ushort j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw ushort
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc ushort
        TssSdtUshort enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw ushort
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }

    //test for operator override
    private void TestTssSdtShort()
    {
        TssSdtShort enc_a = 0;
        short raw_a = 0;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //+
        enc_a = (TssSdtShort)(enc_a + 1);
        raw_a = (short)(raw_a + 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //+=
        enc_a += 2;
        raw_a += 2;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-
        enc_a = (TssSdtShort)(enc_a - 3);
        raw_a = (short)(raw_a - 3);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //-=
        enc_a -= 4;
        raw_a -= 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*
        enc_a = (short)(enc_a * 5);
        raw_a = (TssSdtShort)(raw_a * 5);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //*=
        enc_a *= 6;
        raw_a *= 6;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide
        enc_a = 1024;
        raw_a = 1024;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = (TssSdtShort)(enc_a / 7);
        raw_a = (short)(raw_a / 7);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //divide=
        enc_a /= 8;
        raw_a /= 8;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //++
        enc_a++;
        raw_a++;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        enc_a = 100;
        raw_a = 100;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(raw_a == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        Assert(enc_a++ == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(++enc_a == 101, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;

        //--
        enc_a--;
        raw_a--;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(enc_a-- == 100, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        enc_a = 100;
        Assert(--enc_a == 99, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        raw_a = enc_a;
        //>>
        enc_a = (TssSdtShort)(enc_a >> 1);
        raw_a = (short)(raw_a >> 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //>>=
        enc_a >>= 1;
        raw_a >>= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<
        enc_a = (TssSdtShort)(enc_a << 1);
        raw_a = (short)(raw_a << 1);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //<<=
        enc_a <<= 1;
        raw_a <<= 1;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%
        enc_a = 100 * 4 + 3 % 4;
        raw_a = 100 * 4 + 3 % 4;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //%=
        enc_a %= 10;
        raw_a %= 10;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //~
        enc_a = (TssSdtShort)(~enc_a);
        raw_a = (TssSdtShort)(~raw_a);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        raw_a = (TssSdtShort)(~enc_a);
        enc_a = (TssSdtShort)(~enc_a);
        //^
        enc_a = (TssSdtShort)(enc_a ^ 0x3ff3f);
        raw_a = (TssSdtShort)(raw_a ^ 0x3ff3f);
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());
        //^=
        enc_a ^= 33;
        raw_a ^= 33;
        Assert(enc_a == raw_a, new System.Diagnostics.StackFrame(true).GetFileLineNumber());

        //safe test
        m_raw_unassigned_variable++;
        m_enc_unassigned_variable++;

        Printf("TestTssSdtShort ok.");
    }

    private void TestTssSdtShortPerformace(short i, short cnt)
    { 
        //test raw short
        short raw_a = 100;
        DateTime beg_dt;
        DateTime end_dt;
        short j = i;
        int t1, t2, t3;
        uint mem_before;
        uint mem_after;

        //test for raw short
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //test enc short
        TssSdtShort enc_a;
        enc_a = 100;
        beg_dt = DateTime.Now;
        i = j;

        mem_before = GetAvailPhys();

        for (; i < cnt; i++)
        {
            enc_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond; 
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("enc_a:{0} {1}", enc_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }

        //re test for raw short
        raw_a = 100;
        beg_dt = DateTime.Now;
        i = j;
        mem_before = GetAvailPhys();
        for (; i < cnt; i++)
        {
            raw_a += i;
        }
        end_dt = DateTime.Now;
        t1 = beg_dt.Second * 1000 + beg_dt.Millisecond;
        t2 = end_dt.Second * 1000 + end_dt.Millisecond;
        t3 = t2 - t1;

        mem_after = GetAvailPhys();

        Printf(string.Format("raw_a:{0} {1}", raw_a, t3));
        if (mem_before < mem_after)
        {
            PrintMem(0);
        }
        else
        {
            PrintMem(mem_before - mem_after);
        }
    }


}    //public class TssSdtUnitTest


#endif      //__ENABLE_UNIT_TEST__