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
using UnityEngine.SceneManagement;

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

    public GameObject[,] pieces;
    // private GameObject[,] pieces;
    private List<GameObject> movedPawns;

    public Player white;
    public ChessAI black;
    // public var currentPlayer;
    // public var otherPlayer;
    public bool isPlayer;

    public GameObject player;
    public GameObject ai;

    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        Debug.Log("hoi");
        pieces = new GameObject[8, 8];
        movedPawns = new List<GameObject>();

        player = new GameObject();
        ai = new GameObject();

        // white = player.AddComponent<Player>();
        white = new Player("white", true);
        black = ai.AddComponent<ChessAI>();
        // black = new ChessAI("black");

        // currentPlayer = white;
        // otherPlayer = black;
        isPlayer = true;

        InitialSetup();
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

        Debug.Log("Initial SetUp Done");
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        // Debug.Log("add piece");
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public void AddPieceAI(GameObject prefab, int col, int row)
    {
        
        if(black==null)
        {
            Debug.Log("ai is null");
        }
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        // if(pieceObject == null)
        // {
        //     Debug.Log("pieceObject is null");
        //     return;
        // }

        
        
        black.pieces.Add(pieceObject);
        
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
        Debug.Log("getting moves for piece");
        if(pieceObject == null)
        {
            return new List<Vector2Int>();
        }

        else
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

        
    }

    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Debug.Log("sup MOVING");
        if(piece != null)
        {
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent.type == PieceType.Pawn && !HasPawnMoved(piece))
        {
            movedPawns.Add(piece);
        }
        Debug.Log("going to get Grid for piece");
        Vector2Int startGridPoint = GridForPiece(piece);
        // Debug.Log("x:" + startGridPoint.x + " & z: " + startGridPoint.y);
        if(startGridPoint.x>7 || startGridPoint.x<0 || startGridPoint.y>7 || startGridPoint.y < 0)
        {
            return;
        }
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
        }
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
            if(isPlayer)
            {

                Debug.Log("white wins!");
                SceneManager.LoadScene("Start Scene");
            }

            else
            {
                Debug.Log("black wins!");
                SceneManager.LoadScene("Start Scene");
            }
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());
        }
        if(isPlayer)
        {
            Debug.Log("player captured a piece");
            white.capturedPieces.Add(pieceToCapture);
        }
        else
        {
            Debug.Log("ai captured a piece");
            black.capturedPieces.Add(pieceToCapture);
        }
        pieces[gridPoint.x, gridPoint.y] = null;
        Destroy(pieceToCapture);
    }

    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece, bool isAI)
    {
        if(isAI)
        {
            Debug.Log("checking if piece belongs to AI:" + black.pieces.Contains(piece) +  "and it is ai's turn");
            return black.pieces.Contains(piece);
        }

        else
        {
            Debug.Log("checking if piece belongs to player" + white.pieces.Contains(piece) +  "and it is player's turn");
            return white.pieces.Contains(piece);
        }
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0 || pieces[gridPoint.x, gridPoint.y] == null)
        {
            return null;
        }
        // Debug.Log("piece at " + gridPoint.x + ", " + gridPoint.y);
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece)
    {
        Debug.Log("getting grid for piece");
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    // Debug.Log(" returning " + i + ", " + j);
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

        if(isPlayer)
        {
            if (black.pieces.Contains(piece))
            {
                return false;
            }
        
        }
        else
        {
            if (white.pieces.Contains(piece))
            {
                return false;
            }
        }
        return true;
    }

    public bool getIsPlayer()
    {
        return isPlayer;
    }

    public Board getBoard()
    {
        return board;
    }

    public ChessAI getColorAI()
    {
        return black;
    }
    
    public void NextPlayer()
    {
        // var tempPlayer = currentPlayer;
        // var currentPlayer = otherPlayer;
        // var otherPlayer = tempPlayer;
        if(isPlayer)
        {
            Debug.Log("player's turn to ai's turn");
            isPlayer = false;
            Debug.Log("isPlayer is " + isPlayer);
        
        }
        else
        {
            Debug.Log("ai's turn to player's turn");
            isPlayer = true;
            Debug.Log("isPlayer is " + isPlayer);

        }
    }
}


// /*
//  * Copyright (c) 2018 Razeware LLC
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  *
//  * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
//  * distribute, sublicense, create a derivative work, and/or sell copies of the 
//  * Software in any work that is designed, intended, or marketed for pedagogical or 
//  * instructional purposes related to programming, coding, application development, 
//  * or information technology.  Permission for such use, copying, modification,
//  * merger, publication, distribution, sublicensing, creation of derivative works, 
//  * or sale is expressly withheld.
//  *    
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  */

// using System.Collections.Generic;
// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
//     public static GameManager instance;

//     public Board board;

//     public GameObject whiteKing;
//     public GameObject whiteQueen;
//     public GameObject whiteBishop;
//     public GameObject whiteKnight;
//     public GameObject whiteRook;
//     public GameObject whitePawn;

//     public GameObject blackKing;
//     public GameObject blackQueen;
//     public GameObject blackBishop;
//     public GameObject blackKnight;
//     public GameObject blackRook;
//     public GameObject blackPawn;

//     private GameObject[,] pieces;
//     private List<GameObject> movedPawns;

//     private Player white;
//     private Player black;
//     public Player currentPlayer;
//     public Player otherPlayer;

