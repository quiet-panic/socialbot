using System.Collections.Generic;
using System.Xml.Serialization;
using Bot.XmlSaver;

namespace social_bot.Chats
{
    /// <summary>
    /// Менеджер чатов
    /// </summary>
    public class ChatManager: IXmlSavable
    {
        /// <summary>
        /// Список чатов
        /// </summary>
        [XmlArray("Chats")]
        public List<Chat> Chats { get; set; } = new List<Chat>();

        private readonly string filePath = "chats.txt";

        private static ChatManager instance;

        public static ChatManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChatManager();
                }

                return instance;
            }
        }

        public void Init()
        {
            Instance.Load();
        }

        /// <summary>
        /// Получить чат по специальному коду
        /// </summary>
        /// <param name="_code"></param>
        /// <returns></returns>
        public string GetChatByCode(string _code)
        {
            return Chats.Find(x => x.ChatCode == _code).ChatLink;
        }

        /// <summary>
        /// Добавить чат
        /// </summary>
        /// <param name="_id"></param>
        public void AddChat(long _id)
        {
            if (Chats.Find(x => x.ChatId == _id) == null)
                Chats.Add(new Chat(_id));
        }

        /// <summary>
        /// Удалить чат
        /// </summary>
        /// <param name="_id"></param>
        public void RemoveChat(long _id)
        {
            if (Chats.Find(x => x.ChatId == _id) != null)
                Chats.Remove(new Chat(_id));
        }

        public void Save()
        {
            XmlSaver<ChatManager>.Instance.Save(filePath, Instance);
        }

        public void Load()
        {
            instance = XmlSaver<ChatManager>.Instance.Load(filePath) ?? instance;
        }
    }
}
