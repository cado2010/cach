using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using log4net;
using cachCore.models;
using cachCore.enums;
using cachCore.controllers;
using cachRendering.models;
using cachRendering;
using cacheEngine;
using cacheEngine.models;
using cachCore.utils;
using openingBook.models;

namespace cach
{
    public partial class MainForm : Form
    {
        private Game _game;
        private IBoardRenderer _boardRenderer;
        private Engine _engine;
        private Engine _aidEngine;
        private Random _random;
        private ILog _logger;

        private bool _highlitePosition;
        private Position _hlPosition;

        private OpeningBookNode _obRoot;

        const int tileSize = 80;
        const int gridSize = 8;
        const int BorderSize = 20;

        // event handler of Form Load... init things here
        private void MainForm_Load(object sender, EventArgs e)
        {
            _game = new GameController().CreateGame();
            _boardRenderer = new BoardRenderer();

            var w = new OpeningMessageForm();
            w.Show();
            w.Update();
            w.Invalidate();

            _engine = new Engine(_game.Board, ItemColor.Black);
            w.Close();

            _obRoot = _engine.OpeningBookRoot;
            _aidEngine = new Engine(_game.Board, ItemColor.White, _obRoot);

            _random = new Random();

            labelNextToMove.Text = _game.ToPlay.ToString();
            labelGameStatus.Text = "";

            DoubleBuffered = true;
        }

        public MainForm()
        {
            InitializeComponent();
            _logger = LogManager.GetLogger(GetType().Name); 
        }

        public Game Game => _game;

        private void Render(Graphics g, ItemColor toPlay)
        {
            GraphicsRenderContext grc = new GraphicsRenderContext()
            {
                Board = _game.Board,
                Graphics = g,
                LeftUpperOffset = new Point(0, 0),
                TileSize = Resource1.bK.Size.Width + 20,
                BorderSize = BorderSize,
                ToPlay = toPlay,
                HighlitePosition = _highlitePosition ? _hlPosition : Position.Invalid
            };

            // _logger.Info($"Render: _highlitePosition={_highlitePosition}, _hlPosition={_hlPosition}");
            _boardRenderer.Render(grc);

            grc.Graphics = null;
            grc.TileSize = 30;
            grc.BorderSize = 16;

            // MemoryStream memStream = new MemoryStream();
            // _boardRenderer.RenderAsImage(grc, memStream);
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
                buttonMove.Enabled = buttonAidEngineMove.Enabled = false;

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
                        if (PlayEngineMove(false))
                        {
                            Invalidate();

                            string fen = new GameController().GetFEN(_game, ItemColor.White);
                            _logger.Info($"FEN: {fen}");

                            PopulatePGN();
                        }
                    }
                }

