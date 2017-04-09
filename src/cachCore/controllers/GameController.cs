using log4net;
using cachCore.models;

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
    }
}
