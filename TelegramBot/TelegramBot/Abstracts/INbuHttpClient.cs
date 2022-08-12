using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Data;

namespace TelegramBot.Abstracts
{
    public interface INbuHttpClient
    {
        public Task<List<CurrencyItem>> HttpGet(string url);
    }
}