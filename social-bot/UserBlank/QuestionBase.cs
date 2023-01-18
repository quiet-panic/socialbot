using System.Xml.Serialization;

namespace social_bot.User
{
    /// <summary>
    /// Вопрос заявки
    /// </summary>
    [XmlInclude(typeof(QuestionText))]
    public abstract class QuestionBase
    {
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string QuestionText { get; set; }

        public string ColumnName { get; set; }

        public bool Completed { get; set; } = false;

        /// <summary>
        /// Получает текстовую форму представления ответа
        /// </summary>
        /// <returns></returns>
        public abstract string GetAnswerText();

        public bool GotAnswer()
        {
            return !string.IsNullOrEmpty(GetAnswerText());
        }

        public void Complete()
        {
            Completed = true;
        }
    }
}
