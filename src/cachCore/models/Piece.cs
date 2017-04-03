using System.Collections.Generic;
using Newtonsoft.Json;
using cachCore.enums;
using cachCore.exceptions;

namespace cachCore.models
{
    public abstract class Piece
    {
        /// <summary>
        /// Unique ID of this piece
        /// </summary>
        public string Id { get; private set; }

        public ItemColor PieceColor { get; private set; }

        public Position Position { get; private set; }

        public bool IsAlive { get; private set; }

        public PieceType PieceType { get; private set; }

        public bool HasMoved { get; private set; }

        /// <summary>
        /// Instance tracking map: <id> -> Piece
        /// </summary>
        [JsonIgnore]
        private static Dictionary<string, Piece> _pieceMap;

        static Piece()
        {
            _pieceMap = new Dictionary<string, Piece>();
        }

        public static Piece Get(string id)
        {
            return _pieceMap[id];
        }

        public static void Put(Piece piece)
        {
            _pieceMap[piece.Id] = piece;
        }

        protected Piece(PieceType pieceType, ItemColor pieceColor, Position position, bool isTemp = false)
        {
            PieceType = pieceType;
            PieceColor = pieceColor;
            Position = position;
            IsAlive = true;

            CheckSanity();

            if (!isTemp)
            {
                Id = System.Guid.NewGuid().ToString();
                _pieceMap[Id] = this;
            }
        }

        /// <summary>
        /// Returns all possible paths for this piece (unconstrained)
        /// </summary>
        /// <returns></returns>
        public Movement GetMovement()
        {
            CheckSanity();

            Movement m = GetUnconstrainedMovement();
            m.PruneOutOfBoundPositions();
            return m;
        }

        public void MoveTo(Position position)
        {
            CheckSanity();
            Position = position;
            HasMoved = true;
        }

        public void Kill()
        {
            CheckSanity();
            IsAlive = false;
        }

        /// <summary>
        /// Implemented per derived piece
        /// </summary>
        /// <returns></returns>
        protected abstract Movement GetUnconstrainedMovement();

        private void CheckSanity()
        {
            if (!IsAlive)
            {
                throw new CachException("Piece is dead");
            }
            if (Position.IsInvalid())
            {
                throw new CachException("Piece position is invalid: " + Position);
            }
        }
    }
}
