using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UITools  {
	public static ScrollLoopController CreatScrollLoopControlser(UIContextBase parent , GameObject tager , IList datas , float width_distance , float height_distance , int rownum , string itemview, string itemcontor , string Path , float width_offset = 0.0f , float height_offset = 0.0f ) {
        ScrollLoopController scrollloop = tager.GetComponent<ScrollLoopController>();
        scrollloop.parent = parent;
        scrollloop.cellviewname = itemview;
        scrollloop.cellctrlName = itemcontor;
        scrollloop.path = Path;
		scrollloop.CellSize = new Vector2(width_distance , height_distance);
		scrollloop.NumberOcColumns = rownum;
		if(width_offset != 0f || height_offset != 0f ){
			scrollloop.CellOffset = new Vector2 (width_offset , height_offset);
		}
        UnityEngine.Object[] objs = GManager.ResourcesMgr.getInstance.LoadUIPrefab(Path);
        if(objs == null || objs[0] == null) {
			return null;
        }
        scrollloop.cellPrefab = (GameObject)objs[0];

        scrollloop.InitWithData(datas);
    	
		return scrollloop;
	}
}
