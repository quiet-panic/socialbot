using System.Collections.Generic;

namespace social_bot.Common
{
    /// <summary>
    /// Класс со структурой сообщения ответа пользователю
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Тексты кнопкок reply
        /// </summary>
        public List<string> buttonTexts { get; } = new List<string>();

        /// <summary>
        /// Нужно ли удалять клавиатуру
        /// </summary>
        public bool ClearKeyboard { get; }

        public Answer() { }

        public Answer(string _text, params string[] _buttons)
        {
            Text = _text;
            buttonTexts.AddRange(_buttons);
            ClearKeyboard = false;
        }

        public Answer(string _text, bool _clearKeyboard = true)
        {
            Text = _text;
            ClearKeyboard = _clearKeyboard;
        }
    }
}
