using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace TELEGA
{
    internal static class BotCommands
    {
        public static bool IsReceiveAnswer { get; set; } = true;
        private static List<string> answerList = new XMLCreator().ReadXmlConfig<string>(pathDirectory : default, nameOfFile : "defaultAnswerList");
        
        public static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            string botStandartAnswer = $"{answerList[new Random().Next(0, answerList.Count)]}";

            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "🎲 Добро пожаловать на борт, добрый путник!");
                ConsoleLogMessages("Добро пожаловать на борт, добрый путник!", message, message.Text);
                return;
            }
            if (IsReceiveAnswer) await botClient.SendTextMessageAsync(message.Chat, botStandartAnswer);
                          
            ConsoleLogMessages(botStandartAnswer, message, message.Text);
            
        }
        public static void ConsoleLogMessages(string botStandartAnswer, Message message, string messageText)
        {
            string userName = message.Chat.Username ?? message.Chat.FirstName;
            Console.WriteLine($"{new string('-', Console.BufferWidth-1)}\nНомер чата - {message.Chat.Id}. Время: {message.Date.ToLocalTime()}");
            Console.WriteLine($"{userName} отправил:\t{messageText}");
            if(IsReceiveAnswer)
                Console.WriteLine($"Бот ответил:\t{botStandartAnswer}");
            else
                Console.WriteLine($"Авто-ответ отключен.\nПараметр Receive == false. Что бы включить введите /receive");
        }

        
    }
}
