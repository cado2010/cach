using System;
using System.Collections.Generic;
using System.Drawing;
using cachCore.enums;
using cachCore.models;
using cachRendering.models;

namespace cachRendering
{
    public class BoardRenderer : IBoardRenderer
    {
        private const int GridSize = 8;
        private const int BorderSize = 20;

        private static Dictionary<ItemColor, Dictionary<PieceType, Image>> _pieceImageMap;

        static BoardRenderer()
        {
            _pieceImageMap = new Dictionary<ItemColor, Dictionary<PieceType, Image>>()
            {
                {
                    ItemColor.Black,
                    new Dictionary<PieceType, Image>()
                    {
                        { PieceType.King, Resource.bK },
                        { PieceType.Queen, Resource.bQ },
                        { PieceType.Rook, Resource.bR },
                        { PieceType.Bishop, Resource.bB },
                        { PieceType.Knight, Resource.bN },
                        { PieceType.Pawn, Resource.bp },
                    }
                },
                {
                    ItemColor.White,
                    new Dictionary<PieceType, Image>()
                    {
                        { PieceType.King, Resource.wK },
                        { PieceType.Queen, Resource.wQ },
                        { PieceType.Rook, Resource.wR },
                        { PieceType.Bishop, Resource.wB },
                        { PieceType.Knight, Resource.wN },
                        { PieceType.Pawn, Resource.wp },
                    }
                }
            };
        }

        public BoardRenderer()
        {
        }

        public Image RenderAsImage(IRenderContext renderContext)
        {
            throw new NotImplementedException();
        }

        public void Render(IRenderContext renderContext)
        {
            GraphicsRenderContext grc = renderContext as GraphicsRenderContext;
            Render(grc.Graphics, grc.Board, grc.ToPlay, grc.LeftUpperOffset, grc.TileSize);
        }

        private void PaintBoard(Graphics g, ItemColor toPlay, Point luOffset, int tileSize)
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;
            Brush brush;

            // double for loop to handle all rows and columns
            for (var row = 0; row < GridSize; row++)
            {
                for (var col = 0; col < GridSize; col++)
                {
                    // create new Panel control which will be one 
                    // chess board tile
                    Size sz = new Size(tileSize, tileSize);
                    Point loc = new Point(tileSize * row + luOffset.X + BorderSize, tileSize * col + luOffset.Y + BorderSize);

                    // color the backgrounds
                    if (row % 2 == 0)
                        brush = col % 2 != 0 ? Brushes.DarkGray : Brushes.White;
                    else
                        brush = col % 2 != 0 ? Brushes.White : Brushes.DarkGray;

                    g.FillRectangle(brush, loc.X, loc.Y, tileSize, tileSize);
                }
            }

            for (int col = 0; col < 8; col++)
            {
                PointF loc = new Point(tileSize * col + 35 + luOffset.X + BorderSize, tileSize * 8 + 2 + luOffset.Y + BorderSize);
                int c = toPlay == ItemColor.White ? (97 + col) : (97 + 7 - col);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);

                loc = new Point(tileSize * col + 35 + luOffset.X + BorderSize, luOffset.Y + BorderSize - 16);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);
            }

            for (int row = 0; row < 8; row++)
            {
                PointF loc = new Point(tileSize * 8 + 2 + luOffset.X + BorderSize, tileSize * row + 34 + luOffset.Y + BorderSize);
                int c = toPlay == ItemColor.White ? (49 + 7 - row) : (49 + row);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);

                loc = new Point(luOffset.X + BorderSize - 14, tileSize * row + 34 + luOffset.Y + BorderSize);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);
            }
        }

        private void Render(Graphics g, Board board, ItemColor toPlay, Point luOffset, int tileSize)
        {
            // render board
            PaintBoard(g, toPlay, luOffset, tileSize);

            // render pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int r = toPlay == ItemColor.White ? (7 - row) : row;
                    int c = toPlay == ItemColor.White ? col : (7 - col);

                    BoardSquare square = board[r, c];
                    if (square.IsOccupied())
                    {
                        ItemColor pieceColor = square.Piece.PieceColor;
                        PieceType pieceType = square.Piece.PieceType;
                        Image pieceImage = _pieceImageMap[pieceColor][pieceType];
                        Point loc = new Point(tileSize * col + 8 + luOffset.X + BorderSize, tileSize * row + 8 + luOffset.Y + BorderSize);

                        g.DrawImage(pieceImage, loc);
                    }
                }
            }
        }
    }
}
