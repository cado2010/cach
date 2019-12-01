using log4net;
using cachCore.models;
using cachCore.utils;
using cachCore.enums;

namespace cachCore.controllers
{
    public class GameController
    {
        private ILog _logger;

        public GameController()
        {
            _logger = LogManager.GetLogger(GetType().Name);
        }

        public Game CreateGame()
        {
            Board board = new Board();
            Game game = new Game(board);
            return game;
        }

        public Game CreateGame(string fen)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            Game game = new Game(board);
            return game;
        }

        public string GetFEN(Game game, ItemColor colorToMove)
        {
            return FENSerializer.BoardToFEN(game.Board, colorToMove);
        }
    }
}