//     void Awake()
//     {
//         instance = this;
//     }

//     void Start ()
//     {
//         pieces = new GameObject[8, 8];
//         movedPawns = new List<GameObject>();

//         white = new Player("white", true);
//         black = new Player("black", false);

//         currentPlayer = white;
//         otherPlayer = black;

//         InitialSetup();
//     }

//     private void InitialSetup()
//     {
//         AddPiece(whiteRook, white, 0, 0);
//         AddPiece(whiteKnight, white, 1, 0);
//         AddPiece(whiteBishop, white, 2, 0);
//         AddPiece(whiteQueen, white, 3, 0);
//         AddPiece(whiteKing, white, 4, 0);
//         AddPiece(whiteBishop, white, 5, 0);
//         AddPiece(whiteKnight, white, 6, 0);
//         AddPiece(whiteRook, white, 7, 0);

//         for (int i = 0; i < 8; i++)
//         {
//             AddPiece(whitePawn, white, i, 1);
//         }

//         AddPiece(blackRook, black, 0, 7);
//         AddPiece(blackKnight, black, 1, 7);
//         AddPiece(blackBishop, black, 2, 7);
//         AddPiece(blackQueen, black, 3, 7);
//         AddPiece(blackKing, black, 4, 7);
//         AddPiece(blackBishop, black, 5, 7);
//         AddPiece(blackKnight, black, 6, 7);
//         AddPiece(blackRook, black, 7, 7);

//         for (int i = 0; i < 8; i++)
//         {
//             AddPiece(blackPawn, black, i, 6);
//         }
//     }

//     public void AddPiece(GameObject prefab, Player player, int col, int row)
//     {
//         GameObject pieceObject = board.AddPiece(prefab, col, row);
//         player.pieces.Add(pieceObject);
//         pieces[col, row] = pieceObject;
//     }

//     public void SelectPieceAtGrid(Vector2Int gridPoint)
//     {
//         GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
//         if (selectedPiece)
//         {
//             board.SelectPiece(selectedPiece);
//         }
//     }

//     public List<Vector2Int> MovesForPiece(GameObject pieceObject)
//     {
//         Piece piece = pieceObject.GetComponent<Piece>();
//         Vector2Int gridPoint = GridForPiece(pieceObject);
//         List<Vector2Int> locations = piece.MoveLocations(gridPoint);

//         // filter out offboard locations
//         locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

//         // filter out locations with friendly piece
//         locations.RemoveAll(gp => FriendlyPieceAt(gp));

//         return locations;
//     }

//     public void Move(GameObject piece, Vector2Int gridPoint)
//     {
//         Piece pieceComponent = piece.GetComponent<Piece>();
//         if (pieceComponent.type == PieceType.Pawn && !HasPawnMoved(piece))
//         {
//             movedPawns.Add(piece);
//         }

//         Vector2Int startGridPoint = GridForPiece(piece);
//         pieces[startGridPoint.x, startGridPoint.y] = null;
//         pieces[gridPoint.x, gridPoint.y] = piece;
//         board.MovePiece(piece, gridPoint);
//     }

//     public void PawnMoved(GameObject pawn)
//     {
//         movedPawns.Add(pawn);
//     }

//     public bool HasPawnMoved(GameObject pawn)
//     {
//         return movedPawns.Contains(pawn);
//     }

//     public void CapturePieceAt(Vector2Int gridPoint)
//     {
//         GameObject pieceToCapture = PieceAtGrid(gridPoint);
//         if (pieceToCapture.GetComponent<Piece>().type == PieceType.King)
//         {
//             Debug.Log(currentPlayer.name + " wins!");
//             Destroy(board.GetComponent<TileSelector>());
//             Destroy(board.GetComponent<MoveSelector>());
//         }
//         currentPlayer.capturedPieces.Add(pieceToCapture);
//         pieces[gridPoint.x, gridPoint.y] = null;
//         Destroy(pieceToCapture);
//     }

//     public void SelectPiece(GameObject piece)
//     {
//         board.SelectPiece(piece);
//     }

//     public void DeselectPiece(GameObject piece)
//     {
//         board.DeselectPiece(piece);
//     }

//     public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
//     {
//         return currentPlayer.pieces.Contains(piece);
//     }

//     public GameObject PieceAtGrid(Vector2Int gridPoint)
//     {
//         if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
//         {
//             return null;
//         }
//         return pieces[gridPoint.x, gridPoint.y];
//     }

//     public Vector2Int GridForPiece(GameObject piece)
//     {
//         for (int i = 0; i < 8; i++) 
//         {
//             for (int j = 0; j < 8; j++)
//             {
//                 if (pieces[i, j] == piece)
//                 {
//                     return new Vector2Int(i, j);
//                 }
//             }
//         }

//         return new Vector2Int(-1, -1);
//     }

//     public bool FriendlyPieceAt(Vector2Int gridPoint)
//     {
//         GameObject piece = PieceAtGrid(gridPoint);

//         if (piece == null) {
//             return false;
//         }

//         if (otherPlayer.pieces.Contains(piece))
//         {
//             return false;
//         }

//         return true;
//     }

//     public void NextPlayer()
//     {
//         Player tempPlayer = currentPlayer;
//         currentPlayer = otherPlayer;
//         otherPlayer = tempPlayer;
//     }
// }
