using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelView : UIViewBase {
    private const string scrollPath = "UI/Button";
    public MainPanelView(UIContextBase cnt, GameObject go) :base(cnt , go) {

    }

    public override void OnEnter() {
        GameObject go = Util.DepthFindChild( this.gameObject , "Scrillview");
        List<int> data = new List<int>();
        for (int i = 0; i < 100; i ++){
            data.Add(i);
        }

		UITools.CreatScrollLoopControlser(this._context , go , data , 170 , 100 , 2 , "Pitemview" , "PItemcontrol",scrollPath);
    }
}
