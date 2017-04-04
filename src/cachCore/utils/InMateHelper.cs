using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;

namespace cachCore.utils
{
    /// <summary>
    /// Helper that implements rules to determine if the King of given color and position is in Check or Stale Mate
    /// </summary>
    public class InMateHelper
    {
        private Board _board;
        private ItemColor _pieceColor;
        private ItemColor _enemyColor;

        public InMateHelper(Board board, ItemColor pieceColor)
        {
            _board = board;
            _pieceColor = pieceColor;
            _enemyColor = pieceColor == ItemColor.Black ? ItemColor.White : ItemColor.Black;

            ComputeMate();
        }

        public bool IsCheckMate { get; private set; }

        public bool IsStaleMate { get; private set; }

        /// <summary>
        /// If not in Check Mate, reason
        /// If not in Slate Mate, reason
        /// </summary>
        public string Reason { get; private set; }

        private void ComputeMate()
        {
            IsCheckMate = IsStaleMate = false;

            King king = _board.GetPieces(_pieceColor, PieceType.King)[0] as King;

            var checkHelper = new InCheckHelper(_board, _pieceColor, king.Position);
            if (checkHelper.IsInCheck)
            {
                // Check Mate:
                // 1. King in Check
                // 2. attacking piece cannot be killed
                // 3. King cannot move to any position not in Check

                // 2. can Attacker be killed?
                // check entire army of that color and see if any one of them can kill the attacker
                IList<Piece> army = _board.GetAllActivePieces(_pieceColor);
                Piece killedBy = null;
                foreach (var piece in army)
                {
                    if (_board.IsWithinPieceRange(piece, checkHelper.Attacker.Position))
                    {
                        // yes can be killed, but override King as it may be dangerous to kill
                        // with King
                        if (killedBy == null || killedBy.PieceType == PieceType.King)
                        {
                            killedBy = piece;
                        }
                    }
                }

                // 2. if can be killed only by King, then check the other position
                if (killedBy != null)
                {
                    if (killedBy.PieceType == PieceType.King)
                    {
                        // make sure after killing, King is not in check (in the current Attacker's position)
                        var ch2 = new InCheckHelper(_board, _pieceColor, checkHelper.Attacker.Position);
                        if (!ch2.IsInCheck)
                        {
                            // attacker can be safely killed by King
                            Reason = "Attacker can be killed by King";
                            return;
                        }
                    }
                    else
                    {
                        // attacker can be safely killed
                        Reason = "Attacker can be killed by own Army";
                        return;
                    }
                }
                else
                {
                    // check if King can move
                    Movement m = _board.GetMovement(king);
                    foreach (var path in m.Paths)
                    {
                        foreach (var pos in path)
                        {
                            // if pos is not in Check, then can safely move there
                            var ch2 = new InCheckHelper(_board, _pieceColor, pos);
                            if (!ch2.IsInCheck)
                            {
                                // King can move to safety
                                Reason = "King can move to safety";
                                return;
                            }
                        }
                    }
                }

                // is Check Mate
                IsCheckMate = true;
            }
            else
            {
                // TODO:
                // Stale Mate:
                // 1. King NOT in Check
                // 2. No other Piece exists or can move
                // 3. King cannot move to any position not in Check
            }
        }
    }
}
