using cachCore.enums;

namespace cachCore.models
{
    public abstract class BoardHistoryItem
    {
        public string Id { get; private set; }
        public BoardHistoryType Type { get; private set; }

        protected BoardHistoryItem(BoardHistoryType type)
        {
            Id = System.Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Records previous position of a given piece
    /// </summary>
    public class PiecePositionHistoryItem : BoardHistoryItem
    {
        public string PieceId { get; private set; }
        public Position Position { get; private set; }

        public PiecePositionHistoryItem(string pieceId, Position position) :
            base(BoardHistoryType.PiecePosition)
        {
            PieceId = pieceId;
            Position = position;
        }
    }

    /// <summary>
    /// Records that the specified piece was alive
    /// as this point
    /// </summary>
    public class PieceAliveStatusHistoryItem : BoardHistoryItem
    {
        public string PieceId { get; private set; }

        public PieceAliveStatusHistoryItem(string pieceId) :
            base(BoardHistoryType.PieceAliveStatus)
        {
            PieceId = pieceId;
        }
    }

    /// <summary>
    /// Records pawn promotion to a new piece
    /// </summary>
    public class PiecePromotionHistoryItem : BoardHistoryItem
    {
        public string PieceIdOriginal { get; private set; }

        public string PieceIdPromoted { get; private set; }

        public PiecePromotionHistoryItem(string pieceIdOriginal, string pieceIdPromoted) :
            base(BoardHistoryType.PiecePromotion)
        {
            PieceIdOriginal = pieceIdOriginal;
            PieceIdPromoted = pieceIdPromoted;
        }
    }
}
