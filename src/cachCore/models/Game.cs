using System;
using log4net;
using cachCore.enums;
using cachCore.utils;

namespace cachCore.models
{
    public class Game
    {
        public string Id { get; private set; }

        private readonly Board _board;
        private ItemColor _toPlay;

        private ILog _logger;

        public Board Board {  get { return _board; } }

        public Game(Board board)
        {
            _logger = LogManager.GetLogger(GetType().Name);

            Id = Guid.NewGuid().ToString();
            _logger.Debug($"Game: {Id} created with given board");

            _board = board;
            _toPlay = ItemColor.White;
        }

        public Game(string pgn)
        {
            _logger = LogManager.GetLogger(GetType().Name);

            Id = Guid.NewGuid().ToString();
            _logger.Debug($"Game: {Id} created from PGN: {pgn}");

            _board = new Board();
            _toPlay = ItemColor.White;

            LoadPGN(pgn);
        }

        public MoveErrorType LastMoveError { get; private set; }

        public ItemColor ToPlay { get { return _toPlay; } }

        public void Move(string move, MoveDescriptor inMd = null)
        {
            if (!_board.IsGameOver)
            {
                LastMoveError = _board.Move(_toPlay, move, inMd);
                if (_board.IsGameOver)
                {
                    if (_board.IsCheckMate)
                    {
                        _logger.Info($"Game over: {_board.Winner.ToString()} wins!");
                    }
                    else if (_board.IsStaleMate)
                    {
                        _logger.Info($"Game over: stalemate draw.");
                    }
                    else
                    {
                        _logger.Info($"Game over: draw offer.");
                    }
                }
                else if (LastMoveError == MoveErrorType.Ok)
                {
                    _toPlay = BoardUtils.GetOtherColor(_toPlay);
                }
            }
            else
            {
                _logger.Error($"Move: Game is over");
            }
        }

        public void Resign(ItemColor playerColor)
        {
            if (!_board.IsGameOver)
            {
                _board.Move(playerColor, "($)");
            }
        }

        public void Draw()
        {
            if (!_board.IsGameOver)
            {
                _board.Move(_toPlay, "(=)");
            }
        }

        public bool MoveUndo()
        {
            if (_board.MoveUndo())
            {
                _toPlay = BoardUtils.GetOtherColor(_toPlay);
                return true;
            }

            return false;
        }

        private void LoadPGN(string pgn)
        {
            PGNParser parser = new PGNParser(pgn);
            foreach (var m in parser.Plies)
            {
                Move(m);
            }
        }
    }
}
