using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;
using Code.Core;
public class AppBootStrap : MonoBehaviour {

    public static GameObject ManagerObjBase = null;
	
    void Awake(){
        ManagerObjBase = GameObject.Find(("GameManager"));
        if (ManagerObjBase == null){
            ManagerObjBase = new GameObject("GameManager");
        }
        GameObject.DontDestroyOnLoad(ManagerObjBase);
		GameObject.DontDestroyOnLoad (this);

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = AppConst.GameFrameRate;

        Controller.getinstance.InitGameWork();

	}
	// Use this for initialization
	void Start () {
        Controller.getinstance.StartGameWork();
	}
}
