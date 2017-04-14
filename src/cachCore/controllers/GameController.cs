using log4net;
using cachCore.models;
using cachCore.utils;

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
    }
}
