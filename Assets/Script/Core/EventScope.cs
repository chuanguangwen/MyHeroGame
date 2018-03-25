using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core
{
    public class EventScope
    {
        public Dictionary<string, DelegateObjList> eventTable = new Dictionary<string, DelegateObjList>();
        private EventScope m_parent = null;
        protected List<EventScope> m_chidler = new List<EventScope>();

        public EventScope(EventScope _parent){
            this.m_parent = _parent;
        }
        /// <summary>
        /// 创建子事件对象
        /// </summary>
        /// <returns>The child.</returns>
        public EventScope CreateChild(){
            EventScope scope = new EventScope(this);
            this.m_chidler.Add(scope);
            return scope;
        }
        /// <summary>
        /// 清空父类事件监听
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="deleObject">Dele object.</param>
        public void RemoveParentEvents(string name , Delegate deleObject){
            if(this.m_parent == null){
                return;
            }

            DelegateObjList list;
            if(this.m_parent.eventTable.TryGetValue(name , out list)){
                list.Remove(deleObject);
            }

            this.m_parent.RemoveParentEvents(name , deleObject);
        }
        /// <summary>
        /// 清空事件
        /// </summary>
        public void ClearEvent(){
            foreach(var et in this.eventTable){
                foreach(var _delegat in et.Value.events){
                    RemoveParentEvents(et.Key , _delegat);
                }
            }
            this.eventTable.Clear();
        }

        public void Destroy(){
            this.ClearEvent();
            if(this.m_parent != null){
                this.m_parent.m_chidler.Remove(this);
            }
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <returns>The listen.</returns>
        /// <param name="name">Name.</param>
        /// <param name="deleObject">Dele object.</param>
        public void Listen(string name , CALLBACK handler){
            if(handler == null){
                return;
            }

            DelegateObjList delegateOBJ;
            if(!this.eventTable.TryGetValue(name , out delegateOBJ)){
                delegateOBJ = new DelegateObjList();
                this.eventTable[name] = delegateOBJ;
            }

            delegateOBJ.Add(handler);

            if(this.m_parent != null){
                this.m_parent.Listen(name , handler);
            }
        }
    }
}
