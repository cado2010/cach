using NUnit.Framework;
using cachCore.models;
using cachCore.utils;

namespace cachCoreTests
{
    [TestFixture]
    public class PGNParserTests
    {
        [Test]
        [TestCase("1. e4 e5 2. Nf3", 3, new string[] { "e4", "e5", "Nf3" })]
        [TestCase("1. d4 Nf6 2. Nc3 g6 \n3. e4 h5 4. h3 ", 7,
            new string[] { "d4", "Nf6", "Nc3", "g6", "e4", "h5", "h3" })]
        public void pgn_tests(string pgn, int count, string[] expectedMoves)
        {
            PGNParser p = new PGNParser(pgn);
            Assert.AreEqual(p.Moves.Count, count);
            Assert.AreEqual(p.Moves.Count, expectedMoves.Length);
            int i = 0;
            foreach (var m in p.Moves)
            {
                Assert.AreEqual(m, expectedMoves[i++]);
            }
        }
    }
}
