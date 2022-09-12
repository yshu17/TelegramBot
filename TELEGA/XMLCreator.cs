using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System;

namespace TELEGA
{   
    /// <summary>
    /// Класс создает или читает список настроек из XML файла.
    /// </summary>
    internal class XMLCreator
    {
        static XMLCreator()
        {
            if (!File.Exists(@"xmls\defaultAnswerList.xml")) 
                new XMLCreator().CreateXmlConfig(default, "defaultAnswerList", new XMLCreator().defaultAnswerList);

            if (!File.Exists(@"xmls\defaultHelpList.xml")) 
                new XMLCreator().CreateXmlConfig(default, "defaultHelpList", new XMLCreator().defaultHelpList);
        }
        public string DefaultPath { get; private set; } = @"xmls\";
        
        private List<string> defaultAnswerList = new List<string>
        {
                "Ага...", "Ты же знаешь что я отвечать на вот это не буду?", "И зачем ты мне это написал?",
                "да что ты говоришь...","Ты втираешь мне какую-то дичь", "Ты уже начинаешь надоедать мне"
        };
        private List<string> defaultHelpList = new List<string>
        {     
          "/start - запуск бота.",
          "/stop - остановить бота.",
          "/clear - очистить консоль",
          "/configs - открыть папку с настройками бота.",
          "/receive - включить/выключить авто-ответ на сообщения пользователя.",
          "/quit - закрыть приложение.",
          "/help - список доступных команд.",
          "/leave [id чата] - покинуть чат.",
          "/copy [id чата] [путь с именем] - скопировать сообщение из чата в файл. [В разработке]",
          "/ban [id чата] [id пользователя] - заблокировать пользователя.",
          "/unban [id чата] [id пользователя] - разблокировать пользователя.",
          "/send [id чата] [сообщение] - отправить сообщение собеседнику."
        };

        public void CreateXmlConfig(string pathDirectory,string nameOfFile ,List<string> list)
        {
            nameOfFile ??= "defaultName";
            pathDirectory ??= DefaultPath;
            list ??= defaultAnswerList;

            var serializer = new XmlSerializer(typeof(List<string>));
            using (var writer = new StreamWriter($"{pathDirectory+nameOfFile}.xml")) serializer.Serialize(writer, list);
        }
        
        public List<T> ReadXmlConfig<T>(string pathDirectory, string nameOfFile)
        {         
            pathDirectory ??= DefaultPath;

            var serializer = new XmlSerializer(typeof(List<T>));

            using (var reader = new StreamReader($"{pathDirectory + nameOfFile}.xml")) return (List<T>)serializer.Deserialize(reader);
        }
    }
}
