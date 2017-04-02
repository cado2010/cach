using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;

namespace cachCore.utils
{
    public class InCheckHelper
    {
        private Board _board;
        private ItemColor _pieceColor;
        private ItemColor _enemyColor;

        public InCheckHelper(Board board, ItemColor pieceColor)
        {
            _board = board;
            _pieceColor = pieceColor;
            _enemyColor = pieceColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;

            InCheck = ComputeInCheck();
        }

        public bool InCheck { get; private set; }

        /// <summary>
        /// Computes if the King of the given PieceColor is in Check - can be optimized by analyzing individual
        /// paths instead of forming groups of paths
        /// </summary>
        /// <returns></returns>
        private bool ComputeInCheck()
        {
            // radiate out in all directions from current King pos and check if
            // under attack
            King king = _board.GetPieces(_pieceColor, PieceType.King)[0] as King;

            List<IList<Position>> paths = new List<IList<Position>>();

            // check straight paths for Queen or Rook
            paths.Add(king.GetLeftPath());
            paths.Add(king.GetRightPath());
            paths.Add(king.GetUpPath());
            paths.Add(king.GetDownPath());
            if (CheckPathForEnemyPieceType(PieceType.Queen, paths) ||
                CheckPathForEnemyPieceType(PieceType.Rook, paths))
            {
                return true;
            }

            paths.Clear();

            // check diagonal paths for Queen or Bishop
            paths.Add(king.GetLeftUpPath());
            paths.Add(king.GetRightPath());
            paths.Add(king.GetUpPath());
            paths.Add(king.GetDownPath());
            if (CheckPathForEnemyPieceType(PieceType.Queen, paths) ||
                CheckPathForEnemyPieceType(PieceType.Bishop, paths))
            {
                return true;
            }

            // create a temporary Knight at the King position to check attack from an enemy Knight
            Knight knight = new Knight(_enemyColor, king.Position, isTemp: true);

            Movement m = knight.GetMovement();
            if (CheckPathForEnemyPieceType(PieceType.Knight, m.Paths))
            {
                return true;
            }

            // finally, check attack from Pawns
            if (_pieceColor == ItemColor.White)
            {
                Position p0 = king.Position.LeftUp;
                Position p1 = king.Position.RightUp;
                if (!p0.IsOutOfBounds() && _board[p0].IsOccupiedByPieceOfColor(ItemColor.Black) ||
                    !p1.IsOutOfBounds() && _board[p1].IsOccupiedByPieceOfColor(ItemColor.Black))
                {
                    return true;
                }
            }
            else
            {
                Position p0 = king.Position.LeftDown;
                Position p1 = king.Position.RightDown;
                if (!p0.IsOutOfBounds() && _board[p0].IsOccupiedByPieceOfColor(ItemColor.White) ||
                    !p1.IsOutOfBounds() && _board[p1].IsOccupiedByPieceOfColor(ItemColor.White))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks the given path for an enemy of the given type
        /// </summary>
        /// <param name="pieceType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckPathForEnemyPieceType(PieceType pieceType, IList<IList<Position>> paths)
        {
            foreach (var path in paths)
            {
                foreach (var pos in path)
                {
                    if (_board[pos].IsOccupiedByPieceOfColor(_pieceColor))
                    {
                        // break out of this path and check next path
                        break;
                    }

                    if (_board[pos].IsOccupiedByPieceOfColor(_enemyColor))
                    {
                        Piece p = _board[pos].Piece;
                        if (p.PieceType == pieceType)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
