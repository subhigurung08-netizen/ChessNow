using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessAI : MonoBehaviour
{
   public List<GameObject> pieces;
   public List<GameObject> capturedPieces;
   public int maxPly = 4;
   public int ply;
   public GameManager manager;
   public string name;
   public int forward;


   public struct Move
   {
   	public Vector2Int position;
	public GameObject piece;
   	
	public Move(GameObject chessPiece, Vector2Int pos)
	{
		position = pos;
		piece = chessPiece;
}
   }


   




// Start is called before the first frame update
   void Start()
   {
   	Minimax(manager.board, maxPly, false);
   }


   // Update is called once per frame
   void Update()


   {
      
   }




public ChessAI(string name, bool positiveZMovement)
{
       this.name = name;
       pieces = new List<GameObject>();
       capturedPieces = new List<GameObject>();


       if (positiveZMovement == true)
       {
           this.forward = 1;
       }
       else
       {
           this.forward = -1;
       }
}








   float Minimax(Board board, float depth, bool maximizingPlayer)
   {
      Debug.Log("hello" + depth);
if (depth == 0) {       return EvaluateBoard(board);}   //base case
	List<Move> legalMoves = new List<Move>();
    Debug.Log("elo1");
legalMoves = LegalMoves();
Debug.Log("elo2");
       if (maximizingPlayer)
       {
           float bestScore = float.PositiveInfinity;
           foreach (Move m in legalMoves)
	     {
		   manager.Move(m.piece, m.position); //simulate move
		   Board newBoard = new Board();
               float score = Minimax(manager.board, depth-1, false);
		   if(bestScore > score)
		   {
		   	bestScore = score;
   }
	     }
           return bestScore;
       }
       else
 {
           float bestScore = float.NegativeInfinity;
	     //ArrayList<move> legalMoves = LegalMoves();
           foreach (Move m in legalMoves)
	     {
               manager.Move(m.piece, m.position); //simulate move
		   
               float score = Minimax(manager.board, depth-1, true);
		   if(bestScore < score)
		   {
		   	bestScore = score;
   }
	     }
           return bestScore;


       }
   }


   List<Move> LegalMoves()
   {
    Debug.Log("elo1.1");
	List<Move> legalMoves = new List<Move>();
    Debug.Log("elo1.2");
	foreach (GameObject piece in manager.pieces)
	{
	
List<Vector2Int> positions = new List<Vector2Int>(); 
Debug.Log("elo1.3");
positions = manager.MovesForPiece(piece);
Debug.Log("elo1.4");
foreach (Vector2Int pos in positions)
{
    Debug.Log("elo1.5");
Move move = new Move(piece, pos);
legalMoves.Add(move);


}
}
return legalMoves;
   }


   float EvaluateBoard(Board board)
   {
       int score = 0;


       foreach(GameObject piece in manager.pieces)
       {
           int value = GetPieceValue(piece);


           if(manager.isPlayer)
           {
              score -= value;
           }


           else
           {
               score += value;
           }
       }
       return score;


   }


int GetPieceValue(GameObject piece)
{
	Piece pieceComponent = piece.GetComponent<Piece>();
switch(pieceComponent.type)
{
	case PieceType.Pawn:
	return 1;




case PieceType.Bishop:
	return 3;




case PieceType.Knight:
	return 3;




case PieceType.King:
	return 674000;




case PieceType.Queen:
	return 9;




case PieceType.Rook:
	return 5;


default:
				return 0;
	
}
}




}





