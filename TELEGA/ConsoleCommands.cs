using System;
using System.Collections.Generic;
using Telegram.Bot;
using System.Diagnostics;
using System.Linq;

namespace TELEGA
{
    public static class ConsoleCommands
    {

        public static event Action<string> ColorfullRed, ColorfullYellow, ColorfullGreen;
        public static bool IsLaunched { get; private set; } = false;
        public static void ChooseCommand(string inputCommand)
        {
            var tokens = SplitIntoTokens(inputCommand);
            var command = tokens.FirstOrDefault();

            Dictionary<string, Action<IEnumerable<string>>> commands = new Dictionary<string, Action<IEnumerable<string>>>()
            {
                { "/start",     Command(() => Start() )},
                { "/stop",      Command(() => Stop() )},
                { "/clear",     Command(() => Console.Clear() )},
                { "/receive",   Command(() => ReceiveAnswer() )},
                { "/configs",   Command(() => ConfigFolder() )},
                { "/quit",      Command(() => Quit() )},
                { "/help",      Command(() => Help() )},
                { "/leave",     SingleCommand(() => LeaveChat(tokens.ElementAt(1)) )},
                { "/copy",      MultyCommand(() => CopyTo(tokens.ElementAt(1), tokens.ElementAt(2), tokens.ElementAt(3)) )},
                { "/ban",       MultyCommand(() => BanMember(tokens.ElementAt(1), tokens.ElementAt(2)) )},
                { "/unban",     MultyCommand(() => UnBanMember(tokens.ElementAt(1), tokens.ElementAt(2)) )},
                { "/send",      MultyCommand(() => Send(tokens.ElementAt(1), tokens.Skip(2)) )},
            };

            if (command == null) 
                Help();
            else
            {
                if (commands.ContainsKey(command))
                {
                    try
                    {
                        commands[command](tokens.Skip(1));
                    } 
                    catch (Exception e)
                    {   
                        ColorfullRed?.Invoke("Операция не выполнена: " + e.Message); 
                    }
                }
                else
                    ColorfullYellow?.Invoke("Неизвестная команда: " + command);    
            }
        }

        private static Action<IEnumerable<string>> Command(Action action)
        {
            return args =>
            {
                if (args.Any())
                    throw new ArgumentException("эта команда не поддерживает аргументы");
                action();
            };
        }
        private static Action<IEnumerable<string>> MultyCommand(Action action)
        {
            return args =>
            {
                int i = args.Count();
                if (i<2)
                    throw new ArgumentException("эта команда работает только с двумя аргументами");
                action();
            };
        }
        private static Action<IEnumerable<string>> SingleCommand(Action action)
        {
            return args =>
            {
                int i = args.Count();
                if (i != 1)
                    throw new ArgumentException("эта команда работает только одним аргументом");
                action();
            };
        }
        private static IEnumerable<string> SplitIntoTokens(string inputString)
        {
            var result = inputString.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
             
            return result;
        }

        private static async void Start()
        {
            var me = await MyTelegramBot.botClient.GetMeAsync();

            ColorfullGreen?.Invoke("Запуск..");
            MyTelegramBot.botClient.StartReceiving(updateHandler: MyTelegramBot.HandleUpdateAsync, pollingErrorHandler: MyTelegramBot.HandleErrorAsync,
               receiverOptions: MyTelegramBot.receiverOptions, cancellationToken: MyTelegramBot.cancellationTokenSource.Token);

            ColorfullGreen?.Invoke($"{me.FirstName} запущен.");
            IsLaunched = true;
        }
        internal static async void Stop()
        {
            var me = await MyTelegramBot.botClient.GetMeAsync();
            MyTelegramBot.cancellationTokenSource.Cancel();
            ColorfullGreen?.Invoke($"{me.FirstName} успешно остановлен.");
            //Установка нового токена отмены после ее завершения.
            MyTelegramBot.cancellationTokenSource = MyTelegramBot.NewCanccellationTokenSoure();
            IsLaunched = false;
        }

