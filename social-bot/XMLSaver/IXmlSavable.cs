namespace Bot.XmlSaver
{
    /// <summary>
    /// Может сохраняться в формате XML
    /// </summary>
    public interface IXmlSavable
    {

        void Save();

        void Load();
    }
}
