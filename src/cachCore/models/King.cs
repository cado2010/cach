using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class King : Piece
    {
        public King(ItemColor pieceColor, Position position) :
            base(PieceType.King, pieceColor, position)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();
            paths.Add(new List<Position> { Position.Left });
            paths.Add(new List<Position> { Position.Right });
            paths.Add(new List<Position> { Position.Up });
            paths.Add(new List<Position> { Position.Down });
            paths.Add(new List<Position> { Position.LeftUp });
            paths.Add(new List<Position> { Position.RightUp });
            paths.Add(new List<Position> { Position.LeftDown });
            paths.Add(new List<Position> { Position.RightDown });

            return new Movement(Position, paths);
        }
    }
}
