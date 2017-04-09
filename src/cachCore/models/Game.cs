﻿using System;
using log4net;
using cachCore.enums;

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

            _board = board;
            _toPlay = ItemColor.White;

            Id = Guid.NewGuid().ToString();
            _logger.Debug($"Game: {Id} created");
        }

        public MoveErrorType LastMoveError { get; private set; }

        public ItemColor ToPlay { get { return _toPlay; } }

        public void Move(string move)
        {
            if (!_board.IsGameOver)
            {
                LastMoveError = _board.Move(_toPlay, move);
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
                    _toPlay = _toPlay == ItemColor.White ? ItemColor.Black : ItemColor.White;
                }
            }
            else
            {
                _logger.Error($"Game is over");
            }
        }
    }
}
