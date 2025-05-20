using System;
using MySql.Data.MySqlClient;

namespace BoardMarking
{
    public class DatabaseConnection : IDisposable
    {
        private readonly MySqlConnection _connection;
        private bool _disposed = false;

        public DatabaseConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        public MySqlConnection Connection => _connection;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
