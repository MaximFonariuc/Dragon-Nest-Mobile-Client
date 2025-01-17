using UnityEngine;
using System.Collections;

public class Single<T> where T : new()
{
    private static T s_instance;

    public static T Instance
    {
        get { return GetInstance(); }
    }

    protected Single()
    {
    }

    public static void CreateInstance()
    {
        if (s_instance == null)
        {
            s_instance = new T();

            (s_instance as Single<T>).Init();
        }
    }

    public static void DestroyInstance()
    {
        if (s_instance != null)
        {
            (s_instance as Single<T>).UnInit();
            s_instance = default(T);
        }
    }

    public static T GetInstance()
    {
        if (s_instance == null)
        {
            CreateInstance();
        }
        return s_instance;
    }

    public static bool HasInstance()
    {
        return (s_instance != null);
    }

    public virtual void Init()
    {
    }

    public virtual void UnInit()
    {
    }


}
