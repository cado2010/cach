using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using cachCore.exceptions;

namespace cachCore.utils
{
    public class PGNParser
    {
        public IList<string> Plies { get; private set; }

        private int _moveNumber;

        public PGNParser(string pgn)
        {
            _moveNumber = 1;
            Plies = new List<string>();
            Parse(pgn);
        }

        private void Parse(string pgn)
        {
            string pgnF = null;

            // trim game end result from steps
            Regex r = new Regex(@"(?s)(^.*)\s+(1/2-1/2|0-1|1-0)?$");
            if (r.IsMatch(pgn))
            {
                var mc = r.Matches(pgn);
                if (mc.Count > 0)
                {
                    Match m = mc[0];
                    var gc = m.Groups;
                    if (gc.Count > 1)
                    {
                        pgnF = gc[1].Value;
                    }
                }
            }
            else
                pgnF = pgn;

            if (pgnF == null)
                return;

            string[] parts = pgnF.Trim().Split(new string[] { "\r\n", "\n", " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Matches(@"\d+\.\s*(.+)"))
                {
                    var matches = Regex.Matches(parts[i], @"(\d+)\.\s*(.+)");
                    int num;
                    if (int.TryParse(matches[0].Groups[1].Value, out num))
                    {
                        if (num != _moveNumber)
                        {
                            throw new CachException($"Unexpected move sequence number: {num}");
                        }
                    }
                    _moveNumber++;

                    Plies.Add(matches[0].Groups[2].Value.Trim());
                }
                else if (parts[i].Matches(@"\d+\."))
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
                    Plies.Add(parts[i].Trim());
                }
            }
        }
    }
}
