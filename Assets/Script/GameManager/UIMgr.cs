using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;
public class UIType {
    public string Path { get; private set; }
    public string Name { get; private set; }
    public EnumUILevel Level { get; private set; }
    public UIType(string path , EnumUILevel level) {
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
        Level = level;
    }
    public override string ToString() {
        return string.Format("path : {0} name : {1}", Path, Name);
    }
}
namespace GManager
{
    public class UIMgr : ManagerBase<UIMgr>
    {
        private Dictionary<string, UIType> m_Dict_UIType = new Dictionary<string, UIType>();
        private Dictionary<UIType, GameObject> m_Dict_UIObj = new Dictionary<UIType, GameObject>();

        private Transform m_canvas;

        private List<Transform> m_List_UILevel = new List<Transform>();

        void Start() {
            if (this.m_canvas == null) {
                GameObject go = GameObject.Find("Canvas");
                if (go == null) {
                    go = new GameObject("Canvas");
                    Util.Add<Canvas>(go);
                }
                GameObject.DontDestroyOnLoad(go);
                this.m_canvas = go.transform;
                for (int i = 0 ; i <= 5; i ++) {
                    go = Util.AddNoneChild(this.m_canvas , "level_"+i);
                    this.m_List_UILevel.Add(go.transform);
                }
                foreach (Transform item in this.m_canvas) {
                    GameObject.Destroy(item.gameObject);
                }
            }

            UIContextMgr.getInstance.Push(new MainPanelCnt());
        }

        public UIType GetUIType (string path ,  EnumUILevel level ) {
            if (this.m_Dict_UIType.ContainsKey(path)) {
                return this.m_Dict_UIType[path];
            }
            UIType uitype = new UIType(path, level);
            this.m_Dict_UIType.Add(path, uitype);
            return uitype;
        }
        public GameObject GetUIViewObj(UIType uitype , Transform parentform = null) {
            if (this.m_Dict_UIObj.ContainsKey(uitype) == false || this.m_Dict_UIObj[uitype] == null) {
                UnityEngine.Object[] objs = ResourcesMgr.getInstance.LoadUIPrefab(uitype.Path);
                this.m_Dict_UIObj.Add(uitype, (GameObject)objs[0]);
            }
            GameObject go = Util.AddChild((parentform!=null)?parentform:this.m_canvas, this.m_Dict_UIObj[uitype]) as GameObject;
            return go;
        }

    }
}