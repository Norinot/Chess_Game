using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceManager : MonoBehaviour
{

    [HideInInspector]

    public bool IsKingsAlive = true;  // elég egyértelmű nemde?

    public GameObject mPiecePrefab;

    private List<BasePiece> mWhitePieces = null;
    private List<BasePiece> mBlackPieces = null;


    private string[] mPieceOrder = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "K", "Q", "B", "KN", "R"
    };

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"P", typeof(Pawn)},
        {"R", typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    public void Setup(Board board)
    {
        mWhitePieces = CreatePieces(Color.white, new Color32(80, 124, 159, 255), board);

        mBlackPieces = CreatePieces(Color.black, new Color32(210, 95, 64, 255), board);

        PlacePieces(1, 0, mWhitePieces, board);
        PlacePieces(6, 7, mBlackPieces, board);

        SwitchSides(Color.black);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieces = new List<BasePiece>(); //ide tároljuk azokat a bábúkat az összes bábút annak a helyes csapatába

        for (int i = 0; i < mPieceOrder.Length; i++)
        {

            //Magáért beszél, új objektumként felvesszük és kreáljuk a bábúkat
            
            GameObject newPieceObject = Instantiate(mPiecePrefab);
            newPieceObject.transform.SetParent(transform);  //set parent csak amiatt hogy a hierarchia átláthatóbb legyen majd

            // méret /pozició
            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;
            //tárolás
            string key = mPieceOrder[i];  //azért lett eldobva a fenes megoldás mivel így jövőbeli játékmódok kialakítása egyszerűbb és nincs limitáció pozíciók megtartása nehezebb így mondjuk.
            Type pieceType = mPieceLibrary[key]; //referencia -->>

            //referencia alapján adjuk hozzá a komponenst amit felette meghívtunk (KULCS)
            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            newPieces.Add(newPiece);


            //setup
            newPiece.Setup(teamColor, spriteColor, this);
        }
        return newPieces;
    }

    private void PlacePieces(int pawnRow, int royalityRow,List<BasePiece> pieces,Board board)
    {
        for (int i = 0; i < 8; i++)
        {

            //paraszt sor 
            pieces[i].Place(board.mAllCells[i, pawnRow]);


            //speciális karakter sor
            pieces[i + 8].Place(board.mAllCells[i, royalityRow]);


            //egyszerű felépítés
        }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value)
    {
        foreach (BasePiece piece in allPieces)
        {
            piece.enabled = value;
        }
    }
    public void SwitchSides(Color color)
    {
        if (!IsKingsAlive)
        {

            //reset
            ResetPieces();


            // FELTÁMADÁS
            IsKingsAlive = true;
            //szín csere, így fehér fog kezdeni megint
            color = Color.black;
        }

        // ha a sötét lépése van akkor true a return ha a white piece jön akkor pedig false a return value 
        bool isBlackTurn = color == Color.white ? true : false;


        // interaktivitás, csodabogár rész
        SetInteractive(mWhitePieces, !isBlackTurn);
        SetInteractive(mBlackPieces, isBlackTurn);
    }
    public void ResetPieces()
    {

        //fehér reset
        foreach (BasePiece piece in mWhitePieces)
        {
            piece.Reset();
        }

        //fekete reset
        foreach (BasePiece piece in mBlackPieces)
        {
            piece.Reset();
        }
    }
}
