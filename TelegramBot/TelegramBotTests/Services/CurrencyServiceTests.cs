using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TelegramBot.Abstracts;
using TelegramBot.Data;
using TelegramBot.Services;

namespace TelegramBotTests.Services
{

    [TestClass()]
    public class CurrencyServiceTests
    {
        private readonly Mock<INbuHttpClient> _nbuClientMock = new Mock<INbuHttpClient>();
        private readonly CurrencyService _newCurService =  new CurrencyService(new NbuHttpClient(new HttpClient()));

        [TestMethod()]
        public async Task GetDataFromAPINullTest()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _newCurService.GetDataFromApi(null, null));
        }

        [TestMethod()]
        public async Task GetDataFromAPITest()
        {
            var curService = new CurrencyService(_nbuClientMock.Object);

            _nbuClientMock.Setup(x => x.HttpGet(It.IsAny<string>()))
                .Returns(Task.FromResult<List<CurrencyItem>>(new List<CurrencyItem>()
                {
                    new CurrencyItem()
                    {
                        cc = "USD",
                                exchangedate = "20.12.2021",
                                txt = "Долар США",
                                rate = new decimal(27.2483),
                                r030 = "840"
                    }
                }));
            
            var res = await curService.GetDataFromApi("USD", "20210215");


            Assert.IsNotNull(res);
            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual("USD", res[0].cc);
            Assert.AreEqual("20.12.2021", res[0].exchangedate);
            Assert.AreEqual("840", res[0].r030);
        }

        [TestMethod()]
        public async Task GetAllCurrencyListTestAsync()
        {
            var curService = new CurrencyService(_nbuClientMock.Object);
            _nbuClientMock.Setup(x => x.HttpGet(It.IsAny<string>()))
                .Returns(Task.FromResult<List<CurrencyItem>>(new List<CurrencyItem>()
                {
                    new CurrencyItem()
                    {
                        cc = "USD",
                        exchangedate = "20.12.2021",
                        txt = "Долар США",
                        rate = new decimal(27.2483),
                        r030 = "840"
                    },
                    new CurrencyItem()
                    {
                        cc = "EUR",
                        exchangedate = "20.12.2021",
                        txt = "Євро",
                        rate = new decimal(30.8573),
                        r030 = "978"
                    }

                }));


            var res = await curService.GetAllCurrencyListAsync();

            Assert.AreEqual(res[0].cc, "USD");
            Assert.AreEqual(res[0].txt, "Долар США");
            Assert.AreEqual(res[1].cc, "EUR");
            Assert.AreEqual(res[1].txt, "Євро");
        }
    }
}

