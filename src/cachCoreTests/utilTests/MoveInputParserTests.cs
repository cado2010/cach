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
            MoveInputParser parser = new MoveInputParser("Qe4");
            Assert.AreEqual(parser.PieceType, PieceType.Queen);
            Assert.IsTrue(parser.IsKill == false);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(3, 4)));
        }

        [Test]
        public void qxa1_should_return_queen_kill()
        {
            MoveInputParser parser = new MoveInputParser("Qxa1");
            Assert.AreEqual(parser.PieceType, PieceType.Queen);
            Assert.IsTrue(parser.IsKill == true);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(0, 0)));
        }

        [Test]
        public void nfh8_should_not_return_queen_position()
        {
            MoveInputParser parser = new MoveInputParser("Nfh8");
            Assert.AreNotEqual(parser.PieceType, PieceType.Queen);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(7, 7)));
        }

        [Test]
        public void nfh2_should_return_knight_position_with_start_file()
        {
            MoveInputParser parser = new MoveInputParser("Nfh2");
            Assert.AreEqual(parser.PieceType, PieceType.Knight);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(1, 7)));
            Assert.IsTrue(parser.StartPosition.Column == 5);
            Assert.IsTrue(parser.StartPosition.Row == Position.InvalidCoordinate);
        }

        [Test]
        public void n4h2_should_return_knight_position_with_start_file()
        {
            MoveInputParser parser = new MoveInputParser("N4h2");
            Assert.AreEqual(parser.PieceType, PieceType.Knight);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(1, 7)));
            Assert.IsTrue(parser.StartPosition.Column == Position.InvalidCoordinate);
            Assert.IsTrue(parser.StartPosition.Row == 3);
        }

        [Test]
        public void Ba1_should_return_bishop_position()
        {
            MoveInputParser parser = new MoveInputParser("Ba1");
            Assert.AreEqual(parser.PieceType, PieceType.Bishop);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(0, 0)));
            Assert.IsTrue(parser.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void ba1_should_return_pawn_position_with_start_file()
        {
            MoveInputParser parser = new MoveInputParser("ba1");
            Assert.AreEqual(parser.PieceType, PieceType.Pawn);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(0, 0)));
            Assert.IsTrue(parser.StartPosition.Column == 1);
            Assert.IsTrue(parser.StartPosition.Row == Position.InvalidCoordinate);
        }

        [Test]
        public void e4_should_return_pawn_position()
        {
            MoveInputParser parser = new MoveInputParser("e4");
            Assert.AreEqual(parser.PieceType, PieceType.Pawn);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(3, 4)));
            Assert.IsTrue(parser.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void oo_should_return_king_side_castle()
        {
            MoveInputParser parser = new MoveInputParser("o-o");
            Assert.AreEqual(parser.PieceType, PieceType.King);
            Assert.IsTrue(parser.IsKingSideCastle);
            Assert.IsTrue(parser.TargetPosition.IsSame(Position.Invalid));
            Assert.IsTrue(parser.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void ooo_should_return_queen_side_castle()
        {
            MoveInputParser parser = new MoveInputParser("o-o-o");
            Assert.AreEqual(parser.PieceType, PieceType.King);
            Assert.IsTrue(parser.IsQueenSideCastle);
            Assert.IsTrue(parser.TargetPosition.IsSame(Position.Invalid));
            Assert.IsTrue(parser.StartPosition.IsSame(Position.Invalid));
        }

        [Test]
        public void exf5_should_return_pawn_position_with_kill()
        {
            MoveInputParser parser = new MoveInputParser("exf5");
            Assert.AreEqual(parser.PieceType, PieceType.Pawn);
            Assert.IsTrue(parser.IsKill);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(4, 5)));
            Assert.IsTrue(parser.StartPosition.Row == Position.InvalidCoordinate);
            Assert.IsTrue(parser.StartPosition.Column == 4);
        }

        [Test]
        public void ra1xa8_should_return_rook_position_with_kill()
        {
            MoveInputParser parser = new MoveInputParser("Ra1xa8");
            Assert.AreEqual(parser.PieceType, PieceType.Rook);
            Assert.IsTrue(parser.IsKill);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(7, 0)));
            Assert.IsTrue(parser.StartPosition.IsSame(new Position(0, 0)));
        }

        [Test]
        public void kf5_should_return_king_position()
        {
            MoveInputParser parser = new MoveInputParser("Kf5");
            Assert.AreEqual(parser.PieceType, PieceType.King);
            Assert.IsFalse(parser.IsKill);
            Assert.IsTrue(parser.TargetPosition.IsSame(new Position(4, 5)));
            Assert.IsTrue(parser.StartPosition.Row == Position.InvalidCoordinate);
            Assert.IsTrue(parser.StartPosition.Column == Position.InvalidCoordinate);
        }
    }
}
