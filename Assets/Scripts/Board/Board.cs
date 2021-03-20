using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CellState
{
    None,
    Friendly,
    Enemy,
    Free,
    OutOfBounds
}

public class Board : MonoBehaviour
{
    public GameObject mCellPrefab;

    [HideInInspector]
    public Cell[,] mAllCells = new Cell[8, 8];

    public void Create()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject newCell = Instantiate(mCellPrefab, transform);


                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);

                mAllCells[x, y] = newCell.GetComponent<Cell>();
                mAllCells[x, y].Setup(new Vector2Int(x, y), this);
            }
        }
        for (int x = 0; x < 8; x +=2)
        {
            for (int y = 0; y < 8; y++)
            {
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                mAllCells[finalX, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            }
        }
    }

    public CellState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {

        //Ellenőrizzük a tábla határait 8x8 tehát 0-7 ig 
        if (targetX < 0 || targetX > 7)
        {
            return CellState.OutOfBounds;
        }
        if (targetY < 0 || targetY > 7)
        {
            return CellState.OutOfBounds;
        }

        // ha mindkettő felette valid akkor  a cella jelenlegi státuszát itt kapjuk meg ahol meghívunk előzően eltárolt adatokat
        Cell targetcell = mAllCells[targetX, targetY];


        //ha a cella tartalmaz egy bábút !=null
        if (targetcell.mCurrentPiece != null)
        {

            //ha nem ellenség
            if (checkingPiece.mColor == targetcell.mCurrentPiece.mColor)
            {
                return CellState.Friendly;
            }

            //ha ellenség
            if (checkingPiece.mColor != targetcell.mCurrentPiece.mColor)
            {
                return CellState.Enemy;
            }
        }
        // ha üres minden akkor nincs ott shemmi :O 
        return CellState.Free;
    }
}
