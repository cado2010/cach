using System.Collections.Generic;
using cachCore.enums;
using cachCore.models;

namespace cachCore.rules
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

        public Position SafePosition { get; private set; }

        public Piece OwnArmyKiller { get; private set; }

        public Piece OwnArmyBlocker { get; private set; }

        public Position BlockPosition { get; private set; }

        public Piece CanMovePiece { get; private set;  }

        /// <summary>
        /// If not in Check Mate, reason
        /// If not in Slate Mate, reason
        /// </summary>
        public string Reason { get; private set; }

        private void ComputeMate()
        {
            IsCheckMate = IsStaleMate = false;
            SafePosition = Position.Invalid;
            OwnArmyKiller = OwnArmyBlocker = CanMovePiece = null;

            IList<Piece> kings = _board.GetActivePieces(_pieceColor, PieceType.King);
            King king = kings[0] as King;

            var checkHelper = new InCheckHelper(_board, _pieceColor, king.Position);
            if (checkHelper.IsInCheck)
            {
                ComputeCheckMate(checkHelper, king);
            }
            else
            {
                ComputeStaleMate(checkHelper, king);
            }
        }

        /// <summary>
        /// Check Mate rules:
        /// 1. King in Check
        /// 2. attacking piece cannot be killed
        /// 3. King cannot move to safety
        /// 4. None of own army can move to block in the path of the immediate attacker without endangering the King
        /// </summary>
        /// <param name="icHelper"></param>
        /// <param name="king"></param>
        private void ComputeCheckMate(InCheckHelper icHelper, King king)
        {
            // 1. is already true

            // 2. can Attacker be killed?
            // check entire army of that color and see if any one of them can kill the attacker
            IList<Piece> army = _board.GetAllActivePieces(_pieceColor);
            Piece killedBy = null;
            foreach (var piece in army)
            {
                if (_board.IsWithinPieceRange(piece, icHelper.Attacker.Position))
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
                    var ch2 = new InCheckHelper(_board, _pieceColor, icHelper.Attacker.Position);
                    if (!ch2.IsInCheck)
                    {
                        // attacker can be safely killed by King
                        Reason = "Attacker can be killed by King";
                        OwnArmyKiller = killedBy;

                        return;
                    }
                }
                else
                {
                    // attacker can be safely killed
                    Reason = "Attacker can be killed by own Army";
                    OwnArmyKiller = killedBy;

                    return;
                }
            }

            // 3. check if King can move to safety
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
                        SafePosition = pos;

                        return;
                    }
                }
            }

            // 4. immediate attack path is available and can be blocked by own army (except our own King of course)
            if (icHelper.AttackPath != null)
            {
                foreach (var piece in army)
                {
                    if (piece.PieceType == PieceType.King)
                    {
                        continue;
                    }

                    foreach (var ap in icHelper.AttackPath)
                    {
                        if (_board.IsWithinPieceRange(piece, ap))
                        {
                            // need to perform one last check of whether moving this piece will endanger the King
                            // once more
                            if (!KingInCheckAfterMove(piece, ap, king.Position))
                            {
                                // attack path blocked by own army
                                Reason = "Attack path can be blocked by own Army";
                                OwnArmyBlocker = piece;
                                BlockPosition = ap;

                                return;
                            }
                        }
                    }
                }
            }

            // is Check Mate
            IsCheckMate = true;
        }

        private bool KingInCheckAfterMove(Piece blockAttemptPiece, Position blockAttemptPosition,
            Position kingPosition)
        {
            bool result = false;

            // remember piece current pos
            Position origPos = blockAttemptPiece.Position;

            // attempt the move and check for danger
            _board.mMove(blockAttemptPiece, blockAttemptPosition);
            result = new InCheckHelper(_board, _pieceColor, kingPosition).IsInCheck;

            // after the computation, move the piece back to where it was before
            _board.mMove(blockAttemptPiece, origPos);

            return result;
        }

        /// <summary>
        /// Stale Mate rules:
        /// 1. King NOT in Check
        /// 2. King cannot move to any position not in Check
        /// 3. No other Piece exists or can move
        /// </summary>
        private void ComputeStaleMate(InCheckHelper icHelper, King king)
        {
            // 1. is already true

            // 2. check if King can move
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
                        Reason = "King can move to a safe position";
                        SafePosition = pos;

                        return;
                    }
                }
            }

            // 3. if any piece other than King can move
            IList<Piece> army = _board.GetAllActivePieces(_pieceColor);
            foreach (var piece in army)
            {
                if (piece.PieceType == PieceType.King)
                {
                    continue;
                }

                m = _board.GetMovement(piece);
                foreach (var path in m.Paths)
                {
                    if (path.Count > 0)
                    {
                        // some piece can move
                        Reason = "Some other piece can move";
                        CanMovePiece = piece;

                        return;
                    }
                }
            }

            // is Stale Mate
            IsStaleMate = true;
        }
    }
}
