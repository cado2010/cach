﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using cachCore.enums;
using cachCore.models;
using cachRendering.models;

namespace cachRendering
{
    public class BoardRenderer : IBoardRenderer
    {
        private const int GridSize = 8;

        private static Dictionary<ItemColor, Dictionary<PieceType, Image>> _pieceImageMap;

        private Pen _outlinePen;
        private Pen _lastMovePen;

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
            _outlinePen = new Pen(Pens.Azure.Color, 4);
            _lastMovePen = new Pen(Pens.LightSeaGreen.Color, 4);
        }

        public bool RenderAsImage(IRenderContext renderContext, MemoryStream memStream)
        {
            try
            {
                int imgSize = GridSize * renderContext.TileSize + renderContext.BorderSize * 2;
                using (var bmp = new Bitmap(imgSize, imgSize))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillRectangle(Brushes.LightGray, 0, 0, imgSize, imgSize);

                        Render(g, renderContext.Board, renderContext.ToPlay,
                            renderContext.LeftUpperOffset,
                            renderContext.TileSize, renderContext.BorderSize,
                            Position.Invalid);
                    }

                    memStream.Seek(0, SeekOrigin.Begin);
                    bmp.Save(memStream, ImageFormat.Png);
                    memStream.Seek(0, SeekOrigin.Begin);
                    // Image im = Image.FromStream(memStream);
                    // im.Save(@"e:\tmp\cach\test.png");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public void Render(IRenderContext renderContext)
        {
            GraphicsRenderContext grc = renderContext as GraphicsRenderContext;
            Render(grc.Graphics, grc.Board, grc.ToPlay, grc.LeftUpperOffset, grc.TileSize, grc.BorderSize, grc.HighlitePosition);
        }

        private void PaintBoard(Graphics g, ItemColor toPlay, Point luOffset, int tileSize, int borderSize, string lastMove, Position highlitePosition)
        {
            Brush brush;

            Position lastMovePosition = Position.Invalid;
            int lmUiRow = Position.Invalid.Row;
            int lmUiCol = Position.Invalid.Column;
            if (lastMove != null)
            {
                try
                {
                    lastMovePosition = Position.FromAlgebraic(lastMove.Substring(lastMove.Length - 2));

                    if (toPlay == ItemColor.White)
                    {
                        lmUiRow = 7 - lastMovePosition.Row;
                        lmUiCol = lastMovePosition.Column;
                    }
                    else
                    {
                        lmUiRow = lastMovePosition.Row;
                        lmUiCol = 7 - lastMovePosition.Column;
                    }
                }
                catch(Exception)
                {
                    lmUiRow = Position.Invalid.Row;
                    lmUiCol = Position.Invalid.Column;
                }
            }

            // double for loop to handle all rows and columns
            for (var row = 0; row < GridSize; row++)
            {
                for (var col = 0; col < GridSize; col++)
                {
                    // create new Panel control which will be one 
                    // chess board tile
                    Size sz = new Size(tileSize, tileSize);
                    Point loc = new Point(tileSize * col + luOffset.X + borderSize, tileSize * row + luOffset.Y + borderSize);

                    // color the backgrounds
                    if (row % 2 == 0)
                        brush = col % 2 != 0 ? Brushes.SaddleBrown : Brushes.SandyBrown;
                    else
                        brush = col % 2 != 0 ? Brushes.SandyBrown : Brushes.SaddleBrown;

                    g.FillRectangle(brush, loc.X, loc.Y, tileSize, tileSize);

                    if (col == lmUiCol && row == lmUiRow)
                    {
                        g.DrawRectangle(_lastMovePen, loc.X, loc.Y, tileSize, tileSize);
                    }
                }
            }

            if (highlitePosition.IsValid)
            {
                Point loc;
                if (toPlay == ItemColor.White)
                {
                    loc = new Point(tileSize * highlitePosition.Column + luOffset.X + borderSize,
                        tileSize * (7 - highlitePosition.Row) + luOffset.Y + borderSize);
                }
                else
                {
                    loc = new Point(tileSize * (7 - highlitePosition.Column) + luOffset.X + borderSize,
                        tileSize * highlitePosition.Row + luOffset.Y + borderSize);
                }
                g.DrawRectangle(_outlinePen, loc.X, loc.Y, tileSize, tileSize);
            }

            Font font = SystemFonts.DefaultFont;

            int xOff = tileSize / 2 - 4;
            for (int col = 0; col < GridSize; col++)
            {
                PointF loc = new Point(tileSize * col + xOff + luOffset.X + borderSize,
                    tileSize * GridSize + 2 + luOffset.Y + borderSize);
                int c = toPlay == ItemColor.White ? (97 + col) : (97 + 7 - col);
                g.DrawString(Convert.ToChar(c).ToString(), font, Brushes.Black, loc);

                loc = new Point(tileSize * col + xOff + luOffset.X + borderSize, luOffset.Y + borderSize - 16);
                g.DrawString(Convert.ToChar(c).ToString(), font, Brushes.Black, loc);
            }

            int yOff = tileSize / 2 - font.Height / 2;
            for (int row = 0; row < GridSize; row++)
            {
                PointF loc = new Point(tileSize * GridSize + 2 + luOffset.X + borderSize,
                    tileSize * row + yOff + luOffset.Y + borderSize);
                int c = toPlay == ItemColor.White ? (49 + 7 - row) : (49 + row);
                g.DrawString(Convert.ToChar(c).ToString(), font, Brushes.Black, loc);

                loc = new Point(luOffset.X + borderSize - 12, tileSize * row + yOff + luOffset.Y + borderSize);
                g.DrawString(Convert.ToChar(c).ToString(), font, Brushes.Black, loc);
            }
        }

        private void Render(Graphics g, Board board, ItemColor toPlay, Point luOffset, int tileSize, int borderSize, Position highlitePosition)
        {
            // render board
            PaintBoard(g, toPlay, luOffset, tileSize, borderSize, board.LastMove, highlitePosition);

            // render pieces
            float imgPerc = 1.0f;
            float imgSide = tileSize * imgPerc;
            float imgOff = tileSize * ((1 - imgPerc) / 2.0f);
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int r = toPlay == ItemColor.White ? (7 - row) : row;
                    int c = toPlay == ItemColor.White ? col : (7 - col);

                    BoardSquare square = board[r, c];
                    if (square.IsOccupied())
                    {
                        ItemColor pieceColor = square.Piece.PieceColor;
                        PieceType pieceType = square.Piece.PieceType;
                        Image pieceImage = _pieceImageMap[pieceColor][pieceType];
                        RectangleF loc = new RectangleF(
                            new PointF(tileSize * col + imgOff + luOffset.X + borderSize,
                                tileSize * row + imgOff + luOffset.Y + borderSize),
                            new SizeF(imgSide, imgSide));

                        g.DrawImage(pieceImage, loc);
                    }
                }
            }
        }
    }
}
