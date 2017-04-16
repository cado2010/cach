using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

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

namespace cachBot
{
    public class CachBot
    {
        private readonly TelegramBotClient Bot;

        private Game _game;
        private IBoardRenderer _boardRenderer;
        private IRenderContext _renderContext;

        public CachBot(string botToken)
        {
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
            Debugger.Break();
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

            if (message.Text.ToLower().StartsWith("cach play"))
            {
                // Console.WriteLine("from: " + message.From.Id);

                if (_game != null)
                {
                    await SendMessage(message.Chat.Id, "Cannot start new - game already in progress");
                    return;
                }

                await GameStart(message);

                //await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                ////const string file = @"E:\tmp\cach\test.png";
                ////var fileName = file.Split('\\').Last();

                //var fts = new FileToSend("test", fileStream);

                //// await Bot.SendPhotoAsync(message.Chat.Id, fts, "", false, 0, new ReplyKeyboardMarkup(new[] { new KeyboardButton("reply") }, true));
                //await Bot.SendPhotoAsync(message.Chat.Id, fts, "White to move");
            }
            else if (((message.Text.ToLower() == "cach show")))
            {
                await SendBoardImage(message);
            }
            else if (((message.Text.ToLower() == "cach show white")))
            {
                _renderContext.ToPlay = ItemColor.White;
                await SendBoardImage(message);
            }
            else if (((message.Text.ToLower() == "cach show black")))
            {
                _renderContext.ToPlay = ItemColor.Black;
                await SendBoardImage(message);
            }
            else if (((message.Text.ToLower() == "cach cancel")))
            {
                _game = null;
            }
            else if ((message.Text.ToLower().StartsWith("cach ")))
            {
                if (_game == null)
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

        private async Task GameStart(Message msg)
        {
            _game = new GameController().CreateGame();
            _boardRenderer = new BoardRenderer();

            _renderContext = new GraphicsRenderContext()
            {
                Board = _game.Board,
                Graphics = null,
                LeftUpperOffset = new Point(0, 0),
                TileSize = 30,
                BorderSize = 16,
                ToPlay = ItemColor.White
            };

            await SendBoardImage(msg);
        }

        private async Task SendBoardImage(Message msg)
        {
            await Bot.SendChatActionAsync(msg.Chat.Id, ChatAction.UploadPhoto);
            using (MemoryStream m = new MemoryStream())
            {
                _boardRenderer.RenderAsImage(_renderContext, m);

                var fts = new FileToSend("test", m);
                await Bot.SendPhotoAsync(msg.Chat.Id, fts, $"{_game.ToPlay.ToString()} to play");
            }
        }

        private async Task GameMove(Message msg)
        {
            var move = msg.Text.Substring("cach ".Length).Trim();

            if (move != "")
            {
                _game.Move(move);

                if (_game.LastMoveError != MoveErrorType.Ok)
                {
                    await SendMessage(msg.Chat.Id, $"Move error: {_game.ToPlay.ToString()} cannot make move: {move}, " +
                        $"reason: {_game.LastMoveError.ToString()}");
                }
                else
                {
                    await SendBoardImage(msg);

                    string label = "";
                    if (_game.Board.IsGameOver)
                    {
                        if (_game.Board.IsCheckMate)
                            label = $"Check Mate [{_game.Board.Winner.ToString()}] wins!!";
                        else if (_game.Board.IsStaleMate)
                            label = "Stale Mate";
                        else
                            label = "Draw";
                    }
                    else if (_game.Board.InCheck)
                    {
                        label = $"{_game.Board.PlayerInCheck.ToString()} in Check!";
                    }

                    if (label != "")
                    {
                        await SendMessage(msg.Chat.Id, label);
                    }
                }
            }
        }
    }
}