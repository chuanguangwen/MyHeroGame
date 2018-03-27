using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pitemview : UIViewBase {
    public Pitemview(UIContextBase cnt, GameObject go) : base(cnt,go){
        
    }

	public void SetShowNumber(string str){
		Text text = this.GetGetComponentOnobj<Text> ("Text");
		text.text = str;
	}

}
