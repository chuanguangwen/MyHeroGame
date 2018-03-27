using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using GManager;

public enum AdapationType {
    ModifyColumns,  //修改列，
    ModifyColumnsCenter,//修改列中心，
    FixedColumns,//固定列
    FixedColumnsCenter//固定柱中心
}
public class ScrollLoopController : UIBehaviour {
    public Vector2 CellSize = new Vector2(50f,50f);
    public Vector2 CellOffset;

    [SerializeField]
    private ScrollRect m_scrollRect;
    [SerializeField]
    private ScrollRect m_cellPrefab;
    [SerializeField]
    private int m_numberOfColumns = 1; // 行或列 数量
	public int NumberOcColumns{
		set{
			this.m_numberOfColumns = value;
		}
	}
    [SerializeField]
	private AdapationType m_adapationType = AdapationType.FixedColumns;

    private int m_visibleCellsRowCount;    // 每行显示个数
    private int m_visibleCellsTotalCount;
    private int m_preFirstVisibleIndex;
    private int m_firstVisibleIndex;
    private IList m_allData;
    private bool m_initFinish;
    private int m_preNumberOfColumns;
    private Vector2 m_contentPos;
    private Vector2 m_initCellSize;
    private Vector2 m_initCellOffset;

    private LinkedList<UIitemContextBase> m_localCellsPool = new LinkedList<UIitemContextBase>();
    private LinkedList<UIitemContextBase> m_cellsInUse = new LinkedList<UIitemContextBase>();

    public UIContextBase parent;
    public string cellviewname;
    public string cellctrlName;
    public string path;
    public GameObject cellPrefab;

    private bool horizontal {
        get { return this.m_scrollRect.horizontal; }
    }

    public void InitWithData(IList cellDataList) {
        if (!this.m_initFinish) {
            this.m_initCellSize = this.CellSize;
            this.m_initCellOffset = this.CellOffset;
            this.InitData();
            this.SetCellsPools();
            this.m_initFinish = true;
        }
        this.m_contentPos = this.m_scrollRect.content.anchoredPosition;
        this.m_preNumberOfColumns = this.m_numberOfColumns;
        this.m_allData = cellDataList;
        this.SetInUseCells(0);
        this.SetContentSize();
        int showCount = this.m_visibleCellsTotalCount > this.m_allData.Count ? this.m_allData.Count : this.m_visibleCellsTotalCount;
        for(int i = 0; i < showCount; i++) {
            this.ShowCell(i, true);
        }
    }
    private void InitData() {
        this.CellSize = this.m_initCellSize;
        this.CellOffset = this.m_initCellOffset;
        if (this.horizontal) {
            this.m_visibleCellsRowCount = Mathf.CeilToInt(this.m_scrollRect.viewport.rect.width / this.CellSize.x);
//            if(this.m_adapationType == AdapationType.ModifyColumns){
//                this.m_numberOfColumns = (int)(this.m_scrollRect.viewport.rect.height / CellSize.y);
//            } else if(this.m_adapationType == AdapationType.ModifyColumnsCenter) {
//                this.m_numberOfColumns = (int)(this.m_scrollRect.viewport.rect.height / CellSize.y);
//                float cellHieght = this.CellSize.y;
//                this.CellSize.y = this.m_scrollRect.viewport.rect.height / this.m_numberOfColumns;
//                this.CellOffset.y = (this.CellSize.y - (cellHieght - this.CellOffset.y * 2)) / 2;
//            }else if (this.m_adapationType == AdapationType.FixedColumnsCenter) {
//                float cellHeight = this.CellSize.y;
//                this.CellSize.y = this.m_scrollRect.viewport.rect.height / this.m_numberOfColumns;
//                this.CellOffset.y = (this.CellSize.y - (cellHeight - this.CellOffset.y * 2)) / 2;
//            }
        } else {
            this.m_visibleCellsRowCount = Mathf.CeilToInt(this.m_scrollRect.viewport.rect.height / this.CellSize.y);
//            if(this.m_adapationType == AdapationType.ModifyColumns) {
//                this.m_numberOfColumns = (int)(this.m_scrollRect.viewport.rect.width / this.CellSize.x);
//            } else if (this.m_adapationType == AdapationType.ModifyColumnsCenter) {
//                this.m_numberOfColumns = (int)(this.m_scrollRect.viewport.rect.width / this.CellSize.x);
//                float cellWidth = this.CellSize.x;
//                this.CellSize.x = this.m_scrollRect.viewport.rect.width / this.m_numberOfColumns;
//                this.CellOffset.x = (this.CellSize.x - (cellWidth - this.CellOffset.x * 2)) / 2; 
//            }else if(this.m_adapationType == AdapationType.FixedColumns) {
//                float cellWidth = this.CellSize.x;
//                this.CellSize.x = this.m_scrollRect.viewport.rect.width / this.m_numberOfColumns;
//                this.CellOffset.x = (this.CellSize.x - (cellWidth - this.CellOffset.x * 2)) / 2;
//            }
        }
        this.m_visibleCellsTotalCount = (this.m_visibleCellsRowCount + 1) * this.m_numberOfColumns;
    }

