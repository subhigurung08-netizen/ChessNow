using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessAI : MonoBehaviour
{
//    public static ChessAI inst;
   
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

   void Awake()
{
    bestMoves = new List<Move>();
    capturedPieces = new List<GameObject>();
    undoSimulation = new List<Move>();
    pieces = new List<GameObject>();
    // name = "black";
    // forward = -1;
}




public ChessAI(string name)
{
       this.name = name;
    //    pieces = new List<GameObject>();
    //    capturedPieces = new List<GameObject>();
    //    List<Move> undoSimulation = new List<Move>();
       
        this.forward = -1;

    //    if (positiveZMovement == true)
    //    {
    //        this.forward = 1;
    //    }
    //    else
    //    {
    //        this.forward = -1;
    //    }
}




   float Minimax(Board board, int depth, float alpha, float beta, bool maximizingAI)
{
    
    if (depth == 0)
    {
        return EvaluateBoard(board);
    }

    // List<Move> legalMoves = LegalMoves(maximizingAI);
    Debug.Log("minimax" + depth);
    // Debug.Log($"depth={depth}, maximizing={maximizingAI}, alpha={alpha}, beta={beta}, legalMoves={legalMoves.Count}");
    Debug.Log($"depth={depth}, maximizing={maximizingAI}, alpha={alpha}, beta={beta}");

    // if (legalMoves.Count == 0)
    // {
    //     return EvaluateBoard(board);
    // }

    if (maximizingAI)
    {
        List<Move> legalMoves = LegalMoves(maximizingAI);
        Debug.Log("count of legal moves of simulated ai: " + legalMoves.Count);
        Debug.Log("legal moves are:" + legalMoves);
        if (legalMoves.Count == 0)
    {
        return EvaluateBoard(board);
    }
        float bestScore = float.NegativeInfinity;

        foreach (Move m in legalMoves)
        {
            Vector2Int original = GameManager.instance.GridForPiece(m.piece);

            GameManager.instance.Move(m.piece, m.position);

            float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, false);

            GameManager.instance.Move(m.piece, original);
            

            if (score > bestScore)
            {
                Debug.Log("score is greater than best score");
                bestScore = score;

                if (depth == 2)
                {
                    bestMoves.Clear();
                    bestMoves.Add(m);
                    Debug.Log("Added best move at maxPly");
                }
            }

            if (bestScore > alpha)
            {
                alpha = bestScore;
            }

            if (alpha >= beta)
            {
                Debug.Log($"PRUNE? alpha={alpha}, beta={beta}");
                break;
            }
        }

        return bestScore;
    }
    else
    {

        List<Move> legalMoves = LegalMoves(maximizingAI);
        Debug.Log("count of legal moves of simulated player: " + legalMoves.Count);
        Debug.Log("legal moves are:" + legalMoves);
        if (legalMoves.Count == 0)
    {
        return EvaluateBoard(board);
    }
        float bestScore = float.PositiveInfinity;

        foreach (Move m in legalMoves)
        {
            Vector2Int original = GameManager.instance.GridForPiece(m.piece);

            GameManager.instance.Move(m.piece, m.position);

            float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, true);

            GameManager.instance.Move(m.piece, original);

            if (score < bestScore)
            {
                bestScore = score;
            }

            if (bestScore < beta)
            {
                beta = bestScore;
            }

            if (alpha >= beta)
            {
                Debug.Log($"PRUNE? alpha={alpha}, beta={beta}");
                break;
            }
        }

        return bestScore;
    }
}

   List<Move> LegalMoves(bool isAI)
   {
    Debug.Log("start legal moves");
	List<Move> legalMoves = new List<Move>();
    Debug.Log(GameManager.instance.pieces.GetLength(1));
    
	foreach (GameObject piece in GameManager.instance.pieces)
    {
            
            if(!GameManager.instance.DoesPieceBelongToCurrentPlayer(piece, isAI))
            {
                continue;
            }

            Debug.Log("getting legal moves");
            // if(piece.Equals(null) || GameManager.instance.GridForPiece(piece).Equals(null))
            // {
            //     continue;
            // }
            // Move undo = new Move(piece, GameManager.instance.GridForPiece(piece));
            // Debug.Log("got og position");
            
            // if(undoSimulation!=null)
            // {
            //     Debug.Log("undoSimulation is not null");
            //     undoSimulation.Add(undo);
            //     Debug.Log("added it to undo");
            // }
            

            List<Vector2Int> positions = new List<Vector2Int>(); 
            positions = GameManager.instance.MovesForPiece(piece);
            Debug.Log(positions.Count);
            foreach (Vector2Int pos in positions)
            {
                Debug.Log("getting moves for each piece for each position ");
                if(!piece.Equals(null))
                {
                    Move move = new Move(piece, pos);
                    if(legalMoves!=null)
                    {
                        legalMoves.Add(move);
                    }
                }
                


            }
    }
        Debug.Log("finished legalMoves");
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
              if(GameManager.instance.DoesPieceBelongToCurrentPlayer(piece, false))
              {
                score += value;
              }

              else
              {
                score-= value;
              }
           }


           else
           {
               if(GameManager.instance.DoesPieceBelongToCurrentPlayer(piece, true))
              {
                score+= value;
              }

              else
              {
                score-= value;
              }
               
               
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
    // bestMoves.Clear();
    Minimax(GameManager.instance.board, 2, float.NegativeInfinity, float.PositiveInfinity, true);
    Debug.Log("getting best move" + " pawn forward value:" + forward);
    // if(bestMoves==null)
    // {
    //     Debug.Log("bestMoves is null");
    //     return;
    // }

    
    Debug.Log("minimax done");
    if(undoSimulation!= null)
    {
        Debug.Log("undoing simulation");
    foreach(Move m in undoSimulation)
    {
        if(m.piece == null)
        {
            continue;
        }
        GameManager.instance.Move(m.piece, m.position);
        }
    }
    Debug.Log("simulated moves undone");

    // List<Move> tempMoves = bestMoves;
    // bestMoves.Clear();
    // Debug.Log("bestMoves cleared");
    
    Debug.Log("count of best moves is: " + bestMoves.Count);

    
    // if (!moveLocations.Contains(tempMoves[0].position))
    // {
    //     return;
    //    }

        // if (GameManager.instance.PieceAtGrid(tempMoves[0].position) == null)
        if (GameManager.instance.PieceAtGrid(bestMoves[0].position) == null)
        {
            Debug.Log("going to do best move ai without capture");
            // GameManager.instance.Move(tempMoves[0].piece, tempMoves[0].position);
            GameManager.instance.Move(bestMoves[0].piece, bestMoves[0].position);
            

        }   
        
        else
        {
            Debug.Log("capturing piece");
            // GameManager.instance.CapturePieceAt(tempMoves[tempMoves.Count -1].position);
            // GameManager.instance.Move(tempMoves[tempMoves.Count -1].piece, tempMoves[tempMoves.Count -1].position);
            GameManager.instance.CapturePieceAt(bestMoves[bestMoves.Count -1].position);
            GameManager.instance.Move(bestMoves[bestMoves.Count -1].piece, bestMoves[bestMoves.Count -1].position);
        }
        
    // return tempMoves[0];

    
}


}


















// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;




// public class ChessAI : MonoBehaviour
// {
//   public static ChessAI inst;
 
//   public List<GameObject> pieces;
//   public List<GameObject> capturedPieces;
//   public List<Move> bestMoves;
//   public List<Move> undoSimulation;
 
//   public int maxPly = 1;
//   public int ply;
// //    public GameManager manager;
//   public string name;
//   public int forward;




//   public struct Move
//   {
//    public Vector2Int position;
//    public GameObject piece;
  
//    public Move(GameObject chessPiece, Vector2Int pos)
//    {
//        position = pos;
//        piece = chessPiece;
// }
//   }




 








// // Start is called before the first frame update
//   void Start()
//   {
  
  
//    //GetBestMove();
//   }




//   // Update is called once per frame
//   void Update()




//   {
    
//   }








// public ChessAI(string name, bool positiveZMovement)
// {
//       this.name = name;
//       pieces = new List<GameObject>();
//       capturedPieces = new List<GameObject>();
//       List<Move> undoSimulation = new List<Move>();




//       if (positiveZMovement == true)
//       {
//           this.forward = 1;
//       }
//       else
//       {
//           this.forward = -1;
//       }
// }
















//   float Minimax(Board board, float depth, bool maximizingPlayer)
//   {
//     Debug.Log("minimax" + depth);
//        if (depth == 0)
//        {   
//            Debug.Log("going to evaluate board");
//            return EvaluateBoard(board);
//        }   //base case
//    List<Move> legalMoves = new List<Move>();
//    Debug.Log("elo1");
//    legalMoves = LegalMoves();
//    Debug.Log(legalMoves.Count);
//       if (maximizingPlayer)
//       {
//           float bestScore = float.NegativeInfinity;
//           foreach (Move m in legalMoves)
//         {
//           GameManager.instance.Move(m.piece, m.position); //simulate move
//           bestMoves.Add(m);


         
//            float score = Minimax(GameManager.instance.board, depth-1, false);
//           if(bestScore > score)
//           {
//            bestScore = score;
//            }
//            else
//            {
//                bestMoves.Remove(m);
//            }
          
