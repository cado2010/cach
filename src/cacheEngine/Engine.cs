using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using cachCore.enums;
using cachCore.models;
using cachCore.utils;
using cacheEngine.models;
using openingBook;
using openingBook.models;

namespace cacheEngine
{
    public class Engine
    {
        private Board _board;
        private BoardEvaluator _boardEvaluator;
        private ItemColor _enginePlayerColor;
        private ItemColor _opponentColor;

        private bool _useOpeningBook;
        private OpeningBookNode _root;
        private OpeningBookSearch _obSearch;

        private ILog _logger;

        class SearchMove
        {
            public Piece Piece { get; set; }
            public Position TargetPosition { get; set; }
            public bool IsKill { get; set; }
            public bool IsKingSideCastle { get; set; }
            public bool IsQueenSideCastle { get; set; }
        }

        public Engine(Board board, ItemColor enginePlayerColor, bool useOpeningBook = true)
        {
            _board = board;
            _enginePlayerColor = enginePlayerColor;
            _opponentColor = BoardUtils.GetOtherColor(enginePlayerColor);
            _boardEvaluator = new BoardEvaluator();
            _logger = LogManager.GetLogger(GetType().Name);

            _useOpeningBook = useOpeningBook;
            if (_useOpeningBook)
            {
                OpeningBookBuilder obb = new OpeningBookBuilder();
                obb.Read("cach_openingbook.txt");
                _root = obb.Root;
                _obSearch = new OpeningBookSearch(_root);
            }
        }


        /// <summary>
        /// Searches current Board for moves at given depth and returns a list of possibilities
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IList<MoveChoice> SearchMoves(int depth)
        {
            _logger.Info("----------------------------------------------------------------------");
            _logger.Info($"SearchMoves: depth={depth}");

            if (_useOpeningBook)
            {
                // try Opening Book
                IList<string> obMoves = _obSearch.SearchMoves(_board.GetRawPGNList());
                if (obMoves != null && obMoves.Count > 0)
                {
                    // Opening Book moves as MoveChoice's
                    return obMoves.Select(x => new MoveChoice(null, null, 0, x, true)).ToList();
                }
            }

            List<MoveChoice> searchMoves = new List<MoveChoice>();
            MoveChoice mc = AlphaBetaPruningSearch(depth, BoardEvaluator.MinVal, BoardEvaluator.MaxVal, true, "", searchMoves);
            // _logger.Info($"Result move: {mc}");

            IList<MoveChoice> filteredMoves = FilterMoves(searchMoves, mc);

            foreach (var move in filteredMoves)
            {
                _logger.Info($"FilteredMove: {move}");
            }

            if (filteredMoves.Count == 0)
            {
                _logger.Error($"SearchMoves: Unable to find any moves!!");
            }

            return filteredMoves;
        }

        private IList<MoveChoice> FilterMoves(IList<MoveChoice> searchMoves, MoveChoice moveResult)
        {
            bool hasKills = searchMoves.Any(x => x.MoveDescriptor.IsKill);

            List<MoveChoice> filteredMoves = new List<MoveChoice>();
            foreach (var move in searchMoves)
            {
                _logger.Info($"FilterMoves: {move}");

                if (move.Piece != null)
                {
                    // dont move King until End Game
                    if (move.Piece.PieceType == PieceType.King && !IsEndGame())
                        continue;

                    // dont move Rooks if not yet castled
                    if (move.Piece.PieceType == PieceType.Rook && !_board.Castled(_enginePlayerColor))
                        continue;

                    // dont move Queen if not yet in Middle Game
                    if (move.Piece.PieceType == PieceType.Queen && !IsMiddleGame())
                        continue;
                }

                if (move.Value == moveResult.Value)
                {
                    // select kills as top priority
                    if (hasKills)
                    {
                        if (move.MoveDescriptor.IsKill)
                        {
                            filteredMoves.Add(move);
                        }
                    }
                    else
                    {
                        filteredMoves.Add(move);
                    }
                }
            }

            if (filteredMoves.Count == 0)
            {
                return searchMoves;
            }

            return filteredMoves;
        }

