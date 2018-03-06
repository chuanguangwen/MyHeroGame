using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AppConst
{
    /// <summary>
    /// 当前网络环境状态
    /// </summary>
    public enum NetworkEnum{
        NotReachable = 1,
        AreaNetwork  = 2,
        DataNetwork  = 3
    }

    public const bool DebugMode = false;                       //调试模式-用于内部测试

    public static string AppName = "";                          //应用程序名称
    public const string AssetDir = "Assets";           //素材目录 
    public const string ExtName = ".unity3d";                   //素材扩展名

    public static int GameFrameRate = 60 ; //游戏帧率
	public static int TimeInterval = 1;

    public static int LogLevel = Log.NONE; //输出层级
    public static string luaDataurl = Application.dataPath + "/../Data/LuaScript/";



    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath + "/" + AppName;
        }
    }
}

