using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core {
    public class EventX {

        protected EventScope scope = null;

        protected  EventX() {
            this.InitScope();
        }
        protected virtual void InitScope() {
            this.scope = Eventer.Create();
        }

        public EventScope CreateChild() {
            return this.scope.CreateChild();
        }
        public virtual void OnDestroy() {
            this.scope.Destroy();
            this.scope = null;
        }
    }
}