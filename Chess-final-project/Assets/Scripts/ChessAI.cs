using System.Collections.Generic;
using UnityEngine;

public class ChessAI: MonoBehaviour
{
    public static ChessAI inst;

    public List<GameObject> pieces;
    public List<GameObject> capturedPieces;
    public List<Move> bestMoves;
    public List<Move> undoSimulation;
    public int maxPly;


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

    void Awake()
    {
        bestMoves = new List<Move>();
        capturedPieces = new List<GameObject>();
        undoSimulation = new List<Move>();
        pieces = new List<GameObject>();
        maxPly =1;
        inst = GetComponent<ChessAI>();
    }

    float Minimax(Board board, int depth, float alpha, float beta, bool maximizingAI)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board);
        }

        List<Move> legalMoves = LegalMoves(maximizingAI);

        if (legalMoves.Count == 0)
        {
            return EvaluateBoard(board);
        }

        if (maximizingAI)
        {
            float bestScore = float.NegativeInfinity;

            foreach (Move m in legalMoves)
            {
                Vector2Int original = GameManager.instance.GridForPiece(m.piece);

                GameManager.instance.Move(m.piece, m.position);

                float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, false);

                GameManager.instance.Move(m.piece, original);

                if (score > bestScore)
                {
                    bestScore = score;

                    if (depth == maxPly)
                    {
                        bestMoves.Clear();
                        bestMoves.Add(m);
                    }
                }

                if (bestScore > alpha)
                {
                    alpha = bestScore;
                }

                if (alpha >= beta)
                {
                    break;
                }
            }

            return bestScore;
        }

        else
        {
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
                    break;
                }
            }

            return bestScore;
        }
    }



    List<Move> LegalMoves(bool ifAI)
    {
        List<Move> legalMoves = new List<Move>();
        return new List<Move>();
    }



    float EvaluateBoard(Board board)
  {
      int score = 0;

      foreach(GameObject piece in GameManager.instance.pieces)
      {
          int value = GetPieceValue(piece);
           Debug.Log("got piece value");


        //   if(GameManager.instance.getIsPlayer())
        //   {
        //      if(GameManager.instance.DoesPieceBelongToCurrentPlayer(piece))
        //      {
        //        score += value;
        //      }


        //      else
        //      {
        //        score-= value;
        //      }
        //   }

    //       else
    //       {
    //           if(GameManager.instance.DoesPieceBelongToCurrentPlayer(piece))
    //          {
    //            score+= value;
    //          }


    //          else
    //          {
    //            score-= value;
    //          }
             
             
    //       }
    //   }

            if(pieces.Contains(piece))
            {
                score += value;
            }

            else
            {
                score -= value;
            }

        }
        return score;

    }



    int GetPieceValue(GameObject piece)
    {
   
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



    public Move BestMove()
    {
        bestMoves.Clear();
        Minimax(GameManager.instance.board, maxPly, float.NegativeInfinity, float.PositiveInfinity, true);
        return bestMoves[0];
    }
}