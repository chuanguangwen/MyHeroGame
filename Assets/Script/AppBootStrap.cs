using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;

public class AppBootStrap : MonoBehaviour {

	void Awake(){
		GameObject.DontDestroyOnLoad (this);

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = AppConst.GameFrameRate;

	}
	// Use this for initialization
	void Start () {
		var manager = GameManager.getInstance;
	}

}
