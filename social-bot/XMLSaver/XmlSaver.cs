using System.IO;
using System.Xml.Serialization;

namespace Bot.XmlSaver
{
    /// <summary>
    /// XMl загрузчик
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlSaver<T> where T : IXmlSavable
    {
        private static XmlSaver<T> instance;

        public static XmlSaver<T> Instance => instance ??= new XmlSaver<T>();

        public T Load(string _path)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            T result = default(T);
            if (File.Exists(_path))
            {
                using (FileStream fs = new FileStream(_path, FileMode.Open))
                {
                    result = (T) formatter.Deserialize(fs);
                }
            }

            return result;
        }

        public void Save(string _path, T _target)
        {
            using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(fs, _target);
            }
        }
    }
}
