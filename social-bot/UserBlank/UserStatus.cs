namespace social_bot.User
{
    /// <summary>
    /// Статус членства в чатах
    /// </summary>
    public enum UserStatus
    {
        Inactive, //базовый, не ответил на вопросы
        WaitForApprove, // ответил на вопросы, ждёт подтверждения
        Approved, //подтвержден
        Denied //отклонен
    }
}