                textBoxMove.Focus();
                buttonMove.Enabled = buttonAidEngineMove.Enabled = true;
            }
            catch (Exception ex)
            {
                _logger.Error($"buttonMove_Click: Exception: {ex.Message}", ex);
                throw ex;
            }
        }

        private bool PlayEngineMove(bool aidPlayer)
        {
            bool result;
            var moves = aidPlayer ? _aidEngine.SearchMoves(4) : _engine.SearchMoves(4);
            if (moves.Count > 0)
            {
                int r = _random.Next(0, moves.Count);
                MoveChoice mc = moves[r];
                _logger.Info($"PlayEngineMove: {(aidPlayer ? "(Aiding) " : "")}Engine move={mc}");

                if (mc.FromOpeningBook)
                {
                    _game.Move(mc.Move);
                }
                else
                {
                    _game.Move($"{mc.MoveDescriptor.Move}", mc.MoveDescriptor);
                }

                // get the last moved piece's original position and save it
                PiecePositionHistoryItem pphi = _game.Board.GetLastMoveBoardHistoryItem();
                if (pphi != null)
                {
                    _highlitePosition = true;
                    _hlPosition = pphi.Position;
                }
                else
                {
                    _highlitePosition = false;
                    _hlPosition = Position.Invalid;
                }

                result = true;
            }
            else
            {
                _logger.Error("PlayEngineMove: Error - no Engine move available");
                result = false;
            }

            return result;
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
        private bool _playBlack = false;

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

        private Position GetPositionFromCoord(int x, int y)
        {
            Position pos = Position.Invalid;

            int tileSize = Resource1.bK.Size.Width + 20;
            if (x >= BorderSize && y >= BorderSize &&
                x < BorderSize + 8 * tileSize &&
                y < BorderSize + 8 * tileSize)
            {
                if (_playBlack)
                {
                    pos = new Position((y - BorderSize) / tileSize, 7 - (x - BorderSize) / tileSize);
                }
                else
                {
                    pos = new Position(7 - (y - BorderSize) / tileSize, (x - BorderSize) / tileSize);
                }
            }

            return pos;
        }

        private void SelectPiece(Position pos)
        {
            _highlitePosition = true;
            _hlPosition = pos;

            Invalidate();
        }

        private bool IsKingSideCastleAttempt(Piece piece, Position moveTo)
        {
            return piece.PieceType == PieceType.King &&
                piece.Position.Column == 4 && moveTo.Column == 6;

        }

        private bool IsQueenSideCastleAttempt(Piece piece, Position moveTo)
        {
            return piece.PieceType == PieceType.King &&
                piece.Position.Column == 4 && moveTo.Column == 2;
        }

        private void MovePiece(Position moveTo)
        {
            ItemColor otherColor = BoardUtils.GetOtherColor(_game.ToPlay);
            bool isKill = _game.Board[moveTo].IsOccupiedByPieceOfColor(otherColor);

            Piece piece = _game.Board[_hlPosition].Piece;
            MoveDescriptor md = new MoveDescriptor(piece)
            {
                PieceColor = _game.ToPlay,
                Move = "comp",
                PieceType = piece != null ? piece.PieceType : PieceType.King,
                TargetPosition = moveTo,
                StartPosition = _hlPosition,
                IsKingSideCastle = IsKingSideCastleAttempt(piece, moveTo),
                IsQueenSideCastle = IsQueenSideCastleAttempt(piece, moveTo),
                IsKill = isKill,
                IsDrawOffer = false,
                IsResign = false,
                IsPromotion = false, // TODO
                PromotedPieceType = PieceType.Unknown // TODO
            };
            md.Move = md.MoveDescFromPosition;

            // set the move text and fire the Move button click
            textBoxMove.Text = md.MoveDescFromPosition;
            buttonMove_Click(null, null);

            // if shorter notation doesnt work - try fully qualified notation of move
            if (_game.LastMoveError != MoveErrorType.Ok)
            {
                textBoxMove.Text = md.MoveDescFromPosition_StartQualified;
                buttonMove_Click(null, null);
            }
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null)
            {
                Position pos = GetPositionFromCoord(me.X, me.Y);
                if (pos.IsValid)
                {
                    if (_game.Board[pos].IsOccupiedByPieceOfColor(_game.ToPlay))
                    {
                        SelectPiece(pos);
                    }
                    else if (_highlitePosition)
                    {
                        MovePiece(pos);
                    }
                }
            }
        }

        private void buttonPlayBlack_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this,
                "This will reset the current game", "Play Black", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                _playBlack = true;
                _forceViewSet = true;
                _forceViewColor = ItemColor.Black;
                _alwaysShowColor = ItemColor.Black;

                _game = new GameController().CreateGame();
                _engine = new Engine(_game.Board, ItemColor.White, _obRoot);
                _aidEngine = new Engine(_game.Board, ItemColor.Black, _obRoot);

                _random = new Random();

                labelNextToMove.Text = _game.ToPlay.ToString();
                labelGameStatus.Text = "";

                if (PlayEngineMove(false))
                {
                    Invalidate();

                    string fen = new GameController().GetFEN(_game, ItemColor.White);
                    _logger.Info($"FEN: {fen}");

                    PopulatePGN();
                }
            }
        }

        private void buttonAidEngineMove_Click(object sender, EventArgs e)
        {
            buttonMove.Enabled = buttonAidEngineMove.Enabled = false;

            if (PlayEngineMove(true))
            {
                PopulatePGN();
                Invalidate();
                Update();
            }

            var fen = new GameController().GetFEN(_game, _game.ToPlay);
            _logger.Info($"FEN after 1st player: {fen}");

            if (PlayEngineMove(false))
            {
                PopulatePGN();
                Invalidate();
                Update();
            }

            fen = new GameController().GetFEN(_game, _game.ToPlay);
            _logger.Info($"FEN after 2nd player: {fen}");

            textBoxMove.Focus();
            buttonMove.Enabled = buttonAidEngineMove.Enabled = true;
        }
    }
}
