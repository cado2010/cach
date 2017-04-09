using cachCore.enums;
using cachCore.models;

namespace cachCore.utils
{
    /// <summary>
    /// Move descriptor created from move input string
    /// </summary>
    public class MoveDescriptor
    {
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

        //-------------------------------------------------------------------------------
        // Derived properties

        public bool IsCastle { get { return IsKingSideCastle || IsQueenSideCastle; } }

        public bool IsValid
        {
            get { return PieceType != PieceType.Unknown && (TargetPosition.IsValid || IsCastle); }
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
