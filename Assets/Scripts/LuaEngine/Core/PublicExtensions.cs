//
// PublicExtensions.cs
// Created by huailiang.peng on 2016/02/15 10:39:00
//
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

//#if NLUA
//using LuaTable = NLua.LuaTable;
//#else
//using LuaTable = LuaInterface.LuaTable;
//#endif

public static class PublicExtensions
{
    public static List<Type[]> CastNumberParameters(object[] param, Type[] paramTypes)
    {
        List<Type[]> result = new List<Type[]>();
        int doubleTypeNum = 0;
        for (int i = 0; i < paramTypes.Length; i++)
        {
            if (paramTypes[i] != null && paramTypes[i] == typeof(Double))
            {
                doubleTypeNum++;
                var newParameters = new Type[paramTypes.Length];
                for (int j = 0; j < newParameters.Length; j++)
                {
                    if (i == j)
                        newParameters[j] = typeof(double);
                    else
                        newParameters[j] = paramTypes[j];
                }
                result.Add(newParameters);

                newParameters = new Type[paramTypes.Length];
                for (int j = 0; j < newParameters.Length; j++)
                {
                    if (i == j)
                        newParameters[j] = typeof(float);
                    else
                        newParameters[j] = paramTypes[j];
                }
                result.Add(newParameters);

                newParameters = new Type[paramTypes.Length];
                for (int j = 0; j < newParameters.Length; j++)
                {
                    if (i == j)
                        newParameters[j] = typeof(int);
                    else
                        newParameters[j] = paramTypes[j];
                }
                result.Add(newParameters);

                newParameters = new Type[paramTypes.Length];
                for (int j = 0; j < newParameters.Length; j++)
                {
                    if (i == j)
                        newParameters[j] = typeof(uint);
                    else
                        newParameters[j] = paramTypes[j];
                }
                result.Add(newParameters);
            }
        }
        if (doubleTypeNum == 0)
        {
            result.Add(paramTypes);
            if (paramTypes.Length == 1 && paramTypes[0] == typeof(string))
                result.Add(new Type[0]);
        }

        return result;
    }

    // Invoke method
    public static T CallPublicMethodGeneric<T>(this object obj, string name, params object[] param)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        Type[] argTypes = new Type[param.Length];
        for (int i = 0; i < argTypes.Length; i++)
            argTypes[i] = param[i].GetType();
        List<Type[]> argTypeList = CastNumberParameters(param, argTypes);
        MethodInfo method = null;
        try
        {
            method = type.GetMethod(name, flags);
        }
        catch
        {
            for (int i = 0; i < argTypeList.Count; i++)
            {
                method = type.GetMethod(name, argTypeList[i]);
                if (method != null)
                    break;
            }
        }
        if (method == null)
            return default(T);

