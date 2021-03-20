using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        GetComponent<Image>().sprite = Resources.Load<Sprite>("Knight_White");
    }
    private void CreateCellPAth(int flipper)
    {
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;
        
        //2 fel, le, balra, jobbra, 8 helyre tud lépni, 2+1 bal jobbra -1 ha úgy van flipper vagy pozitív vagy negatív felső 4 vagy alsó négy

        MatchesState(currentX - 2, currentY + (1 * flipper));

        MatchesState(currentX - 1, currentY + (2 * flipper));

        MatchesState(currentX + 1, currentY + (2 * flipper));

        MatchesState(currentX + 2, currentY + (1 * flipper));
    }

    protected override void CheckPathing()
    {
        CreateCellPAth(1);

        CreateCellPAth(-1);
    }

    private void MatchesState(int targetX, int targetY)
    {
        CellState cellState = CellState.None;
        cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);

        if (cellState != CellState.Friendly && cellState != CellState.OutOfBounds)
        {
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
        }
    }
}