        /// <summary>
        /// Returns a flat list of all possible next immediate moves for the given piece color
        /// in tuples of the form { Piece, TargetPosition }
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        private IList<SearchMove> GetAllPossibleNextMoves(ItemColor pieceColor)
        {
            List<SearchMove> moves = new List<SearchMove>(100);

            IList<Piece> pieces = _board.GetAllActivePieces(pieceColor);
            foreach (var piece in pieces)
            {
                // get all position movements of given piece and flatten out the paths into tuples
                // containing a piece and a possible position to move to - this will form the list of
                // raw child nodes of "next possible moves" of the given color for the current board
                Movement movement = _board.GetMovement(piece);
                if (!movement.IsEmpty)
                {
                    // iterate through all paths
                    foreach (var path in movement.Paths)
                    {
                        // iterate through all positions in a path
                        foreach (var position in path)
                        {
                            var move = new SearchMove
                            {
                                Piece = piece,
                                TargetPosition = position,
                                IsKill = _board[position].IsOccupied(),
                                IsKingSideCastle = false,
                                IsQueenSideCastle = false
                            };

                            if (move.IsKill)
                            {
                                moves.Insert(0, move);
                            }
                            else
                            {
                                moves.Add(move);
                            }
                        }
                    }
                }
            }

            if (pieceColor == _enginePlayerColor && !_board.Castled(_enginePlayerColor))
            {
                moves.Add(new SearchMove
                {
                    Piece = null,
                    TargetPosition = Position.Invalid,
                    IsKill = false,
                    IsKingSideCastle = true,
                    IsQueenSideCastle = false
                });
                moves.Add(new SearchMove
                {
                    Piece = null,
                    TargetPosition = Position.Invalid,
                    IsKill = false,
                    IsKingSideCastle = false,
                    IsQueenSideCastle = true
                });
            }

            //var sortedMoves = (from move in moves
            //                   orderby move.IsKill descending
            //                   select move).ToList();
            // var sortedMoves = moves;

            //string msg = $"GetAllPossibleNextMoves({pieceColor}): ";
            //foreach (var move in moves)
            //{
            //    MoveDescriptor md = CreateMoveDescriptor(move);
            //    msg += $"{md.Move}, ";
            //}
            //_logger.Debug(msg);

            return moves;
        }

        private MoveDescriptor CreateMoveDescriptor(SearchMove move)
        {
            Piece piece = move.Piece;

            bool isPromotion = piece != null && piece.PieceType == PieceType.Pawn &&
                ((_enginePlayerColor == ItemColor.White && move.TargetPosition.Row == 8) ||
                (_enginePlayerColor == ItemColor.Black && move.TargetPosition.Row == 0));

            MoveDescriptor md = new MoveDescriptor(piece)
            {
                PieceColor = _enginePlayerColor,
                Move = "comp",
                PieceType = piece != null ? piece.PieceType : PieceType.King,
                TargetPosition = move.TargetPosition,
                StartPosition = piece != null ? piece.Position : Position.Invalid,
                IsKingSideCastle = move.IsKingSideCastle,
                IsQueenSideCastle = move.IsQueenSideCastle,
                IsKill = move.IsKill,
                IsDrawOffer = false,
                IsResign = false,
                IsPromotion = isPromotion,
                PromotedPieceType = isPromotion ? PieceType.Queen : PieceType.Unknown
            };
            md.Move = md.MoveDescFromPosition;

            return md;
        }