        private static void Help()
        {
            Console.Clear();
            string title = " Список команд My Botinok ";
            int margin = (((Console.BufferWidth - 1) / 2) - title.Length / 2);
            var helpList = new XMLCreator().ReadXmlConfig<string>(pathDirectory: default, nameOfFile: "defaultHelpList");
            Console.WriteLine($"{new string('-', margin-1)} {title} {new string('-', margin)}");
            foreach (var item in helpList)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(new string('-', Console.BufferWidth-1));
        }
        private static void ConfigFolder() => Process.Start("explorer.exe", new XMLCreator().DefaultPath);
        private static void Quit() => Process.GetCurrentProcess().Kill();
        private static void ReceiveAnswer()
        {
            if(BotCommands.IsReceiveAnswer)
                BotCommands.IsReceiveAnswer= false;
            else
                BotCommands.IsReceiveAnswer = true;
            Console.WriteLine(BotCommands.IsReceiveAnswer);
        }

        private static async void Send(string chatId, IEnumerable<string> message)
        {
            try
            {
                if (IsLaunched)
                {
                    string dump = "";
                    foreach (var item in message)
                    {
                        dump += $"{item} ";
                    }
                    await MyTelegramBot.botClient.SendTextMessageAsync(chatId, dump, cancellationToken: MyTelegramBot.cancellationTokenSource.Token);
                }
                else throw new Exception("Что бы использовать данную команду нужно запустить бота. Для запуска введите /start"); 
            }
            catch (Exception e)
            {
                Console.WriteLine("Cообщение не отправлено");
                Console.WriteLine("Операция не выполнена: " + e.Message);
            }
        }
        private static async void CopyTo(string v1, string v2, string v3) //TODO: Почитать как работает CopyMessageAsync
        {
            try
            {
                if (IsLaunched)
                    await MyTelegramBot.botClient.CopyMessageAsync(v1, v2, Convert.ToInt32(v3)); 
                else throw new Exception("Что бы использовать данную команду нужно запустить бота. Для запуска введите /start");
            }
            catch (Exception e)
            {
                ColorfullRed?.Invoke("Бот не покинул чат\nОперация не выполнена: " + e.Message);
     
            }
        }
        private static async void LeaveChat(string v1)
        {
            try
            {
                if (IsLaunched)
                {
                    await MyTelegramBot.botClient.LeaveChatAsync(v1);
                    Console.WriteLine("Бот покинул чат");
                }
                else throw new Exception("Что бы использовать данную команду нужно запустить бота. Для запуска введите /start");
            }
            catch (Exception e)
            {
                ColorfullRed.Invoke($"Бот не покинул чат\nОперация не выполнена: " + e.Message);
            }
        }
        private static async void BanMember(string v1,string v2) 
        {  
            try
            {
                if (IsLaunched)
                {
                    await MyTelegramBot.botClient.BanChatMemberAsync(v1, Convert.ToInt64(v2));
                    ColorfullGreen?.Invoke($"Пользователь с Id-{v1} был успешно забанен.");
                }
                else throw new Exception("Что бы использовать данную команду нужно запустить бота. Для запуска введите /start");
            }
            catch (Exception e)
            {
                ColorfullRed.Invoke($"Пользователь с Id-{v1} не был забанен.\nОперация не выполнена: " + e.Message);   
            } 
        }
        private static async void UnBanMember(string v1, string v2)
        {
            try
            {
                if (IsLaunched)
                {
                    await MyTelegramBot.botClient.BanChatMemberAsync(v1, Convert.ToInt64(v2));
                    Console.WriteLine($"Пользователь с Id-{v1} был забанен.");
                }
                else throw new Exception("Что бы использовать данную команду нужно запустить бота. Для запуска введите /start");
            }
            catch (Exception e)
            {
                ColorfullRed.Invoke($"Пользователь с Id-{v1} не был разбанен.\nОперация не выполнена: " + e.Message); 
            }
        }
    }
}
