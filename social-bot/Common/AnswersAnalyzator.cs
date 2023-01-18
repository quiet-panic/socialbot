using System;
using System.Collections.Generic;
using social_bot.Chats;
using social_bot.User;
using Telegram.Bot.Types;

namespace social_bot.Common
{
    public class AnswersAnalyzator
    {
        /// <summary>
        /// Хранит соответствия между чатом и стадией, на которой он находится
        /// </summary>
        private static Dictionary<long, StagesEnum> stages = new Dictionary<long, StagesEnum>();

        public Action<UserBlank> OnBlankSended;
        public Action<long> OnUserApproved;
        public Action<long> OnUserDenied;

        /// <summary>
        /// Возвращает ответ бота
        /// </summary>
        /// <param name="_message"> Сообщение пользователя </param>
        /// <returns></returns>
        public Answer GetAnswer(Message _message)
        {
            long chatId = _message.Chat.Id;
            Telegram.Bot.Types.User user = _message.From;
            string userMessage = _message.Text;

            if (!stages.ContainsKey(chatId))
                stages.Add(chatId, StagesEnum.MainMenu);

            //можно вызывать в любой момент

            #region universal

            if (userMessage == TagManager.Start)
            {
                return BackToMainMenu(chatId, TagManager.Welcome);
            }

            if (userMessage == TagManager.Admin)
            {
                if (BlankManager.Instance.AdminList.Contains(chatId))
                {
                    return BackToMainMenu(chatId, TagManager.AlreadyAdmin);
                }
                else
                {
                    stages[chatId] = StagesEnum.Password;
                    return new Answer(TagManager.Verification, TagManager.Back);
                }
            }

            if (userMessage == TagManager.NoAdmin)
            {
                if (BlankManager.Instance.AdminList.Contains(chatId))
                {
                    BlankManager.Instance.RemoveAdmin(chatId);
                    return BackToMainMenu(chatId, TagManager.RemovedAdmin);
                }
            }

            #endregion

            if (_message.ReplyToMessage != null && BlankManager.Instance.AdminList.Contains(chatId))
            {
                string replyText = _message.ReplyToMessage.Text;
                string[] parts = replyText.Split('/');
                long id = long.Parse(parts[1]);
                UserBlank blank =
                    BlankManager.Instance.Blanks.Find(x => x.ID == id && x.Status == UserStatus.WaitForApprove);
                if (blank != null)
                {
                    if (userMessage == TagManager.Approve)
                    {
                        blank.Status = UserStatus.Approved;
                        stages[id] = StagesEnum.CareerQuestion;
                        OnUserApproved?.Invoke(id);
                    }
                    else if (userMessage == TagManager.Deny)
                    {
                        blank.Status = UserStatus.Denied;
                        OnUserDenied?.Invoke(id);
                    }
                }

                return new Answer(TagManager.SolutionSent);
            }

            if (stages[chatId] == StagesEnum.Password)
            {
                if (userMessage.ToLower() == TagManager.Password)
                {
                    BlankManager.Instance.AddAdmin(chatId);
                    return BackToMainMenu(chatId, TagManager.RightPassword);
                }

                if (userMessage == TagManager.Back)
                    return BackToMainMenu(chatId, TagManager.Welcome);
                return new Answer(TagManager.WrongPassword, false);
            }

            if (stages[chatId] == StagesEnum.MainMenu)
            {
                if (userMessage == TagManager.Represent)
                {
                    stages[chatId] = StagesEnum.Blank;
                    UserBlank blank = BlankManager.Instance.CreateBlank(user.Id);
                    blank.UserName = user.Username;
                    return new Answer(blank.GetNextQuestion().QuestionText, TagManager.BackToMainMenu);
                }

                if (userMessage == TagManager.GetUserInfo)
                {
                    stages[chatId] = StagesEnum.GetUserInfo;
                    return new Answer(TagManager.SendUserName, TagManager.BackToMainMenu);
                }
            }

            if (stages[chatId] == StagesEnum.Blank)
            {
                if (userMessage == TagManager.BackToMainMenu)
                {
                    BlankManager.Instance.RemoveUncompletedBlanks(user.Id);
                    return BackToMainMenu(chatId, TagManager.Welcome);
                }

                UserBlank blank = BlankManager.Instance.GetCurrentRequest(user.Id);
                QuestionBase question = blank.GetNextQuestion();
                if (string.IsNullOrEmpty(userMessage))
                {
                    return new Answer(TagManager.EmptyAnswer + Environment.NewLine + question.QuestionText, false);
                }
                else
                {
                    if (question is QuestionText textQuestion)
                    {
                        textQuestion.SetAnswer(userMessage);
                    }
                }

                question = blank.GetNextQuestion();
                if (question == null) //ответы даны на все вопросы
                {
                    blank.Status = UserStatus.WaitForApprove;
                    BlankManager.Instance.SendBlanks();
                    OnBlankSended?.Invoke(blank);
                    return BackToMainMenu(chatId, TagManager.WaitForApprove);
                }

                return new Answer(question.QuestionText, TagManager.BackToMainMenu);
            }

            if (stages[chatId] == StagesEnum.CareerQuestion)
            {
                stages[chatId] = StagesEnum.CoffeeQuestion;
                if (userMessage == TagManager.Yes)
                {
                    return new Answer(
                        string.Format(TagManager.CareerBot,
                            ChatManager.Instance.GetChatByCode("CAREER")) + Environment.NewLine
                                                                          + TagManager.CoffeeBotQuestion,
                        TagManager.Yes, TagManager.No);
                }
                else if (userMessage == TagManager.No)
                    return new Answer(TagManager.CoffeeBotQuestion, TagManager.Yes, TagManager.No);
            }

            if (stages[chatId] == StagesEnum.CoffeeQuestion)
            {
                stages[chatId] = StagesEnum.MainMenu;
                if (userMessage == TagManager.Yes)
                {
                    return new Answer(string.Format(TagManager.CoffeeBot, ChatManager.Instance.GetChatByCode("COFFEE")));
                }
                else if (userMessage == TagManager.No)
                    return BackToMainMenu(chatId, TagManager.RegistrationCompleted);
            }

            if (stages[chatId] == StagesEnum.GetUserInfo)
            {
                if (userMessage == TagManager.BackToMainMenu)
                {
                    return BackToMainMenu(chatId, TagManager.Welcome);
                }

                string userInfo = BlankManager.Instance.GetUserInfo(userMessage);
                if (string.IsNullOrEmpty(userInfo))
                    return BackToMainMenu(chatId, TagManager.NoUserName);
                return BackToMainMenu(chatId, userInfo);
            }

            return null;
        }


        /// <summary>
        /// Выводит кнопки главного меню и заданный текст
        /// </summary>
        /// <param name="_chatID"></param>
        /// <param name="_text"></param>
        /// <returns></returns>
        private static Answer BackToMainMenu(long _chatID, string _text)
        {
            stages[_chatID] = StagesEnum.MainMenu;
            if (BlankManager.Instance.IsAdmin(_chatID) || BlankManager.Instance.UserApproved(_chatID))
                return new Answer(_text, TagManager.GetUserInfo);
            return new Answer(_text, TagManager.Represent);
        }
    }
}
