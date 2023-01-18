using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using social_bot.Chats;
using social_bot.Common;
using social_bot.SpreadSheetSaver;
using social_bot.User;

namespace social_bot
{
    internal class TelegramBotService
    {
        // you can also inject other services
        private ILogger<TelegramBotService> _logger;

        public TelegramBotService(ILogger<TelegramBotService> logger)
        {
            _logger = logger;
        }

        public void StartBot()
        {
            BotClient.Instance.Init();
            BlankManager.Instance.Init();
            ChatManager.Instance.Init();
            AirTableSaver.Instance.Load();
        }
    }
}