    private void SetCellsPools() {
        int outSideCount = this.m_localCellsPool.Count + this.m_cellsInUse.Count - this.m_visibleCellsTotalCount;

        if (outSideCount > 0) {
            while (outSideCount > 0) {
                outSideCount--;
                LinkedListNode<UIitemContextBase> cell = this.m_localCellsPool.Last;
                this.m_localCellsPool.RemoveLast();
                Destroy(cell.Value.gameObject);
            }
        }else if(outSideCount < 0) {
            for (int i = 0; i < outSideCount * -1; i++) {
                GameObject go = Util.AddChild(this.m_scrollRect.content.transform, this.cellPrefab);
                UIitemContextBase item = Util.InstantiationClass<UIitemContextBase>(this.cellctrlName, new object[] { this.cellviewname, this.path, this.parent });
                item.InitView(go);
                this.m_localCellsPool.AddLast(item);
            }
        }
    }

    private void SetInUseCells(int visibleCellCount) {
        int outSideCount = this.m_cellsInUse.Count - visibleCellCount;
        while (outSideCount > 0) {
            outSideCount--;
            this.m_cellsInUse.Last.Value.gameObject.SetActive(false);
            this.m_localCellsPool.AddLast(this.m_cellsInUse.Last.Value);
            this.m_cellsInUse.RemoveLast();
        }
    }

    private void SetContentSize() {
        int cellOneWayCount = (int)Math.Ceiling((float)this.m_allData.Count / this.m_numberOfColumns);
        if (this.horizontal) {
            this.m_scrollRect.content.sizeDelta = new Vector2(cellOneWayCount * this.CellSize.x, this.m_scrollRect.content.sizeDelta.y);
        } else {
            this.m_scrollRect.content.sizeDelta = new Vector2(this.m_scrollRect.content.sizeDelta.x , cellOneWayCount * this.CellSize.y);
        }
    }

