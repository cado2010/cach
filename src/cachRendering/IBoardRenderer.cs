using System.Drawing;

namespace cachRendering
{
    interface IBoardRenderer
    {
        void Render(Graphics g, Point leftUpper);
        Image Render();
    }
}
