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
        public void not_in_check_test(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InCheckHelper(board, pieceColor);
            Assert.IsFalse(icHelper.IsInCheck);
        }

        [Test]
        [TestCase("3k4/8/8/8/8/8/8/3QK3 w - -", ItemColor.Black)]
        [TestCase("3k4/8/8/3P4/7B/8/8/3QK3 w - -", ItemColor.Black)]
        public void in_check_test(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var icHelper = new InCheckHelper(board, pieceColor);
            Assert.IsTrue(icHelper.IsInCheck);
        }
    }
}
