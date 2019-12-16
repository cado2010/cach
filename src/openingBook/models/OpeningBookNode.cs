using System;
using System.Collections.Generic;
using System.IO;

namespace openingBook.models
{
    public class OpeningBookNode
    {
        private string _ply;
        private string _originalPly;
        private Dictionary<string, OpeningBookNode> _children;

        public OpeningBookNode()
        {
            Ply = "";
            VariationName = "";
            _children = new Dictionary<string, OpeningBookNode>(50);
        }

        public OpeningBookNode(string ply, IList<OpeningBookNode> children = null, string variationName = "")
        {
            Ply = ply;

            _children = new Dictionary<string, OpeningBookNode>(50);
            if (children != null)
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }

            VariationName = variationName;
        }

        public OpeningBookNode AddChild(string ply, IList<OpeningBookNode> children = null, string variationName = "")
        {
            OpeningBookNode node = new OpeningBookNode(ply, children, variationName);
            AddChild(node);

            return node;
        }

        public void AddChild(OpeningBookNode child)
        {
            if (child.Ply != null)
            {
                _children[child.Ply] = child;
            }
        }

        public void RemoveChild(string childPly)
        {
            if (_children.ContainsKey(childPly))
            {
                _children.Remove(childPly);
            }
        }

        public OpeningBookNode FindChild(string childPly)
        {
            if (_children.ContainsKey(childPly))
            {
                return _children[childPly];
            }
            else
            {
                return null;
            }
        }

        public IReadOnlyDictionary<string, OpeningBookNode> Children { get { return _children; } }

        /// <summary>
        /// Half move at this node without adornments
        /// </summary>
        public string Ply
        {
            get
            {
                return _ply;
            }

            private set
            {
                _originalPly = value;
                _ply = FixPly(_originalPly);
            }
        }

        public string OriginalPly
        {
            get { return _originalPly; }
        }

        public bool IsRoot => Ply == null || Ply == "";

        public string VariationName { get; private set; }

        /// <summary>
        /// Writes this and children (as specified) into the given stream
        /// </summary>
        /// <param name="writeStream"></param>
        public void Write(StreamWriter writeStream)
        {
            // written in format:
            // <num_children>||<OriginalPly>||<var_name>
            writeStream.WriteLine($"{_children.Count}||{OriginalPly}||{VariationName}");

            // write children
            foreach (var child in _children)
            {
                child.Value.Write(writeStream);
            }
        }

        /// <summary>
        /// Reads tree from given stream
        /// </summary>
        /// <param name="readStream"></param>
        /// <returns></returns>
        public bool Read(StreamReader readStream)
        {
            // read in format:
            // <num_children>||<OriginalPly>||<var_name>
            string line = readStream.ReadLine();
            if (line == null || line.Trim() == "")
            {
                return false;
            }

            string[] parts = line.Trim().Split(new string[] { "||" }, StringSplitOptions.None);

            int childrenCount = Int32.Parse(parts[0]);

            // read this node
            Ply = parts[1];
            VariationName = parts[2];

            // read children
            for (int i = 0; i < childrenCount; i++)
            {
                OpeningBookNode child = new OpeningBookNode();
                child.Read(readStream);

                _children[child.Ply] = child;
            }

            return true;
        }

        private string FixPly(string ply)
        {
            string fixedPly = ply;

            if (fixedPly.EndsWith("!!") || fixedPly.EndsWith("??") || fixedPly.EndsWith("!?") || fixedPly.EndsWith("?!"))
            {
                fixedPly = fixedPly.Substring(0, fixedPly.Length - 2);
            }
            if (fixedPly.EndsWith("+") || fixedPly.EndsWith("#") || fixedPly.EndsWith("!") || fixedPly.EndsWith("?"))
            {
                fixedPly = fixedPly.Substring(0, fixedPly.Length - 1);
            }

            return fixedPly;
        }
    }
}
