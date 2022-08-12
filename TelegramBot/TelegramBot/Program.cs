using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot.Abstracts;
using TelegramBot.Services;

namespace TelegramBot
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            IConfiguration config;
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();



            var serviceCollection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<ITelegramBotClient, TelegramBotClient>(client => new TelegramBotClient(config.GetSection("Token").Value))
                .AddSingleton<ICurrencyService, CurrencyService>()
                .AddSingleton<Bot>();

            serviceCollection.AddHttpClient<INbuHttpClient, NbuHttpClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(config.GetSection("BaseUrl").Value));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var newBotApp = serviceProvider.GetRequiredService<Bot>();
            await newBotApp.Start();
        }
    }
}

