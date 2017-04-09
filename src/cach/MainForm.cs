using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using cachCore.models;
using cachCore.enums;
using cachCore.controllers;

namespace cach
{
    public partial class MainForm : Form
    {
        private Game _game;

        private Dictionary<ItemColor, Dictionary<PieceType, Image>> _pieceImageMap;

        const int tileSize = 80;
        const int gridSize = 8;

        // event handler of Form Load... init things here
        private void MainForm_Load(object sender, EventArgs e)
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            _game = new GameController().CreateGame();
            _pieceImageMap = new Dictionary<ItemColor, Dictionary<PieceType, Image>>()
            {
                {
                    ItemColor.Black,
                    new Dictionary<PieceType, Image>()
                    {
                        { PieceType.King, Resource1.bK },
                        { PieceType.Queen, Resource1.bQ },
                        { PieceType.Rook, Resource1.bR },
                        { PieceType.Bishop, Resource1.bB },
                        { PieceType.Knight, Resource1.bN },
                        { PieceType.Pawn, Resource1.bp },
                    }
                },
                {
                    ItemColor.White,
                    new Dictionary<PieceType, Image>()
                    {
                        { PieceType.King, Resource1.wK },
                        { PieceType.Queen, Resource1.wQ },
                        { PieceType.Rook, Resource1.wR },
                        { PieceType.Bishop, Resource1.wB },
                        { PieceType.Knight, Resource1.wN },
                        { PieceType.Pawn, Resource1.wp },
                    }
                }
            };

            labelNextToMove.Text = _game.ToPlay.ToString();
            labelGameStatus.Text = "";
        }

        public MainForm()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
        }

        private void PaintBoard(Graphics g, int tileSize)
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;
            Brush brush;

            // double for loop to handle all rows and columns
            for (var row = 0; row < gridSize; row++)
            {
                for (var col = 0; col < gridSize; col++)
                {
                    // create new Panel control which will be one 
                    // chess board tile
                    Size sz = new Size(tileSize, tileSize);
                    Point loc = new Point(tileSize * row, tileSize * col);

                    // color the backgrounds
                    if (row % 2 == 0)
                        brush = col % 2 != 0 ? Brushes.DarkGray : Brushes.White;
                    else
                        brush = col % 2 != 0 ? Brushes.White : Brushes.DarkGray;

                    g.FillRectangle(brush, loc.X, loc.Y, tileSize, tileSize);
                }
            }
        }

        private void Render(Graphics g, ItemColor toPlay)
        {
            PaintBoard(g, Resource1.bK.Size.Width + 20);
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int r = toPlay == ItemColor.White ? (7 - row) : row;
                    int c = toPlay == ItemColor.White ? col : (7 - col);

                    BoardSquare square = _game.Board[r, c];
                    if (square.IsOccupied())
                    {
                        ItemColor pieceColor = square.Piece.PieceColor;
                        PieceType pieceType = square.Piece.PieceType;
                        Image pieceImage = _pieceImageMap[pieceColor][pieceType];
                        Point loc = new Point(tileSize * col + 8, tileSize * row + 8);

                        g.DrawImage(pieceImage, loc);
                    }
                }
            }

            for (int col = 0; col < 8; col++)
            {
                PointF loc = new Point(tileSize * col + 35, tileSize * 8 + 2);
                int c = toPlay == ItemColor.White ? (97 + col) : (97 + 7 - col);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);
            }

            for (int row = 0; row < 8; row++)
            {
                PointF loc = new Point(tileSize * 8 + 2, tileSize * row + 34);
                int c = toPlay == ItemColor.White ? (49 + 7 - row) : (49 + row);
                g.DrawString(Convert.ToChar(c).ToString(), SystemFonts.DefaultFont, Brushes.Black, loc);
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Render(e.Graphics, _game.ToPlay);
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            string move = textBoxMove.Text.Trim();
            if (move != "")
            {
                _game.Move(move);

                if (_game.LastMoveError != MoveErrorType.Ok)
                {
                    MessageBox.Show(this, $"Move error: {_game.ToPlay.ToString()} cannot make move: {move}");
                }
                else
                {
                    labelNextToMove.Text = _game.ToPlay.ToString();
                    if (_game.Board.IsGameOver)
                    {
                        if (_game.Board.IsCheckMate)
                            labelGameStatus.Text = $"Check Mate [{_game.Board.Winner.ToString()} wins";
                        else if (_game.Board.IsStaleMate)
                            labelGameStatus.Text = "Stale Mate";
                        else
                            labelGameStatus.Text = "Draw";
                    }
                    else if (_game.Board.InCheck)
                    {
                        labelGameStatus.Text = $"{_game.Board.PlayerInCheck.ToString()} in Check!";
                    }
                }

                Invalidate();
            }
        }
    }
}
