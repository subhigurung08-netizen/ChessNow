/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Board board;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    // private GameObject[,] pieces;
    public GameObject[,] pieces;
    private List<GameObject> movedPawns;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    //Adding isPlayer attribute 
    private bool isAI;

    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        pieces = new GameObject[8, 8];
        movedPawns = new List<GameObject>();

        white = new Player("PLAYER", true);
        black = new Player("black", false);

        currentPlayer = white;
        otherPlayer = black;

        //isAI starts out false since player goes first
        isAI = false;

        InitialSetup();
    }

    //added GetIsAI method for other methods in other classes to access isAI attribute
    public bool GetIsAI()
    {
        return isAI;
    }

    //added GetPlayer method for other methods in other classes to access player
    public Player GetPlayer()
    {
        return white;
    }

    //added GetPiecesGM for other methods in other classes to access pieces 2D array
    public GameObject[,] GetPiecesGM()
    {
        return pieces;
    }


    private void InitialSetup()
    {
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 1, 0);
        AddPiece(whiteBishop, white, 2, 0);
        AddPiece(whiteQueen, white, 3, 0);
        AddPiece(whiteKing, white, 4, 0);
        AddPiece(whiteBishop, white, 5, 0);
        AddPiece(whiteKnight, white, 6, 0);
        AddPiece(whiteRook, white, 7, 0);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(whitePawn, white, i, 1);
        }

        AddPieceAI(blackRook, 0, 7);
        AddPieceAI(blackKnight, 1, 7);
        AddPieceAI(blackBishop, 2, 7);
        AddPieceAI(blackQueen, 3, 7);
        AddPieceAI(blackKing, 4, 7);
        AddPieceAI(blackBishop, 5, 7);
        AddPieceAI(blackKnight, 6, 7);
        AddPieceAI(blackRook, 7, 7);

        for (int i = 0; i < 8; i++)
        {
            AddPieceAI(blackPawn, i, 6);
        }
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    // Adding another separate method to add ai pieces
    public void AddPieceAI(GameObject prefab, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        ChessAI.inst.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint);

        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        // filter out locations with friendly piece
        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        return locations;
    }

    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent.type == PieceType.Pawn && !HasPawnMoved(piece))
        {
            movedPawns.Add(piece);
        }

        Vector2Int startGridPoint = GridForPiece(piece);
        // Debug.Log("the starting position of this piece is: x: " + startGridPoint.x + " and y: " + gridPoint.y);
        //debug to find cause of index error
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        Debug.Log("piece moved to: x: " + gridPoint.x + "and y: " + gridPoint.y);
        board.MovePiece(piece, gridPoint);
    }

    public void PawnMoved(GameObject pawn)
    {
        movedPawns.Add(pawn);
    }

    public bool HasPawnMoved(GameObject pawn)
    {
        return movedPawns.Contains(pawn);
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        if (pieceToCapture.GetComponent<Piece>().type == PieceType.King)
        {
            // Debug.Log(currentPlayer.name + " wins!");
            //does not work if ai wins so replacing it
            if(isAI)
            {
                Debug.Log(ChessAI.inst.GetName() + " wins!");
                
                Win.instWin.ShowWinner(ChessAI.inst.GetName());
            }

            else
            {
                Debug.Log(currentPlayer.name + " wins!");
                Win.instWin.ShowWinner(currentPlayer.name);
            }
            
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());

        }
        
        
        // currentPlayer.capturedPieces.Add(pieceToCapture);
        //replaced this with if else statement since current player is only applicable when isAI is false
        if(isAI)
        {
            ChessAI.inst.GetCapturedPiecesAI().Add(pieceToCapture);

        }

        else
        {
            currentPlayer.capturedPieces.Add(pieceToCapture);
        }
        pieces[gridPoint.x, gridPoint.y] = null;
        Destroy(pieceToCapture);
        Debug.Log("a piece was destroyed");
        //adding debug log to see whether piece was destroyed 

    }


    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        //return currentPlayer.pieces.Contains(piece);
        //replacing this because current player only applicable when isAI is false

        if(isAI)
        {
            return ChessAI.inst.GetPiecesAI().Contains(piece);
        }

        else
        {
            return currentPlayer.pieces.Contains(piece);
        }
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) {
            return false;
        }

        // if (otherPlayer.pieces.Contains(piece))
        // {
        //     return false;
        // }
        
        //replacing above if statement since other player variable is no longer valid 

        if(isAI)
        {
            if(!ChessAI.inst.GetPiecesAI().Contains(piece))
            {
                return false;
            }
        }

        else
        {
            if(ChessAI.inst.GetPiecesAI().Contains(piece))
            {
                return false;
            }
        }

        return true;
    }

    public void NextPlayer()
    {
        // Player tempPlayer = currentPlayer;
        // currentPlayer = otherPlayer;
        // otherPlayer = tempPlayer;
        
        //current player should remain the same


        //Adding if and else statements to change isAI when turn switches
        if(isAI)
        {
            isAI = false;
        }

        else
        {
            isAI = true;
            //calling BestMove() to start ai's turn
            ChessAI.inst.BestMove();
            //after ai moves then isAI goes back to false
            isAI = false;
            
        }

    }


    
}