    private void ShowCell(int cellIndex , bool scrollingPositive) {
        UIitemContextBase tempCell = GetCellFromPool(scrollingPositive);
        this.PositionCell(tempCell.gameObject, cellIndex);
        if(cellIndex < this.m_allData.Count) {
            tempCell.gameObject.SetActive(true);
            tempCell.Init(null, this.m_allData[cellIndex], cellIndex);
        } else {
            tempCell.gameObject.SetActive(false);
        }
    }
    private UIitemContextBase GetCellFromPool(bool scrollingPositive) {
        if (this.m_localCellsPool.Count == 0)
            return null;
        LinkedListNode<UIitemContextBase> cell = this.m_localCellsPool.First;
        this.m_localCellsPool.RemoveFirst();

        if (scrollingPositive)
            this.m_cellsInUse.AddLast(cell);
        else
            this.m_cellsInUse.AddFirst(cell);
        return cell.Value;
    }
    private void PositionCell(GameObject go , int index) {
        int rowMod = index % this.m_numberOfColumns;
        if (horizontal)
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * (index / this.m_numberOfColumns) + this.CellOffset.x, -rowMod * CellSize.y + this.CellOffset.y);
        else
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(CellSize.x * rowMod + this.CellOffset.x, -(index / this.m_numberOfColumns) * CellSize.y + this.CellOffset.y);
    }

	protected override void OnRectTransformDimensionsChange ()
	{
		base.OnRectTransformDimensionsChange ();
		if (this.m_initFinish) {
			this.InitData();
			SetInUseCells(this.m_visibleCellsTotalCount);
			this.SetCellsPools();
			if(this.m_preNumberOfColumns == this.m_numberOfColumns) {
				Refresh();
			} else {
				this.m_scrollRect.content.anchoredPosition = this.m_contentPos;
				this.InitWithData (this.m_allData);
			}
		}
	}

	public void Refresh(){
		int allColumns = Mathf.CeilToInt (this.m_allData.Count / (float)this.m_numberOfColumns);
		int maxFirestIndex = allColumns - this.m_visibleCellsRowCount - 1;
		if (maxFirestIndex >= 0)
			this.m_firstVisibleIndex = this.m_firstVisibleIndex > maxFirestIndex ? maxFirestIndex : this.m_firstVisibleIndex;
		this.m_preFirstVisibleIndex = this.m_firstVisibleIndex;

		this.SetInUseCells (0);
		this.SetContentSize ();
		int needShowCount = this.m_allData.Count - this.m_firstVisibleIndex * this.m_numberOfColumns;
		needShowCount = needShowCount > this.m_visibleCellsTotalCount ? this.m_visibleCellsTotalCount : needShowCount;
		for (int i = 0; i < needShowCount; i++) {
			this.ShowCell(i + this.m_firstVisibleIndex * this.m_numberOfColumns , true);
		}
	}

	public void UpdateCellData(System.Object data, int index = -1) {
		if(index == -1) {
			index = this.m_allData.IndexOf(data);
		}
		if(index < 0) {
			return;
		}
		foreach(UIitemContextBase scrollCell in this.m_cellsInUse) {
			if(scrollCell.DataIndex == index) {
				scrollCell.DataObject = data;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		this.CalculateCurrentIndex();
		this.InternalCellsUpdate();
	}

	private void CalculateCurrentIndex(){
		if (horizontal)
			this.m_firstVisibleIndex = (int)(-this.m_scrollRect.content.anchoredPosition.x / this.CellSize.x);
		else
			this.m_firstVisibleIndex = (int)(this.m_scrollRect.content.anchoredPosition.y / this.CellSize.y);
		int limit = Mathf.CeilToInt(this.m_allData.Count / (float)this.m_numberOfColumns) - this.m_visibleCellsRowCount;
		if( this.m_firstVisibleIndex < 0 || limit <= 0)
			this.m_firstVisibleIndex = 0;
		else if( this.m_firstVisibleIndex >= limit) {
			this.m_firstVisibleIndex = limit - 1;
		}
	}
	private void InternalCellsUpdate(){
		if (this.m_preFirstVisibleIndex != this.m_firstVisibleIndex) {
			bool scrollingPositive = this.m_preFirstVisibleIndex < this.m_firstVisibleIndex;
			int indexData = Mathf.Abs (this.m_preFirstVisibleIndex - this.m_firstVisibleIndex);
			int deltaSign = scrollingPositive ? +1 : -1 ;
			for(int i = 1 ; i <= indexData ; i++){
				UpdateContent(this.m_preFirstVisibleIndex + i * deltaSign , scrollingPositive);
			}
			this.m_preFirstVisibleIndex = this.m_firstVisibleIndex;
		}
	}

	private void UpdateContent(int cellIndex , bool scrollingPositive){
		int index = scrollingPositive ? ((cellIndex - 1) * this.m_numberOfColumns) + (this.m_visibleCellsTotalCount) : (cellIndex * this.m_numberOfColumns);

		for(int i = 0 ; i < this.m_numberOfColumns ; i ++){
			this.FreeCell (scrollingPositive);
			this.ShowCell (index + i, scrollingPositive);
		}
	}

	private void FreeCell(bool scrollingPositive) {
		LinkedListNode<UIitemContextBase> cell = null;
		if (scrollingPositive) {
			cell = this.m_cellsInUse.First;
			this.m_cellsInUse.RemoveFirst ();
			this.m_localCellsPool.AddLast (cell);
		} else {
			cell = this.m_cellsInUse.Last;
			this.m_cellsInUse.RemoveLast ();
			this.m_localCellsPool.AddFirst (cell);
		}
		//   cell.Value.gameObject.SetActive(false);
	}
}
