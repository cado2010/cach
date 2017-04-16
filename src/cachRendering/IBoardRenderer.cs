using cachRendering.models;
using System.IO;

namespace cachRendering
{
    public interface IBoardRenderer
    {
        void Render(IRenderContext renderContext);
        bool RenderAsImage(IRenderContext renderContext, MemoryStream memStream);
    }
}
