using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PItemcontrol : UIitemContextBase {
    public PItemcontrol(string panelname, string path, UIContextBase parent) : base(panelname,path,parent){
        
    }
    private Pitemview view
    {
        get
        {
            return (Pitemview)this.viewBase;
        }
    }

    public override void Init(ScrollLoopController controller, System.Object data, int index)
    {
        base.Init(controller , data , index);
        Log.Debug(data , data.GetType());
    }
}
