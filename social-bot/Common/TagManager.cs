namespace social_bot.Common
{
    /// <summary>
    /// Менеджер тегов
    /// </summary>
    public static class TagManager
    {

        #region StandartCommands

        public const string Help = "/help";
        public const string Start = "/start";

        #endregion

        #region CommonTags

        public const string Next = "Далее";
        public const string Back = "Назад";
        public const string Exception = "Что-то пошло не так.";
        public const string Welcome = "Добро пожаловать! Выберите действие";
        public const string Yes = "Да";
        public const string No = "Нет";

        #endregion

        #region AdminCommands

        public const string Approve = "Подтвердить";
        public const string Deny = "Отклонить";

        public const string Admin = "/admin";
        public const string NoAdmin = "/noadmin";
        public const string Password = "dvhq2813";
        public const string Verification = "Чтобы стать администратором, введите пароль.";
        public const string WrongPassword = "Неверный пароль.";
        public const string RightPassword = "Вы назначены администратором. Теперь вы можете подтверждать и отклонять анкеты пользователей.";
        public const string AlreadyAdmin = "Вы уже являетесь администратором. Чтобы удалить себя из списка администраторов отправьте команду /noadmin .";
        public const string RemovedAdmin = "Вы больше не являетесь администратором. Чтобы снова добавить себя в список введите /admin .";

        #endregion

        #region CustomCommands

        public const string BackToMainMenu = "В главное меню.";
        public const string Represent = "Хочу представиться";
        public const string GetUserInfo = "Получить информацию о пользователе";
        public const string SendUserName = "Пришлите псевдоним пользователя";
        public const string NoUserName = "Описание пользователя с таким псевдонимом не найдено";
        public const string EmptyAnswer = "Пожалуйста, ответьте на вопрос.";
        public const string WaitForApprove = "Заявка отправлена! Пожалуйста, ожидайте подтверждения от модератора чата.";
        public const string AccessApproved = "Поздравляем, ваша заявка одобрена! Пройдите по ссылке, чтобы вступить в основной чат социализации {0}";
        public const string AccessDenied = "К сожалению, в доступе отказано.";
        public const string SolutionSent = "Отправлено";
        public const string CareerBotQuestion = "Готовы ли вы рекомендовать участников клуба в свою компанию?";
        public const string CareerBot = "Предлагаем вам зарегистрироваться также в карьерном боте {0}";
        public const string CoffeeBotQuestion = "Хотите ли вы участвовать в знакомствах с участниками клуба?";
        public const string CoffeeBot = "Тогда вступите в кофебот {0}";
        public const string RegistrationCompleted = "Регистрация окончена. Удачи!";

        #endregion
    }
}
