using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UITools  {
    public static void CreatScrollLoopControlser(UIContextBase parent , GameObject tager , IList datas , int maxnumber , int rownum , string itemview, string itemcontor , string Path ) {
        ScrollLoopController scrollloop = tager.GetComponent<ScrollLoopController>();
        scrollloop.parent = parent;
        scrollloop.cellviewlName = itemview;
        scrollloop.cellctrlName = itemcontor;
        scrollloop.path = Path;
        UnityEngine.Object[] objs = GManager.ResourcesMgr.getInstance.LoadUIPrefab(Path);
        if(objs == null || objs[0] == null) {
            return;
        }
        scrollloop.cellPrefab = (GameObject)objs[0];

        scrollloop.initWithData(datas);
    }
}
