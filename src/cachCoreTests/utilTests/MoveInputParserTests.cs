using NUnit.Framework;
using cachCore.models;
using cachCore.utils;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class MoveInputParserTests
    {
        [Test]
        public void qe4_should_return_queen_not_kill()
        {
            MoveDescriptor md = new MoveInputParser("Qe4").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Queen);
            Assert.IsTrue(md.IsKill == false);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(3, 4)));
        }

        [Test]
        public void qxa1_should_return_queen_kill()
        {
            MoveDescriptor md = new MoveInputParser("Qxa1").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Queen);
            Assert.IsTrue(md.IsKill == true);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(0, 0)));
        }

        [Test]
        public void nfh8_should_not_return_queen_position()
        {
            MoveDescriptor md = new MoveInputParser("Nfh8").MoveDescriptor;
            Assert.AreNotEqual(md.PieceType, PieceType.Queen);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(7, 7)));
        }

        [Test]
        public void nfh2_should_return_knight_position_with_start_file()
        {
            MoveDescriptor md = new MoveInputParser("Nfh2").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Knight);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(1, 7)));
            Assert.IsTrue(md.StartPosition.Column == 5);
            Assert.IsTrue(md.StartPosition.Row == Position.InvalidCoordinate);
        }

        [Test]
        public void n4h2_should_return_knight_position_with_start_file()
        {
            MoveDescriptor md = new MoveInputParser("N4h2").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Knight);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(1, 7)));
            Assert.IsTrue(md.StartPosition.Column == Position.InvalidCoordinate);
            Assert.IsTrue(md.StartPosition.Row == 3);
        }

        [Test]
        public void Ba1_should_return_bishop_position()
        {
            MoveDescriptor md = new MoveInputParser("Ba1").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Bishop);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(0, 0)));
            Assert.IsTrue(md.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void ba1_should_return_pawn_position_with_start_file()
        {
            MoveDescriptor md = new MoveInputParser("ba1").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Pawn);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(0, 0)));
            Assert.IsTrue(md.StartPosition.Column == 1);
            Assert.IsTrue(md.StartPosition.Row == Position.InvalidCoordinate);
        }

        [Test]
        public void e4_should_return_pawn_position()
        {
            MoveDescriptor md = new MoveInputParser("e4").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Pawn);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(3, 4)));
            Assert.IsTrue(md.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void oo_should_return_king_side_castle()
        {
            MoveDescriptor md = new MoveInputParser("o-o").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.King);
            Assert.IsTrue(md.IsKingSideCastle);
            Assert.IsTrue(md.TargetPosition.IsSame(Position.Invalid));
            Assert.IsTrue(md.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void ooo_should_return_queen_side_castle()
        {
            MoveDescriptor md = new MoveInputParser("o-o-o").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.King);
            Assert.IsTrue(md.IsQueenSideCastle);
            Assert.IsTrue(md.TargetPosition.IsSame(Position.Invalid));
            Assert.IsTrue(md.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void exf5_should_return_pawn_position_with_kill()
        {
            MoveDescriptor md = new MoveInputParser("exf5").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Pawn);
            Assert.IsTrue(md.IsKill);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(4, 5)));
            Assert.IsTrue(md.StartPosition.Row == Position.InvalidCoordinate);
            Assert.IsTrue(md.StartPosition.Column == 4);
        }

        [Test]
        public void ra1xa8_should_return_rook_position_with_kill()
        {
            MoveDescriptor md = new MoveInputParser("Ra1xa8").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.Rook);
            Assert.IsTrue(md.IsKill);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(7, 0)));
            Assert.IsTrue(md.StartPosition.IsSame(new Position(0, 0)));
        }

        [Test]
        public void kf5_should_return_king_position()
        {
            MoveDescriptor md = new MoveInputParser("Kf5").MoveDescriptor;
            Assert.AreEqual(md.PieceType, PieceType.King);
            Assert.IsFalse(md.IsKill);
            Assert.IsTrue(md.TargetPosition.IsSame(new Position(4, 5)));
            Assert.IsTrue(md.StartPosition.Row == Position.InvalidCoordinate);
            Assert.IsTrue(md.StartPosition.Column == Position.InvalidCoordinate);
        }
    }
}
