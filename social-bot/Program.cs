using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using social_bot.Chats;
using social_bot.Common;
using social_bot.SpreadSheetSaver;
using social_bot.User;

namespace social_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            TelegramBotService myService = host.Services.GetRequiredService<TelegramBotService>();
            myService.StartBot();

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // remove the hosted service
                    services.AddTransient<TelegramBotService>();

                    // register your services here.
                });
    }
}
