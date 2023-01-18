using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using social_bot.Chats;
using social_bot.User;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Chat = social_bot.Chats.Chat;

namespace social_bot.Common
{
    /// <summary>
    /// Класс взаимодействия с телегой
    /// </summary>
    public class BotClient
    {
        private static BotClient instance;

        public static BotClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BotClient();
                }

                return instance;
            }
        }

        private readonly long myId = 5918020502;

        /// <summary>
        /// Бот
        /// </summary>
        private ITelegramBotClient botClient;

        /// <summary>
        /// Анализатор ответов пользователя
        /// </summary>
        private AnswersAnalyzator answersAnalyzator;

        public void Init()
        {
            answersAnalyzator = new AnswersAnalyzator();

            answersAnalyzator.OnBlankSended += SendUserForApprove;
            answersAnalyzator.OnUserApproved += OnUserApproved;
            answersAnalyzator.OnUserDenied += OnUserDenied;

            botClient = new TelegramBotClient(BotSettings.Token);
            CancellationTokenSource cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
        }

        public async Task HandleErrorAsync(ITelegramBotClient _botClient, Exception _exception, CancellationToken _cancellationToken)
        {
            Console.WriteLine("error: " + _exception.Message);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient _botClient, Update _update, CancellationToken _cancellationToken)
        {
            if (_update.Type == UpdateType.MyChatMember)
            {
                //добавление/удаление бота из админов
                if (_update.ChatMember.NewChatMember.User.Id == myId)
                {
                    if (_update.ChatMember.NewChatMember.Status == ChatMemberStatus.Administrator)
                        ChatManager.Instance.AddChat(_update.ChatMember.Chat.Id);
                    else if (_update.ChatMember.OldChatMember.Status == ChatMemberStatus.Administrator &&
                             _update.ChatMember.NewChatMember.Status != ChatMemberStatus.Administrator)
                        ChatManager.Instance.RemoveChat(_update.ChatMember.Chat.Id);
                }
                else
                {
                    //TODO проверять представление в бд и банить, если нет
                    /*if (_update.ChatMember.NewChatMember.Status == ChatMemberStatus.Member &&
                        !BlankManager.Instance.UserApproved(_update.ChatMember.NewChatMember.User.Id))
                    {
                        await botClient.BanChatMemberAsync(_update.ChatMember.Chat.Id,
                            _update.ChatMember.NewChatMember.User.Id);
                    }*/
                }
            }

            Message message = _update.Message;
            try
            {
                if (message != null)
                {
                    Answer answer = answersAnalyzator.GetAnswer(message);

                    if (answer == null)
                        return;

                    IReplyMarkup keyboard = null;
                    if (answer.buttonTexts.Any())
                    {
                        List<KeyboardButton[]> buttons = new List<KeyboardButton[]>();
                        for (int i = 0; i < answer.buttonTexts.Count; i++)
                        {
                            string buttonText = answer.buttonTexts[i];
                            if (i % 2 == 0)
                            {
                                if (i == answer.buttonTexts.Count - 1)
                                    buttons.Add(new KeyboardButton[1]);
                                else buttons.Add(new KeyboardButton[2]);
                            }

                            buttons[buttons.Count - 1][i % 2] = new KeyboardButton(buttonText);
                        }

                        keyboard = new ReplyKeyboardMarkup(buttons.ToArray())
                        {
                            ResizeKeyboard = true
                        };
                    }
                    else if (answer.ClearKeyboard) keyboard = new ReplyKeyboardRemove();


                    if (keyboard != null)
                        await botClient.SendTextMessageAsync(message.Chat.Id, answer.Text, ParseMode.Html, replyMarkup: keyboard);
                    else
                        await botClient.SendTextMessageAsync(message.Chat.Id, answer.Text, ParseMode.Html);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                if (message != null)
                    await botClient.SendTextMessageAsync(message.Chat.Id, TagManager.Exception);
            }
        }

        /// <summary>
        /// Посылает анкету пользователя на подтверждение админу
        /// </summary>
        public void SendUserForApprove(UserBlank _userBlank)
        {
            IReplyMarkup keyboard = new ReplyKeyboardMarkup( new[] {
                new KeyboardButton(TagManager.Approve), new KeyboardButton(TagManager.Deny)
                })
            {
                ResizeKeyboard = true
            };
            //рассылка анкеты всем админам
            foreach (long admin in BlankManager.Instance.AdminList)
            {
                botClient.SendTextMessageAsync(admin, _userBlank.GetAdminForm(), ParseMode.Html, replyMarkup: keyboard);
            }
        }

        /// <summary>
        /// Пользователь подтвержден
        /// </summary>
        /// <param name="_userID"></param>
        private async void OnUserApproved(long _userID)
        {
            //разбанить пользователя во всех чатах, если был забанен
            foreach (Chat chat in ChatManager.Instance.Chats)
            {
                await botClient.UnbanChatMemberAsync(chat.ChatId, _userID, true);
            }

            await botClient.SendTextMessageAsync(_userID, string.Format(TagManager.AccessApproved, ChatManager.Instance.GetChatByCode("MAIN")), ParseMode.Html);

            await Task.Delay(1000);

            IReplyMarkup keyboard = new ReplyKeyboardMarkup(new[] {
                new KeyboardButton(TagManager.Yes), new KeyboardButton(TagManager.No)
            })
            {
                ResizeKeyboard = true
            };
            await botClient.SendTextMessageAsync(_userID, string.Format(TagManager.CareerBotQuestion), ParseMode.Html, replyMarkup: keyboard);
        }

        /// <summary>
        /// Пользователь отклонен
        /// </summary>
        /// <param name="_userID"></param>
        private async void OnUserDenied(long _userID)
        {
            await botClient.SendTextMessageAsync(_userID, string.Format(TagManager.AccessDenied), ParseMode.Html);
        }
    }
}
