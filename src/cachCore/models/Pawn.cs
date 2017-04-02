using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class Pawn : Piece
    {
        public Pawn(ItemColor pieceColor, Position position) :
            base(PieceType.Pawn, pieceColor, position)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();

            if (PieceColor == ItemColor.White)
            {
                paths.Add(new List<Position> { Position.Up });
                paths.Add(new List<Position> { Position.LeftUp });
                paths.Add(new List<Position> { Position.RightUp });
                if (Position.Row == 2)
                {
                    paths.Add(new List<Position> { Position.Up.Up });
                }
            }
            else
            {
                paths.Add(new List<Position> { Position.Down });
                paths.Add(new List<Position> { Position.LeftDown });
                paths.Add(new List<Position> { Position.RightDown });
                if (Position.Row == 7)
                {
                    paths.Add(new List<Position> { Position.Down.Down });
                }
            }

            return new Movement(Position, paths);
        }
    }
}
