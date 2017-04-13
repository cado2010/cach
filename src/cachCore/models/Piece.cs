using System.Collections.Generic;
using System.Collections.Concurrent;
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

        public bool IsPromoted { get; private set; }
        public Piece PromotedTo { get; private set; }

        /// <summary>
        /// Instance tracking map: <id> -> Piece
        /// </summary>
        [JsonIgnore]
        private static ConcurrentDictionary<string, Piece> _pieceMap;

        static Piece()
        {
            _pieceMap = new ConcurrentDictionary<string, Piece>();
        }

        public static Piece Get(string id)
        {
            return _pieceMap[id];
        }

        public static void Put(Piece piece)
        {
            _pieceMap[piece.Id] = piece;
        }

        public static void Remove(string id)
        {
            Piece p;
            _pieceMap.TryRemove(id, out p);
        }

        public static void Remove(IList<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                Piece p;
                _pieceMap.TryRemove(piece.Id, out p);
            }
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
                _pieceMap.TryAdd(Id, this);
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

        public void Unkill()
        {
            if (IsAlive)
            {
                throw new CachException("Piece is alive");
            }

            IsAlive = true;
        }

        public void Promote(Piece promotedTo)
        {
            IsPromoted = true;
            PromotedTo = promotedTo;
        }

        public void UnPromote()
        {
            IsPromoted = false;
            PromotedTo = null;
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
            if (Position.IsInvalid)
            {
                throw new CachException("Piece position is invalid: " + Position);
            }
        }
    }
}
