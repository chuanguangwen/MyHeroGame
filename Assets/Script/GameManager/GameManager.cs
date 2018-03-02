using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;

public delegate void UnParametricMethod();
public delegate void ParametricMethod(params Object[] args);

namespace GManager
{   
	public class GameManager : SingLetonMono<GameManager> {

		// Use this for initialization
		void Awake () {
			this.Init ();
		}

		void Init(){
			/// 初始化管理器
            var res = ResourcesMgr.getInstance;  //加载管理器
            var sound = SoundMgr.getInstance;    //声音管理器
            var sencem = SenceMgr.getInstance;   //场景管理器
            var lua = LuaMgr.getInstance;        //lua管理器
            var ui = UIManager.getInstance;      //ui管理器
            var time = TimeMgr.getInstance;      //时间管理器

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.targetFrameRate = AppConst.GameFrameRate;
        }
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
