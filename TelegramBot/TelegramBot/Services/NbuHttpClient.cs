using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TelegramBot.Abstracts;
using TelegramBot.Data;

namespace TelegramBot.Services
{
    public class NbuHttpClient : INbuHttpClient
    {
        private readonly HttpClient _httpClient;
        public NbuHttpClient(HttpClient httpClient)
        {
            this._httpClient = httpClient ??
                               throw new ArgumentNullException(nameof(httpClient), "httpClient can't be empty");
        }
        public async Task<List<CurrencyItem>> HttpGet(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "url can't be empty");
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + url);

            var items = GetJsonFromResponse(responseMessage);

            return await items;
        }

        private async Task<List<CurrencyItem>> GetJsonFromResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage), "url can't be empty");
            }

            responseMessage.EnsureSuccessStatusCode();
            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<CurrencyItem>>(jsonString);

            return items;
        }
    }
}
