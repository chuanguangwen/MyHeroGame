using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;
namespace Code.Core
{
    public class Controller : IController{
        public static void StarUp(object[] args)
        {
            /// 初始化管理器
            GameMgr.Create();  //加载管理器
            ResourcesMgr.Create();  //加载管理器
            SoundMgr.Create();    //声音管理器
            SenceMgr.Create();   //场景管理器
            LuaMgr.Create();        //lua管理器
            UIMgr.Create();      //ui管理器
            TimerMgr.Create();      //时间管理器
        }

        private static IController instance;
        public static IController getinstance
        {
            get{
                if(instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }
        }

        private EventScope scope = null;


        public Controller(){
            scope = Eventer.Create();
        }
        public virtual void InitGameWork(){
            scope = Eventer.Create();
            this.scope.Listen(GameConst.START_UP , StarUp);
        }

        public virtual void StartGameWork(){
            Eventer.Fire(GameConst.START_UP);

        }

        public virtual void Destroy(){
            this.scope.Destroy();
            this.scope = null;
        }
    }
}