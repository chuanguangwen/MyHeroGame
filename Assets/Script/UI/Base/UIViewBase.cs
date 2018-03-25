using Code.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Code.Core;
public class UIViewBase : EventX{
    protected GameObject gameObject;
    protected Transform transform; 

    protected UIContextBase _context = null;
    public UIType _uiType{ get; private set;}
    public UIViewBase(UIContextBase cnt, GameObject go) {
        this.gameObject = go.gameObject;
        this.transform = go.transform;

        this._context = cnt;
        this._uiType = cnt.uiType;
    }

    public void OnDestroy() {
        base.OnDestroy();
        this.OnExit();
    }
    public virtual void OnEnter() {
    }

    public virtual void OnExit() {
        GameObject.Destroy(gameObject);
    }

    public virtual void OnPause() {
    }

    public virtual void OnResume() {
    }


    public void SetPosition(Vector3 pos) {
        this.transform.localPosition = pos;
    }

    public void SetRotation(Quaternion qua) {
        this.transform.localRotation = qua;
    }

    public void SetScale(Vector3 scale) {
        this.transform.localScale = scale;
    }

    protected GameObject GetObjectForName(string name) {
        GameObject go = null;
        
        return go;
    }
}
