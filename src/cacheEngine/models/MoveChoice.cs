using cachCore.models;

namespace cacheEngine.models
{
    public class MoveChoice
    {
        public Piece Piece { get; private set; }
        public MoveDescriptor MoveDescriptor { get; private set; }
        public int Value { get; private set; }
        public string Move { get; private set; }
        public bool FromOpeningBook { get; private set; }

        public MoveChoice(Piece piece, MoveDescriptor md, int value, string move = null, bool fromOpeningBook = false)
        {
            Piece = piece;
            MoveDescriptor = md;
            Value = value;
            Move = move;
            FromOpeningBook = fromOpeningBook;
        }

        public override string ToString()
        {
            return $"MoveChoice: Color={(MoveDescriptor != null ? MoveDescriptor.PieceColor.ToString() : "")}, " +
                $"Piece={(MoveDescriptor != null ? MoveDescriptor.PieceType.ToString() : "")}, " +
                $"Move={(MoveDescriptor != null ? MoveDescriptor.Move : "")}, Value={Value}, " +
                $"Move={Move ?? "[none]"}, FromOpeningBook={FromOpeningBook}";
        }
    }
}
