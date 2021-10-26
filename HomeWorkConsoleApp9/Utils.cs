using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace HomeWorkConsoleApp9
{
    public class Utils
    {
        /// <summary>
        /// Проверка на правильность введённых данных с клавиатуры
        /// </summary>
        /// <param name="minValue">Минимальное допустимое значение</param>
        /// <param name="maxValue">Максимальное допустимое значение</param>
        /// <returns>Результат чтения строки (null не проходит по условиям)</returns>
        public static long? WaitEnterPass(long minValue, long maxValue, bool okSpace)
        {
            string input = Console.ReadLine();
            bool result = long.TryParse(input, out long outNumber);
            if (result && outNumber >= minValue && outNumber <= maxValue)
            {
                return outNumber;
            }
            else if (input.Equals("") && okSpace)
            {
                return -1;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Ожидает пока игрок введёт число в правильном диапазоне
        /// </summary>
        /// <param name="text">Текст который выводиться перед тем как ввести число</param>
        /// <param name="minValue">Минимальное допустимое значение</param>
        /// <param name="maxValue">Максимальное допустимое значение</param>
        /// <returns>правильно введенное число</returns>
        public static long WaitEnterPassAddText(string text, long minValue = long.MinValue, long maxValue = long.MaxValue, bool okSpace = true)
        {
            while (true)
            {
                Console.Write(text);
                var readNumberOrNull = WaitEnterPass(minValue, maxValue, okSpace);

                if (readNumberOrNull != null)
                {
                    return (long)readNumberOrNull;
                }
                else
                {
                    Console.WriteLine("Ошибка ввода");
                }
            }
        }

        private readonly string filename = "bots.db";

        private void AddBot()
        {


            Console.Write("Название для бота: ");
            var name = Console.ReadLine();
            Console.Write("Токен для telegram: ");
            var token = Console.ReadLine();

            List<StructBot> bots = new();

            var reader = ReadBotAsync();

            if (reader != null)
            {
                bots = reader.ToList();
            }

            StructBot bot = new()
            {
                BotName = name,
                Token = token
            };

            bots.Add(bot);
            FileStream fs = FileWait();
            XmlSerializer x = new(typeof(StructBot[]));
            TextWriter writer = new StreamWriter(fs);
            x.Serialize(writer, bots.ToArray());

        }

        private FileStream FileWait()
        {
            FileStream fs;
            while (true)
            {
                fs = new(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (fs.CanRead)
                {
                    Thread.Sleep(1000);
                    break;
                }
            }

            return fs;
        }

        private StructBot[] ReadBotAsync() {
            StructBot[] outer;
                
            XmlSerializer x = new(typeof(StructBot[]));
            FileStream fs = FileWait();

            TextReader reader = new StreamReader(fs);
            try
            {
                outer = (StructBot[])x.Deserialize(reader);
            }
            catch (InvalidOperationException) {
                outer = Array.Empty<StructBot>();
            }
            return outer;
        }

        public StructBot SwitchBot() {
            long select;
            StructBot[] listBot;
            bool isNotCreatedBot = true;
            while (true)
            {
                listBot = ReadBotAsync();
                var countBot = listBot.Length;
                if (countBot > 0 && isNotCreatedBot)
                {
                    string listBots = "Выберите бота:\nДля добавления бота нажмите пробел\n";
                    int index = 1;
                    foreach (var item in listBot)
                    {
                        listBots += $"{index++}) {item.BotName}\n";
                    }

                    select = WaitEnterPassAddText(listBots, 1, countBot);
                    if (select == -1)
                    {
                        isNotCreatedBot = false;
                    }
                    else {
                        break;
                    }
                    
                }
                else
                {
                    AddBot();
                    Console.WriteLine($"Для добавление ещё одного бота нажмите \"Y\"");
                    Console.WriteLine($"Для продолжения нажмите любую клавишу");
                    var key = Console.ReadKey();//Чтение нажатой кнопки
                    if (key.Key == ConsoleKey.Y)
                    {
                        isNotCreatedBot = false;
                    }
                    else {
                        isNotCreatedBot = true;
                    }
                    Console.WriteLine();
                    Thread.Sleep(1000);
                }
            }
            return listBot[select-1];
        }
    }
}
