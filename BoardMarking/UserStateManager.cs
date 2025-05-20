using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BoardMarking
{
    internal class UserStateManager
    {
        // Словник для зберігання стану користувачів
        private Dictionary<long, User> _userStates = new Dictionary<long, User>();
        private readonly DatabaseService _databaseService;

        public UserStateManager(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task SaveUserStatesAsync(long userId, StateType state, Message message)
        {
            if (!_userStates.ContainsKey(userId))
            {
                _userStates.Add(userId, new User(message.From.Id, message.Text, message.Chat.FirstName + " " + message.Chat.LastName, message.Chat.Username, state));
            }
        }
        public async Task<StateType> GetUserStateAsync(long userId)
        {
            if (_userStates.ContainsKey(userId))
            {
                _userStates.TryGetValue(userId, out var user);
                return user.State;
            }
            else
            {
                await Console.Out.WriteLineAsync("Error during getting State");
                return StateType.None;
            }
        }
        public async Task<Message> GetLastBotMessageAsync(long userId)
        {
            if (_userStates.ContainsKey(userId))
            {
                _userStates.TryGetValue(userId, out var user);
                return user.LastBotMessage;
            }
            else
            {
                await Console.Out.WriteLineAsync("Error during getting Last Bot Message");
                return null;
            }
        }
        public async Task SetUserStateAsync(long userId, StateType state)
        {
            if (_userStates.ContainsKey(userId))
            {
                _userStates.TryGetValue(userId, out var user);
                user.State = state;
            }
            else
            {
                await Console.Out.WriteLineAsync("Error during setting State");
            }
        }
        public async Task SetLastBotMessageAsync(long userId, Message message)
        {
            if (_userStates.ContainsKey(userId))
            {
                _userStates.TryGetValue(userId, out var user);
                user.LastBotMessage = message;
            }
            else
            {
                await Console.Out.WriteLineAsync("Error during setting Last Bot Message");
            }
        }
        public async Task<User> GetUserAsync(long userId)
        {
            if (_userStates.ContainsKey(userId))
            {
                _userStates.TryGetValue(userId, out var user);
                return user;
            }
            else
            {
                await Console.Out.WriteLineAsync("Error during getting User");
                return new User();
            }
        }
    }
}
