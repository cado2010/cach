using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using cachCore;
using cachCore.utils;
using System.Text;

namespace openingBook.utils
{
    /// <summary>
    /// Class that reads a .pgn file (or direct string content) that contains multiple games
    /// with event info tags etc and returns a list of parsed Plies
    /// </summary>
    public class PGNMultiGameParser
    {
        private int _maxGamesPerFile;
        private ILog _logger;

        public IList<IList<string>> MultiGamePlies { get; private set; }

        public PGNMultiGameParser(string pgnFilePathOrString, bool isFile = true, int maxGamesPerFile = 1000)
        {
            _logger = LogManager.GetLogger(GetType().Name);
            _maxGamesPerFile = maxGamesPerFile;
            Parse(pgnFilePathOrString, isFile);
        }

        private void Parse(string pgnFilePathOrString, bool isFile = true)
        {
            try
            {
                MultiGamePlies = new List<IList<string>>();

                if (isFile)
                {
                    using (StreamReader file = new StreamReader(pgnFilePathOrString))
                    {
                        ParseStream(file);
                    }
                }
                else
                {
                    // PGN multi-line string
                    // convert string to stream
                    byte[] byteArray = Encoding.ASCII.GetBytes(pgnFilePathOrString);
                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            ParseStream(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception while parsing file/string: {pgnFilePathOrString}, message: {ex.Message}", ex);
                throw ex;
            }
        }

        private void ParseStream(StreamReader streamReader)
        {
            MultiGamePlies = new List<IList<string>>();

            string line;
            bool inPgn = false;
            string pgn = "";
            int gameCount = 0;

            try
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == "" || line.Matches(@"^\[.*\]$"))
                    {
                        // if currently reading game, parse the game read so far and exit reading mode
                        if (inPgn)
                        {
                            inPgn = false;
                            ParseGame(pgn);
                            pgn = "";

                            if (_maxGamesPerFile > 0 && ++gameCount >= _maxGamesPerFile)
                            {
                                break;
                            }
                        }

                        // ignore event tag info
                        continue;
                    }
                    else if (line.Matches(@"^1\."))
                    {
                        if (inPgn)
                        {
                            throw new Exception("Invalid syntax - encountered start of game when already reading game PGN");
                        }

                        inPgn = true;
                        pgn = line;
                    }
                    else if (inPgn)
                    {
                        pgn += " " + line;
                    }
                }

                // parse the last game if there was no empty line after it
                if (inPgn)
                {
                    inPgn = false;
                    ParseGame(pgn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ParseGame(string pgn)
        {
            if (pgn != null && pgn != "")
            {
                PGNParser parser = new PGNParser(pgn);
                MultiGamePlies.Add(parser.Plies);
            }
        }
    }
}
