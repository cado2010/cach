using System;
using System.Collections.Generic;
using System.Linq;
using openingBook.models;

namespace openingBook
{
    public class OpeningBookSearch
    {
        private OpeningBookNode _root;

        public OpeningBookSearch(OpeningBookNode root)
        {
            _root = root;
        }

        /// <summary>
        /// Given the current state of the game, if the game has not progressed past Opening Book
        /// depth or has not diverged from the Opening Book, then returns all the next possible moves
        /// from Opening Book
        /// </summary>
        /// <param name="pgnGame"></param>
        /// <returns></returns>
        public IList<string> SearchMoves(IList<string> pgnGame)
        {
            if (pgnGame.Count >= OpeningBookBuilder.MaxBookDepthPlies)
            {
                // Opening Book only goes so far
                return null;
            }

            bool canFindFromOb = true;
            OpeningBookNode node = _root;
            foreach (var ply in pgnGame)
            {
                OpeningBookNode f = node.FindChild(ply);
                if (f == null)
                {
                    // game is diverging from Opening Book - we can no longer use OB to suggest
                    // next move from it
                    canFindFromOb = false;
                    break;
                }
                node = f;
            }

            if (canFindFromOb && node.Children.Count > 0)
            {
                // return all children from last matched node in Opening Book
                return node.Children.Values.Select(x => x.Ply).ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
