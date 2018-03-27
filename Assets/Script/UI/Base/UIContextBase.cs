using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GManager;
using System;
using System.Reflection;
using Code.Core;
public class UIContextBase : EventX {
    /// <summary>
    ///  是否加入UI堆中
    /// </summary>
    protected bool _inStackClose = true;
    public bool IsinStack {
        get { return this._inStackClose; }
    }
    public UIViewBase viewBase { get; private set; }
    public UIType uiType = null;

    private string m_panelname;
    private List<UIitemContextBase> m_List_ItemCont = new List<UIitemContextBase>();

    public UIContextBase(string panelname, string path, EnumUILevel level = EnumUILevel.Level_One){
        this.m_panelname = panelname;
        this.uiType = UIMgr.getInstance.GetUIType(path, level);
    }
    /// <summary>
    /// 界面预制加载完成后初始化界面
    /// </summary>
    /// <param name="go"></param>
    public virtual void InitView(GameObject go) {
        this.gameObject = go;
        this.transform = go.transform;
        this.viewBase = Util.InstantiationClass<UIViewBase>(this.m_panelname, new object[] { this, go });

        this.viewBase.OnEnter();
        this.OnEnter();
    }

    public override void OnDestroy() {
        base.OnDestroy();
        //先把ITEM删除
        this.FireDestroyForItem();
        this.viewBase.OnDestroy();
        this.OnExit();
    }

    public virtual void OnEnter() {
    }
    public virtual void OnExit() {
    }
    public virtual void OnPause() {
        this.viewBase.OnPause();
    }
    public virtual void OnResume() {
        this.viewBase.OnResume();
    }

    private void FireDestroyForItem() {
        for(int i = 0; i < this.m_List_ItemCont.Count; i++) {
            this.m_List_ItemCont[i].OnDestroy();
        }
    }
    public void InNewItemCrete( UIitemContextBase item) {
        if (!this.m_List_ItemCont.Contains(item)) {
            this.m_List_ItemCont.Add(item);
        }
    }
    public void OutItemDestroy(UIitemContextBase item) {
        if (this.m_List_ItemCont.Contains(item)) {
            this.m_List_ItemCont.Remove(item);
        }
    }

    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

    public void SetPosition(Vector3 pos) {
        this.viewBase.SetPosition(pos);
    }

    public void SetRotation(Quaternion qua) {
        this.viewBase.SetRotation(qua);
    }

    public void SetScale(Vector3 scale) {
        this.viewBase.SetScale(scale);
    }
}
