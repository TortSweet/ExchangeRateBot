using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Abstracts;
using TelegramBot.Data;

namespace TelegramBot
{
    public class Bot
    {
        private readonly ICurrencyService _currencyService;
        private readonly ITelegramBotClient _newTelegramBotClient;


        public Bot(ICurrencyService currencyService, ITelegramBotClient newTelegramBotClient)
        {
            this._currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService), "Can't be null!");
            this._newTelegramBotClient = newTelegramBotClient ?? throw new ArgumentNullException(nameof(newTelegramBotClient), "Can't be null!");
        }

        public async Task Start()
        {
            var me = await _newTelegramBotClient.GetMeAsync();
            Console.Title = me.Username ?? "Exchange bot";

            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            _newTelegramBotClient.StartReceiving(HandlerUpdateAsync,
                HandlerErrorAsync,
                receiverOptions,
                default);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.WriteLine($"{me.Id}");
            Console.ReadLine();

        }

        private Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            #region Null check

            if (botClient == null)
            {
                throw new ArgumentNullException(nameof(botClient), "Can't be null!");
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception), "Can't be null!");
            }

            #endregion


            var exceptionMessage = exception switch
            {
                ApiRequestException apiReqException =>
                    $"Error code:\n[{apiReqException.ErrorCode}], error message: \"{apiReqException.Message}\"",
                _ => exception.ToString()
            };

            Console.WriteLine(exceptionMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            #region Null check

            if (botClient == null)
            {
                throw new ArgumentNullException(nameof(botClient), "Can't be null!");
            }

            if (update == null)
            {
                throw new ArgumentNullException(nameof(update), "Can't be null!");
            }
            #endregion

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                await CatchReplyMessage(message, botClient);
                await CatchCommand(botClient, message);
            }
        }

        private async Task CatchCommand(ITelegramBotClient botClient, Message message)
        {
            switch (message.Text.ToLower())
            {
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat, BotConstants.GreetingMsg);

                    return;
                case "/exchange":
                    await botClient.SendTextMessageAsync(message.Chat, text: "/exchange",
                        replyMarkup: new ForceReplyMarkup() { InputFieldPlaceholder = BotConstants.ExampleReq, });

                    return;
                case "/help":
                    await botClient.SendTextMessageAsync(message.Chat, GetAnswer(_currencyService.GetAllCurrencyListAsync().Result));

                    return;
            }
        }

        private async Task CatchReplyMessage(Message message, ITelegramBotClient botClient)
        {
            if (message.ReplyToMessage?.Text!.StartsWith("/exchange") != true) return;
            var getParams = GetParams(message.Text);

             await AnswerChoice(getParams, botClient, message);
        }

        private async Task AnswerChoice((string currency, string date) getParams, ITelegramBotClient botClient, Message message)
        {
            if (getParams.currency == "Wrong currency" && getParams.date == "Invalid date!")
            {
                await botClient.SendTextMessageAsync(message.Chat, BotConstants.WrongParamNumber);
                return;
            }
            else if (getParams.currency == "Wrong currency")
            {
                await botClient.SendTextMessageAsync(message.Chat, BotConstants.WrongCurrencyValue);
                return;
            }
            else if (getParams.date == "Invalid date!")
            {
                await botClient.SendTextMessageAsync(message.Chat, BotConstants.WrongDateValue);
                return;
            }
            else
            {
                var getApiData = _currencyService.GetDataFromApi(getParams.currency, getParams.date);

                await botClient.SendTextMessageAsync(message.Chat, GetAnswer(getApiData.Result));
            }
        }


        private (string currency, string date) GetParams(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text can't be null", nameof(text));
            }

            var txtSplit = text.Split();
            if (txtSplit.Length != 2)
            {
                return ("Wrong currency", "Invalid date!");
            }

            var currency = txtSplit[0];

            DateTime date;
            try
            {
                date = DateTime.Parse(txtSplit[1]);
                if (date > DateTime.Now)
                {
                    return (currency, "Invalid date!");
                }
                else if (string.IsNullOrEmpty(currency))
                {
                    return ("Wrong currency", date.ToString("yyyyMMdd"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (currency, "Invalid date!");
            }

            return (currency, date.ToString("yyyyMMdd"));
        }
        private string GetAnswer(List<CurrencyItem> getApiData)
        {
            if (getApiData == null)
            {
                throw new ArgumentNullException("Data from API can't be null", nameof(getApiData));
            }

            return getApiData.Count switch
            {
                0 => BotConstants.WrongCurrencyValue,
                1 when getApiData[0].cc == null => BotConstants.WrongDateValue,
                > 1 => getApiData.Aggregate("", (current, item) => current + $"{item.cc} - {item.txt}\n"),
                _ => $"1 {getApiData[0].cc} cost {getApiData[0].rate} UAH at {getApiData[0].exchangedate}"
            };
        }
    }
}

