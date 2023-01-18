namespace social_bot.User
{
    /// <summary>
    /// Вопрос заявки с текстовым ответом
    /// </summary>
    public class QuestionText : QuestionBase
    {
        /// <summary>
        /// Текстовый ответ
        /// </summary>
        public string AnswerText { get; set; }

        public QuestionText() { }

        public override string GetAnswerText()
        {
            return AnswerText;
        }

        public void SetAnswer(string _answerText)
        {
            if (string.IsNullOrEmpty(_answerText))
                return;
            Complete();
            AnswerText = _answerText;
        }
    }
}
