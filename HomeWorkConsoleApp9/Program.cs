using System;
using System.Threading.Tasks;

namespace HomeWorkConsoleApp9
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BotTelegram botTelegram = new();
            
            Utils utils = new();
            var bot = utils.SwitchBot();

            await botTelegram.InitAsync(bot.Token);

            Console.WriteLine();
        }
    }
}
