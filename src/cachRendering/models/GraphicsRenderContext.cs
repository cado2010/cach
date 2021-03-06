﻿using System.Drawing;
using cachCore.models;
using cachCore.enums;
using System;

namespace cachRendering.models
{
    public class GraphicsRenderContext : IRenderContext
    {
        public Board Board { get; set; }

        public ItemColor ToPlay { get; set; }

        public Point LeftUpperOffset { get; set; }

        public int TileSize { get; set; }

        public int BorderSize { get; set; }

        public Graphics Graphics { get; set; }

        public Position HighlitePosition { get; set; }
    }
}
