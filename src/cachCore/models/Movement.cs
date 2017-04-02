using System.Collections.Generic;

namespace cachCore.models
{
    public class Movement
    {
        public Position Start { get; private set; }
        public IList<IList<Position>> Paths { get; private set; }
        public bool Constrained { get; private set; }

        public Movement(Position start, IList<IList<Position>> paths, bool constrained = false)
        {
            Start = start;
            Paths = paths;
            Constrained = constrained;
        }

        /// <summary>
        /// Returns true if this movement includes the given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Includes(Position position)
        {
            bool includes = false;

            foreach (var path in Paths)
            {
                foreach (var pos in path)
                {
                    if (pos.IsSame(position))
                    {
                        includes = true;
                        break;
                    }
                }
            }

            return includes;
        }

        public void PruneOutOfBoundPositions()
        {
            IList<IList<Position>> prunedPaths = new List<IList<Position>>();

            foreach (var path in Paths)
            {
                List<Position> prunedPath = new List<Position>();

                // remove positions that are out of bounds
                foreach (var pos in path)
                {
                    if (!pos.IsOutOfBounds())
                        prunedPath.Add(pos);
                }

                if (prunedPath.Count > 0)
                {
                    prunedPaths.Add(prunedPath);
                }
            }

            Paths = prunedPaths;
        }
    }
}
