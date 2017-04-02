using cachCore.enums;
using cachCore.exceptions;

namespace cachCore.models
{
    public class BoardSquare
    {
        public Position Position { get; private set; }
        public ItemColor SquareColor { get; private set; }
        public Piece Piece { get; private set; }

        public BoardSquare(Position position, ItemColor squareColor, Piece piece = null)
        {
            Position = position;
            SquareColor = squareColor;
            Piece = piece;
        }

        public bool IsOccupied()
        {
            return Piece != null;
        }

        public bool IsOccupiedByPieceOfColor(ItemColor pieceColor)
        {
            return Piece != null && Piece.PieceColor == pieceColor;
        }

        public void SetPiece(Piece piece)
        {
            if (piece == null || !piece.IsAlive)
            {
                throw new CachException("Invalid piece");
            }

            Piece = piece;
        }

        public void RemovePiece()
        {
            Piece = null;
        }
    }
}
