using System;
using System.Collections.Generic;
using UnityEngine;
namespace Code.Core
{
    public class Eventer
    {
        //事件静态全局根
        public static EventScope globe = new EventScope(null); 

        public static EventScope Create(){
            return globe.CreateChild();
        }

        
        public static void Fire(string name , object[] args){
            DelegateObjList dol;
            if(globe.eventTable.TryGetValue(name , out dol)){
                dol.Enter();
                int count = dol.events.Count;
                for (int i = 0; i < count; i ++){
                    CALLBACK callBack = dol.events[i] as CALLBACK;
                    callBack(args);
                }
                dol.Leave();
            }
        }

        public static void Fire(string name)
        {
            Fire(name, new object[0] {});
        }

        public static void Fire(string name, object[] args1, object[] args2)
        {
            Fire(name, new object[] { args1, args2 });
        }

        public static void Fire(string name, object[] args1, object[] args2, object[] args3)
        {
            Fire(name, new object[] { args1, args2, args3 });
        }

        public static void Fire(string name, object[] args1, object[] args2, object[] args3, object[] args4)
        {
            Fire(name, new object[] { args1, args2, args3 ,args4 });
        }
    }
}