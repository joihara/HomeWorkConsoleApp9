using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.WebRequestMethods;

namespace HomeWorkConsoleApp9
{
    public class ActionBot
    {
        public static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Используйте:\n" +
                                 "/message - переписка с ботом\n" +
                                 "/file    - отправить файл боту\n";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: usage,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        public static async Task<Message> Other(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Введите комманду:\n/start";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: usage,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        public static async Task<Message> DownloadFile(ITelegramBotClient botClient, Message message)
        {
            Directory.CreateDirectory("Files");

            var selectId = message.Type switch
            {
                MessageType.Photo => message.Photo[0].FileId,
                MessageType.Audio => message.Audio.FileId,
                MessageType.Video => message.Video.FileId,
                MessageType.Voice => message.Voice.FileId,
                MessageType.Document => message.Document.FileId,
                MessageType.Sticker => message.Sticker.FileId,
                _ => throw new NotImplementedException()
            };

            var nameFile = message.Type switch
            {
                MessageType.Photo => message.Photo[0].FileId,
                MessageType.Audio => message.Audio.FileName,
                MessageType.Video => message.Video.FileName,
                MessageType.Voice => message.Voice.FileId,
                MessageType.Document => message.Document.FileName,
                MessageType.Sticker => message.Sticker.FileId,
                _ => throw new NotImplementedException()
            };

            var file = await botClient.GetFileAsync(selectId);
            FileStream fs = new (@$"Files/{nameFile}", FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();

            return await botClient.SendTextMessageAsync(message.Chat.Id, "Файл сохранён");
        }

        public static async Task<Message> SendFile(ITelegramBotClient botClient, Message message)
        {
            string[] files = message.Text.Split(' ',2);
            if (files.Length > 1)
            {
                var file = files[1];
                FileStream stream;
                try
                {
                    stream = System.IO.File.OpenRead($@"Files/{file}");
                }
                catch (FileNotFoundException)
                {
                    return await botClient.SendTextMessageAsync(message.Chat.Id, "Файл не существует");
                }
                InputOnlineFile inputOnlineFile = new(stream, file);
                return await botClient.SendDocumentAsync(message.Chat.Id, inputOnlineFile);
            }
            return await botClient.SendTextMessageAsync(message.Chat.Id, "Не введено название файла");
        }

        public static async Task<Message> GetListFile(ITelegramBotClient botClient, Message message)
        {
            var list = Directory.GetFiles("Files");

            var outMessage = "";
            int index = 1;
            foreach (var item in list)
            {
                outMessage += $"{index++}){item.Split("\\")[1]}\n";
            }

            return await botClient.SendTextMessageAsync(message.Chat.Id, outMessage);
        }
    }
}
