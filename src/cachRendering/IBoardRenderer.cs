using System.Drawing;
using cachRendering.models;

namespace cachRendering
{
    public interface IBoardRenderer
    {
        void Render(IRenderContext renderContext);
        Image RenderAsImage(IRenderContext renderContext);
    }
}
