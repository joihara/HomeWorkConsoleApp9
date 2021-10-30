using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWorkConsoleApp9
{
    public class BotTelegram
    {
        static TelegramBotClient bot;

        public async Task InitAsync(string token)
        {

            #region exc

            ////// https://hidemyna.me/ru/proxy-list/?maxtime=250#list

            //// Содержит параметры HTTP-прокси для System.Net.WebRequest класса.
            //var proxy = new WebProxy()
            //{
            //    Address = new Uri($"http://77.87.240.74:3128"),
            //    UseDefaultCredentials = false,
            //    //Credentials = new NetworkCredential(userName: "login", password: "password")
            //};

            //// Создает экземпляр класса System.Net.Http.HttpClientHandler.
            //var httpClientHandler = new HttpClientHandler() { Proxy = proxy };

            //// Предоставляет базовый класс для отправки HTTP-запросов и получения HTTP-ответов 
            //// от ресурса с заданным URI.
            //HttpClient hc = new(httpClientHandler);

            //bot = new TelegramBotClient(token, hc);

            #endregion
            Console.Clear();
            bot = new TelegramBotClient(token);
            var me = await bot.GetMeAsync();
            Console.Title = me.Username;

            using var cts = new CancellationTokenSource();

            bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            Console.WriteLine($"Индетификатор: {me.Id} Имя: {me.FirstName}");
            Console.ReadLine();

            cts.Cancel();
        }

        private void Command() {
            while (true) {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Обработка ошибок
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Метод обработки событий
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }

        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Неизвестный тип обновления: {update.Type}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод обработки сообщений пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            bool selectId = message.Type switch
            {
                MessageType.Photo => true,
                MessageType.Voice => true,
                MessageType.Document => true,
                _ => false
            };
            if (message.Type == MessageType.Text)
            {
                var action = message.EntityValues.First() switch
                {
                    "/start" => ActionBot.Usage(botClient, message),
                    "/SendFile" => ActionBot.SendFile(botClient, message),
                    "/Files"=> ActionBot.GetListFile(botClient, message),
                    "/News" => ActionBot.GetNews(botClient, message),
                    _ => ActionBot.Other(botClient, message)
                };

                var sentMessage = await action;
                Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
            } else if (selectId) {
                await ActionBot.DownloadFile(botClient, message);
            }
            
        }


    }
}
