using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        return GetBestMove(board);
    }

    int[] pieceValues = { 0, 100, 300, 300, 500, 900,10000 };

    Move GetBestMove(Board board)
    {
        float bestScore = board.IsWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];

        foreach (Move move in moves)
        {

            
 // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    bestMove = move;
                    break;
                }
// Test if this move gives checkmate
    

    bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
            board.MakeMove(move); // Make the move on the board
            float score = Minimax(board, move, 3, float.NegativeInfinity, float.PositiveInfinity, !board.IsWhiteToMove);
            board.UndoMove(move); // Undo the move after evaluation
            
            if (board.IsWhiteToMove)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }

        return bestMove;
    }


    float evaluate(Board board, Move move)
    {
        PieceList[] pieceLists = board.GetAllPieceLists();
        float eval = 0f;

        // Rewards for piece values (pawns, knights, bishops, rooks, queens)
        for (int i = 0; i < pieceLists.Length; i++)
        {
            if (i <= 5)
            {
                eval += pieceValues[i % 6] * pieceLists[i].Count;
            }
            else
            {
                eval -= pieceValues[i % 6] * pieceLists[i].Count;
            }
        }

        // Additional rewards for specific piece positions
        foreach (var pieceList in pieceLists)
        {
            foreach (var piece in pieceList)
            {
                if (piece.IsPawn)
                {
                    // Pawns get rewarded for moving down the board
                    int pawnRow = piece.IsWhite ? piece.Square.Rank : 7 - piece.Square.Rank;
                    eval += 10 * pawnRow;
                }
                else if (piece.IsKnight || piece.IsBishop)
                {
                    // Knights and bishops get rewarded for being in the center (e4, e5, d4, d5)
                    int centerDistance = Math.Min(Math.Min(piece.Square.Rank, 7 - piece.Square.Rank),
                                                  Math.Min(piece.Square.File, 7 - piece.Square.File));
                    eval += piece.IsKnight ? 15 * centerDistance : 10 * centerDistance;

                    // Extra punishment for knights on the edge of the board
                    if (piece.IsKnight && (centerDistance == 3 || centerDistance == 4))
                    {
                        eval -= 50;
                    }
                }
            }
        }

        // Reward the king for being near the edge when there are many pieces on the board
        int totalPieceCount = pieceLists.Sum(pl => pl.Count);
        int kingEdgeDistance = Math.Min(Math.Min(pieceLists[10][0].Square.Rank, 7 - pieceLists[10][0].Square.Rank),
                                        Math.Min(pieceLists[10][0].Square.File, 7 - pieceLists[10][0].Square.File));
        eval += 10 * kingEdgeDistance * (totalPieceCount / 16f);

       

        return eval;
    }

    float Minimax(Board board, Move move, int depth, float alpha, float beta, bool isMaximizing)
{
    if (depth == 0)
        return evaluate(board, move);

    Move[] moves = board.GetLegalMoves();

    if (isMaximizing)
    {
        float maxScore = float.NegativeInfinity;
        foreach (Move childMove in moves)
        {
            board.MakeMove(childMove); // Make the move on the board
            float childScore = Minimax(board, childMove, depth - 1, alpha, beta, false);
            board.UndoMove(childMove); // Undo the move after evaluation

            maxScore = Math.Max(maxScore, childScore);
            alpha = Math.Max(alpha, childScore);
            if (beta <= alpha)
                break;
        }
        return maxScore;
    }
    else // Minimizing player (black's turn)
    {
        float minScore = float.PositiveInfinity;
        foreach (Move childMove in moves)
        {
            board.MakeMove(childMove); // Make the move on the board
            float childScore = Minimax(board, childMove, depth - 1, alpha, beta, true);
            board.UndoMove(childMove); // Undo the move after evaluation

            minScore = Math.Min(minScore, childScore);
            beta = Math.Min(beta, childScore);
            if (beta <= alpha)
                break;
        }
        return minScore;
    }
}
}