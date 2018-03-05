using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Code.Core
{
    public class MonoBehaviourX : MonoBehaviour
    {
        protected EventScope scope = null;

        protected MonoBehaviourX(){
            this.scope = Eventer.Create();
        }
        protected virtual void OnDestroy(){
            this.scope.Destroy();
            this.scope = null;
        }
    }
}
