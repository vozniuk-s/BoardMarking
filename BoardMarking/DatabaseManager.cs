using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Google.Protobuf;
using MySql.Data.MySqlClient;
using Telegram.Bot.Types;

namespace BoardMarking
{
    internal class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> IsUserExistAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT COUNT(*) FROM users WHERE userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            return Convert.ToBoolean(result);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsUserExist(). Database error: {ex.Message}");
                return false;
            }
        }
        public async Task<StateType> GetUserStateAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT state FROM users WHERE userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            switch (result)
                            {
                                case "OnMarking":
                                    return StateType.OnMarking;
                                default:
                                    return StateType.None;
                            }
                        }
                        else
                        {
                            return StateType.None;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserStateAsync(). Database error: {ex.Message}");
                return StateType.None;
            }
        }
        public async Task<bool> IsTodayExistEventAsync(DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT COUNT(*) FROM events WHERE date = @date LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@date", date);

                        var result = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsTodayExistEventAsync(). Database error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IsAlreadyMarckedAsync(long userId, DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT COUNT(*) FROM users_at_events WHERE event_date = @event_date AND userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Передаємо параметри в запит
                        command.Parameters.AddWithValue("@event_date", date);
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IsAlreadyMarckedAsync(). Database error: {ex.Message}");
                return false;
            }
        }
        public async Task AddUserToEventAsync(long userId, DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "INSERT INTO users_at_events (userId, event_date) VALUES (@userId, @event_date)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@event_date", date);

                        int result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddUserToEventAsync(). Database error: {ex.Message}");
            }
        }
        public async Task<bool> SaveUserAsync(long userId, User user)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "INSERT INTO users (userId, telegram_username, telegram_name, name, state) VALUES (@userId, @telegram_username, @telegram_name, @name, @state)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@telegram_username", user.TelegramUsername);
                        command.Parameters.AddWithValue("@telegram_name", user.TelegramName);
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@state", user.State.ToString());

                        int result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error while: {ex.Message}");
                return false;
            }
        }



        ///
       /* public async Task<bool> IsUserCompleteRegistrationAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT registration_completed FROM users WHERE chatid = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            return Convert.ToBoolean(result);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IsUserAlreadyExistAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT COUNT(*) FROM users WHERE userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IsUserFromNUWEEAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT is_from_nuwee FROM users WHERE chatid = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            return Convert.ToBoolean(result);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
        public async Task UpdateSomeUserFieldsAsync(long userId, User user)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    // Перевірка на існування
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE userId = @userId";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            // Оновлення існуючого запису
                            string updateQuery = "UPDATE users SET full_name = @full_name, state = @state, is_from_nuwee = @is_from_nuwee, institute = @institute, registration_completed = @registration_completed, user_group = @user_group WHERE userId = @userId";
                            using (var updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@userId", userId);
                                updateCommand.Parameters.AddWithValue("@full_name", user.FullName);
                                updateCommand.Parameters.AddWithValue("@state", user.State.ToString());
                                updateCommand.Parameters.AddWithValue("@is_from_nuwee", user.IsFromNuwee);
                                updateCommand.Parameters.AddWithValue("@institute", user.Institute);
                                updateCommand.Parameters.AddWithValue("@registration_completed", user.RegistrationCompleted);
                                updateCommand.Parameters.AddWithValue("@user_group", user.Group);

                                int updateResult = await updateCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }       
        public async Task<User> GetUser(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT chatId, telegram_username, telegram_name, full_name, state, is_admin, is_from_nuwee, on_checking, institute, registration_completed, user_group FROM users WHERE userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                StateType state;

                                switch (reader.GetString(reader.GetOrdinal("state")))
                                {
                                    case "OnMarking": state = StateType.OnMarking;
                                        break;
                                        default: state = StateType.None; break;
                                }

                                var user = new User
                                {
                                    Chatid = reader.GetInt64(reader.GetOrdinal("chatId")),
                                    TelegramUsername = reader.GetString(reader.GetOrdinal("telegram_username")),
                                    TelegramName = reader.GetString(reader.GetOrdinal("telegram_name")),
                                    FullName = reader.GetString(reader.GetOrdinal("full_name")),
                                    State = state,
                                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("is_admin")),
                                    OnCheck = reader.GetBoolean(reader.GetOrdinal("on_checking")),
                                    IsFromNuwee = reader.GetBoolean(reader.GetOrdinal("is_from_nuwee")),
                                    Institute = reader.GetString(reader.GetOrdinal("institute")),
                                    RegistrationCompleted = reader.GetBoolean(reader.GetOrdinal("registration_completed")),
                                    Group = reader.GetString(reader.GetOrdinal("user_group"))
                                };

                                return user;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
        }
        public async Task SetUserOnMarkingStateAsync(long userId, bool value)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    // Перевірка на існування
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE userId = @userId";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            // Оновлення існуючого запису
                            string updateQuery = "UPDATE users SET on_checking = @on_checking WHERE userId = @userId";
                            using (var updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@userId", userId);
                                updateCommand.Parameters.AddWithValue("@on_checking", value);

                                int updateResult = await updateCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        public async Task<bool> IsAdminAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT is_admin FROM users WHERE chatid = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            return Convert.ToBoolean(result);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IsAlreadyMarckedTodayAsync(long userId, DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT COUNT(*) FROM users_at_events WHERE event_date = @event_date AND userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Передаємо параметри в запит
                        command.Parameters.AddWithValue("@event_date", date);
                        command.Parameters.AddWithValue("@userId", userId);

                        var result = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }
        public async Task AddNewEventAsync(long userId, DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "INSERT INTO events (date, createdBy) VALUES (@date, @createdBy)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@date", date);
                        command.Parameters.AddWithValue("@createdBy", userId);

                        int result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        public async Task<User> GetUserByCheckingStatusAsync()
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT chatId, telegram_username, telegram_name, full_name, state, is_admin, is_from_nuwee, on_checking, institute, registration_completed FROM users WHERE on_checking = 1 LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                StateType state;

                                switch (reader.GetString(reader.GetOrdinal("state")))
                                {
                                    case "OnMarking":
                                        state = StateType.OnMarking;
                                        break;
                                    default: state = StateType.None; break;
                                }

                                var user = new User
                                {
                                    Chatid = reader.GetInt64(reader.GetOrdinal("chatId")),
                                    TelegramUsername = reader.GetString(reader.GetOrdinal("telegram_username")),
                                    TelegramName = reader.GetString(reader.GetOrdinal("telegram_name")),
                                    FullName = reader.GetString(reader.GetOrdinal("full_name")),
                                    State = state,
                                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("is_admin")),
                                    OnCheck = reader.GetBoolean(reader.GetOrdinal("on_checking")),
                                    IsFromNuwee = reader.GetBoolean(reader.GetOrdinal("is_from_nuwee")),
                                    Institute = reader.GetString(reader.GetOrdinal("institute")),
                                    RegistrationCompleted = reader.GetBoolean(reader.GetOrdinal("registration_completed"))
                                };

                                return user;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
        }
        public async Task SetUsersAtEventsCheckedFieldAsync(long userId, bool value, DateTime date)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    // Перевірка на існування
                    string checkQuery = "SELECT COUNT(*) FROM users_at_events WHERE event_date = @event_date AND userId = @userId LIMIT 1";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        checkCommand.Parameters.AddWithValue("@event_date", date);
                        int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            // Оновлення існуючого запису
                            string updateQuery = "UPDATE users_at_events SET checked = @checked WHERE event_date = @event_date AND userId = @userId LIMIT 1";
                            using (var updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@userId", userId);
                                updateCommand.Parameters.AddWithValue("@event_date", date);
                                updateCommand.Parameters.AddWithValue("@checked", value);

                                int updateResult = await updateCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        public async Task AddUserOneVisitDayAsync(long userId)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    // Перевірка на існування
                    string checkQuery = "SELECT COUNT(*) FROM users_visit_parameters WHERE userId = @userId";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            // Оновлення існуючого запису
                            string updateQuery = "UPDATE users_visit_parameters SET days_visited = days_visited + 1 WHERE userId = @userId";
                            using (var updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@userId", userId);

                                int updateResult = await updateCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
        public async Task SetUserMissedDaysAsycn(long userId, int days)
        {
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "INSERT INTO users_visit_parameters (userId, days_missed) VALUES (@userId, @days_missed)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@days_missed", days);

                        int result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error while: {ex.Message}");
            }
        }
        public async Task<int[]> GetUserVisitParametersAsync(long userId)
        {
            int[] parameters = new int[2];
            try
            {
                using (var dbConnection = new DatabaseConnection(_connectionString))
                {
                    var connection = dbConnection.Connection;

                    string query = "SELECT days_visited, days_missed FROM users_visit_parameters WHERE userId = @userId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            parameters[0] = reader.GetInt32(reader.GetOrdinal("days_visited"));
                            parameters[1] = reader.GetInt32(reader.GetOrdinal("days_missed"));

                            return parameters;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return null;
            }
        }*/
    }
}
