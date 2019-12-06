﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using log4net;
using cachCore.models;
using cachCore.enums;
using cachCore.controllers;
using cachRendering.models;
using cachRendering;
using cacheEngine;
using cacheEngine.models;

namespace cach
{
    public partial class MainForm : Form
    {
        private Game _game;
        private IBoardRenderer _boardRenderer;
        private Engine _engine;
        private Random _random;
        private ILog _logger;

        const int tileSize = 80;
        const int gridSize = 8;

        // event handler of Form Load... init things here
        private void MainForm_Load(object sender, EventArgs e)
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            _game = new GameController().CreateGame();
            _boardRenderer = new BoardRenderer();
            _engine = new Engine(_game.Board, ItemColor.Black);
            _random = new Random();

            labelNextToMove.Text = _game.ToPlay.ToString();
            labelGameStatus.Text = "";

            DoubleBuffered = true;
        }

        public MainForm()
        {
            // log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();

            _logger = LogManager.GetLogger(GetType().Name); 
        }

        private void Render(Graphics g, ItemColor toPlay)
        {
            GraphicsRenderContext grc = new GraphicsRenderContext()
            {
                Board = _game.Board,
                Graphics = g,
                LeftUpperOffset = new Point(0, 0),
                TileSize = Resource1.bK.Size.Width + 20,
                BorderSize = 20,
                ToPlay = toPlay
            };
            _boardRenderer.Render(grc);

            grc.Graphics = null;
            grc.TileSize = 30;
            grc.BorderSize = 16;

            MemoryStream memStream = new MemoryStream();
            _boardRenderer.RenderAsImage(grc, memStream);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ItemColor renderColor = checkBoxAlwaysCurrent.Checked ? _alwaysShowColor : 
                (_forceViewSet ? _forceViewColor : _game.ToPlay);
            Render(e.Graphics, renderColor);
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            try
            {
                buttonMove.Enabled = false;

                string move = textBoxMove.Text.Trim();
                if (move != "")
                {
                    _game.Move(move);

                    bool moveOk;
                    if (_game.LastMoveError != MoveErrorType.Ok)
                    {
                        moveOk = false;
                        MessageBox.Show(this, $"Move error: {_game.ToPlay.ToString()} cannot make move: {move}, " +
                            $"reason: {_game.LastMoveError.ToString()}");
                    }
                    else
                    {
                        moveOk = true;
                        UpdateGameStatus();
                    }

                    _forceViewSet = false;
                    textBoxMove.Text = "";
                    PopulatePGN();

                    Invalidate();
                    Update();

                    if (moveOk && !_game.Board.IsGameOver)
                    {
                        var moves = _engine.SearchMoves(4);
                        if (moves.Count > 0)
                        {
                            int r = _random.Next(0, moves.Count);
                            MoveChoice mc = moves[r];
                            _logger.Info($"Engine move: {mc}");

                            _game.Move($"{mc.MoveDescriptor.Move}", mc.MoveDescriptor);
                            Invalidate();

                            string fen = new GameController().GetFEN(_game, ItemColor.White);
                            _logger.Info($"FEN: {fen}");

                            PopulatePGN();
                        }
                    }
                }

                textBoxMove.Focus();
                buttonMove.Enabled = true;
            }
            catch (Exception ex)
            {
                _logger.Error($"buttonMove_Click: Exception: {ex.Message}", ex);
                throw ex;
            }
        }

        private void PopulatePGN()
        {
            listBoxPGN.Items.Clear();
            IList<string> pgn = _game.Board.GetPGNList();

            foreach (var move in pgn)
            {
                listBoxPGN.Items.Add(move);
            }
        }

        private void UpdateGameStatus()
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

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            _game.MoveUndo();
            UpdateGameStatus();

            PopulatePGN();
            Invalidate();
        }

        private void buttonDumpFEN_Click(object sender, EventArgs e)
        {
            textBoxMove.Text = new GameController().GetFEN(_game, ItemColor.White);
        }
    }
}
