using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using cachCore.enums;
using cachCore.models;
using cachCore.utils;
using cacheEngine.models;

namespace cacheEngine
{
    public class Engine
    {
        private Board _board;
        private BoardEvaluator _boardEvaluator;
        private ItemColor _enginePlayerColor;
        private ItemColor _opponentColor;

        private ILog _logger;

        public Engine(Board board, ItemColor enginePlayerColor)
        {
            _board = board;
            _enginePlayerColor = enginePlayerColor;
            _opponentColor = BoardUtils.GetOtherColor(enginePlayerColor);
            _boardEvaluator = new BoardEvaluator();
            _logger = LogManager.GetLogger(GetType().Name);
        }

        /// <summary>
        /// Returns a flat list of all possible next immediate moves for the given piece color
        /// in tuples of the form { Piece, TargetPosition }
        /// </summary>
        /// <param name="pieceColor"></param>
        /// <returns></returns>
        private IList<dynamic> GetAllPossibleNextMoves(ItemColor pieceColor)
        {
            List<dynamic> moves = new List<dynamic>();

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
                            moves.Add(new { Piece = piece, TargetPosition = position, IsKill = _board[position].IsOccupied() });
                        }
                    }
                }
            }

            var sortedMoves = (from move in moves
                               orderby move.IsKill descending
                               select move).ToList();
            // var sortedMoves = moves;

            string msg = $"GetAllPossibleNextMoves({pieceColor}): ";
            foreach (var move in sortedMoves)
            {
                MoveDescriptor md = CreateMoveDescriptor(move);
                msg += $"{md.Move}, ";
            }
            _logger.Debug(msg);

            return sortedMoves;
        }

        private MoveDescriptor CreateMoveDescriptor(dynamic move)
        {
            Piece piece = move.Piece;

            MoveDescriptor md = new MoveDescriptor(piece)
            {
                PieceColor = piece.PieceColor,
                Move = "comp",
                PieceType = piece.PieceType,
                TargetPosition = move.TargetPosition,
                StartPosition = piece.Position,
                IsKingSideCastle = false,
                IsQueenSideCastle = false,
                IsKill = move.IsKill,
                IsDrawOffer = false,
                IsResign = false,
                IsPromotion = false,
                PromotedPieceType = PieceType.Unknown
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
                IList<dynamic> moves = GetAllPossibleNextMoves(_enginePlayerColor);

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

                            //if (mc.Value > maxMcValue)
                            //{
                            //    // clear any moves collected so far - new max found
                            //    searchMoves.Clear();
                            //}

                            //if (mc.Value >= maxMcValue)
                            //{
                            //    maxMcValue = mc.Value;

                            //    // store new max move
                            //    MoveChoice maxMc = new MoveChoice(piece, md, maxMcValue);
                            //    searchMoves.Add(maxMc);
                            //}

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
                IList<dynamic> moves = GetAllPossibleNextMoves(_opponentColor);

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

        /// <summary>
        /// Searches current Board for moves at given depth and returns a list of possibilities
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IList<MoveChoice> SearchMoves(int depth)
        {
            _logger.Info("----------------------------------------------------------------------");
            _logger.Info($"SearchMoves: depth={depth}");

            List<MoveChoice> searchMoves = new List<MoveChoice>();
            MoveChoice mc = AlphaBetaPruningSearch(depth, BoardEvaluator.MinVal, BoardEvaluator.MaxVal, true, "", searchMoves);
            // _logger.Info($"Result move: {mc}");

            List<MoveChoice> filteredMoves = new List<MoveChoice>();
            foreach (var move in searchMoves)
            {
                _logger.Info($"SearchMove: {move}");
                if (move.Value == mc.Value && move.Piece.PieceType != PieceType.King)
                {
                    filteredMoves.Add(move);
                }
            }

            if (filteredMoves.Count == 0)
            {
                filteredMoves = searchMoves;
            }

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
    }
}
