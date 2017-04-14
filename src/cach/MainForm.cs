using System;
using System.Drawing;
using System.Windows.Forms;
using cachCore.models;
using cachCore.enums;
using cachCore.controllers;
using cachRendering.models;
using cachRendering;

namespace cach
{
    public partial class MainForm : Form
    {
        private Game _game;
        private IBoardRenderer _boardRenderer;

        const int tileSize = 80;
        const int gridSize = 8;

        // event handler of Form Load... init things here
        private void MainForm_Load(object sender, EventArgs e)
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            _game = new GameController().CreateGame();
            _boardRenderer = new BoardRenderer();

            labelNextToMove.Text = _game.ToPlay.ToString();
            labelGameStatus.Text = "";

            DoubleBuffered = true;
        }

        public MainForm()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
        }

        private void Render(Graphics g, ItemColor toPlay)
        {
            GraphicsRenderContext grc = new GraphicsRenderContext()
            {
                Board = _game.Board,
                Graphics = g,
                LeftUpperOffset = new Point(0, 0),
                TileSize = Resource1.bK.Size.Width + 20,
                ToPlay = toPlay
            };
            _boardRenderer.Render(grc);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ItemColor renderColor = checkBoxAlwaysCurrent.Checked ? _alwaysShowColor : 
                (_forceViewSet ? _forceViewColor : _game.ToPlay);
            Render(e.Graphics, renderColor);
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            string move = textBoxMove.Text.Trim();
            if (move != "")
            {
                _game.Move(move);

                if (_game.LastMoveError != MoveErrorType.Ok)
                {
                    MessageBox.Show(this, $"Move error: {_game.ToPlay.ToString()} cannot make move: {move}, " +
                        $"reason: {_game.LastMoveError.ToString()}");
                }
                else
                {
                    labelNextToMove.Text = _game.ToPlay.ToString();
                    if (_game.Board.IsGameOver)
                    {
                        if (_game.Board.IsCheckMate)
                            labelGameStatus.Text = $"Check Mate [{_game.Board.Winner.ToString()}] wins!!";
                        else if (_game.Board.IsStaleMate)
                            labelGameStatus.Text = "Stale Mate";
                        else
                            labelGameStatus.Text = "Draw";
                    }
                    else if (_game.Board.InCheck)
                    {
                        labelGameStatus.Text = $"{_game.Board.PlayerInCheck.ToString()} in Check!";
                    }
                    else
                    {
                        labelGameStatus.Text = "";
                    }
                }

                _forceViewSet = false;
                textBoxMove.Text = "";
                Invalidate();
            }
        }

        private bool _forceViewSet = false;
        private ItemColor _forceViewColor;
        private ItemColor _alwaysShowColor = ItemColor.White;

        private void buttonWhiteView_Click(object sender, EventArgs e)
        {
            _forceViewSet = true;
            _forceViewColor = ItemColor.White;
            Invalidate();
        }

        private void buttonBlackView_Click(object sender, EventArgs e)
        {
            _forceViewSet = true;
            _forceViewColor = ItemColor.Black;
            Invalidate();
        }

        private void checkBoxAlwaysShowCurrent_CheckedChanged(object sender, EventArgs e)
        {
            _alwaysShowColor = _forceViewSet ? _forceViewColor : _game.ToPlay;
            Invalidate();
        }

        private void buttonCreateBoard_Click(object sender, EventArgs e)
        {
            if (textBoxMove.Text.Trim() != "" &&
                MessageBox.Show(this, "Current board will be lost! Continue?", "Warning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
            {
                try
                {
                    _game = new GameController().CreateGame(textBoxMove.Text.Trim());
                    textBoxMove.Text = "";

                    Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while creating board from FEN: {ex.Message}");
                }
            }
        }
    }
}
