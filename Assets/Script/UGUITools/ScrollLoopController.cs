using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public enum AdapationType {
    ModifyColumns,
    ModifyColumnsCenter,
    FixedColumns,
    FixedColumnsCenter
}
public class ScrollLoopController : UIBehaviour {
    public Vector2 CellSize = new Vector2(50, 50);
    public Vector2 cellOffset;

    public UIContextBase parent;
    public GameObject cellPrefab;
    public string cellviewlName;
    public string cellctrlName;
    public string path;

    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private int numberOfColumns = 1;  //表示并排显示几个，比如是上下滑动，当此处为2时表示一排有两个cell -1表示系统自动判断个数
    [SerializeField]
    private AdapationType adapationType = AdapationType.ModifyColumns;

    private int visibleCellsRowCount;
    private int visibleCellsTotalCount;
    private int preFirstVisibleIndex ;
    private int firstVisibleIndex;
    private IList allData;
    private bool initFinish;
    private int preNumberOfColumns;
    private Vector2 contentPos;
    private Vector2 initCellSize ;
    private Vector2 initCellOffset;

    private LinkedList<UIitemContextBase> localCellsPool = new LinkedList<UIitemContextBase>();
    private LinkedList<UIitemContextBase> cellsInUse = new LinkedList<UIitemContextBase>();

    private bool horizontal {
        get {return scrollRect.horizontal; }
    }

