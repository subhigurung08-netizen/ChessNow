using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessAI : MonoBehaviour
{
   public List<GameObject> pieces;
   public List<GameObject> capturedPieces;
   public List<Move> bestMoves;
   public List<Move> undoSimulation;
   
   public int maxPly = 1;
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
   	
    
    GetBestMove();
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
       List<Move> undoSimulation = new List<Move>();


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
     Debug.Log("HELLO" + depth);
if (depth == 0) {       return EvaluateBoard(board);}   //base case
	List<Move> legalMoves = new List<Move>();
    Debug.Log("elo1");
    legalMoves = LegalMoves();
    Debug.Log(legalMoves.Count);
       if (maximizingPlayer)
       {
           float bestScore = float.NegativeInfinity;
           foreach (Move m in legalMoves)
	     {
		   manager.Move(m.piece, m.position); //simulate move
           bestMoves.Add(m);

		   
            float score = Minimax(manager.board, depth-1, false);
		   if(bestScore > score)
		   {
		   	bestScore = score;
            }
            else
            {
                bestMoves.Remove(m);
            }
            
	    }
           return bestScore;
       }
       else
        {
           float bestScore = float.PositiveInfinity;
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
    Debug.Log("elo2");
	List<Move> legalMoves = new List<Move>();
    Debug.Log(manager.pieces.GetLength(0));
    
	foreach (GameObject piece in manager.pieces)
    {
            
            Debug.Log("getting legal moves");
            if(manager.GridForPiece(piece).Equals(null) || piece.Equals(null))
            {
                continue;
            }
            Move undo = new Move(piece, manager.GridForPiece(piece));
            Debug.Log("got to og position");
            
            if(undoSimulation!=null)
            {
                undoSimulation.Add(undo);
            }
            Debug.Log("added it to undo");

            List<Vector2Int> positions = new List<Vector2Int>(); 
            positions = manager.MovesForPiece(piece);
            Debug.Log(positions.Count);
            foreach (Vector2Int pos in positions)
            {
                Debug.Log("elo4");
                Move move = new Move(piece, pos);
                Debug.Log("elo5");
                if(legalMoves!=null)
                {
                    legalMoves.Add(move);
                }
                Debug.Log("elo6");


            }
    }
Debug.Log("elo7");
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

Move GetBestMove()
{
Debug.Log("getting best move");
Minimax(manager.board, 1, false);
Debug.Log("minimax done");
foreach(Move m in undoSimulation)
{
    manager.Move(m.piece, m.position);
}
Debug.Log("similated moves undone");
List<Move> tempMoves = bestMoves;
bestMoves.Clear();
Debug.Log("bestMoves cleared");
return tempMoves[0];
}





}








