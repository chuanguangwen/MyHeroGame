using System;
using System.Collections.Generic;
using UnityEngine;
namespace Code.Core
{
    public delegate void CALLBACK(object[] args);
    public class DelegateObjList
    {
        public class DyamicDelegate{
            public Delegate callback;
            public bool append;
        }

        public List<Delegate> events = new List<Delegate>(); //事件列表
        public List<DyamicDelegate> delayProcesList = null;//事件缓存
        public bool accessEvent = false;   //  访问事件

        private void AddDynamicDelegate(Delegate dele , bool append){
            if(this.delayProcesList == null){
                this.delayProcesList = new List<DyamicDelegate>();
            }
            DyamicDelegate dyamicdelegat = new DyamicDelegate();
            dyamicdelegat.callback = dele;
            dyamicdelegat.append = append;
            this.delayProcesList.Add(dyamicdelegat);
        }

        public void Add(Delegate f){
            if(this.accessEvent){
                this.AddDynamicDelegate(f , true);
            }else{
                this.events.Add(f);
            }
        }

        public void Remove(Delegate f){
            if(this.accessEvent){
                this.AddDynamicDelegate(f,false);
            }else{
                this.events.Remove(f);
            }
        }

        public void Enter(){
            this.accessEvent = true;
        }

        public void Leave(){
            this.accessEvent = false;

            if(this.delayProcesList == null){
                return;
            }
            for (int i = 0; i < this.delayProcesList.Count; i ++){
                var cb = this.delayProcesList[i];
                if(cb.append){
                    events.Add(cb.callback);
                }else{
                    events.Remove((cb.callback));
                }
            }

            this.delayProcesList.Clear();
        }
    }
}
