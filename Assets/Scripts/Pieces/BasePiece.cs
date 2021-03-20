using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear; //White/Black reset kezelése majd egyszer valamikor

    protected Cell mOriginalCell = null;   //Eredeti pozi
    protected Cell mCurrentCell = null;    //Jelenlegi pozi

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Cell mTargetCell = null;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor,PieceManager newPieceManager) 
    {
        mPieceManager = newPieceManager;

        mColor = newTeamColor; 
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>(); //RectTransform = rectangle pozíciók, méretek, pivot pont, anchorok

    }

    public void Place(Cell newCell)
    {

        //cellák dyló hátha yóó
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;


        //objektum akármi  objektum része mittom én
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        Kill();

        Place(mOriginalCell);
    }
    public virtual void Kill()
    {
        mCurrentCell.mCurrentPiece = null;
        gameObject.SetActive(false);
    }

    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {

        //target
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        //minden cellát ellenőrizzünk le 
        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;


            // cella státusz
            //this = referenceia BasePiece-re
            CellState cellState = CellState.None;
            cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);


            // ha ellenfél van benne akkor adjuk hozzá a listához és break
            if (cellState == CellState.Enemy)
            {
                mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
                break;
            }
            //ha a cella nem üres akkor break
            if (cellState != CellState.Free)
            {
                break;
            }
            //listához adás
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
        }
    }
    protected virtual void CheckPathing()
    {
        //lényegében az X irányba fogunk mozogni az X értéke alapján, itt igazából minden ugyan azt csinálja csak x,y,z kordokon
        //horizontálisan
        CreateCellPath(1, 0, mMovement.x);
        CreateCellPath(-1, 0, mMovement.x);

        //vertikálisan

        CreateCellPath(0, 1, mMovement.y);
        CreateCellPath(0, -1, mMovement.y);

        //Felső átló
        CreateCellPath(1, 1, mMovement.z);
        CreateCellPath(-1, 1, mMovement.z);

        //alsó átló

        CreateCellPath(-1, -1, mMovement.z);
        CreateCellPath(1, -1, mMovement.z);

        // Nem a legszebb megoldás biztos meg lehetett volna oldani egy for ciklusban de nem sikerült és nem akarok időt húzni vele. Működik.
    }

    protected void ShowCells()
    {
        foreach (Cell cell in mHighlightedCells)    
            cell.mOutlineImage.enabled = true;  
    }
    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = false;

        mHighlightedCells.Clear();     
    }

    protected virtual void Move()
    {

        // ha van ellenség bábú az útban akkor remove
        mTargetCell.RemovePiece();

        // nullázzunk a memóriából hogy ott volt valami
        mCurrentCell.mCurrentPiece = null;

        //lépések itt történik az, hogy lépünk majd egyszer.
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;

        //mozgás a táblán 
        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        //tesztelés
        CheckPathing();
        //illegális cellák
        ShowCells();
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        transform.position += (Vector3)eventData.delta;


        foreach (Cell cell in mHighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition)) 
            {
                mTargetCell = cell;
                break;
            }
            mTargetCell = null; 
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        ClearCells();

        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }
        Move();

        mPieceManager.SwitchSides(mColor);
    }
}

