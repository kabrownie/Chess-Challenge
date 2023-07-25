
using ChessChallenge.API;
using System;

namespace ChessChallenge.Application
{

    public class MyBot : IChessBot
    {
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();

            // Pick a random move to play if nothing better is found
            Random rng = new();
            Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
            int highestValueCapture = 0;

            foreach (Move move in allMoves)
            {
                // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    moveToPlay = move;
                    break;
                }

            //     // Find highest value capture
            //     Piece capturedPiece = board.GetPiece(move.TargetSquare);
            //     int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];

            //     if (capturedPieceValue > highestValueCapture)
            //     {
            //         moveToPlay = move;
            //         highestValueCapture = capturedPieceValue;
            //     }
            // }

            // Find highest value capture that doesn't lead to a disadvantaged exchange
Piece capturedPiece = board.GetPiece(move.TargetSquare);
int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
Piece movingPiece = board.GetPiece(move.StartSquare);
int movingPieceValue = pieceValues[(int)movingPiece.PieceType];

// Check if the captured piece is defended by a higher-value piece
bool isDefendedByHigherValuePiece = false;
foreach (Move opponentMove in board.GetLegalMoves(board.GetOpponentPlayer()))
{
     if (opponentMove.TargetSquare == move.TargetSquare)
    {
        Piece defenderPiece = board.GetPiece(opponentMove.StartSquare);
        int defenderPieceValue = pieceValues[(int)defenderPiece.PieceType];

        if (defenderPieceValue > movingPieceValue)
        {
            isDefendedByHigherValuePiece = true;
            break;
        }
    }
}
 
if (capturedPieceValue > movingPieceValue || isDefendedByHigherValuePiece)
{
    // This is a disadvantageous exchange or the captured piece is defended by a higher-value piece, consider other moves
    continue;
}
if (capturedPieceValue > highestValueCapture)
{
    // This capture is not disadvantageous, and it has a higher value than previous captures
    moveToPlay = move;
    highestValueCapture = capturedPieceValue;
}

            }
            return moveToPlay;
            }

        // Test if this move gives checkmate
       private bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
    }
}
//TODO
//dont capture if the more value  piece will be capture by less value piece]
//move away a larger value to where iit is not attacked by smaller value
//avoid basic schooler mates
//learn basic strategy fork pin etc