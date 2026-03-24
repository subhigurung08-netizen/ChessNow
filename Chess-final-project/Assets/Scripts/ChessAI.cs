using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessAI : MonoBehaviour
{
   public static ChessAI inst;
   
   public List<GameObject> pieces;
   public List<GameObject> capturedPieces;
   public List<Move> bestMoves;
   public List<Move> undoSimulation;
   
   public int maxPly = 1;
   public int ply;
//    public GameManager manager;
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
   	
    
    //GetBestMove();
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
     Debug.Log("minimax" + depth);
        if (depth == 0) 
        {    
            Debug.Log("going to evaluate board");
            return EvaluateBoard(board);
        }   //base case
	List<Move> legalMoves = new List<Move>();
    Debug.Log("elo1");
    legalMoves = LegalMoves();
    Debug.Log(legalMoves.Count);
       if (maximizingPlayer)
       {
           float bestScore = float.NegativeInfinity;
           foreach (Move m in legalMoves)
	     {
		   GameManager.instance.Move(m.piece, m.position); //simulate move
           bestMoves.Add(m);

		   
            float score = Minimax(GameManager.instance.board, depth-1, false);
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
               GameManager.instance.Move(m.piece, m.position); //simulate move
		   
               float score = Minimax(GameManager.instance.getBoard(), depth-1, true);
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
    Debug.Log(GameManager.instance.pieces.GetLength(0));
    
	foreach (GameObject piece in GameManager.instance.pieces)
    {
            
            Debug.Log("getting legal moves");
            // if(piece.Equals(null) || GameManager.instance.GridForPiece(piece).Equals(null))
            // {
            //     continue;
            // }
            Move undo = new Move(piece, GameManager.instance.GridForPiece(piece));
            Debug.Log("got to og position");
            
            if(undoSimulation!=null)
            {
                undoSimulation.Add(undo);
            }
            Debug.Log("added it to undo");

            List<Vector2Int> positions = new List<Vector2Int>(); 
            positions = GameManager.instance.MovesForPiece(piece);
            Debug.Log(positions.Count);
            foreach (Vector2Int pos in positions)
            {
                Debug.Log("elo4");
                if(!piece.Equals(null) && !pos.Equals(null))
                {
                    Move move = new Move(piece, pos);
                    Debug.Log("elo5");
                    if(legalMoves!=null)
                    {
                        legalMoves.Add(move);
                    }
                }
                Debug.Log("elo6");


            }
    }
Debug.Log("elo7 finished legalMoves");
return legalMoves;
   }


   float EvaluateBoard(Board board)
   {
       int score = 0;


       foreach(GameObject piece in GameManager.instance.pieces)
       {
           int value = GetPieceValue(piece);
            Debug.Log("got piece value");

           if(GameManager.instance.getIsPlayer())
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
    Debug.Log("getting piece value");
    if(piece == null)
    {
        return 0;
    }
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

public void BestMove()
{
    Minimax(GameManager.instance.board, 1, false);
    Debug.Log("getting best move");
    if(bestMoves==null)
    {
        return;
    }

    
    Debug.Log("minimax done");
    if(undoSimulation!= null)
    {
        Debug.Log("undoing simulation");
    foreach(Move m in undoSimulation)
    {
        if(m.piece == null || m.position == null)
        {
            continue;
        }
        GameManager.instance.Move(m.piece, m.position);
        }
    }
    Debug.Log("similated moves undone");

    List<Move> tempMoves = bestMoves;
    bestMoves.Clear();
    Debug.Log("bestMoves cleared");
    
    
    // if (!moveLocations.Contains(tempMoves[0].position))
    // {
    //     return;
    //    }

        if (GameManager.instance.PieceAtGrid(tempMoves[0].position) == null)
        {
            Debug.Log("going to do best move ai");
            GameManager.instance.Move(tempMoves[0].piece, tempMoves[0].position);
            

        }   
        
        else
        {
            GameManager.instance.CapturePieceAt(tempMoves[0].position);
            GameManager.instance.Move(tempMoves[0].piece, tempMoves[0].position);
        }
    // return tempMoves[0];

    
}





}








