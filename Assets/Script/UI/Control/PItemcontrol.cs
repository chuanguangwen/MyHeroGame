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

	public override void ConfigureCellData ()
	{
		this.view.SetShowNumber (this.DataObject.ToString());
	}
}
