using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Bot.XmlSaver;
using social_bot.SpreadSheetSaver;

namespace social_bot.User
{
    /// <summary>
    /// Менеджер заявок
    /// </summary>
    public class BlankManager : IXmlSavable
    {
        [XmlArray("Blanks")]
        public List<UserBlank> Blanks { get; set; } = new List<UserBlank>();

        /// <summary>
        /// Список айдишников админов
        /// </summary>
        [XmlArray("Admins")]
        public List<long> AdminList { get; set; } = new List<long>();

        /// <summary>
        /// Пустая заявка, содержащая только вопросы
        /// </summary>
        public UserBlank Template => instance.Blanks.FirstOrDefault();

        private readonly string filePath = "blank_manager.txt";

        private static BlankManager instance;

        public static BlankManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BlankManager();
                }

                return instance;
            }
        }

        public void Init()
        {
            Instance.Load();
        }

        public UserBlank CreateBlank(long _id)
        {
            UserBlank blank = Template.GetCopy();
            blank.ID = _id;
            Blanks.Add(blank);
            return blank;
        }

        /// <summary>
        /// Подтвержден ли пользователь
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public bool UserApproved(long _id)
        {
            return Blanks.Find(x => x.ID == _id && x.Status == UserStatus.Approved) != null;
        }

        /// <summary>
        /// Является ли пользователь админом
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public bool IsAdmin(long _id)
        {
            return AdminList.Contains(_id);
        }

        /// <summary>
        /// Возвращает незавершенную анкету пользователя
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public UserBlank GetCurrentRequest(long _id)
        {
            return Blanks.Find(x => x.ID == _id && !x.IsCompleted());
        }

        /// <summary>
        /// Удаляет начатую и незаконченную заявку
        /// </summary>
        /// <param name="_id"></param>
        public void RemoveUncompletedBlanks(long _id)
        {
            Blanks.RemoveAll(x => x.ID == _id && !x.IsCompleted());
        }

        /// <summary>
        /// Отправляет бланки в таблицу 
        /// </summary>
        public void SendBlanks()
        {
            Save();
            List<string[]> rows = new List<string[]>();
            string[] questions = new string[Template.Questions.Length + 2];
            questions[0] = "";
            questions[1] = "";
            for (var i = 2; i < Template.Questions.Length; i++)
            {
                questions[i] = Template.Questions[i].QuestionText;
            }
            rows.Add(questions);

            foreach (UserBlank blank in Blanks)
            {
                string[] answers = new string[blank.Questions.Length + 2];
                answers[0] = blank.ID.ToString();
                answers[1] = blank.Status.ToString();
                for (var i = 2; i < blank.Questions.Length; i++)
                {
                    answers[i] = blank.Questions[i].GetAnswerText();
                }
                rows.Add(answers);
            }

            AirTableSaver.Instance.Save();
        }

        /// <summary>
        /// Получает инфо о пользователи по имени
        /// </summary>
        /// <param name="_userName"></param>
        /// <returns></returns>
        public string GetUserInfo(string _userName)
        {
            UserBlank userBlank = Blanks.Find(x => x.UserName == _userName);
            return userBlank?.Description;
        }

        /// <summary>
        /// Добавляет админа
        /// </summary>
        /// <param name="_id"></param>
        public void AddAdmin(long _id)
        {
            AdminList.Add(_id);
            Save();
        }

        /// <summary>
        /// Удаляет админа
        /// </summary>
        /// <param name="_id"></param>
        public void RemoveAdmin(long _id)
        {
            AdminList.Remove(_id);
            Save();
        }

        public void Save()
        {
            XmlSaver<BlankManager>.Instance.Save(filePath, Instance);
        }

        public void Load()
        {
            instance = XmlSaver<BlankManager>.Instance.Load(filePath) ?? instance;
        }
    }
}
