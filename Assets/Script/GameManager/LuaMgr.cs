using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;
using Code.Core;
namespace GManager
{

    public class LuaMgr : ManagerBase<LuaMgr>{
        LuaEnv luaenv = null;
        // Use this for initialization
        void Awake()
        {
        }

        void Start()
        {
            luaenv = new LuaEnv();
        }
        void Update()
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
        }

        void OnDestroy()
        {
            luaenv.Dispose();
        }

        public void InitStart()
        {
            luaenv.DoString("require 'lua/main'");
        }


        public object[] DoFile(string filename)
        {
            return null;
        }

        // Update is called once per frame
        public object[] CallFunction(string funcName, params object[] args)
        {
            return null;
        }
        public void Close()
        {
        }
        /// <summary>
        /// 清理内存
        /// </summary>
        public void LuaGC() { }
    }
}