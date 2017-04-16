using System.Drawing;
using cachCore.models;
using cachCore.enums;

namespace cachRendering.models
{
    public interface IRenderContext
    {
        Board Board { get; set; }
        ItemColor ToPlay { get; set; }
        int TileSize { get; set; }
        int BorderSize { get; set; }
        Point LeftUpperOffset { get; set; }
    }
}
