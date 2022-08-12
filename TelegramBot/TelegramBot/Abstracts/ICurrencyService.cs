using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Data;

namespace TelegramBot.Abstracts
{
    public interface ICurrencyService
    {
        public Task<List<CurrencyItem>> GetDataFromApi(string valCode, string date);
        public Task<List<CurrencyItem>> GetAllCurrencyListAsync();
    }
}
