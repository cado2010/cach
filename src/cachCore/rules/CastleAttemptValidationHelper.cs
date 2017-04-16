using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;
using cachCore.utils;

namespace cachCore.rules
{
    /// <summary>
    /// Helper that implements rules to determine if a Castle attempt is possible
    /// </summary>
    public class CastleAttemptValidationHelper
    {
        private Board _board;
        private ItemColor _pieceColor;
        private ItemColor _enemyColor;
        private bool _isKingSideCastle;
        private Position _kingPositionAfterCastle;
        private Position _rookPositionAfterCastle;

        public CastleAttemptValidationHelper(Board board, ItemColor pieceColor, bool isKingSideCastle)
        {
            _board = board;
            _pieceColor = pieceColor;
            _enemyColor = pieceColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;
            _isKingSideCastle = isKingSideCastle;

            CanCastle = ComputeCanCastle();
        }

        public bool CanCastle { get; private set; }

        public King King { get; private set; }

        public Rook Rook { get; private set; }

        public Position KingPositionAfterCastle { get; private set; }

        public Position RookPositionAfterCastle { get; private set; }

        /// <summary>
        /// rules:
        /// 1. Your king has been moved earlier in the game.
        /// 2. The rook that castles has been moved earlier in the game.
        /// 3. There are pieces standing between your king and rook.
        /// 4. The king is in check.
        /// 5. The king moves through a square that is attacked by a piece of the opponent.
        /// 6. The king would be in check after castling.
        /// </summary>
        /// <returns></returns>
        private bool ComputeCanCastle()
        {
            KingPositionAfterCastle = Position.Invalid;
            RookPositionAfterCastle = Position.Invalid;

            King king = _board.GetActivePieces(_pieceColor, PieceType.King)[0] as King;

            // 1.
            if (king.HasMoved)
            {
                return false;
            }

            int row = BoardUtils.GetPieceStartRow(_pieceColor);
            int rookCol;
            List<Position> checkPositions;
            List<Position> mustBeEmptyPositions;
            if (_isKingSideCastle)
            {
                rookCol = 7;
                Position kpr = king.Position.Right;
                Position kprr = kpr.Right;
                checkPositions = new List<Position>()
                {
                    king.Position,
                    kpr,
                    kprr
                };
                mustBeEmptyPositions = new List<Position>()
                {
                    kpr,
                    kprr
                };

                _kingPositionAfterCastle = kprr;
                _rookPositionAfterCastle = kpr;
            }
            else
            {
                // Queen side castle
                rookCol = 0;
                Position kpl = king.Position.Left;
                Position kpll = kpl.Left;
                checkPositions = new List<Position>()
                {
                    king.Position,
                    kpl,
                    kpll,
                };
                mustBeEmptyPositions = new List<Position>()
                {
                    kpl,
                    kpll,
                    king.Position.Left.Left.Left,
                };

                _kingPositionAfterCastle = kpll;
                _rookPositionAfterCastle = kpl;
            }
            Position rookPos = new Position(row, rookCol);

            // 2.
            BoardSquare rookSquare = _board[rookPos];
            if (!rookSquare.IsOccupiedByPieceOfColorAndType(_pieceColor, PieceType.Rook))
            {
                return false;
            }
            if (rookSquare.Piece.HasMoved)
            {
                return false;
            }

            // 3.
            foreach (var pos in mustBeEmptyPositions)
            {
                if (_board[pos].IsOccupied())
                {
                    return false;
                }
            }

            // 4., 5., 6.
            foreach (var pos in checkPositions)
            {
                var checkHelper = new InCheckHelper(_board, _pieceColor, pos);
                if (checkHelper.IsInCheck)
                {
                    return false;
                }
            }

            // the King can Castle - setup various helper members and return true
            King = king;
            Rook = rookSquare.Piece as Rook;
            KingPositionAfterCastle = _kingPositionAfterCastle;
            RookPositionAfterCastle = _rookPositionAfterCastle;
            return true;
        }
    }
}
