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
        [TestCase("2r2k2/8/8/3q4/8/8/4R3/3KN3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/3q4/1B6/8/4P3/3KN3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/3q4/1B6/1B6/4P3/3KN3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/B2q4/8/8/4P3/3KN3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/B2q4/1P6/5N2/4P3/3KR3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/3q4/8/8/4P3/rNRKR3 w - -", ItemColor.White)]
        public void not_in_mate_tests(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var imHelper = new InMateHelper(board, pieceColor);
            Assert.IsFalse(imHelper.IsCheckMate);
        }

        [Test]
        [TestCase("7Q/8/8/8/8/8/7R/k1K5 w - -", ItemColor.Black)]
        [TestCase("2r2k2/8/8/B2q4/1P6/8/4P3/3KR3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/3q4/b7/1N6/4P3/3KR3 w - -", ItemColor.White)]
        [TestCase("2r2k2/8/8/3q4/8/8/4P3/rN1KR3 w - -", ItemColor.White)]
        public void in_mate_tests(string fen, ItemColor pieceColor)
        {
            Board board = FENSerializer.BoardFromFEN(fen);
            var imHelper = new InMateHelper(board, pieceColor);
            Assert.IsTrue(imHelper.IsCheckMate);
        }
    }
}
