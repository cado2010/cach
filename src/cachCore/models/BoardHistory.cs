using System;
using System.Collections.Generic;
using System.Linq;
using cachCore.enums;

namespace cachCore.models
{
    public class BoardHistory
    {
        private Stack<BoardHistoryItem> _boardHistoryStack;

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

        public IList<BoardMove> Moves => _moveStack.ToList();

        public string LastMove { get; private set; }

        /// <summary>
        /// Stores current position of the given piece
        /// </summary>
        /// <param name="piece"></param>
        public void PushPosition(Piece piece, BoardStatus boardStatus)
        {
            BoardHistoryItem hi = new PiecePositionHistoryItem(_moveNumber, _moveStepNumber,
                piece.Id, piece.Position, piece.HasMoved, boardStatus, piece.PieceType, piece.PieceColor);
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

            LastMove = move;
        }

        /// <summary>
        /// Removes the last in board history item and returns it
        /// </summary>
        /// <returns></returns>
        public BoardHistoryItem Pop()
        {
            BoardHistoryItem hi = _boardHistoryStack.Pop();

            LastMove = null;

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

                    LastMove = bm.Move;
                }
            }

            return hi;
        }

        /// <summary>
        /// Returns the last board move history item to get access to the original position
        /// of the last moved piece
        /// </summary>
        /// <returns></returns>
        public PiecePositionHistoryItem GetLastMoveBoardHistoryItem()
        {
            PiecePositionHistoryItem pphi = null;

            foreach (var bhi in _boardHistoryStack)
            {
                PiecePositionHistoryItem item = bhi as PiecePositionHistoryItem;
                if (item != null)
                {
                    pphi = item;
                    break;
                }
            }

            return pphi;
        }

        public string GetPGN()
        {
            string pgn = "";

            if (_moveStack.Count > 0)
            {
                BoardMove[] moves = _moveStack.ToArray();
                Array.Reverse(moves);
                int moveNumber = 1;
                for (int i = 0; i < moves.Length; i += 2, moveNumber++)
                {
                    string m = FixMove(moves[i].Move);
                    pgn += $"{moveNumber}. {m} ";
                    if (i + 1 < moves.Length)
                    {
                        m = FixMove(moves[i + 1].Move);
                        pgn += $"{m} ";
                    }
                }
            }

            return pgn;
        }

        public IList<string> GetPGNList()
        {
            List<string> pgn = new List<string>();

            if (_moveStack.Count > 0)
            {
                BoardMove[] moves = _moveStack.ToArray();
                Array.Reverse(moves);
                int moveNumber = 1;
                for (int i = 0; i < moves.Length; i += 2, moveNumber++)
                {
                    string m = FixMove(moves[i].Move);
                    string pgnItem = $"{moveNumber}. {m} ";
                    if (i + 1 < moves.Length)
                    {
                        m = FixMove(moves[i + 1].Move);
                        pgnItem += $"{m} ";
                    }

                    pgn.Add(pgnItem);
                }
            }

            return pgn;
        }

        public IList<string> GetRawPGNList()
        {
            List<string> pgn = new List<string>();

            if (_moveStack.Count > 0)
            {
                IList<BoardMove> moves = _moveStack.ToList();
                moves.Reverse();
                return moves.Select(x => x.Move).ToList();
            }

            return pgn;
        }

        public bool IsEmpty { get { return _boardHistoryStack.Count == 0; } }

        /// <summary>
        /// Returns true if player of given color has castled
        /// </summary>
        /// <param name="playerColor"></param>
        /// <returns></returns>
        public bool Castled(ItemColor playerColor)
        {
            bool castled = false;
            foreach (var move in _moveStack)
            {
                if (move.PieceColor == playerColor && (move.Move == "o-o" || move.Move == "o-o-o"))
                {
                    castled = true;
                    break;
                }
            }

            return castled;
        }

        /// <summary>
        /// Heuristics to determine if we appear to be in middle game -
        /// defined as: 
        /// At least 3 of the minor pieces have moved on each side and additionally
        /// 4+ pawn moves made
        /// </summary>
        /// <returns></returns>
        public bool IsMiddleGameLike()
        {
            int wMinorMoves = 0, bMinorMoves = 0;
            int wPawnMoves = 0, bPawnMoves = 0;

            foreach (var bhi in _boardHistoryStack)
            {
                if (bhi.Type == BoardHistoryType.PiecePosition)
                {
                    PiecePositionHistoryItem pphi = bhi as PiecePositionHistoryItem;
                    if (pphi.PieceType == PieceType.Bishop || pphi.PieceType == PieceType.Knight)
                    {
                        if (pphi.PieceColor == ItemColor.White)
                            wMinorMoves++;
                        else if (pphi.PieceColor == ItemColor.Black)
                            bMinorMoves++;
                    }
                    else if (pphi.PieceType == PieceType.Pawn)
                    {
                        if (pphi.PieceColor == ItemColor.White)
                            wPawnMoves++;
                        else if (pphi.PieceColor == ItemColor.Black)
                            bPawnMoves++;
                    }
                }
            }

            return wMinorMoves >= 2 && bMinorMoves >= 2 && wPawnMoves >= 3 && bPawnMoves >= 3;
        }

        /// <summary>
        /// Stack level / current stack top MoveStepNumber - used when multiple items need to be popped for one step
        /// </summary>
        public int PeekMoveStepNumber
        {
            get { return _boardHistoryStack.Count > 0 ? _boardHistoryStack.Peek().MoveStepNumber : 0; }
        }

        private string FixMove(string move)
        {
            char fm = move[0];

            switch (fm)
            {
                case 'k':
                    fm = 'K';
                    break;

                case 'q':
                    fm = 'Q';
                    break;

                case 'r':
                    fm = 'R';
                    break;

                case 'n':
                    fm = 'N';
                    break;

                // Bishop will already be a "B" so no replacement required

                case 'A':
                    fm = 'a';
                    break;

                case 'C':
                    fm = 'c';
                    break;

                case 'D':
                    fm = 'd';
                    break;

                case 'E':
                    fm = 'e';
                    break;

                case 'F':
                    fm = 'f';
                    break;

                case 'G':
                    fm = 'g';
                    break;

                case 'H':
                    fm = 'h';
                    break;
            }

            string fmove = fm + move.Substring(1);
            return fmove;
        }
    }
}