        /// <summary>
        /// Alpha-Beta pruning deep search for moves - algorithm from:
        /// https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="maximizingPlayer"></param>
        /// <returns></returns>
        private MoveChoice AlphaBetaPruningSearch(int depth, int alpha, int beta, bool maximizingPlayer,
            string currMovePath, IList<MoveChoice> searchMoves = null)
        {
            _logger.Debug($"ABPS: depth={depth}, alpha={alpha}, beta={beta}, maximizingPlayer={maximizingPlayer}");

            if (depth == 0 || _board.IsCheckMate || _board.IsStaleMate)
            {
                int value = _boardEvaluator.Evaluate(_board, _enginePlayerColor);

                _logger.Info($"ABPS: depth={depth}, alpha={alpha}, beta={beta}, maximizingPlayer={maximizingPlayer}, returning value={value}, currMovePath={currMovePath}");
                return new MoveChoice(null, null, value);
            }

            if (maximizingPlayer)
            {
                int value = BoardEvaluator.MinVal;
                int maxMcValue = BoardEvaluator.MinVal;

                // get all possible nodes (moves) for Engine
                IList<SearchMove> moves = GetAllPossibleNextMoves(_enginePlayerColor);

                MoveChoice mc = null;
                MoveDescriptor md = null;
                Piece piece = null;
                string basePath = currMovePath;

                bool foundOne = false;

                foreach (var move in moves)
                {
                    if (alpha >= beta && !move.IsKill)
                    {
                        _logger.Info($"ABPS: depth={depth}, maxzer pruning exit: alpha={alpha}, beta={beta}, currMovePath={currMovePath}");
                        break;
                    }

                    // perform Board move
                    Piece prevPiece = piece;
                    MoveDescriptor prevMd = md;

                    piece = move.Piece;
                    md = CreateMoveDescriptor(move);
                    string mvDesc = md.Move;

                    MoveErrorType err = _board.Move(_enginePlayerColor, mvDesc, md);
                    currMovePath = basePath + " -> " + _enginePlayerColor + ":" + mvDesc;
                    _logger.Debug($"ABPS: maximizing: depth={depth}, trying move={currMovePath}");

                    if (err != MoveErrorType.Ok)
                    {
                        _logger.Debug($"ABPS: error={err} in move={mvDesc}");
                        piece = prevPiece;
                        md = prevMd;
                        continue;
                    }

                    // recurse: deep search for the best possible move
                    mc = AlphaBetaPruningSearch(depth - 1, alpha, beta, false, currMovePath);
                    if (mc == null)
                    {
                        _logger.Warn($"ABPS: next level (maxzer to minzer dive) search returned null move");
                    }
                    else
                    {
                        value = Math.Max(value, mc.Value);
                        alpha = Math.Max(alpha, value);

                        // top layer search moves list maintenance
                        if (searchMoves != null)
                        {
                            _logger.Debug($"ABPS depth4 result: mcValue={mc.Value}, value={value}, alpha={alpha}, beta={beta}, currMovePath={currMovePath}");

                            if (alpha > maxMcValue)
                            {
                                maxMcValue = alpha;

                                searchMoves.Clear();

                                // store new max move
                                MoveChoice maxMc = new MoveChoice(piece, md, maxMcValue);
                                searchMoves.Add(maxMc);

                                foundOne = true;
                            }
                            else if (foundOne && mc.Value == maxMcValue)
                            {
                                // alternate move?
                                MoveChoice maxMc = new MoveChoice(piece, md, mc.Value);
                                searchMoves.Add(maxMc);
                            }
                        }
                    }

                    // undo the move from this step
                    _board.MoveUndo();

                    //if (alpha >= beta)
                    //{
                    //    _logger.Info($"ABPS: depth={depth}, maxzer pruning exit: alpha={alpha}, beta={beta}, currMovePath={currMovePath}");
                    //    break;
                    //}
                }

                _logger.Debug($"ABPS: depth={depth}, alpha={alpha}, beta={beta}, maxzer returning value = {value}");
                return new MoveChoice(null, null, value);
            }
            else
            {
                // minimizing player
                int value = BoardEvaluator.MaxVal;

                // get all possible nodes (moves) for opponent
                IList<SearchMove> moves = GetAllPossibleNextMoves(_opponentColor);

                MoveChoice mc = null;
                string basePath = currMovePath;

                foreach (var move in moves)
                {
                    if (beta <= alpha && !move.IsKill)
                    {
                        _logger.Info($"ABPS: depth={depth}, minzer pruning exit: alpha={alpha}, beta={beta}, currMovePath={currMovePath}");
                        break;
                    }

                    // perform Board move
                    MoveDescriptor md = CreateMoveDescriptor(move);
                    string mvDesc = md.Move;

                    MoveErrorType err = _board.Move(_opponentColor, md.Move, md);
                    currMovePath = basePath + " -> " + _opponentColor + ":" + mvDesc;
                    _logger.Debug($"ABPS: minimizing: depth={depth}, trying move={currMovePath}");

                    if (err != MoveErrorType.Ok)
                    {
                        _logger.Debug($"ABPS: error={err} in move={mvDesc}");
                        continue;
                    }

                    // recurse: deep search for the best possible move
                    mc = AlphaBetaPruningSearch(depth - 1, alpha, beta, true, currMovePath);
                    if (mc == null)
                    {
                        _logger.Warn($"ABPS: next level (minzer to maxzer dive) search returned null move");
                    }
                    else
                    {
                        value = Math.Min(value, mc.Value);
                        beta = Math.Min(beta, value);
                    }

                    // undo the move from this step
                    _board.MoveUndo();

                    //if (beta <= alpha)
                    //{
                    //    _logger.Info($"ABPS: depth={depth}, minzer pruning exit: alpha={alpha}, beta={beta}, currMovePath={currMovePath}");
                    //    break;
                    //}
                }

                _logger.Debug($"ABPS: depth={depth}, alpha={alpha}, beta={beta}, minzer returning value = {value}");
                return new MoveChoice(null, null, value);
            }
        }

        private bool IsMiddleGame()
        {
            return _boardEvaluator.IsMiddleGame(_board);
        }

        private bool IsEndGame()
        {
            return _boardEvaluator.IsEndGame(_board);
        }
    }
}
