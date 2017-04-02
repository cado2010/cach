using NUnit.Framework;
using cachCore.models;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class BoardInitTests
    {
        [Test]
        public void test_board_square_colors()
        {
            Board board = new Board();
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
        public void test_board_piece_colors()
        {
            Board board = new Board();
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
        public void test_board_black_piece_types()
        {
            test_board_piece_types(ItemColor.Black);
        }

        [Test]
        public void test_board_white_piece_types()
        {
            test_board_piece_types(ItemColor.White);
        }

        private void test_board_piece_types(ItemColor pieceColor)
        {
            int row = pieceColor == ItemColor.Black ? 7 : 0;

            Board board = new Board();
            Assert.IsTrue(board[row, 0].Piece.PieceType == PieceType.Rook);
            Assert.IsTrue(board[row, 1].Piece.PieceType == PieceType.Knight);
            Assert.IsTrue(board[row, 2].Piece.PieceType == PieceType.Bishop);
            Assert.IsTrue(board[row, 3].Piece.PieceType == PieceType.Queen);
            Assert.IsTrue(board[row, 4].Piece.PieceType == PieceType.King);
            Assert.IsTrue(board[row, 5].Piece.PieceType == PieceType.Bishop);
            Assert.IsTrue(board[row, 6].Piece.PieceType == PieceType.Knight);
            Assert.IsTrue(board[row, 7].Piece.PieceType == PieceType.Rook);

            row = pieceColor == ItemColor.Black ? 6 : 1;
            for (int col = 0; col < 8; col++)
            {
                Assert.IsTrue(board[row, col].Piece.PieceType == PieceType.Pawn);
            }
        }
    }
}
