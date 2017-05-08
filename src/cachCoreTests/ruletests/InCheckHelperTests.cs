using NUnit.Framework;
using cachCore.models;
using cachCore.utils;
using cachCore.rules;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class InCheckHelperTests
    {
        [Test]
        [TestCase("3q4/3p4/8/8/8/3P4/8/3K4 w - -", ItemColor.White)]
        [TestCase("3q4/8/8/8/8/3P4/8/3K4 w - -", ItemColor.White)]
        [TestCase("3k4/8/8/3P4/8/8/8/3QK3 w - -", ItemColor.Black)]
        [TestCase("7Q/8/8/8/8/8/k7/2K4R w - -", ItemColor.Black)]
        [TestCase("6k1/5ppp/8/2p5/1p5P/r2P4/1K6/r7 w - -", ItemColor.White)]
        public void not_in_check_test(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InCheckHelper(board, pieceColor);
            Assert.IsFalse(icHelper.IsInCheck);
        }

        [Test]
        [TestCase("3k4/8/8/8/8/8/8/3QK3 w - -", ItemColor.Black)]
        [TestCase("3k4/8/8/3P4/7B/8/8/3QK3 w - -", ItemColor.Black)]
        [TestCase("8/7P/8/8/8/8/7R/kK6 w - -", ItemColor.White)]
        [TestCase("8/7P/8/8/8/8/7R/kK6 w - -", ItemColor.Black)]
        public void in_check_test(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InCheckHelper(board, pieceColor);
            Assert.IsTrue(icHelper.IsInCheck);
        }
    }
}
