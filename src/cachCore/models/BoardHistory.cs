using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class BoardHistory
    {
        private Stack<BoardHistoryItem> _boardHistoryStack;

        internal class BoardMove
        {
            /// <summary>
            /// This is the "official" move number which consists of both a White and a Black move
            /// </summary>
            internal int MoveNumber { get; set; }

            internal int MoveStepNumber { get; set; }
            internal ItemColor PieceColor { get; set; }
            internal string Move { get; set; }
        }
        private Stack<BoardMove> _moveStack;

        /// <summary>
        /// This is the "official" move number which consists of both a White and a Black move
        /// </summary>
        private int _moveNumber;

        /// <summary>
        /// This number changes on every color move
        /// </summary>
        private int _moveStepNumber;

        public BoardHistory()
        {
            _moveNumber = _moveStepNumber = 1;
            _boardHistoryStack = new Stack<BoardHistoryItem>();
            _moveStack = new Stack<BoardMove>();
        }

        /// <summary>
        /// Stores current position of the given piece
        /// </summary>
        /// <param name="piece"></param>
        public void PushPosition(Piece piece, BoardStatus boardStatus)
        {
            BoardHistoryItem hi = new PiecePositionHistoryItem(_moveNumber, _moveStepNumber,
                piece.Id, piece.Position, piece.HasMoved, boardStatus);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Stores alive status of the given piece
        /// </summary>
        /// <param name="piece"></param>
        public void PushAliveStatus(Piece piece)
        {
            BoardHistoryItem hi = new PieceAliveStatusHistoryItem(_moveNumber, _moveStepNumber, piece.Id);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Stores promotion details of originalPiece -> promotedPiece
        /// </summary>
        /// <param name="originalPiece"></param>
        /// <param name="promotedPiece"></param>
        public void PushPromotion(Piece originalPiece, Piece promotedPiece)
        {
            BoardHistoryItem hi = new PiecePromotionHistoryItem(_moveNumber, _moveStepNumber,
                originalPiece.Id, promotedPiece.Id);
            _boardHistoryStack.Push(hi);
        }

        /// <summary>
        /// Records moves into Move History
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <param name="move"></param>
        public void RecordMove(ItemColor pieceColor, string move)
        {
            _moveStack.Push(new BoardMove
            {
                MoveNumber = _moveNumber,
                MoveStepNumber = _moveStepNumber,
                PieceColor = pieceColor,
                Move = move
            });

            // Black's move advances move number
            if (pieceColor == ItemColor.Black)
            {
                _moveNumber++;
            }

            // either color move advances move step number
            _moveStepNumber++;
        }

        /// <summary>
        /// Removes the last in board history item and returns it
        /// </summary>
        /// <returns></returns>
        public BoardHistoryItem Pop()
        {
            BoardHistoryItem hi = _boardHistoryStack.Pop();

            // if there are recorded moves for current board history, pop them too
            if (_moveStack.Count > 0)
            {
                BoardMove bm = _moveStack.Peek();
                if (bm.MoveNumber == hi.MoveNumber && bm.MoveStepNumber == hi.MoveStepNumber)
                {
                    _moveStack.Pop();

                    // update current step numbers
                    _moveNumber = bm.MoveNumber;
                    _moveStepNumber = bm.MoveStepNumber;
                }
            }

            return hi;
        }

        public bool IsEmpty { get { return _boardHistoryStack.Count == 0; } }

        /// <summary>
        /// Stack level / current stack top MoveStepNumber - used when multiple items need to be popped for one step
        /// </summary>
        public int PeekMoveStepNumber
        {
            get { return _boardHistoryStack.Count > 0 ? _boardHistoryStack.Peek().MoveStepNumber : 0; }
        }
    }
}
