using System;
using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class Bishop : Piece
    {
        public Bishop(ItemColor pieceColor, Position position) :
            base(PieceType.Bishop, pieceColor, position)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();
            paths.Add(GetLeftUpPath());
            paths.Add(GetRightUpPath());
            paths.Add(GetLeftDownPath());
            paths.Add(GetRightDownPath());

            return new Movement(Position, paths);
        }
    }
}
