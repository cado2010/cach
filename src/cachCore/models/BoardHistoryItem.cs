using cachCore.enums;

namespace cachCore.models
{
    public abstract class BoardHistoryItem
    {
        public string Id { get; private set; }
        public BoardHistoryType Type { get; private set; }
        public int MoveNumber { get; private set; }
        public int MoveStepNumber { get; private set; }

        protected BoardHistoryItem(BoardHistoryType type, int moveNumber, int moveStepNumber)
        {
            Id = System.Guid.NewGuid().ToString();
            Type = type;
            MoveNumber = moveNumber;
            MoveStepNumber = moveStepNumber;
        }
    }

    /// <summary>
    /// Records previous position of a given piece
    /// </summary>
    public class PiecePositionHistoryItem : BoardHistoryItem
    {
        public string PieceId { get; private set; }
        public Position Position { get; private set; }
        public BoardStatus BoardStatus { get; private set; }
        public bool HasMoved { get; private set; }
        public PieceType PieceType { get; private set; }
        public ItemColor PieceColor { get; private set; }

        public PiecePositionHistoryItem(int moveNumber, int moveStepNumber,
            string pieceId, Position position, bool hasMoved, BoardStatus boardStatus, PieceType pieceType, ItemColor pieceColor) :
            base(BoardHistoryType.PiecePosition, moveNumber, moveStepNumber)
        {
            PieceId = pieceId;
            Position = position;
            BoardStatus = boardStatus;
            HasMoved = hasMoved;
            PieceType = pieceType;
            PieceColor = pieceColor;
        }
    }

    /// <summary>
    /// Records that the specified piece was alive
    /// as this point
    /// </summary>
    public class PieceAliveStatusHistoryItem : BoardHistoryItem
    {
        public string PieceId { get; private set; }

        public PieceAliveStatusHistoryItem(int moveNumber, int moveStepNumber, string pieceId) :
            base(BoardHistoryType.PieceAliveStatus, moveNumber, moveStepNumber)
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

        public PiecePromotionHistoryItem(int moveNumber, int moveStepNumber, string pieceIdOriginal, string pieceIdPromoted) :
            base(BoardHistoryType.PiecePromotion, moveNumber, moveStepNumber)
        {
            PieceIdOriginal = pieceIdOriginal;
            PieceIdPromoted = pieceIdPromoted;
        }
    }
}
