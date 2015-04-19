using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Common
{
	/// <summary>
	/// Singleton Class.
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				T instance = FindObjectOfType<T>();
				if (instance == null)
				{
					instance = CreateInstance();
				}
				return instance;
			}
		}
			
		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		protected static T CreateInstance()
		{
			GameObject singleton = new GameObject(typeof(T).Name);
			instance = singleton.AddComponent<T>();

			DontDestroyOnLoad(singleton);

			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.LogWarning("Created singleton : " + singleton);
			#endif

			return instance;
		}
	}

}