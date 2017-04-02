using System;
using System.Collections.Generic;
using cachCore.enums;

namespace cachCore.models
{
    public class Rook : Piece
    {
        public Rook(ItemColor pieceColor, Position position) :
            base(PieceType.Rook, pieceColor, position)
        {
        }

        protected override Movement GetUnconstrainedMovement()
        {
            List<IList<Position>> paths = new List<IList<Position>>();
            paths.Add(GetLeftPath());
            paths.Add(GetRightPath());
            paths.Add(GetUpPath());
            paths.Add(GetDownPath());

            return new Movement(Position, paths);
        }
    }
}
