//
// PrivateExtensions.cs
// Created by huailiang.peng on 2016/04/15 11:39:07
//
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;

public static class PrivateExtensions 
{

	// Invoke method
	public static T CallPrivateMethodGeneric<T>(this object obj, string name, params object[] param)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		Type[] argTypes = new Type[param.Length];
		for (int i=0; i<argTypes.Length; i++)
			argTypes [i] = param [i].GetType ();
		List<Type[]> argTypeList = PublicExtensions.CastNumberParameters (param, argTypes);
		MethodInfo method = null;
		try{
			method = type.GetMethod (name, flags);
		} catch {
			for (int i=0; i<argTypeList.Count; i++) {
				method = type.GetMethod (name, argTypeList [i]);
				if (method != null)
					break;
			}
		}
		if (method == null)
			return default (T);

		ParameterInfo[] pars = method.GetParameters();
		object[] convertedParameters = new object[pars.Length];
		for (int i=0; i<pars.Length; i++) 
		{
			if (pars[i].ParameterType != typeof(object))
				convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
			else
				convertedParameters[i] = param[i];
		}
		return (T)method.Invoke(obj, convertedParameters);
	}

	public static object CallPrivateMethod(this object obj, string name, params object[] param)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		Type[] argTypes = new Type[param.Length];
		for (int i=0; i<argTypes.Length; i++)
			argTypes [i] = param [i].GetType ();
		List<Type[]> argTypeList = PublicExtensions.CastNumberParameters (param, argTypes);
		MethodInfo method = null;
		try{
			method = type.GetMethod (name, flags);
		} catch {
			for (int i=0; i<argTypeList.Count; i++) {
				method = type.GetMethod (name, argTypeList [i]);
				if (method != null)
					break;
			}
		}
		if (method == null)
			return null;

		ParameterInfo[] pars = method.GetParameters();
		object[] convertedParameters = new object[pars.Length];
		for (int i=0; i<pars.Length; i++) 
		{
			if (pars[i].ParameterType != typeof(object))
				convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
			else
				convertedParameters[i] = param[i];
		}
		return method.Invoke(obj, convertedParameters);
	}
	
	public static object CallStaticPrivateMethod(string typeName, string name, params object[] param)
	{
		BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		Type type = Type.GetType (typeName);
		Type[] argTypes = new Type[param.Length];
		for (int i=0; i<argTypes.Length; i++)
			argTypes [i] = param [i].GetType ();
		List<Type[]> argTypeList = PublicExtensions.CastNumberParameters (param, argTypes);
		MethodInfo method = null;
		try{
			method = type.GetMethod (name, flags);
		} catch {
			for (int i=0; i<argTypeList.Count; i++) {
				method = type.GetMethod (name, argTypeList [i]);
				if (method != null)
					break;
			}
		}
		if (method == null)
			return null;
		
		ParameterInfo[] pars = method.GetParameters();
		object[] convertedParameters = new object[pars.Length];
		for (int i=0; i<pars.Length; i++) 
		{
			if (pars[i].ParameterType != typeof(object))
				convertedParameters[i] = Convert.ChangeType(param[i], pars[i].ParameterType);
			else
				convertedParameters[i] = param[i];
		}
		return method.Invoke(null, convertedParameters);
	}

	// Get feild, property
	public static T GetPrivateFieldGeneric<T>(this object obj, string name)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		FieldInfo field = PublicExtensions.GetFieldInfo (type, name, flags);
		if (field != null)
			return (T)field.GetValue(obj);
		else
			return (T)default(T);
	}
	public static object GetPrivateField(this object obj, string name)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		FieldInfo field = PublicExtensions.GetFieldInfo (type, name, flags);
		if (field != null)
			return field.GetValue(obj);
		else
			return null;
	}
	public static object GetStaticPrivateField(string typeName, string name)
	{
		BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
		Type type = Type.GetType (typeName);
		FieldInfo field = PublicExtensions.GetFieldInfo (type, name, flags);
		if (field != null)
			return field.GetValue(null);
		else
			return null;
	}

	public static T GetPrivatePropertyGeneric<T>(this object obj, string name)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		PropertyInfo field = PublicExtensions.GetPropertyInfo(type, name, flags);
		if (field != null)
			return (T)field.GetGetMethod(true).Invoke (obj, null);
		else
			return default(T);
	}
	public static object GetPrivateProperty(this object obj, string name)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		PropertyInfo field = PublicExtensions.GetPropertyInfo(type, name, flags);
		if (field != null)
			return field.GetGetMethod(true).Invoke (obj, null);
		else
			return null;
	}
	public static object GetStaticPrivateProperty(string typeName, string name)
	{
		BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = Type.GetType (typeName);
		PropertyInfo field = PublicExtensions.GetPropertyInfo(type, name, flags);
		if (field != null)
			return field.GetValue(null, null);
		else
			return null;
	}

	// Set field, propertry
	public static void SetPrivateField(this object obj, string name, object value)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		FieldInfo field = PublicExtensions.GetFieldInfo(type, name, flags);
		if (field != null) {
			if (field.FieldType == typeof(int)){
				var number = Convert.ToInt32 (value);
				field.SetValue(obj, number);
				return;
			} else if (field.FieldType == typeof(float)) {
				var number = Convert.ToSingle (value);
				field.SetValue(obj, number);
				return;
			} else if (field.FieldType == typeof(long)) {
				var number = Convert.ToInt64 (value);
				field.SetValue(obj, number);
				return;
            }
            else if (field.FieldType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(obj, number);
                return;
            }
            field.SetValue (obj, value);
		}
	}
	public static void SetStaticPrivateField(string typeName, string name, object value)
	{
		BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = Type.GetType(typeName);
		FieldInfo field = PublicExtensions.GetFieldInfo(type, name, flags);
		if (field != null) {
			if (field.FieldType == typeof(int)){
				var number = Convert.ToInt32 (value);
				field.SetValue(null, number);
				return;
			} else if (field.FieldType == typeof(float)) {
				var number = Convert.ToSingle (value);
				field.SetValue(null, number);
				return;
			} else if (field.FieldType == typeof(long)) {
				var number = Convert.ToInt64 (value);
				field.SetValue(null, number);
				return;
			}
            else if (field.FieldType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(null, number);
                return;
            }
            field.SetValue (null, value);
		}
	}
	
	public static void SetPrivateProperty(this object obj, string name, object value)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
		Type type = obj.GetType();
		PropertyInfo field = PublicExtensions.GetPropertyInfo (type, name, flags);
		if (field != null) {
			if (field.PropertyType == typeof(int)){
				var number = Convert.ToInt32 (value);
				field.SetValue(obj, number, null);
				return;
			} else if (field.PropertyType == typeof(float)) {
				var number = Convert.ToSingle (value);
				field.SetValue(obj, number, null);
				return;
			} else if (field.PropertyType == typeof(long)) {
				var number = Convert.ToInt64 (value);
				field.SetValue(obj, number, null);
				return;
			}
            else if (field.PropertyType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(obj, number, null);
                return;
            }
            field.SetValue (obj, value, null);
		}
	}
	public static void SetStaticPrivateProperty(string typeName, string name, object value)
	{
		BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
		Type type = Type.GetType (typeName);
		PropertyInfo field = PublicExtensions.GetPropertyInfo (type, name, flags);
		if (field != null) {
			if (field.PropertyType == typeof(int)){
				var number = Convert.ToInt32 (value);
				field.SetValue(null, number, null);
				return;
			} else if (field.PropertyType == typeof(float)) {
				var number = Convert.ToSingle (value);
				field.SetValue(null, number, null);
				return;
			} else if (field.PropertyType == typeof(long)) {
				var number = Convert.ToInt64 (value);
				field.SetValue(null, number, null);
				return;
			}
            else if (field.PropertyType == typeof(uint))
            {
                var number = Convert.ToUInt32(value);
                field.SetValue(null, number, null);
                return;
            }
            field.SetValue (null, value, null);
		}
	}
}

