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

        /// <summary>
        /// Piece Square Table (PST) contains adjustment values based on position of Piece on the board:
        /// map: ItemColor(White/Black) -> { map: PieceType -> int[8,8] array of adjustment values }
        /// To use, the BoardEvaluator uses the Piece's position to lookup an adjustment number from the table
        /// to add to the base value of a Piece
        /// </summary>
        private Dictionary<ItemColor, Dictionary<PieceType, int[,]>> _pieceSquareTable;

        public BoardEvaluator()
        {
            _pieceValue = new Dictionary<PieceType, int>();
            _pieceValue[PieceType.King] = 20000;
            _pieceValue[PieceType.Queen] = 900;
            _pieceValue[PieceType.Rook] = 500;
            _pieceValue[PieceType.Bishop] = 330;
            _pieceValue[PieceType.Knight] = 320;
            _pieceValue[PieceType.Pawn] = 100;

            CreatePST();
        }

        private void CreatePST()
        {
            _pieceSquareTable = new Dictionary<ItemColor, Dictionary<PieceType, int[,]>>()
            {
                {  ItemColor.Black, new Dictionary<PieceType, int[,]>() },
                {  ItemColor.White, new Dictionary<PieceType, int[,]>() }
            };

            _pieceSquareTable[ItemColor.White][PieceType.Pawn] = new int[8, 8]
            {
                { 0,  0,  0,  0,  0,  0,  0,  0 },
                { 5, 10, 10,-20,-20, 10, 10,  5 },
                { 5, -5,-10,  0,  0,-10, -5,  5 },
                { 0,  0,  0, 20, 20,  0,  0,  0 },
                { 5,  5, 10, 25, 25, 10,  5,  5 },
                { 10, 10, 20, 30, 30, 20, 10, 10 },
                { 50, 50, 50, 50, 50, 50, 50, 50 },
                { 0,  0,  0,  0,  0,  0,  0,  0 }
            };

            // PSTs for Black are the PSTs for White reflected on the X-axis
            foreach (var pieceType in _pieceSquareTable[ItemColor.White].Keys)
            {
                int[,] pst = _pieceSquareTable[ItemColor.White][pieceType];
                _pieceSquareTable[ItemColor.Black][pieceType] = new int[8, 8];
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        _pieceSquareTable[ItemColor.Black][pieceType][r, c] =
                            _pieceSquareTable[ItemColor.White][pieceType][7 - r, c];
                    }
                }
            }
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
                    boardVal = 30000;
                else
                    boardVal = -30000;
            }

            // TODO: better heuristics needed for computing the value of each piece
            // for example: location and movement of a piece needs to contribute to +ve
            // or -ve adjustments on base value of a piece

            foreach (var piece in playerPieces)
            {
                boardVal += _pieceValue[piece.PieceType];

                // adjust based on PST
                boardVal += LookupPST(piece);
            }
            foreach (var piece in opponentPieces)
            {
                boardVal -= _pieceValue[piece.PieceType];

                // adjust based on PST
                boardVal -= LookupPST(piece);
            }

            if (board.IsInCheck(playerColor))
            {
                boardVal -= 450;
            }
            else if (board.IsInCheck(opponentColor))
            {
                boardVal += 450;
            }

            return boardVal;
        }

        /// <summary>
        /// Looks up the Piece Square Table (PST) to return an adjustment of piece value based on Piece Color, Type
        /// and Position
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        private int LookupPST(Piece piece)
        {
            int pstAdj = 0;

            if (piece.Position.IsValid && _pieceSquareTable.ContainsKey(piece.PieceColor))
            {
                if (_pieceSquareTable[piece.PieceColor].ContainsKey(piece.PieceType))
                {
                    pstAdj = _pieceSquareTable[piece.PieceColor][piece.PieceType][piece.Position.Row, piece.Position.Column];
                }
            }

            return pstAdj;
        }
    }
}
