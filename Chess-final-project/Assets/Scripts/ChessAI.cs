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
        maxPly = 3;
        inst = GetComponent<ChessAI>();
    }

    public List<GameObject> GetPiecesAI()
    {
        return pieces;
    }

    public List<GameObject> GetCapturedPiecesAI()
    {
        return capturedPieces;
    }

    public string GetName()
    {
        return "AI";
    }

    float Minimax(Board board, int depth, float alpha, float beta, bool maximizingAI)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board);
        }

        List<Move> legalMoves = LegalMoves(maximizingAI);
        Debug.Log($"depth={depth}, maximizing={maximizingAI}, alpha={alpha}, beta={beta}, legalMoves={legalMoves.Count}");

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
                if(GameManager.instance.PieceAtGrid(m.position) != false)
                {
                    GameObject ogPiece = GameManager.instance.PieceAtGrid(m.position);

                    GameManager.instance.Move(m.piece, m.position);
                    float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, false);

                    GameManager.instance.Move(m.piece, original);
                    GameManager.instance.GetPiecesGM()[m.position.x, m.position.y] = ogPiece;

                    if (score > bestScore)
                    {
                        bestScore = score;

                        if (depth == maxPly)
                        {
                            bestMoves.Clear();
                            bestMoves.Add(m);
                        }
                    }
                }

                else
                {
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
            float bestScore = float.PositiveInfinity;

            foreach (Move m in legalMoves)
            {
                Vector2Int original = GameManager.instance.GridForPiece(m.piece);

                if(GameManager.instance.PieceAtGrid(m.position) != false)
                {
                    GameObject ogPiece = GameManager.instance.PieceAtGrid(m.position);

                    GameManager.instance.Move(m.piece, m.position);
                    float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, true);

                    GameManager.instance.Move(m.piece, original);
                    GameManager.instance.GetPiecesGM()[m.position.x, m.position.y] = ogPiece;

                    if (score < bestScore)
                    {
                        bestScore = score;
                    }

                }

                else
                {
                    GameManager.instance.Move(m.piece, m.position);
                    float score = Minimax(GameManager.instance.board, depth - 1, alpha, beta, true);

                    GameManager.instance.Move(m.piece, original);

                    if (score < bestScore)
                    {
                        bestScore = score;
                    }
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
        List<Move> legalMoves = new List<Move>();
        if(isAI)
        {
            Debug.Log("The number of ai pieces is " + pieces.Count);
            foreach(GameObject piece in GameManager.instance.GetPiecesGM())
            {
                if(pieces.Contains(piece))
                {
                    Debug.Log("The number of positions this ai piece can move is:" + GameManager.instance.MovesForPiece(piece).Count);
                    foreach(Vector2Int pos in GameManager.instance.MovesForPiece(piece))
                    {
                        legalMoves.Add(new Move(piece,pos));
                    }
                }
            }
        }

        else
        {
            foreach(GameObject piece in GameManager.instance.GetPiecesGM())
            {
                if(GameManager.instance.GetPlayer().pieces.Contains(piece))
                {
                    foreach(Vector2Int pos in GameManager.instance.MovesForPiece(piece))
                    {
                        legalMoves.Add(new Move(piece,pos));
                    }
                }

            }
        }

        return legalMoves;

    }



    float EvaluateBoard(Board board)
  {
      int score = 0;

      foreach(GameObject piece in GameManager.instance.pieces)
      {
          int value = GetPieceValue(piece);


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



    public void BestMove()
    {
        bestMoves.Clear();
        float score = Minimax(GameManager.instance.board, maxPly, float.NegativeInfinity, float.PositiveInfinity, true);
        Debug.Log("" + GameManager.instance.PieceAtGrid(bestMoves[0].position));

        if (GameManager.instance.PieceAtGrid(bestMoves[0].position) == null)
        {
            Debug.Log("ai does not capture and best move to: x: " + bestMoves[0].position.x + " and y: " + bestMoves[0].position.y);
            GameManager.instance.Move(bestMoves[0].piece, bestMoves[0].position);
            Score.instan.ScoreUpdate(score);
        }
        else
        {
            Debug.Log("ai captures and best move to: x: " + bestMoves[0].position.x + " and y: " + bestMoves[0].position.y);
            GameManager.instance.CapturePieceAt(bestMoves[0].position);
            GameManager.instance.Move(bestMoves[0].piece, bestMoves[0].position);
            Score.instan.ScoreUpdate(score);
        }
    }
}