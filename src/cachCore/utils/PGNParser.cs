using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using cachCore.exceptions;

namespace cachCore.utils
{
    public class PGNParser
    {
        public IList<string> Moves { get; private set; }

        private int _moveNumber;

        public PGNParser(string pgn)
        {
            _moveNumber = 1;
            Moves = new List<string>();
            Parse(pgn);
        }

        private void Parse(string pgn)
        {
            string[] parts = pgn.Trim().Split(new string[] { "\r\n", "\n", " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Matches(@"\d+\."))
                {
                    var matches = Regex.Matches(parts[i], @"(\d+)\.");
                    int num;
                    if (int.TryParse(matches[0].Groups[1].Value, out num))
                    {
                        if (num != _moveNumber)
                        {
                            throw new CachException($"Unexpected move sequence number: {num}");
                        }
                    }
                    _moveNumber++;
                }
                else
                {
                    Moves.Add(parts[i].Trim());
                }
            }
        }
    }
}
