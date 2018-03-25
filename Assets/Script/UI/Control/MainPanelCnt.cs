using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelCnt : UIContextBase {
    private const string path = "UI/MaxinPanel";
    public MainPanelCnt() : base("MainPanelView" , path, EnumUILevel.Level_Main) {

    }
    private MainPanelView view {
        get {
            return (MainPanelView)this.viewBase;
        }
    }

    public override void OnEnter() {
    }
}
