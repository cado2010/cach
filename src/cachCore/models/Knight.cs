using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class Knight : Piece
    {
        public Knight(ItemColor pieceColor, Position position, bool isTemp = false) :
            base(PieceType.Knight, pieceColor, position, isTemp)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();
            paths.Add(new List<Position> { Position.Left.Down.Down });
            paths.Add(new List<Position> { Position.Left.Left.Down });
            paths.Add(new List<Position> { Position.Left.Up.Up });
            paths.Add(new List<Position> { Position.Left.Left.Up });
            paths.Add(new List<Position> { Position.Right.Down.Down });
            paths.Add(new List<Position> { Position.Right.Right.Down });
            paths.Add(new List<Position> { Position.Right.Up.Up });
            paths.Add(new List<Position> { Position.Right.Right.Up });

            return new Movement(Position, paths);
        }
    }
}
