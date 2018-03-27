using Code.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

	public override void OnDestroy() {
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
		GameObject go = Util.DepthFindChild(this.gameObject , name) as GameObject;
		return go;
    }

	protected T GetGetComponentOnobj<T>(string name) where T : Component{
		GameObject go = this.GetObjectForName (name);
		T com = go.GetComponent<T> ();
		if (com == null) {
			Log.Error (string.Format ("Object {0} is havent on {1] Component!!!", name, typeof(T)));
		}
		return com;
	}
}
