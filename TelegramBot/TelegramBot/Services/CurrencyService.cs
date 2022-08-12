using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Abstracts;
using TelegramBot.Data;

namespace TelegramBot.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly INbuHttpClient _httpClientServices;
        public CurrencyService(INbuHttpClient httpClientServices)
        {
            this._httpClientServices = httpClientServices ?? throw new ArgumentNullException(nameof(httpClientServices), "httpClientServices can't be null");
        }

        public async Task<List<CurrencyItem>> GetDataFromApi(string currencyCode, string date)
        {
            #region Null check
            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentNullException(nameof(currencyCode), "Valcode code can't be null");
            }
            if (string.IsNullOrEmpty(date))
            {
                throw new ArgumentNullException(nameof(date), "Valiable Date code can't be null");
            }
            #endregion

            var urlParameters = $"NBUStatService/v1/statdirectory/exchange?valcode={currencyCode}&date={date}&json";
            var items = await _httpClientServices.HttpGet(urlParameters);

            return items;
        }
        public async Task<List<CurrencyItem>> GetAllCurrencyListAsync()
        {
            var urlParameters = "NBUStatService/v1/statdirectory/exchange?json";
            var items = await _httpClientServices.HttpGet(urlParameters);

            return items;
        }
    }
}
