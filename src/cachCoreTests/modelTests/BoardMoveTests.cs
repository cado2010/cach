using NUnit.Framework;
using cachCore.enums;
using cachCore.models;
using cachCore.utils;

namespace cachCoreTests
{
    [TestFixture]
    public class BoardMoveTests
    {
        [Test]
        public void white_e4_should_succeed()
        {
            Board board = new Board();
            Assert.AreEqual(board.Move(ItemColor.White, "e4"), MoveErrorType.Ok);
        }

        [Test]
        public void white_e5_should_fail()
        {
            Board board = new Board();
            Assert.AreNotEqual(board.Move(ItemColor.White, "e5"), MoveErrorType.Ok);
        }

        [Test]
        public void black_c5_should_succeed()
        {
            Board board = new Board();
            Assert.AreEqual(board.Move(ItemColor.Black, "c5"), MoveErrorType.Ok);
        }

        [Test]
        public void white_c5_should_fail()
        {
            Board board = new Board();
            Assert.AreNotEqual(board.Move(ItemColor.White, "c5"), MoveErrorType.Ok);
        }

        [Test]
        public void white_Nc3_should_succeed()
        {
            Board board = new Board();
            Assert.AreEqual(board.Move(ItemColor.White, "Nc3"), MoveErrorType.Ok);
        }

        [Test]
        public void white_Nc2_should_fail()
        {
            Board board = new Board();
            Assert.AreNotEqual(board.Move(ItemColor.White, "Nc2"), MoveErrorType.Ok);
        }

        [Test]
        public void two_knights_can_reach_should_fail_with_correct_code()
        {
            Board board = FENSerializer.BoardFromFEN("8/8/8/8/8/3N4/8/3N4 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Nf2"), MoveErrorType.MoreThanOnePieceInRange);
        }

        [Test]
        public void all_queen_moves_should_fail()
        {
            Board board = new Board();
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qd3"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qd4"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qa4"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qa1"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.Black, "Qd3"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.Black, "Qd4"), MoveErrorType.Ok);
        }

        [Test]
        public void configured_queen_moves_should_work_appropriately_1()
        {
            Board board = FENSerializer.BoardFromFEN("3k4/8/8/8/4PP2/4Q3/8/3K4 w - -");

            // blocked by own
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qe5"), MoveErrorType.Ok);
            Assert.AreNotEqual(board.Move(ItemColor.White, "Qg5"), MoveErrorType.Ok);

            // this should be ok
            Assert.AreEqual(board.Move(ItemColor.White, "Qc5"), MoveErrorType.Ok);
        }

        [Test]
        public void configured_queen_moves_should_work_appropriately_2()
        {
            // this should be ok - killing
            Board board = FENSerializer.BoardFromFEN("3k4/8/8/2b5/4PP2/4Q3/8/3K4 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Qc5"), MoveErrorType.Ok);

            board = FENSerializer.BoardFromFEN("3k4/8/8/2b5/4PP2/4Q3/8/3K4 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Qxc5"), MoveErrorType.Ok);
        }

        [Test]
        public void incheck_move_not_allowed_1()
        {
            Board board = FENSerializer.BoardFromFEN("3k4/8/8/2b5/4PP2/4Q2p/6K1/8 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Qc5"), MoveErrorType.KingInCheck);
        }

        [Test]
        public void incheck_move_not_allowed_2()
        {
            Board board = FENSerializer.BoardFromFEN("3k4/8/8/8/4PP2/3pQ3/4K3/8 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Qc5"), MoveErrorType.KingInCheck);
        }

        [Test]
        public void incheck_move_allowed_1()
        {
            // this should be ok - killing
            Board board = FENSerializer.BoardFromFEN("3k4/8/8/8/4PP2/3pQ3/4K3/8 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "Qxd3"), MoveErrorType.Ok);
        }

        [Test]
        public void pawn_x_pawn_should_succeed()
        {
            Board board = FENSerializer.BoardFromFEN("2k5/8/8/8/3p4/8/2P5/5K2 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "c3"), MoveErrorType.Ok);
            Assert.AreEqual(board.Move(ItemColor.Black, "dxc3"), MoveErrorType.Ok);
        }

        [Test]
        public void en_passant_should_succeed_1()
        {
            Board board = FENSerializer.BoardFromFEN("2k5/8/8/8/3p4/8/2P5/5K2 w - -");
            Assert.AreEqual(board.Move(ItemColor.White, "c4"), MoveErrorType.Ok);
            Assert.AreEqual(board.Move(ItemColor.Black, "dxc3"), MoveErrorType.Ok);
        }

        [Test]
        public void en_passant_should_succeed_2()
        {
            Board board = FENSerializer.BoardFromFEN("2k5/3p4/8/2P5/8/8/8/5K2 w - -");
            Assert.AreEqual(board.Move(ItemColor.Black, "d5"), MoveErrorType.Ok);
            Assert.AreEqual(board.Move(ItemColor.White, "cxd6"), MoveErrorType.Ok);
        }

        [Test]
        [TestCase(ItemColor.White, "Kb1")]
        public void king_moves_should_fail(ItemColor pieceColor, string move)
        {
            Board board = FENSerializer.BoardFromFEN("8/7P/8/8/8/8/7R/k1K5 w - -");
            Assert.AreNotEqual(board.Move(pieceColor, move), MoveErrorType.Ok);
        }
    }
}
