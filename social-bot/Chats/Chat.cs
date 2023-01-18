namespace social_bot.Chats
{
    /// <summary>
    /// Чат, который ведет бот
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Айдишник чата
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Код для быстрого доступа
        /// </summary>
        public string ChatCode { get; set; }

        /// <summary>
        /// Ссылка на приглашение в чат
        /// </summary>
        public string ChatLink { get; set; }

        public Chat() { }

        public Chat(long _chatID)
        {
            ChatId = _chatID;
        }
    }
}
