using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;
namespace Code.Core{
    public abstract class SingLetonMono<T> : MonoBehaviourX where T : SingLetonMono<T>{
		private static volatile T instance;
        private static object syncRoot = new object();

        public static void Create(){
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = AppBootStrap.ManagerObjBase.GetComponent<T>();
                        if (instance == null)
                        {
                            instance = AppBootStrap.ManagerObjBase.AddComponent<T>();
                        }
                    }
                }
            }
        }
		public static T getInstance
		{
			get{
				if (instance == null) {
					lock (syncRoot) {
                        if (instance == null) {
                            instance = AppBootStrap.ManagerObjBase.GetComponent<T>();
                            if (instance == null)
                            {
                                instance = AppBootStrap.ManagerObjBase.AddComponent<T>();
                            }
						}
					}
				}

				return instance;
			}
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }
	}
}
