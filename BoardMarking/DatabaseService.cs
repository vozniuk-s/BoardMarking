using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BoardMarking
{
    internal class DatabaseService
    {
        private readonly DatabaseManager _databaseManager;

        public DatabaseService(string connectionString)
        {
            _databaseManager = new DatabaseManager(connectionString);
        }
        public async Task<bool> IsUserExistAsync(long userId)
        {
            return await _databaseManager.IsUserExistAsync(userId);
        }

        public async Task<StateType> GetUserStateAsync(long userId)
        {
            return await _databaseManager.GetUserStateAsync(userId);

        }
        public async Task<bool> IsTodayExistEventAsync(DateTime date)
        {
            return await _databaseManager.IsTodayExistEventAsync(date);
        }
        public async Task<bool> IsAlreadyMarckedAsync(long userId, DateTime date)
        {
            return await _databaseManager.IsAlreadyMarckedAsync(userId, date);
        }
        public async Task AddUserToEventAsync(long userId, DateTime date)
        {
            await _databaseManager.AddUserToEventAsync(userId, date);
        }
        public async Task SaveUserAsync(long userId, User user)
        {
            await _databaseManager.SaveUserAsync(userId, user);
        }
    }
}
