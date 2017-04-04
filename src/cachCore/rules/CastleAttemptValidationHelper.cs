using System.Collections.Generic;
using cachCore.enums;
using cachCore.exceptions;
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
        private MoveInputParser _moveInput;

        public CastleAttemptValidationHelper(Board board, ItemColor pieceColor, MoveInputParser moveInput)
        {
            if (!_moveInput.IsKingSideCastle && !_moveInput.IsQueenSideCastle)
            {
                throw new CachException("Move input does not specify a Castle attempt");
            }

            _board = board;
            _pieceColor = pieceColor;
            _moveInput = moveInput;
            _enemyColor = pieceColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;

            CanCastle = ComputeCanCastle();
        }

        public bool CanCastle { get; private set; }

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
            King king = _board.GetPieces(_pieceColor, PieceType.King)[0] as King;

            // 1.
            if (king.HasMoved)
            {
                return false;
            }

            int row = BoardUtils.GetPieceStartRow(_pieceColor);
            int rookCol;
            List<Position> checkPositions;
            List<Position> mustBeEmptyPositions;
            if (_moveInput.IsKingSideCastle)
            {
                rookCol = 7;
                checkPositions = new List<Position>()
                {
                    king.Position,
                    king.Position.Right,
                    king.Position.Right.Right,
                };
                mustBeEmptyPositions = new List<Position>()
                {
                    king.Position.Right,
                    king.Position.Right.Right,
                };
            }
            else
            {
                rookCol = 0;
                checkPositions = new List<Position>()
                {
                    king.Position,
                    king.Position.Left,
                    king.Position.Left.Left,
                };
                mustBeEmptyPositions = new List<Position>()
                {
                    king.Position.Left,
                    king.Position.Left.Left,
                    king.Position.Left.Left.Left,
                };
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

            // the King can Castle
            return true;
        }
    }
}
