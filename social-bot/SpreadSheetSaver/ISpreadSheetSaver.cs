using System.Collections.Generic;

namespace social_bot.SpreadSheetSaver
{
    /// <summary>
    /// Интерфейс сохранения в таблицу
    /// </summary>
    interface ISpreadSheetSaver
    {
        void Save();

        void Load();
    }
}
