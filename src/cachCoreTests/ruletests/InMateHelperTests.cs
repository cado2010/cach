using NUnit.Framework;
using cachCore.models;
using cachCore.utils;
using cachCore.rules;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class InMateHelperTests
    {
        [Test]
        [TestCase("8/8/8/8/8/k7/1Q5R/2K5 w - -", ItemColor.Black)]
        public void not_in_mate_tests(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InMateHelper(board, pieceColor);
            Assert.IsFalse(icHelper.IsCheckMate);
        }

        [Test]
        [TestCase("7Q/8/8/8/8/8/7R/k1K5 w - -", ItemColor.Black)]
        public void in_mate_tests(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InMateHelper(board, pieceColor);
            Assert.IsTrue(icHelper.IsCheckMate);
        }
    }
}
