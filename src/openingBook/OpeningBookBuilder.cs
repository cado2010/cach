using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using openingBook.models;
using openingBook.utils;

namespace openingBook
{
    public class OpeningBookBuilder
    {
        public static readonly int MaxBookDepth = 10; // max number of moves in opening book
        public static readonly int MaxBookDepthPlies = MaxBookDepth * 2; // max number of plies in opening book

        public OpeningBookNode Root { get; private set; }

        private ILog _logger;

        public OpeningBookBuilder()
        {
            _logger = LogManager.GetLogger(GetType().Name);
            Root = new OpeningBookNode("");
        }

        /// <summary>
        /// Writes the OpeningBook into the given file
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(path))
                {
                    Root.Write(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Write: Exception writing OpeningBook into: {path}, message: {ex.Message}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Reads an OpeningBook from the given file
        /// </summary>
        /// <param name="path"></param>
        public void Read(string path)
        {
            try
            {
                Root = new OpeningBookNode();
                using (StreamReader stream = new StreamReader(path))
                {
                    Root.Read(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Write: Exception reading OpeningBook from: {path}, message: {ex.Message}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Adds to the current Opening Book tree by reading games from the given file (or string)
        /// </summary>
        /// <param name="pgnFilePathOrString"></param>
        /// <param name="isFile"></param>
        public void BuildFromPGN(string pgnFilePathOrString, bool isFile = true)
        {
            try
            {
                PGNMultiGameParser mgp = new PGNMultiGameParser(pgnFilePathOrString, isFile);
                foreach (var pgn in mgp.MultiGamePlies)
                {
                    AddFromPGN(pgn);
                }

                _logger.Info($"BuildFromPGN: parsed {mgp.MultiGamePlies.Count} games");
            }
            catch(Exception ex)
            {
                _logger.Error($"BuildFromPGN: Exception while parsing: {pgnFilePathOrString}, message: {ex.Message}",
                    ex);
            }
        }

        private void AddFromPGN(IList<string> pgn)
        {
            // first, start from root and find the deepest node in the tree that matches
            // the current game progression path
            OpeningBookNode node = Root;
            int i = 0;
            foreach (var ply in pgn)
            {
                OpeningBookNode f = node.FindChild(ply);
                if (f == null)
                {
                    // current node is the deepest node where the game progression until
                    // ply index "i-1" is matched, ply[i..n] needs to be added to the tree
                    break;
                }
                node = f;
                i++;
            }

            // next, add remainder of the game into the tree
            for (; i < MaxBookDepthPlies && i < pgn.Count; i++)
            {
                string ply = pgn[i];
                node = node.AddChild(ply);
            }
        }
    }
}
