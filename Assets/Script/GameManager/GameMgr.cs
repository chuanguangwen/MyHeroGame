using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;

public delegate void UnParametricMethod();
public delegate void ParametricMethod(params Object[] args);

namespace GManager
{
    public class GameMgr : ManagerBase<GameMgr>{
        // Use this for initialization
        void Awake()
        {
            this.Init();
        }

        void Init()
        {

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Controller.getinstance.Destroy();
        }
    }
}
