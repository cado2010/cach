using System;
using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class Queen : Piece
    {
        public Queen(ItemColor pieceColor, Position position) :
            base(PieceType.Queen, pieceColor, position)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();
            paths.Add(Position.GetLeftPath());
            paths.Add(Position.GetRightPath());
            paths.Add(Position.GetUpPath());
            paths.Add(Position.GetDownPath());
            paths.Add(Position.GetLeftUpPath());
            paths.Add(Position.GetRightUpPath());
            paths.Add(Position.GetLeftDownPath());
            paths.Add(Position.GetRightDownPath());

            return new Movement(Position, paths);
        }
    }
}
