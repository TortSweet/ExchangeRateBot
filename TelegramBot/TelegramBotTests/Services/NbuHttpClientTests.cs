using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TelegramBot.Services;

namespace TelegramBotTests.Services
{
    [TestClass()]
    public class NbuHttpClientTests
    {
        private readonly NbuHttpClient _httpClientClass = new(new HttpClient());

        [TestMethod()]
        public void HttpGetNullTest()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _httpClientClass.HttpGet(null));
        }


        public class HttpMessageHandlerMock : HttpMessageHandler
        {
            private readonly HttpStatusCode _code;
            private readonly HttpResponseMessage _response;

            public HttpMessageHandlerMock(HttpStatusCode code)
            {
                this._code = code;
            }
            public HttpMessageHandlerMock(HttpResponseMessage response)
            {
                this._response = response;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_response != null)
                {
                    return Task.FromResult(_response);
                }

                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = (HttpStatusCode) _code,
                });
            }
        }

        [TestMethod()]
        public async Task HttpGetTest()
        {
            var mockHttpClient = new Mock<HttpClient>(new HttpMessageHandlerMock(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"r030\":840,\"txt\":\"Долар США\",\"rate\":27.2483,\"cc\":\"USD\",\"exchangedate\":\"20.12.2021\"}")
            }));

            var newNbuClient = new NbuHttpClient(mockHttpClient.Object);

            var result = newNbuClient.HttpGet("https://bank.gov.ua");

            Assert.IsNotNull(result);
        }
    }
}