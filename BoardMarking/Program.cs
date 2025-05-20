using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Newtonsoft.Json;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Configuration;

namespace BoardMarking
{

    internal class Program
    {
        private static readonly string jsonPath = "../../Configuration.json";
        private static TelegramBotClient _botClient;
        private static UserStateManager _userStateManager;
        private static DatabaseService _databaseService;
        private static CommandHandler _commandHandler;
        //private static Configuration _configuration;
        private static GoogleSheetsManager _googleSheetsManager;

        static async Task Main(string[] args)
        {
            // Ініціалізація
            string json = System.IO.File.ReadAllText(jsonPath);
            //_configuration = JsonConvert.DeserializeObject<Configuration>(json);
            _botClient = new TelegramBotClient("bot");
            _databaseService = new DatabaseService($"Server={"localhost"};Database={"bg_database"};User ID={"root"};Password={"psw"};Charset={"utf8mb4"};");
            _userStateManager = new UserStateManager(_databaseService);
            //_commandHandler = new CommandHandler(_botClient, _userStateManager, _databaseService, _configuration);
            _commandHandler = new CommandHandler(_botClient, _userStateManager, _databaseService);

            // Запуск бота
            var cts = new System.Threading.CancellationTokenSource();
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } // Типи отримуваних оновлень
                },
                cancellationToken: cts.Token
            );

            Console.WriteLine("Bot started.");
            Console.ReadKey();
            cts.Cancel();
        }
        
        // Метод, який отримує оновлення з чату
        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, System.Threading.CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                if (update.Message.Text.StartsWith("/"))
                {
                    await HandleCommandAsync(update.Message);
                }
                else
                {
                    await HandleTextMessageAsync(update.Message);
                }
            }
        }
        // Обробка, якщо введено команду
        private static async Task HandleCommandAsync(Message message)
        {
            switch (message.Text)
            {
                case "/start":
                    await _commandHandler.HandleRegisterCommandAsync(message.From.Id);
                    break;
                case "/mark":
                    await _commandHandler.HandleMarkCommandAsync(message.From.Id);
                    break;
            }
        }
        // Обробка, якщо введено/відправлене кнопкою текстове повідомлення
        private static async Task HandleTextMessageAsync(Message message)
        {
            long userId = message.From.Id;

            // Отриманя результатів при представлені
            await _userStateManager.SaveUserStatesAsync(userId, StateType.OnMarking, message);
            await _commandHandler.HandleRegisterCommandAsync(userId);
        }

        // Метод для обробки помилок
        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine($"Main Exception: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}