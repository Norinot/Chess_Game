using UnityEngine;
using UnityEngine.UI;

public class King : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        mMovement = new Vector3Int(1,1,1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("King_White");
    }

    public override void Kill()
    {
        base.Kill();
        mPieceManager.IsKingsAlive = false;
    }
}
