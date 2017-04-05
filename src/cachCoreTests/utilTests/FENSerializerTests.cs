using NUnit.Framework;
using cachCore.models;
using cachCore.enums;
using cachCore.utils;

namespace cachCoreTests
{
    [TestFixture]
    public class FENSerializerTests
    {
        [Test]
        public void fenser_test_board_square_colors()
        {
            Board board = FENSerializer.BoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -");
            Assert.IsTrue(board[0, 0].SquareColor == ItemColor.Black);
            Assert.IsTrue(board[1, 1].SquareColor == ItemColor.Black);
            Assert.IsTrue(board[0, 1].SquareColor == ItemColor.White);
            Assert.IsTrue(board[1, 0].SquareColor == ItemColor.White);
            Assert.IsTrue(board[7, 7].SquareColor == ItemColor.Black);
            Assert.IsTrue(board[2, 0].SquareColor == ItemColor.Black);
            Assert.IsTrue(board[3, 0].SquareColor == ItemColor.White);
            Assert.IsTrue(board[4, 5].SquareColor == ItemColor.White);
        }

        [Test]
        public void fenser_test_board_piece_colors()
        {
            Board board = FENSerializer.BoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -");
            Assert.IsTrue(board[0, 0].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[0, 1].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[1, 0].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[1, 7].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[6, 0].Piece.PieceColor == ItemColor.Black);
            Assert.IsTrue(board[6, 1].Piece.PieceColor == ItemColor.Black);
            Assert.IsTrue(board[7, 0].Piece.PieceColor == ItemColor.Black);
            Assert.IsTrue(board[7, 7].Piece.PieceColor == ItemColor.Black);

            Assert.IsTrue(board[2, 2].Piece == null);
            Assert.IsTrue(board[3, 3].Piece == null);
            Assert.IsTrue(board[4, 4].Piece == null);
            Assert.IsTrue(board[5, 5].Piece == null);
            Assert.IsTrue(board[6, 6].Piece != null);
        }

        [Test]
        public void fenser_test_board_black_piece_types()
        {
            testBoardPieceTypes(ItemColor.Black);
        }

        [Test]
        public void fenser_test_board_white_piece_types()
        {
            testBoardPieceTypes(ItemColor.White);
        }

        [Test]
        public void fenser_test_specific_fen_1()
        {
            Board board = FENSerializer.BoardFromFEN("8/8/8/3k4/3Q4/3K4/8/8 w - -");
            Position pos0 = Position.FromAlgebraic("D3");
            Position pos1 = Position.FromAlgebraic("d4");
            Position pos2 = Position.FromAlgebraic("d5");
            Assert.IsTrue(board[pos0].Piece.PieceType ==
                PieceType.King && board[pos0].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[pos1].Piece.PieceType ==
                PieceType.Queen && board[pos1].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(board[pos2].Piece.PieceType ==
                PieceType.King && board[pos2].Piece.PieceColor == ItemColor.Black);
        }

        [Test]
        public void fenser_test_specific_fen_2()
        {
            Board board = FENSerializer.BoardFromFEN("8/8/8/3k4/3n1r2/8/5K2/8 w - -");
            Position pos0 = Position.FromAlgebraic("f2");
            Position pos1 = Position.FromAlgebraic("f3");
            Position pos2 = Position.FromAlgebraic("F4");
            Position pos3 = Position.FromAlgebraic("d4");
            Position pos4 = Position.FromAlgebraic("d5");
            Assert.IsTrue(board[pos0].Piece.PieceType == PieceType.King &&
                board[pos0].Piece.PieceColor == ItemColor.White);
            Assert.IsTrue(!board[pos1].IsOccupied());
            Assert.IsTrue(board[pos2].Piece.PieceType == PieceType.Rook &&
                board[pos2].Piece.PieceColor == ItemColor.Black);
            Assert.IsTrue(board[pos3].Piece.PieceType == PieceType.Knight &&
                board[pos3].Piece.PieceColor == ItemColor.Black);
            Assert.IsTrue(board[pos4].Piece.PieceType == PieceType.King &&
                board[pos4].Piece.PieceColor == ItemColor.Black);
        }

        private void testBoardPieceTypes(ItemColor pieceColor)
        {
            int row = BoardUtils.GetPieceStartRow(pieceColor);

            Board board = FENSerializer.BoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -");
            Assert.IsTrue(board[row, 0].Piece.PieceType == PieceType.Rook);
            Assert.IsTrue(board[row, 1].Piece.PieceType == PieceType.Knight);
            Assert.IsTrue(board[row, 2].Piece.PieceType == PieceType.Bishop);
            Assert.IsTrue(board[row, 3].Piece.PieceType == PieceType.Queen);
            Assert.IsTrue(board[row, 4].Piece.PieceType == PieceType.King);
            Assert.IsTrue(board[row, 5].Piece.PieceType == PieceType.Bishop);
            Assert.IsTrue(board[row, 6].Piece.PieceType == PieceType.Knight);
            Assert.IsTrue(board[row, 7].Piece.PieceType == PieceType.Rook);

            row = BoardUtils.GetPawnStartRow(pieceColor);
            for (int col = 0; col < 8; col++)
            {
                Assert.IsTrue(board[row, col].Piece.PieceType == PieceType.Pawn);
            }
        }
    }
}
