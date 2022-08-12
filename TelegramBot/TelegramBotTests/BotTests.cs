using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot;
using TelegramBot.Abstracts;
using TelegramBot.Data;
using TelegramBot.Services;
using ReceiverOptions = Telegram.Bot.Extensions.Polling.ReceiverOptions;

namespace TelegramBotTests
{
    [TestClass()]
    public class BotTests
    {
        private readonly Mock<ICurrencyService> _currencyServiceMock = new();
        private readonly Mock<ITelegramBotClient> _newTelegramBotClientMock = new();
        private const int _result = 0;

        [TestMethod()]
        public void StartTest()
        {
            Bot testBot = new Bot(_currencyServiceMock.Object, _newTelegramBotClientMock.Object);

            _ = testBot.Start();

            Assert.AreEqual(_result, Environment.ExitCode);
        }
        
        [TestMethod()]
        public void VerifyTests()
        {
           
            _currencyServiceMock.VerifyAll();
            _newTelegramBotClientMock.VerifyAll();
        }

        private Task HandlerUpdateAsync(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            var mockHttp = new Mock<NbuHttpClient>();
            var curServ = new CurrencyService(mockHttp.Object);
            var result = curServ.GetDataFromApi("usd", "20.12.2022");
            return result;
        }

        private Task HandlerErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return Task.CompletedTask;
        }

        [TestMethod()]
        public void StartNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Bot(null, null));
        }
    }
}