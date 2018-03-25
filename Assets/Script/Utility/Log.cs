
using System;
using UnityEngine;

public class Log
{
    public const int DEBUG = 1;
    public const int INFO = 2;
    public const int WARN = 3;
    public const int ERROR = 4;
    public const int NONE = 5;

    private static string getmsg(object[] args) {
        string str = "[ ";
        for (int i = 0; i < args.Length; i++) {
            if (i > 0) str += " , ";
            str += args[i];
        }
        str += " ]";
       return str;
    }
    public static void Debug(params object[] args)
    {
        if (AppConst.LogLevel <= Log.DEBUG)
        {
            UnityEngine.Debug.Log(getmsg(args));
        }
    }

    public static void Info(params object[] args)
    {
        if (AppConst.LogLevel <= Log.INFO)
        {
            UnityEngine.Debug.Log(getmsg(args));
        }
    }

    public static void Warn(params object[] args)
    {
        if (AppConst.LogLevel <= Log.WARN)
        {
            UnityEngine.Debug.LogWarning(getmsg(args));
        }
    }

    public static void Error(params object[] args)
    {
        if (AppConst.LogLevel <= Log.ERROR)
        {
            UnityEngine.Debug.LogError(getmsg(args));
        }
    }

    public static void Error(Exception e)
    {
        if (AppConst.LogLevel <= Log.ERROR)
        {
            UnityEngine.Debug.LogError(e);
        }
    }
}