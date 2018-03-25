using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIitemContextBase : UIContextBase {
    protected UIContextBase parent;
    private System.Object dataObject;

    public UIitemContextBase(string panelname, string path , UIContextBase parent ) : base( panelname , path ) {
        this.parent = parent;
        this.parent.InNewItemCrete(this);
        this.scope = this.parent.CreateChild();
    }

    protected override void InitScope() {}

    public override void OnDestroy() {
        this.parent.OutItemDestroy(this);
        base.OnDestroy();
    }

    #region scroll view item
    private int dataIndex;
    public int DataIndex {
        get { return dataIndex; }
    }

    public System.Object DataObject {
        get { return dataObject; }
        set {
            dataObject = value;
            configureCellData();
        }
    }
    private ScrollLoopController controller;
    public ScrollLoopController Controller {
        get { return controller; }
    }

    public virtual void Init(ScrollLoopController controller, System.Object data, int index) {
        this.controller = controller;
        dataObject = data;
        dataIndex = index;
    }
    #endregion
    public virtual void Init(System.Object datas) {
        this.DataObject = datas;
    }

    public virtual void configureCellData() { }
}