//        }
//           return bestScore;
//       }
//       else
//        {
//           float bestScore = float.PositiveInfinity;
//         //ArrayList<move> legalMoves = LegalMoves();
//           foreach (Move m in legalMoves)
//         {
//               GameManager.instance.Move(m.piece, m.position); //simulate move
         
//               float score = Minimax(GameManager.instance.getBoard(), depth-1, true);
//           if(bestScore < score)
//           {
//            bestScore = score;
//            }
      
//         }
//          return bestScore;




//       }
      
     
//   }




//   List<Move> LegalMoves()
//   {
//    Debug.Log("elo2");
//    List<Move> legalMoves = new List<Move>();
//    Debug.Log(GameManager.instance.pieces.GetLength(0));
  
//    foreach (GameObject piece in GameManager.instance.pieces)
//    {
          
//            Debug.Log("getting legal moves");
//            // if(piece.Equals(null) || GameManager.instance.GridForPiece(piece).Equals(null))
//            // {
//            //     continue;
//            // }
//            Move undo = new Move(piece, GameManager.instance.GridForPiece(piece));
//            Debug.Log("got to og position");
          
//            if(undoSimulation!=null)
//            {
//                undoSimulation.Add(undo);
//            }
//            Debug.Log("added it to undo");


//            List<Vector2Int> positions = new List<Vector2Int>();
//            positions = GameManager.instance.MovesForPiece(piece);
//            Debug.Log(positions.Count);
//            foreach (Vector2Int pos in positions)
//            {
//                Debug.Log("elo4");
//                if(!piece.Equals(null) && !pos.Equals(null))
//                {
//                    Move move = new Move(piece, pos);
//                    Debug.Log("elo5");
//                    if(legalMoves!=null)
//                    {
//                        legalMoves.Add(move);
//                    }
//                }
//                Debug.Log("elo6");




//            }
//    }
// Debug.Log("elo7 finished legalMoves");
// return legalMoves;
//   }




//   float EvaluateBoard(Board board)
//   {
//       int score = 0;




//       foreach(GameObject piece in GameManager.instance.pieces)
//       {
//           int value = GetPieceValue(piece);
//            Debug.Log("got piece value");


//           if(GameManager.instance.getIsPlayer())
//           {
//              score -= value;
//           }




//           else
//           {
//               score += value;
//           }
//       }
//       return score;




//   }




// int GetPieceValue(GameObject piece)
// {
//    Debug.Log("getting piece value");
//    if(piece == null)
//    {
//        return 0;
//    }
//    Piece pieceComponent = piece.GetComponent<Piece>();
// switch(pieceComponent.type)
// {
//    case PieceType.Pawn:
//    return 1;








// case PieceType.Bishop:
//    return 3;








// case PieceType.Knight:
//    return 3;








// case PieceType.King:
//    return 674000;








// case PieceType.Queen:
//    return 9;








// case PieceType.Rook:
//    return 5;




// default:
//                return 0;
  
// }
// }


// public void BestMove()
// {
//    Minimax(GameManager.instance.board, 1, false);
//    Debug.Log("getting best move");
//    if(bestMoves==null)
//    {
//        return;
//    }


  
//    Debug.Log("minimax done");
//    if(undoSimulation!= null)
//    {
//        Debug.Log("undoing simulation");
//    foreach(Move m in undoSimulation)
//    {
//        if(m.piece == null || m.position == null)
//        {
//            continue;
//        }
//        GameManager.instance.Move(m.piece, m.position);
//        }
//    }
//    Debug.Log("similated moves undone");


//    List<Move> tempMoves = bestMoves;
//    bestMoves.Clear();
//    Debug.Log("bestMoves cleared");
  
  
//    // if (!moveLocations.Contains(tempMoves[0].position))
//    // {
//    //     return;
//    //    }


//        if (GameManager.instance.PieceAtGrid(tempMoves[0].position) == null)
//        {
//            Debug.Log("going to do best move ai");
//            GameManager.instance.Move(tempMoves[0].piece, tempMoves[0].position);
          


//        }  
      
//        else
//        {
//            GameManager.instance.CapturePieceAt(tempMoves[0].position);
//            GameManager.instance.Move(tempMoves[0].piece, tempMoves[0].position);
//        }
//    // return tempMoves[0];


  
// }










// }








