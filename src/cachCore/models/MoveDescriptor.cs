using cachCore.enums;
using cachCore.models;
using System.Collections.Concurrent;

namespace cachCore.models
{
    /// <summary>
    /// Move descriptor created from move input string
    /// </summary>
    public class MoveDescriptor
    {
        public ItemColor PieceColor { get; set; }

        public string Move { get; set; }

        /// <summary>
        /// Piece Type specified by this move, must be available
        /// </summary>
        public PieceType PieceType { get; set; }

        /// <summary>
        /// Where input specifies piece to move to, must be specified or Castle
        /// </summary>
        public Position TargetPosition { get; set; }

        /// <summary>
        /// Where input specifies piece is moving from (this may be partial or unavailable)
        /// </summary>
        public Position StartPosition { get; set; }

        public bool IsKingSideCastle { get; set; }

        public bool IsQueenSideCastle { get; set; }

        public bool IsKill { get; set; }

        /// <summary>
        /// Offer to draw is indicated by move input string: "(=)"
        /// </summary>
        public bool IsDrawOffer { get; set; }

        /// <summary>
        /// Resigns
        /// </summary>
        public bool IsResign { get; set; }

        /// <summary>
        /// If this move specified a promotion of pawn reaching Rank 1 or 8
        /// </summary>
        public bool IsPromotion { get; set; }

        /// <summary>
        /// If IsPromotion == true, then what Piece type player specified
        /// </summary>
        public PieceType PromotedPieceType { get; set; }

        private static ConcurrentDictionary<PieceType, string> _pieceTypePrefix;
        static MoveDescriptor()
        {
            _pieceTypePrefix = new ConcurrentDictionary<PieceType, string>();
            _pieceTypePrefix.TryAdd(PieceType.King, "K");
            _pieceTypePrefix.TryAdd(PieceType.Queen, "Q");
            _pieceTypePrefix.TryAdd(PieceType.Rook, "R");
            _pieceTypePrefix.TryAdd(PieceType.Bishop, "B");
            _pieceTypePrefix.TryAdd(PieceType.Knight, "N");
        }

        public string MoveDescFromPosition
        {
            get
            {
                if (IsKingSideCastle)
                    return "o-o";
                else if (IsQueenSideCastle)
                    return "o-o-o";
                else if (IsResign)
                    return "resign";

                string moveDesc = "";
                if (PieceType == PieceType.Pawn)
                {
                    if (IsKill)
                    {
                        moveDesc = StartPosition.ToAlgebraic()[0] + "x";
                    }
                    moveDesc += TargetPosition.ToAlgebraic();
                }
                else
                {
                    string prefix = _pieceTypePrefix[PieceType];
                    moveDesc = prefix + (IsKill ? "x" : "") + TargetPosition.ToAlgebraic();
                }

                if (IsPromotion)
                {
                    moveDesc += "=" + _pieceTypePrefix[PromotedPieceType];
                }

                return moveDesc;
            }
        }

        //-------------------------------------------------------------------------------
        // Derived properties

        public bool IsCastle { get { return IsKingSideCastle || IsQueenSideCastle; } }

        public bool IsValid
        {
            get { return IsDrawOffer || IsResign ||
                    (PieceType != PieceType.Unknown && (TargetPosition.IsValid || IsCastle)); }
        }

        /// <summary>
        /// True if some component of StartPosition is available
        /// </summary>
        public bool IsStartPositionInfoAvailable
        {
            get
            {
                return StartPosition.Row != Position.InvalidCoordinate ||
                    StartPosition.Column != Position.InvalidCoordinate;
            }
        }
    }
}
