using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

using cachCore.models;
using cachCore.enums;
using cachRendering;
using cachCore.controllers;
using cachRendering.models;
using cachCore.utils;

namespace cachBot
{
    public class CachBot
    {
        private readonly TelegramBotClient Bot;

        internal class CachBotUserContext
        {
            internal string UserName { get; set; }
            internal int UserId { get; set; }
            internal ItemColor PlayerColor { get; set; }
        }

        internal enum CoordinatedAction
        {
            None = 0,
            Undo,
            Draw
        }

        internal class GameContext
        {
            internal Game Game { get; set; }
            internal IBoardRenderer BoardRenderer { get; set; }
            internal IRenderContext RenderContext { get; set; }
            internal Dictionary<ItemColor, CachBotUserContext> UserContextMap { get; set; }
            internal CoordinatedAction ActionRequested { get; set; }
            internal int ActionRequestor { get; set; }
        }

        /// <summary>
        /// map: chatId -> GameContext
        /// </summary>
        private ConcurrentDictionary<long, GameContext> _gameContextMap;

        public CachBot(string botToken)
        {
            _gameContextMap = new ConcurrentDictionary<long, GameContext>();

            Bot = new TelegramBotClient(botToken);

            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            // Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            // Debugger.Break();
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        //private async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        //{
        //}

        private async Task SendMessage(long chatId, string msg)
        {
            await Bot.SendTextMessageAsync(chatId, msg);
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.ToLower().Matches(@"^cach\shelp"))
            {
                await SendHelp(message);
            }
            else if (message.Text.ToLower().Matches(@"^cach\splay\swhite"))
            {
                await GameStart(message, ItemColor.White);
            }
            else if (message.Text.ToLower().Matches(@"^cach\splay\sblack"))
            {
                await GameStart(message, ItemColor.Black);
            }
            else if (message.Text.ToLower().Matches(@"^cach\sshow\swhite"))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                GameContext gc = GetGameContext(message.Chat.Id);
                gc.RenderContext.ToPlay = ItemColor.White;
                await SendBoardImage(message.Chat.Id);
            }
            else if (message.Text.ToLower().Matches(@"^cach\sshow\sblack"))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                GameContext gc = GetGameContext(message.Chat.Id);
                gc.RenderContext.ToPlay = ItemColor.Black;
                await SendBoardImage(message.Chat.Id);
            }
            else if (message.Text.ToLower().Matches(@"^cach\sshow"))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await SendBoardImage(message.Chat.Id);
            }
            else if ((message.Text.ToLower().Matches(@"^cach\scancel")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await SendMessage(message.Chat.Id, "Ending current game");

                GameContext gc = GetGameContext(message.Chat.Id);
                gc.Game = null;
                gc.BoardRenderer = null;
                gc.RenderContext = null;
                _gameContextMap.TryRemove(message.Chat.Id, out gc);
            }
            else if ((message.Text.ToLower().Matches(@"^cach\sundo")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GameUndo(message);
            }
            else if ((message.Text.ToLower().Matches(@"^cach\sdraw")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GameDraw(message);
            }
            else if ((message.Text.ToLower().Matches(@"^cach\sresign")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GameResign(message);
            }
            else if ((message.Text.ToLower().Matches(@"^cach\spgn")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GamePGN(message);
            }
            else if ((message.Text.ToLower().Matches("^cach load ")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GameLoadPGN(message);
            }
            else if ((message.Text.ToLower().Matches("^cach ")))
            {
                if (!IsGameInProgress(message))
                {
                    await SendMessage(message.Chat.Id, "No game in progress");
                    return;
                }

                await GameMove(message);
            }
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        private GameContext GetGameContext(long chatId)
        {
            GameContext gc = null;
            _gameContextMap.TryGetValue(chatId, out gc);
            return gc;
        }

        private void SetGameContext(long chatId, GameContext gc)
        {
            _gameContextMap.TryAdd(chatId, gc);
        }

        private bool IsGameInProgress(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            return gc != null && gc.Game != null && !gc.Game.Board.IsGameOver;
        }

        private bool HasGameStarted(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            return gc != null && gc.Game != null;
        }

        private bool HasGameEnded(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            return gc != null && gc.Game != null && gc.Game.Board.IsGameOver;
        }

        private async Task GameStart(Message msg, ItemColor playerColor)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            if (IsGameInProgress(msg))
            {
                await SendMessage(msg.Chat.Id, "A game is already in progress, resign or cancel it to start another");
                return;
            }
            else if (gc == null)
            {
                // create a context to initiate required info for a game to start
                gc = new GameContext()
                {
                    Game = null,
                    BoardRenderer = null,
                    RenderContext = null,
                    UserContextMap = new Dictionary<ItemColor, CachBotUserContext>()
                };
                gc.UserContextMap[playerColor] = new CachBotUserContext()
                {
                    UserName = GetUserName(msg),
                    UserId = msg.From.Id,
                    PlayerColor = playerColor
                };

                SetGameContext(msg.Chat.Id, gc);

                ItemColor other = BoardUtils.GetOtherColor(playerColor);
                await SendMessage(msg.Chat.Id, $"Waiting for another person to choose {other.ToString()}");
            }
            else
            {
                gc.UserContextMap[playerColor] = new CachBotUserContext()
                {
                    UserName = msg.From.FirstName,
                    UserId = msg.From.Id,
                    PlayerColor = playerColor
                };

                // if both players are ready, start the game (note that a person can play against themselves)
                if (gc.UserContextMap.ContainsKey(ItemColor.Black) &&
                    gc.UserContextMap.ContainsKey(ItemColor.White))
                {
                    await SendMessage(msg.Chat.Id, $"Game start: {gc.UserContextMap[ItemColor.White].UserName} " +
                        $"vs {gc.UserContextMap[ItemColor.Black].UserName}");

                    gc.Game = new GameController().CreateGame();
                    gc.BoardRenderer = new BoardRenderer();
                    gc.RenderContext = new GraphicsRenderContext()
                    {
                        Board = gc.Game.Board,
                        Graphics = null,
                        LeftUpperOffset = new Point(0, 0),
                        TileSize = 30,
                        BorderSize = 16,
                        ToPlay = ItemColor.White
                    };

                    await SendBoardImage(msg.Chat.Id);
                }
                else
                {
                    ItemColor other = BoardUtils.GetOtherColor(playerColor);
                    await SendMessage(msg.Chat.Id, $"Waiting for another person to choose {other.ToString()}");
                }
            }
        }

        private async Task SendBoardImage(long chatId)
        {
            GameContext gc = GetGameContext(chatId);
            if (gc == null)
            {
                // TODO: log error
                return;
            }

            await Bot.SendChatActionAsync(chatId, ChatAction.UploadPhoto);
            using (MemoryStream m = new MemoryStream())
            {
                gc.BoardRenderer.RenderAsImage(gc.RenderContext, m);

                var fts = new FileToSend("test", m);

                var playerCtx = gc.UserContextMap[gc.Game.ToPlay];
                var imageMessage = gc.Game.Board.IsGameOver ? "Game over" :
                    $"{GetNameAndColor(playerCtx.UserName, playerCtx.PlayerColor)} to play";

                await Bot.SendPhotoAsync(chatId, fts, imageMessage);
            }
        }

        private async Task GameUndo(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            if (gc.ActionRequested == CoordinatedAction.Undo)
            {
                // check to make sure other player is confirming
                var otherPlayerCtx = gc.UserContextMap[ItemColor.White].UserId == gc.ActionRequestor ?
                    gc.UserContextMap[ItemColor.Black] : gc.UserContextMap[ItemColor.White];
                if (msg.From.Id != otherPlayerCtx.UserId)
                {
                    await SendMessage(msg.Chat.Id, $"Error: Undo request has to be approved by " +
                        $"{GetNameAndColor(otherPlayerCtx.UserName, otherPlayerCtx.PlayerColor)}");
                    return;
                }

                if (gc.Game.MoveUndo())
                {
                    await SendBoardImage(msg.Chat.Id);
                }
                else
                {
                    await SendMessage(msg.Chat.Id, "Undo not possible at this time");
                }

                gc.ActionRequested = CoordinatedAction.None;
                gc.ActionRequestor = 0;
            }
            else
            {
                // initiate undo request
                gc.ActionRequested = CoordinatedAction.Undo;
                gc.ActionRequestor = msg.From.Id;

                var otherPlayerCtx = gc.UserContextMap[ItemColor.White].UserId == gc.ActionRequestor ?
                    gc.UserContextMap[ItemColor.Black] : gc.UserContextMap[ItemColor.White];

                await SendMessage(msg.Chat.Id, $"Undo requested by {GetNameAndColor(gc, msg)}, " +
                    $"waiting for {GetNameAndColor(otherPlayerCtx.UserName, otherPlayerCtx.PlayerColor)} to approve");
            }
        }

        private async Task GameDraw(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            if (gc.ActionRequested == CoordinatedAction.Draw)
            {
                // check to make sure other player is confirming
                var otherPlayerCtx = gc.UserContextMap[ItemColor.White].UserId == gc.ActionRequestor ?
                    gc.UserContextMap[ItemColor.Black] : gc.UserContextMap[ItemColor.White];
                if (msg.From.Id != otherPlayerCtx.UserId)
                {
                    await SendMessage(msg.Chat.Id, $"Error: Draw offer has to be approved by " +
                        $"{GetNameAndColor(otherPlayerCtx.UserName, otherPlayerCtx.PlayerColor)}");
                    return;
                }

                gc.Game.Draw();

                await SendBoardImage(msg.Chat.Id);
                await CheckSendGameStatus(msg);

                gc.ActionRequested = CoordinatedAction.None;
                gc.ActionRequestor = 0;
            }
            else
            {
                // initiate draw request
                gc.ActionRequested = CoordinatedAction.Draw;
                gc.ActionRequestor = msg.From.Id;

                var otherPlayerCtx = gc.UserContextMap[ItemColor.White].UserId == gc.ActionRequestor ?
                    gc.UserContextMap[ItemColor.Black] : gc.UserContextMap[ItemColor.White];

                await SendMessage(msg.Chat.Id, $"Draw offer by {GetNameAndColor(gc, msg)}, " +
                    $"waiting for {GetNameAndColor(otherPlayerCtx.UserName, otherPlayerCtx.PlayerColor)} to approve");
            }
        }

        private async Task GameResign(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            ItemColor playerColor = GetPlayerColor(gc, msg);

            Game game = gc.Game;
            game.Resign(playerColor);

            await SendBoardImage(msg.Chat.Id);

            await CheckSendGameStatus(msg);
        }

        private async Task GamePGN(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);

            string pgn = gc.Game.Board.GetPGN();
            if (pgn.Trim().Length > 0)
            {
                await SendMessage(msg.Chat.Id, pgn);
            }
        }

        private ItemColor GetPlayerColor(GameContext gc, Message msg)
        {
            return gc.UserContextMap[ItemColor.White].UserId == msg.From.Id ? ItemColor.White : ItemColor.Black;
        }

        private string GetUserName(Message msg)
        {
            return msg.From.FirstName;
        }

        private string GetNameAndColor(GameContext gc, Message msg)
        {
            return $"{GetUserName(msg)} ({GetPlayerColor(gc, msg)})";
        }

        private string GetNameAndColor(string userName, ItemColor playerColor)
        {
            return $"{userName} ({playerColor.ToString()})";
        }

        private async Task GameLoadPGN(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            string pgn = msg.Text.Substring("cach load ".Length);

            // create new game with PGN
            gc.Game = new Game(pgn);

            // reset other things
            gc.RenderContext.Board = gc.Game.Board;
            gc.ActionRequested = CoordinatedAction.None;
            gc.ActionRequestor = 0;

            await SendMessage(msg.Chat.Id, "Loading game from PGN ...");
            await SendBoardImage(msg.Chat.Id);
            await CheckSendGameStatus(msg);
        }

        private async Task GameMove(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);

            // check player turn
            if (gc.UserContextMap[gc.Game.ToPlay].UserId != msg.From.Id)
            {
                await SendMessage(msg.Chat.Id, $"It's not your turn to play {GetUserName(msg)}");
                return;
            }

            Game game = gc.Game;
            var move = msg.Text.Substring("cach ".Length).Trim();

            if (move != "")
            {
                game.Move(move);

                if (game.LastMoveError != MoveErrorType.Ok)
                {
                    await SendMessage(msg.Chat.Id, $"Error: {game.ToPlay.ToString()} cannot make move: {move}, " +
                        $"reason: {game.LastMoveError.ToString()}");
                }
                else
                {
                    // clear any undo request pending
                    gc.ActionRequested = CoordinatedAction.None;
                    gc.ActionRequestor = 0;

                    await SendBoardImage(msg.Chat.Id);

                    await CheckSendGameStatus(msg);
                }
            }
        }

        private async Task CheckSendGameStatus(Message msg)
        {
            GameContext gc = GetGameContext(msg.Chat.Id);
            Game game = gc.Game;

            string label = "";
            if (game.Board.IsGameOver)
            {
                if (game.Board.IsResign)
                    label = $"Game over: Player resigned, [{game.Board.Winner.ToString()}] wins!!";
                else if (game.Board.IsCheckMate)
                    label = $"Game over: Checkmate [{game.Board.Winner.ToString()}] wins!!";
                else if (game.Board.IsStaleMate)
                    label = "Game over: Stalemate Draw";
                else
                    label = "Game over: Draw";

                // remove player contexts
                gc.UserContextMap = new Dictionary<ItemColor, CachBotUserContext>();
            }
            else if (game.Board.InCheck)
            {
                label = $"{game.Board.PlayerInCheck.ToString()} in Check!";
            }

            if (label != "")
            {
                await SendMessage(msg.Chat.Id, label);
            }
        }

        private async Task SendHelp(Message msg)
        {
            var help = @"
Cach commands are of the format: cach <command>
where <command> can be one of:
help: this help menu
play black: start game as Black
play white: start game as White
move <move specification>: player move in alebraic notation
show: display current board image
show black: switch to player as Black and display board image
show white: switch to player as White and display board image
undo: perform half-move undo after both players type undo
draw: end game as Draw after both players type draw
resign: end game as other player wins
cancel: terminate current game
";

            await SendMessage(msg.Chat.Id, help);
        }
    }
}