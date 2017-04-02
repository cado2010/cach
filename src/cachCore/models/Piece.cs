using System.Collections.Generic;
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

        /// <summary>
        /// Instance tracking map: <id> -> Piece
        /// </summary>
        private static Dictionary<string, Piece> _pieceMap;

        static Piece()
        {
            _pieceMap = new Dictionary<string, Piece>();
        }

        public static Piece Get(string id)
        {
            return _pieceMap[id];
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
        }

        public void Kill()
        {
            CheckSanity();
            IsAlive = false;
        }

        public IList<Position> GetLeftPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.Left;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.Left;
            }

            return path;
        }

        public IList<Position> GetRightPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.Right;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.Right;
            }

            return path;
        }

        public IList<Position> GetUpPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.Up;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.Up;
            }

            return path;
        }

        public IList<Position> GetDownPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.Down;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.Down;
            }

            return path;
        }

        public IList<Position> GetLeftUpPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.LeftUp;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.LeftUp;
            }

            return path;
        }

        public IList<Position> GetRightUpPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.RightUp;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.RightUp;
            }

            return path;
        }

        public IList<Position> GetLeftDownPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.LeftDown;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.LeftDown;
            }

            return path;
        }

        public IList<Position> GetRightDownPath()
        {
            IList<Position> path = new List<Position>();
            Position n = Position.RightDown;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.RightDown;
            }

            return path;
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
