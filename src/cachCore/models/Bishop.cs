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
            paths.Add(Position.GetLeftUpPath());
            paths.Add(Position.GetRightUpPath());
            paths.Add(Position.GetLeftDownPath());
            paths.Add(Position.GetRightDownPath());

            return new Movement(Position, paths);
        }
    }
}
