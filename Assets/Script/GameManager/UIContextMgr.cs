using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;
using System;
using System.Reflection;

namespace GManager {
    ///界面控制类
    public class UIContextMgr : ManagerBase<UIContextMgr> {
        private Dictionary<EnumUILevel, UIContextStackClass> m_Dict_contextStackClass = new Dictionary<EnumUILevel, UIContextStackClass>();
        public UIContextMgr() {
            foreach(EnumUILevel level in Enum.GetValues(typeof(EnumUILevel))) {
                this.m_Dict_contextStackClass[level] = new UIContextStackClass(level);
            }
        }
        public void Push(UIContextBase nextContext) {
            this.m_Dict_contextStackClass[nextContext.uiType.Level].Push(nextContext);
          //  CleanLater(nextContext.uiType.Level);
        }
        public void Pop(UIContextBase context) {
            this.m_Dict_contextStackClass[context.uiType.Level].Pop();
        }
        public void CleanLater(EnumUILevel level) {
            if (level == EnumUILevel.Level_Top ) return;
            for (int i = level.GetHashCode() + 1; i <= EnumUILevel.Level_Top.GetHashCode(); i++) {
                this.m_Dict_contextStackClass[(EnumUILevel)EnumUILevel.Parse(typeof(EnumUILevel), i.ToString())].Clear();
            }
        }
        public UIContextBase PeekOrNull() {
            for (int i = EnumUILevel.Level_Top.GetHashCode(); i >= EnumUILevel.Level_One.GetHashCode(); i--){
                UIContextStackClass contextstackclass = this.m_Dict_contextStackClass[(EnumUILevel)EnumUILevel.Parse(typeof(EnumUILevel), i.ToString())];
                if (contextstackclass.PeekOrNull() != null) {
                    return contextstackclass.PeekOrNull();
                }
            }
            return null;
        }

        /// <summary>
        /// 界面添加子控制物
        /// </summary>
        /// <param name="parent"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public UIitemContextBase AddChild(UIContextBase parent, Transform parentObj, string path , string panelname , string panelcnt) {
            UIitemContextBase context = Util.InstantiationClass<UIitemContextBase>(panelcnt, new object[] { panelname, path, parent });
            GameObject go = UIMgr.getInstance.GetUIViewObj(context.uiType , parentObj);
            context.InitView(go);
            return context;
        }
    }

    /// <summary>
    /// 界面控制存储类
    /// </summary>
    class UIContextStackClass {
        public EnumUILevel level;
        private Stack<UIContextBase> m_contextStack = new Stack<UIContextBase>();
        public UIContextStackClass(EnumUILevel level) {
            this.level = level;
        }
        public void Push(UIContextBase nextContext) {
            if (this.m_contextStack.Count != 0) {
                UIContextBase cont = this.m_contextStack.Peek();
                cont.OnPause();
            }
            GameObject go = UIMgr.getInstance.GetUIViewObj(nextContext.uiType);
            nextContext.InitView(go);
            this.m_contextStack.Push(nextContext);
        }
        public void Pop() {
            if (this.m_contextStack.Count != 0) {
                UIContextBase cnt = this.m_contextStack.Peek();
                this.m_contextStack.Pop();
                cnt.OnDestroy();
            }
            if (this.m_contextStack.Count != 0) {
                UIContextBase cnt = this.m_contextStack.Peek();
                this.m_contextStack.Pop();
                cnt.OnResume();
            }
        }
        public void Clear() {
            if (this.m_contextStack.Count == 0) return;
            while (this.m_contextStack.Count != 0) {
                UIContextBase cnt = this.m_contextStack.Peek();
                if (cnt.IsinStack) {
                    this.m_contextStack.Pop();
                    cnt.OnDestroy();
                }
            }
        }
        public UIContextBase PeekOrNull() {
            if (this.m_contextStack.Count != 0) {
                return this.m_contextStack.Peek();
            }
            return null;
        }
    }
}