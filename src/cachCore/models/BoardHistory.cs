using System.Collections.Generic;

namespace cachCore.models
{
    public class BoardHistory
    {
        private Stack<BoardHistoryItem> _boardHistoryStack;

        public BoardHistory()
        {
            _boardHistoryStack = new Stack<BoardHistoryItem>();
        }

        /// <summary>
        /// Stores current position of the given piece
        /// </summary>
        /// <param name="piece"></param>
        public void PushPosition(Piece piece)
        {
            BoardHistoryItem hi = new PiecePositionHistoryItem(piece.Id, piece.Position);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Stores alive status of the given piece
        /// </summary>
        /// <param name="piece"></param>
        public void PushAliveStatus(Piece piece)
        {
            BoardHistoryItem hi = new PieceAliveStatusHistoryItem(piece.Id);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Stores promotion details of originalPiece -> promotedPiece
        /// </summary>
        /// <param name="originalPiece"></param>
        /// <param name="promotedPiece"></param>
        public void PushPromotion(Piece originalPiece, Piece promotedPiece)
        {
            BoardHistoryItem hi = new PiecePromotionHistoryItem(originalPiece.Id, promotedPiece.Id);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Removes the last in board history item and returns it
        /// </summary>
        /// <returns></returns>
        public BoardHistoryItem Pop()
        {
            return _boardHistoryStack.Pop();
        }

        /// <summary>
        /// Stack level - used when multiple items need to be popped for one step
        /// </summary>
        public int Level { get { return _boardHistoryStack.Count; } }
    }
}
