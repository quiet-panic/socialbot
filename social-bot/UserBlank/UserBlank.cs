using System.Linq;
using System.Text;

namespace social_bot.User
{
    /// <summary>
    /// Класс, описывающий анкету пользователя
    /// </summary>
    public class UserBlank
    {
        /// <summary>
        /// Айдишник юзера из телеграма
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Имя пользователя в тг (если есть)
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Вопросы заявки
        /// </summary>
        public QuestionBase[] Questions { get; set; }

        /// <summary>
        /// Статус подтверждения
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Inactive;

        /// <summary>
        /// Описание пользователя
        /// </summary>
        public string Description { get; set; }

        public string RowId { get; set; }

        public UserBlank() { }

        public UserBlank GetCopy()
        {
            UserBlank userBlank = new UserBlank()
            {
                ID = this.ID,
                Questions = new QuestionBase[Questions.Length],
                Status = this.Status,
                Description = this.Description
            };

            for (var index = 0; index < this.Questions.Length; index++)
            {
                if (Questions[index] is QuestionText)
                    userBlank.Questions[index] = new QuestionText()
                    {
                        Completed = false,
                        ColumnName = Questions[index].ColumnName,
                        QuestionText = Questions[index].QuestionText
                    };
            }

            return userBlank;
        }

        public QuestionBase GetNextQuestion()
        {
            foreach (QuestionBase question in Questions)
            {
                if (!question.Completed)
                    return question;
            }

            return null;
        }

        public bool IsCompleted()
        {
            return Questions.All(x => x.Completed);
        }

        /// <summary>
        /// Сообщение для админа при подтверждении заявки
        /// </summary>
        /// <returns></returns>
        public string GetAdminForm()
        {
            StringBuilder form = new StringBuilder();
            form.Append("Новая заявка от /" + ID + "/");
            form.AppendLine();
            for (int i = 0; i < Questions.Length; i++)
            {
                form.AppendLine(Questions[i].QuestionText + " : " + Questions[i].GetAnswerText());
            }

            return form.ToString();
        }
    }
}