    public void initWithData(IList cellDataList) {
        if(!initFinish) {
            initCellSize = CellSize;
            initCellOffset = cellOffset;
            initData();
            SetCellsPool();
            initFinish = true;
        }
        contentPos = scrollRect.content.anchoredPosition;
        preNumberOfColumns = numberOfColumns;
        allData = cellDataList;
        SetInUseCells(0);
        setContentSize();
        int showCount = visibleCellsTotalCount > cellDataList.Count ? cellDataList.Count : visibleCellsTotalCount;
        for(int i = 0; i < showCount; i++) {
            ShowCell(i , true);
        }
    }
    void initData() {
        CellSize = initCellSize;
        cellOffset = initCellOffset;
        if(horizontal) {
            visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.width / CellSize.x);
            if(adapationType == AdapationType.ModifyColumns) {
                numberOfColumns = (int)(scrollRect.viewport.rect.height / CellSize.y);
            } else if(adapationType == AdapationType.ModifyColumnsCenter) {
                numberOfColumns = (int)(scrollRect.viewport.rect.height / CellSize.y);
                float cellHeight = CellSize.y;
                CellSize.y = scrollRect.viewport.rect.height / numberOfColumns;
                cellOffset.y = (CellSize.y - (cellHeight - cellOffset.y * 2)) / 2;
            } else if(adapationType == AdapationType.FixedColumnsCenter) {
                float cellHeight = CellSize.y;
                CellSize.y = scrollRect.viewport.rect.height / numberOfColumns;
                cellOffset.y = (CellSize.y - (cellHeight - cellOffset.y * 2)) / 2;
            }
            
        } else { 
            visibleCellsRowCount = Mathf.CeilToInt(scrollRect.viewport.rect.height / CellSize.y);
            if(adapationType == AdapationType.ModifyColumns) {
                numberOfColumns = (int)(scrollRect.viewport.rect.width / CellSize.x);
            } else if(adapationType == AdapationType.ModifyColumnsCenter) {
                numberOfColumns = (int)(scrollRect.viewport.rect.width / CellSize.x);
                float cellWidth = CellSize.x;
                CellSize.x = scrollRect.viewport.rect.width / numberOfColumns;
                cellOffset.x = (CellSize.x - (cellWidth - cellOffset.x * 2)) / 2;
            } else if(adapationType == AdapationType.FixedColumnsCenter) {
                float cellWidth = CellSize.x;
                CellSize.x = scrollRect.viewport.rect.width / numberOfColumns;
                cellOffset.x = (CellSize.x - (cellWidth - cellOffset.x * 2)) / 2;
            }
        }
        visibleCellsTotalCount = (visibleCellsRowCount + 1) * numberOfColumns;
    }

    void setContentSize() {
        int cellOneWayCount = (int)Math.Ceiling((float)allData.Count / numberOfColumns);
        if(horizontal) {
            scrollRect.content.sizeDelta = new Vector2(cellOneWayCount * CellSize.x, scrollRect.content.sizeDelta.y);
        } else {
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, cellOneWayCount * CellSize.y);
        }
    }
    
    protected override void OnRectTransformDimensionsChange() {
        if(initFinish) {
            initData();
            SetInUseCells(visibleCellsTotalCount);
            SetCellsPool();
            if(preNumberOfColumns == numberOfColumns) {
                refresh();
            } else {
                scrollRect.content.anchoredPosition = contentPos;
                initWithData(allData);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        CalculateCurrentIndex();
        InternalCellsUpdate();
    }

    void ShowCell(int cellIndex , bool scrollingPositive) {
        UIitemContextBase tempCell = GetCellFromPool(scrollingPositive);
        Log.Debug(tempCell , tempCell.gameObject);
        PositionCell(tempCell.gameObject, cellIndex);
        if(cellIndex < allData.Count) {
            tempCell.gameObject.SetActive(true);
            tempCell.Init(null, allData[cellIndex], cellIndex);
        } else {
            tempCell.gameObject.SetActive(false);
        }
    }

    void CalculateCurrentIndex() {
        if(horizontal)
            firstVisibleIndex = (int)(-scrollRect.content.anchoredPosition.x / CellSize.x);
        else
            firstVisibleIndex = (int)(scrollRect.content.anchoredPosition.y / CellSize.y);
        int limit = Mathf.CeilToInt(allData.Count / (float)numberOfColumns) - visibleCellsRowCount;
        Log.Debug(firstVisibleIndex , limit);
        if(firstVisibleIndex < 0 || limit <= 0)
            firstVisibleIndex = 0;
        else if(firstVisibleIndex >= limit) {
            firstVisibleIndex = limit - 1;
        }
    }

    void InternalCellsUpdate() {
        if(preFirstVisibleIndex != firstVisibleIndex) {
            bool scrollingPositive = preFirstVisibleIndex < firstVisibleIndex;
            int indexDelta = Mathf.Abs(preFirstVisibleIndex - firstVisibleIndex);

            int deltaSign = scrollingPositive ? +1 : -1;

            for(int i = 1; i <= indexDelta; i++)
                UpdateContent(preFirstVisibleIndex + i * deltaSign, scrollingPositive);

            preFirstVisibleIndex = firstVisibleIndex;
        }
    }

    void UpdateContent(int cellIndex, bool scrollingPositive) {
        int index = scrollingPositive ? ((cellIndex - 1) * numberOfColumns) + (visibleCellsTotalCount) : (cellIndex * numberOfColumns);
        for(int i = 0; i < numberOfColumns; i++) {
            FreeCell(scrollingPositive);
            ShowCell(index + i, scrollingPositive);
        }
    }

    void FreeCell(bool scrollingPositive) {
        LinkedListNode<UIitemContextBase> cell = null;
        if(scrollingPositive) {
            cell = cellsInUse.First;
            cellsInUse.RemoveFirst();
            localCellsPool.AddLast(cell);
        } else {
            cell = cellsInUse.Last;
            cellsInUse.RemoveLast();
            localCellsPool.AddFirst(cell);
        }
     //   cell.Value.gameObject.SetActive(false);
    }

    UIitemContextBase GetCellFromPool(bool scrollingPositive) {
        Log.Debug("GetCellFromPool = " , localCellsPool);
        if(localCellsPool.Count == 0)
            return null;
        LinkedListNode<UIitemContextBase> cell = localCellsPool.First;
        localCellsPool.RemoveFirst();

        Log.Debug("GetCellFromPool = ",cellsInUse  , cell.Value);
        if(scrollingPositive)
            cellsInUse.AddLast(cell);
        else
            cellsInUse.AddFirst(cell);
        return cell.Value;
    }

    void PositionCell(GameObject go, int index) {
        int rowMod = index % numberOfColumns;
        if (horizontal)
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * (index / numberOfColumns) + cellOffset.x, -rowMod * CellSize.y + cellOffset.y);
        else
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * rowMod + cellOffset.x, -(index / numberOfColumns) * CellSize.y + cellOffset.y);
    }

    void SetInUseCells(int visibleCellCount) {
        int outSideCount = cellsInUse.Count - visibleCellCount;
        while(outSideCount > 0) {
            outSideCount--;
            cellsInUse.Last.Value.gameObject.SetActive(false);
            localCellsPool.AddLast(cellsInUse.Last.Value);
            cellsInUse.RemoveLast();
        }
    }

    void SetCellsPool() {
        if (cellPrefab == null) return;
        int outSideCount = localCellsPool.Count + cellsInUse.Count - visibleCellsTotalCount;
        if(outSideCount > 0) {
            while(outSideCount > 0) {
                outSideCount--;
                LinkedListNode<UIitemContextBase> cell = localCellsPool.Last;
                localCellsPool.RemoveLast();
                Destroy(cell.Value.gameObject);
            }
        } else if(outSideCount < 0) {
            for(int i=0; i< -outSideCount; i++ ) {
                GameObject go = Util.AddChild(scrollRect.content.transform, cellPrefab.gameObject) as GameObject;
                object[] objs = new object[] { this.cellviewlName , this.path , this.parent };
                Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                UIitemContextBase context = (UIitemContextBase)assembly.CreateInstance(this.cellctrlName, true, BindingFlags.CreateInstance, null, objs , null, null); //通过制定类完全限定名，动态获取对象实例
                localCellsPool.AddLast(context);
            }
        }
    }

    public void updateCellData(System.Object data, int index = -1) {
        if(index == -1) {
            index = allData.IndexOf(data);
        }
        if(index < 0) {
            return;
        }
        foreach(UIitemContextBase scrollCell in cellsInUse) {
            if(scrollCell.DataIndex == index) {
                scrollCell.DataObject = data;
            }
        }
    }

    public void refresh() {
        int allColumns = Mathf.CeilToInt(allData.Count / (float)numberOfColumns);
        int maxFirstIndex = allColumns - visibleCellsRowCount - 1;
        if(maxFirstIndex >= 0)
            firstVisibleIndex = firstVisibleIndex > maxFirstIndex ? maxFirstIndex : firstVisibleIndex;
        preFirstVisibleIndex = firstVisibleIndex;

        SetInUseCells(0);
        setContentSize();
        int needShowCount = allData.Count - firstVisibleIndex * numberOfColumns;
        needShowCount = needShowCount > visibleCellsTotalCount ? visibleCellsTotalCount : needShowCount;
        for(int i = 0; i < needShowCount; i++) {
            ShowCell(i + firstVisibleIndex * numberOfColumns, true);
        }
    }
}
