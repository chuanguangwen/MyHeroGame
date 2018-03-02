using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;
namespace Code.Core{
	public abstract class SingLetonMono<T> : MonoBehaviour where T : SingLetonMono<T>{
		private static volatile T instance;
		private static object syncRoot = new object ();
		public static T getInstance
		{
			get{
				if (instance == null) {
					lock (syncRoot) {
						if (instance == null) {
							T[] instances = (T[])GameObject.FindObjectsOfType (typeof(T));
							if (instances != null) {
								for (var i = 0; i < instances.Length; i++) {
									GameObject.Destroy (instances [i].gameObject);
								}
							}
							GameObject go = new GameObject ();
							go.name = typeof(T).Name;
							instance = go.AddComponent<T> ();
							GameObject.DontDestroyOnLoad (go);
						}
					}
				}

				return instance;
			}
		}

		public virtual void OnDestroy()
		{
			instance = null;
		}

	}
}
