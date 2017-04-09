using NUnit.Framework;
using cachCore.models;
using cachCore.utils;
using cachCore.rules;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class CastleAttemptValidationHelperTests
    {
        [Test]
        public void castle_attempt_test1()
        {
            // this Castle attempt must fail
            Board board = FENSerializer.BoardFromFEN("8/8/1k6/1b6/8/8/5PPP/4K2R w - -");
            var cavHelper = new CastleAttemptValidationHelper(board, ItemColor.White, true);
            Assert.IsFalse(cavHelper.CanCastle);
        }

        [Test]
        public void castle_attempt_test2()
        {
            // this Castle attempt must succeed
            Board board = FENSerializer.BoardFromFEN("8/8/1k6/1b6/6R1/8/4PP1P/4K2R w - -");
            var cavHelper = new CastleAttemptValidationHelper(board, ItemColor.White, true);
            Assert.IsTrue(cavHelper.CanCastle);
            Position kp = new Position(0, 6);
            Assert.IsTrue(cavHelper.KingPositionAfterCastle.IsSame(kp));
            Assert.IsTrue(cavHelper.RookPositionAfterCastle.IsSame(kp.Left));
        }

        [Test]
        public void castle_attempt_test3()
        {
            // this Castle attempt must fail
            Board board = FENSerializer.BoardFromFEN("8/8/1k6/1b6/6r1/8/4PP1P/4K2R w - -");
            var cavHelper = new CastleAttemptValidationHelper(board, ItemColor.White, true);
            Assert.IsFalse(cavHelper.CanCastle);
        }

        [Test]
        public void castle_attempt_test4()
        {
            // this Castle attempt must fail
            Board board = FENSerializer.BoardFromFEN("8/8/1k6/1b6/8/8/4PP1P/4K3 w - -");
            var cavHelper = new CastleAttemptValidationHelper(board, ItemColor.White, true);
            Assert.IsFalse(cavHelper.CanCastle);
        }
    }
}
