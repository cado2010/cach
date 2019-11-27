using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;
using cachCore.utils;

namespace cacheEngine
{
    class BoardEvaluator
    {
        public static int MaxVal = Int32.MaxValue;
        public static int MinVal = Int32.MinValue;

        private Dictionary<PieceType, int> _pieceValue;

        public BoardEvaluator()
        {
            _pieceValue = new Dictionary<PieceType, int>();
            _pieceValue[PieceType.King] = 10000;
            _pieceValue[PieceType.Queen] = 1000;
            _pieceValue[PieceType.Rook] = 525;
            _pieceValue[PieceType.Bishop] = 350;
            _pieceValue[PieceType.Knight] = 350;
            _pieceValue[PieceType.Pawn] = 100;
        }

        /// <summary>
        /// Primary Board eval function used by the Engine
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerColor"></param>
        /// <returns></returns>
        public int Evaluate(Board board, ItemColor playerColor)
        {
            int boardVal = 0;
            ItemColor opponentColor = BoardUtils.GetOtherColor(playerColor);

            IList<Piece> playerPieces = board.GetAllActivePieces(playerColor);
            IList<Piece> opponentPieces = board.GetAllActivePieces(opponentColor);

            if (board.IsCheckMate)
            {
                if (board.Winner == playerColor)
                    boardVal = 20000;
                else
                    boardVal = -20000;
            }

            // TODO: better heuristics needed for computing the value of each piece
            // for example: location and movement of a piece needs to contribute to +ve
            // or -ve adjustments on base value of a piece

            foreach (var piece in playerPieces)
            {
                boardVal += _pieceValue[piece.PieceType];
            }
            foreach (var piece in opponentPieces)
            {
                boardVal -= _pieceValue[piece.PieceType];
            }

            //if (board.IsInCheck(playerColor))
            //{
            //    boardVal -= 5000;
            //}
            //else if (board.IsInCheck(opponentColor))
            //{
            //    boardVal += 5000;
            //}

            return boardVal;
        }
    }
}