        ParameterInfo[] pars = method.GetParameters();
        object[] convertedParameters = new object[pars.Length];
        for (int i = 0; i < pars.Length; i++)
        {
            if (pars[i].ParameterType != typeof(object))
                convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
            else
                convertedParameters[i] = param[i];
        }
        return (T)method.Invoke(obj, convertedParameters);
    }

    public static object CallPublicMethod(this object obj, string name, params object[] param)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        Type[] argTypes = new Type[param.Length];
        for (int i = 0; i < argTypes.Length; i++)
            argTypes[i] = param[i].GetType();
        List<Type[]> argTypeList = CastNumberParameters(param, argTypes);
        MethodInfo method = null;
        try
        {
            method = type.GetMethod(name, flags);
        }
        catch
        {
            for (int i = 0; i < argTypeList.Count; i++)
            {
                method = type.GetMethod(name, argTypeList[i]);
                if (method != null)
                    break;
            }
        }
        if (method == null)
            return null;

        ParameterInfo[] pars = method.GetParameters();
        object[] convertedParameters = new object[pars.Length];
        for (int i = 0; i < pars.Length; i++)
        {
            if (pars[i].ParameterType != typeof(object))
                convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
            else
                convertedParameters[i] = param[i];
        }
        return method.Invoke(obj, convertedParameters);
    }

    public static object CallStaticPublicMethod(string typeName, string name, params object[] param)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        Type type = Type.GetType(typeName);
        Type[] argTypes = new Type[param.Length];
        for (int i = 0; i < argTypes.Length; i++)
            argTypes[i] = param[i].GetType();
        List<Type[]> argTypeList = CastNumberParameters(param, argTypes);
        MethodInfo method = null;
        try
        {
            method = type.GetMethod(name, flags);
        }
        catch
        {
            for (int i = 0; i < argTypeList.Count; i++)
            {
                method = type.GetMethod(name, argTypeList[i]);
                if (method != null)
                    break;
            }
        }
        if (method == null)
            return null;

        ParameterInfo[] pars = method.GetParameters();
        object[] convertedParameters = new object[pars.Length];
        for (int i = 0; i < pars.Length; i++)
        {
            if (pars[i].ParameterType != typeof(object))
                convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
            else
                convertedParameters[i] = param[i];
        }
        return method.Invoke(null, convertedParameters);
    }

    // Get feild, property
    public static T GetPublicFieldGeneric<T>(this object obj, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        FieldInfo field = GetFieldInfo(type, name, flags);
        if (field != null)
            return (T)field.GetValue(obj);
        else
            return default(T);
    }
    public static object GetPublicField(this object obj, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        FieldInfo field = GetFieldInfo(type, name, flags);
        if (field != null)
            return field.GetValue(obj);
        else
            return null;
    }
    public static object GetStaticPublicField(string typeName, string name)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
        Type type = Type.GetType(typeName);
        FieldInfo field = GetFieldInfo(type, name, flags);
        if (field != null)
            return field.GetValue(null);
        else
            return null;
    }
    public static FieldInfo GetFieldInfo(Type type, string name, BindingFlags flags)
    {
        if (type == null)
            return null;
        FieldInfo field = type.GetField(name, flags);
        if (field == null && type.BaseType != null)
            return GetFieldInfo(type.BaseType, name, flags);
        return field;
    }

    public static T GetPublicPropertyGeneric<T>(this object obj, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        PropertyInfo field = GetPropertyInfo(type, name, flags);
        if (field != null)
            return (T)field.GetGetMethod(false).Invoke(obj, null);
        else
            return default(T);
    }
    public static object GetPublicProperty(this object obj, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        PropertyInfo field = GetPropertyInfo(type, name, flags);
        if (field != null)
            return field.GetGetMethod(false).Invoke(obj, null);
        else
            return null;
    }
    public static object GetStaticPublicProperty(string typeName, string name)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = Type.GetType(typeName);
        PropertyInfo field = GetPropertyInfo(type, name, flags);
        if (field != null)
            return field.GetValue(null, null);
        return null;
    }

    public static PropertyInfo GetPropertyInfo(Type type, string name, BindingFlags flags)
    {
        if (type == null)
            return null;
        PropertyInfo property = type.GetProperty(name, flags);
        if (property == null && type.BaseType != null)
            return GetPropertyInfo(type.BaseType, name, flags);
        return property;
    }

    // Set field, propertry
    public static void SetPublicField(this object obj, string name, object value)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        FieldInfo field = GetFieldInfo(type, name, flags);
        if (field != null)
        {
            if (field.FieldType == typeof(int))
            {
                var number = Convert.ToInt32(value);
                field.SetValue(obj, number);
                return;
            }
            else if (field.FieldType == typeof(float))
            {
                var number = Convert.ToSingle(value);
                field.SetValue(obj, number);
                return;
            }
            else if (field.FieldType == typeof(long))
            {
                var number = Convert.ToInt64(value);
                field.SetValue(obj, number);
                return;
            }
            else if (field.FieldType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(obj, number);
                return;
            }
            field.SetValue(obj, value);
        }
    }
    public static void SetStaticPublicField(string typeName, string name, object value)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = Type.GetType(typeName);
        FieldInfo field = GetFieldInfo(type, name, flags);
        if (field != null)
        {
            if (field.FieldType == typeof(int))
            {
                var number = Convert.ToInt32(value);
                field.SetValue(null, number);
                return;
            }
            else if (field.FieldType == typeof(float))
            {
                var number = Convert.ToSingle(value);
                field.SetValue(null, number);
                return;
            }
            else if (field.FieldType == typeof(long))
            {
                var number = Convert.ToInt64(value);
                field.SetValue(null, number);
                return;
            }
            else if (field.FieldType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(null, number);
                return;
            }
            field.SetValue(null, value);
        }
    }

    public static void SetPublicProperty(this object obj, string name, object value)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        Type type = obj.GetType();
        PropertyInfo field = GetPropertyInfo(type, name, flags);
        if (field != null)
        {
            if (field.PropertyType == typeof(int))
            {
                var number = Convert.ToInt32(value);
                field.SetValue(obj, number, null);
                return;
            }
            else if (field.PropertyType == typeof(float))
            {
                var number = Convert.ToSingle(value);
                field.SetValue(obj, number, null);
                return;
            }
            else if (field.PropertyType == typeof(long))
            {
                var number = Convert.ToInt64(value);
                field.SetValue(obj, number, null);
                return;
            }
            else if (field.PropertyType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(obj, number, null);
                return;
            }
            field.SetValue(obj, value, null);
        }
    }
    public static void SetStaticPublicProperty(string typeName, string name, object value)
    {
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
        Type type = Type.GetType(typeName);
        PropertyInfo field = GetPropertyInfo(type, name, flags);
        if (field != null)
        {
            if (field.PropertyType == typeof(int))
            {
                var number = Convert.ToInt32(value);
                field.SetValue(null, number, null);
                return;
            }
            else if (field.PropertyType == typeof(float))
            {
                var number = Convert.ToSingle(value);
                field.SetValue(null, number, null);
                return;
            }
            else if (field.PropertyType == typeof(long))
            {
                var number = Convert.ToInt64(value);
                field.SetValue(null, number, null);
                return;
            }
            else if (field.PropertyType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(null, number, null);
                return;
            }
            field.SetValue(null, value, null);
        }
    }
}

