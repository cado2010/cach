using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using cachCore.exceptions;

namespace cachCore.models
{
    public struct Position
    {
        private static Position INVALID;
        static Position()
        {
            INVALID = new Position(-1, -1);
        }

        /// <summary>
        /// a1 = (row=0, col=0) -> h8 = (7, 7)
        /// </summary>
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        [JsonIgnore]
        public static Position Invalid
        {
            get { return INVALID; }
        }

        [JsonIgnore]
        public static int InvalidCoordinate
        {
            get { return -1; }
        }

        public bool IsInvalid
        {
            get { return Row == InvalidCoordinate || Column == InvalidCoordinate || IsOutOfBounds(); }
        }

        public bool IsValid
        {
            get { return !IsInvalid; }
        }

        public string ToAlgebraic()
        {
            if (!IsOutOfBounds())
            {
                string rank = (Row + 1).ToString();
                string file = Convert.ToChar(97 + Column).ToString();
                return file + rank;
            }
            else
            {
                return "INVALID";
            }
        }

        /// <summary>
        /// Returns a Position from a given algebraic notation for like e4
        /// </summary>
        /// <param name="algPos"></param>
        /// <returns></returns>
        public static Position FromAlgebraic(string algPos)
        {
            if (algPos.Length != 2)
            {
                throw new CachException("Invalid algebraic notation: " + algPos);
            }
            int col = ((int) algPos[0]) - (char.IsLower(algPos[0]) ? 97 : 65);
            if (col < 0 || col > 7)
            {
                throw new CachException("Invalid File in algebraic notation: " + algPos);
            }

            int row = ((int) algPos[1]) - 49;
            if (row < 0 || row > 7)
            {
                throw new CachException("Invalid Rank in algebraic notation: " + algPos);
            }

            return new Position(row, col);
        }

        public static int ColumnFromFile(char file)
        {
            int col = ((int) file) - 97;
            if (col < 0 || col > 7)
            {
                throw new CachException("Invalid File in algebraic notation: " + file);
            }

            return col;
        }

        public static int RowFromRank(char rank)
        {
            int row = ((int) rank) - 49;
            if (row < 0 || row > 7)
            {
                throw new CachException("Invalid Rank in algebraic notation: " + rank);
            }

            return row;
        }

        public bool IsOutOfBounds()
        {
            return Column < 0 || Row < 0 || Column > 7 || Row > 7;
        }

        public bool IsSameRank(Position other)
        {
            return this.Row == other.Row;
        }

        public bool IsSameFile(Position other)
        {
            return this.Column == other.Column;
        }

        public bool IsSame(Position other)
        {
            return this.Row == other.Row && this.Column == other.Column;
        }

        public bool IsAt(int row, int col)
        {
            return Row == row && Column == col;
        }

        /// <summary>
        /// All movement directions are relative to White
        /// </summary>
        [JsonIgnore]
        public Position Up
        {
            get
            {
                return new Position(Row + 1, Column);
            }
        }

        [JsonIgnore]
        public Position Down
        {
            get
            {
                return new Position(Row - 1, Column);
            }
        }

        [JsonIgnore]
        public Position Left
        {
            get
            {
                return new Position(Row, Column - 1);
            }
        }

        [JsonIgnore]
        public Position Right
        {
            get
            {
                return new Position(Row, Column + 1);
            }
        }

        [JsonIgnore]
        public Position LeftUp
        {
            get
            {
                return new Position(Row + 1, Column - 1);
            }
        }

        [JsonIgnore]
        public Position RightUp
        {
            get
            {
                return new Position(Row + 1, Column + 1);
            }
        }

        [JsonIgnore]
        public Position LeftDown
        {
            get
            {
                return new Position(Row - 1, Column - 1);
            }
        }

        [JsonIgnore]
        public Position RightDown
        {
            get
            {
                return new Position(Row - 1, Column + 1);
            }
        }

        public IList<Position> GetLeftPath()
        {
            IList<Position> path = new List<Position>();
            Position n = this.Left;
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
            Position n = this.Right;
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
            Position n = this.Up;
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
            Position n = this.Down;
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
            Position n = this.LeftUp;
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
            Position n = this.RightUp;
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
            Position n = this.LeftDown;
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
            Position n = this.RightDown;
            while (!n.IsOutOfBounds())
            {
                path.Add(n);
                n = n.RightDown;
            }

            return path;
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}
