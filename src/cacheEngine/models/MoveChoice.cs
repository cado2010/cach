﻿using cachCore.models;

namespace cacheEngine.models
{
    public class MoveChoice
    {
        public Piece Piece { get; private set; }
        public MoveDescriptor MoveDescriptor { get; private set; }
        public int Value { get; private set; }

        public MoveChoice(Piece piece, MoveDescriptor md, int value)
        {
            Piece = piece;
            MoveDescriptor = md;
            Value = value;
        }

        public override string ToString()
        {
            return $"MoveChoice: Color={MoveDescriptor.PieceColor}, Piece={MoveDescriptor.PieceType}, Move={MoveDescriptor.Move}, Value={Value}";
        }
    }
}
