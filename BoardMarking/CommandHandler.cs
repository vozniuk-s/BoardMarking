using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BoardMarking
{
    internal class CommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserStateManager _userStateManager;
        private readonly DatabaseService _databaseService;
        //private readonly Configuration _configuration;   // Configuration - для Configuration.json в корні


        public CommandHandler(ITelegramBotClient botClient, UserStateManager userStateManager, DatabaseService databaseService)
        {
            _botClient = botClient;
            _userStateManager = userStateManager;
            _databaseService = databaseService;
            //_configuration = configuration;
        }

        // Робота з повідомленням
        private async Task SendMessageAsync(long chatId, string text)
        {
            // chatId = userId

            await DeleteLastMessageAsync(chatId);

            var lastBotMessage = await _botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: text,
                   disableNotification: true
                   );

            await _userStateManager.SetLastBotMessageAsync(chatId, lastBotMessage);
        }
        private async Task SendMessageAsync(long chatId, string text, InlineKeyboardMarkup inlineKeyboard)
        {
            // chatId = userId

            await DeleteLastMessageAsync(chatId);

            var lastBotMessage = await _botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: text,
                   replyMarkup: inlineKeyboard,
                   disableNotification: true
                   );

            await _userStateManager.SetLastBotMessageAsync(chatId, lastBotMessage);
        }
        private async Task SendMessageAsync(long chatId, string text, ReplyKeyboardMarkup keyboard)
        {
            // chatId = userId

            await DeleteLastMessageAsync(chatId);

            var lastBotMessage = await _botClient.SendTextMessageAsync(
                   chatId: chatId,
                   text: text,
                   replyMarkup: keyboard,
                   disableNotification: true
                   );

            await _userStateManager.SetLastBotMessageAsync(chatId, lastBotMessage);
        }
        private async Task DeleteLastMessageAsync(long userId)
        {
            // chatId = userId
            if (await _userStateManager.GetLastBotMessageAsync(userId) == null)
                return;

            var messageId = (await _userStateManager.GetLastBotMessageAsync(userId)).MessageId;

            try
            {
                await _botClient.DeleteMessageAsync(userId, messageId);
                Console.WriteLine($"Message {_userStateManager.GetLastBotMessageAsync(userId).Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete message {messageId}: {ex.Message}");
            }
        }

        // Команди для всіх
        public async Task HandleRegisterCommandAsync(long userId)
        {
            if(!await _databaseService.IsUserExistAsync(userId) || await _databaseService.GetUserStateAsync(userId) == StateType.None)
                await RegistrationAsync(userId);
        }
        public async Task HandleMarkCommandAsync(long userId)
        {
            if (await _databaseService.GetUserStateAsync(userId) == StateType.OnMarking || !await _databaseService.IsUserExistAsync(userId))
            {
                if (!await _databaseService.IsAlreadyMarckedAsync(userId, DateTime.Today))
                {
                    if (await _databaseService.IsTodayExistEventAsync(DateTime.Today))
                    {
                        if (DateTime.Now.Hour < 20 && DateTime.Now.Hour >= 16)
                            await MarkingAsync(userId);
                        else
                            await SendMessageAsync(userId, "Сьогоднішня подія закінчилася, або ще не розпочалася");
                    }
                    else
                        await SendMessageAsync(userId, "Сьогодні немає активної події");
                }
                else
                {
                    await SendMessageAsync(userId, "Вас уже відмічено");
                }
            }
            else
            {
                await RegistrationAsync(userId);
            }
        }

        // Виконавчі методи
        private async Task RegistrationAsync(long userId)
        {
            var chatId = userId;

            switch (await _userStateManager.GetUserStateAsync(userId))
            {
                case StateType.None:
                    await SendMessageAsync(chatId, "Представте себе, наприклад, вкажіть повне ім'я або псевдонім");
                    await Console.Out.WriteLineAsync($"Registartion: {userId}");
                    break;
                case StateType.OnMarking:
                    await _databaseService.SaveUserAsync(userId, await _userStateManager.GetUserAsync(userId));
                    await SendMessageAsync(chatId, "Використайте команду /mark для відмітки");
                    break;
            }
        } 

        private async Task MarkingAsync(long userId)
        {
            var chatId = userId;

            await AddUserToEventAsync(userId);
            await SendMessageAsync(chatId, "Вас відмічено");
        }
        // Добавити юзера на івент, як присутнього
        private async Task AddUserToEventAsync(long userId)
        {
            await _databaseService.AddUserToEventAsync(userId, DateTime.Today);
        }
    }
}
