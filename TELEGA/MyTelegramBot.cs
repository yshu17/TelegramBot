using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;


namespace TELEGA
{
    public class MyTelegramBot
    {
        public static ITelegramBotClient botClient = new TelegramBotClient(Configuration.BotToken);     //Устанавливаем токен для работы с нашим ботом
        public static CancellationTokenSource cancellationTokenSource = NewCanccellationTokenSoure();     //Токен отмены для Task
        public static ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,                 // receive all update types
        };
        
        private static void Main()
        {
            ConsoleCommands.ColorfullRed += new ColorfullMessages().DisplayRedMessage;
            ConsoleCommands.ColorfullYellow += new ColorfullMessages().DisplayYellowMessage;
            ConsoleCommands.ColorfullGreen += new ColorfullMessages().DisplayGreenMessage;

            Console.WriteLine("Запуск бота - /start.\nCписок доступных команд - /help.");

            while (true) ConsoleCommands.ChooseCommand(Console.ReadLine());
        } 
        public static CancellationTokenSource NewCanccellationTokenSoure() => new CancellationTokenSource(); 

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
           
           
            //Обработчик Update - Этот объект представляет входящее обновление.
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await BotCommands.HandleMessage(botClient, update.Message);
                return;
            }
        }
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            botClient.GetMeAsync();
            Console.WriteLine($"Бот {botClient.GetMeAsync().Result.FirstName} не запущен.");
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            ConsoleCommands.Stop();
            Console.WriteLine("Бот аварийно остановлен. Что бы запустить бота введите /start");
            return Task.CompletedTask;

        }
    }
}