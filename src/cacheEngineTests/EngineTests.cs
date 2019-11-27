using NUnit.Framework;
using cachCore.models;
using cacheEngine;
using cachCore.enums;

namespace cacheEngineTests
{
    [TestFixture]
    public class EngineTests
    {
        [Test]
        public void Two_level_deep_search()
        {
            Board board = new Board();
            Engine engine = new Engine(board, ItemColor.White);

            var moves = engine.SearchMoves(2);
            Assert.NotNull(moves);
            Assert.Positive(moves.Count);
        }
    }
}
