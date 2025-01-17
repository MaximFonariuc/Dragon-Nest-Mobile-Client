using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Assets.SDK
{
	public static class Singleton<T> where T : class
	{
		internal static volatile T _instance;

		private static object _lock = new object();

		public static T GetInstance()
		{
			if (_instance == null)
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						Type type = typeof(T);
						ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
						_instance = ctor.Invoke(new object[0]) as T;
						if (_instance == null)
						{
							throw new Exception("Singleton<T> : T must have a none public ctor.");
						}
					}
				}
			}
			return _instance;
		}
	}
}
