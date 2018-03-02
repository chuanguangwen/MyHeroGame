using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;
using XLua;
using System.IO;
namespace GManager
{

    public class LuaMgr : SingLetonMono<LuaMgr>
    {
        public static LuaEnv luaenv = new LuaEnv();
        // Use this for initialization
        void Start()
        {
            //Debug.Log(Application.dataPath);
            //string url = GManager.AppConst.luaDataurl + "Boot";
            //Debug.Log(url);


            //string fileAddress = System.IO.Path.Combine(GManager.AppConst.luaDataurl, "Boot.lua");
            ////Debug.Log(fileAddress);
            ////StreamReader r = new StreamReader(fileAddress);
            ////LuaManager.luaenv.DoString( r.ReadToEnd());

            //int pos = url.IndexOf('/');
            //LuaMgr.luaenv.DoString(url.Substring(0, pos) , Path.GetFileName("Boot"));  最终没对
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}