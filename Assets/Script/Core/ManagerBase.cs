using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;

namespace Code.Core
{
    public class ManagerBase<T> : SingLetonMono<T> where T : SingLetonMono<T>
    {
        private LuaMgr m_LuaMgr;
        private ResourcesMgr m_ResMgr;
        //private NetworkManager m_NetMgr;
        private SoundMgr m_SoundMgr;
        private TimerMgr m_TimerMgr;
        //private ThreadManager m_ThreadMgr;
        private GameMgr m_GameManager;
        private UIMgr m_UIManager;
        private SenceMgr m_SenceManager;

        private LuaMgr LuaManager
        {
            get
            {
                if (m_LuaMgr == null)
                {
                    m_LuaMgr = LuaMgr.getInstance;
                }
                return m_LuaMgr;
            }
        }

        private ResourcesMgr ResManager
        {
            get
            {
                if (m_ResMgr == null)
                {
                    m_ResMgr = ResourcesMgr.getInstance;
                }
                return m_ResMgr;
            }
        }

        private SoundMgr SoundManager
        {
            get
            {
                if (m_SoundMgr == null)
                {
                    m_SoundMgr = SoundMgr.getInstance;
                }
                return m_SoundMgr;
            }
        }
        private TimerMgr TimerManager
        {
            get
            {
                if (m_TimerMgr == null)
                {
                    m_TimerMgr = TimerMgr.getInstance;
                }
                return m_TimerMgr;
            }
        }

        private GameMgr GameManager
        {
            get
            {
                if (m_GameManager == null)
                {
                    m_GameManager = GameMgr.getInstance;
                }
                return m_GameManager;
            }
        }
        private UIMgr UIManager
        {
            get
            {
                if (m_UIManager == null)
                {
                    m_UIManager = UIMgr.getInstance;
                }
                return m_UIManager;
            }
        }
        private SenceMgr SenceManager
        {
            get
            {
                if (m_SenceManager == null)
                {
                    m_SenceManager = SenceMgr.getInstance;
                }
                return m_SenceManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
