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
            paths.Add(GetLeftPath());
            paths.Add(GetRightPath());
            paths.Add(GetUpPath());
            paths.Add(GetDownPath());
            paths.Add(GetLeftUpPath());
            paths.Add(GetRightUpPath());
            paths.Add(GetLeftDownPath());
            paths.Add(GetRightDownPath());

            return new Movement(Position, paths);
        }
    }
}
