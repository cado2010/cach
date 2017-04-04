﻿using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;

namespace cachCore.utils
{
    /// <summary>
    /// Helper that implements rules to determine if the King of given color and position is in Check
    /// </summary>
    public class InCheckHelper
    {
        private Board _board;
        private ItemColor _pieceColor;
        private ItemColor _enemyColor;
        private Position _kingPosition;

        public InCheckHelper(Board board, ItemColor pieceColor, Position kingPosition)
        {
            _board = board;
            _pieceColor = pieceColor;
            _kingPosition = kingPosition;
            _enemyColor = pieceColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;

            IsInCheck = ComputeInCheck();
        }

        public bool IsInCheck { get; private set; }

        public Piece Attacker { get; private set; }

        /// <summary>
        /// Computes if the King of the given PieceColor is in Check - can be optimized by analyzing individual
        /// paths instead of forming groups of paths
        /// </summary>
        /// <returns></returns>
        private bool ComputeInCheck()
        {
            // radiate out in all directions from current King pos and check if
            // under attack

            List<IList<Position>> paths = new List<IList<Position>>();

            // check straight paths for Queen or Rook
            paths.Add(_kingPosition.GetLeftPath());
            paths.Add(_kingPosition.GetRightPath());
            paths.Add(_kingPosition.GetUpPath());
            paths.Add(_kingPosition.GetDownPath());
            if (CheckPathForEnemyPieceType(PieceType.Queen, paths) ||
                CheckPathForEnemyPieceType(PieceType.Rook, paths))
            {
                return true;
            }

            paths.Clear();

            // check diagonal paths for Queen or Bishop
            paths.Add(_kingPosition.GetLeftUpPath());
            paths.Add(_kingPosition.GetRightPath());
            paths.Add(_kingPosition.GetUpPath());
            paths.Add(_kingPosition.GetDownPath());
            if (CheckPathForEnemyPieceType(PieceType.Queen, paths) ||
                CheckPathForEnemyPieceType(PieceType.Bishop, paths))
            {
                return true;
            }

            // create a temporary Knight at the King position to check attack from an enemy Knight
            Knight knight = new Knight(_enemyColor, _kingPosition, isTemp: true);

            Movement m = knight.GetMovement();
            if (CheckPathForEnemyPieceType(PieceType.Knight, m.Paths))
            {
                return true;
            }

            // finally, check attack from Pawns
            if (_pieceColor == ItemColor.White)
            {
                Position p0 = _kingPosition.LeftUp;
                Position p1 = _kingPosition.RightUp;
                if (!p0.IsOutOfBounds() && _board[p0].IsOccupiedByPieceOfColor(ItemColor.Black))
                {
                    Attacker = _board[p0].Piece;
                    return true;
                }
                else if (!p1.IsOutOfBounds() && _board[p1].IsOccupiedByPieceOfColor(ItemColor.Black))
                {
                    Attacker = _board[p1].Piece;
                    return true;
                }
            }
            else
            {
                Position p0 = _kingPosition.LeftDown;
                Position p1 = _kingPosition.RightDown;
                if (!p0.IsOutOfBounds() && _board[p0].IsOccupiedByPieceOfColor(ItemColor.White))
                {
                    Attacker = _board[p0].Piece;
                    return true;
                }
                else if (!p1.IsOutOfBounds() && _board[p1].IsOccupiedByPieceOfColor(ItemColor.White))
                {
                    Attacker = _board[p1].Piece;
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
                    if (_board[pos].IsOccupiedByPieceOfColor(_pieceColor) &&
                        !_board[pos].IsOccupiedByPieceOfColorAndType(_pieceColor, PieceType.King))
                    {
                        // own piece blocking, so break out of this path and check next path
                        // note that while checking a given Position, own King cannot block -
                        // this is possible because we use the same algorithm to determine possible
                        // Check conditions during Castling where the square checked is NOT occupied
                        // currently by the King
                        break;
                    }

                    if (_board[pos].IsOccupiedByPieceOfColor(_enemyColor))
                    {
                        Piece p = _board[pos].Piece;
                        if (p.PieceType == pieceType)
                        {
                            Attacker = p;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
