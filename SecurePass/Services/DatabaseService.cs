using Microsoft.Data.Sqlite;
using password_manager.models;
using System.Collections.ObjectModel;

namespace password_manager.Services
{
    public class DatabaseService
    {
        private readonly string _connStr;

        public DatabaseService(string dbPath = "passwords.db")
        {
            _connStr = $"Data Source={dbPath}";
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();

            // Таблица для записей
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PasswordEntries (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Username TEXT NOT NULL,
                    EncryptedPassword TEXT NOT NULL,
                    Category TEXT,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            cmd.ExecuteNonQuery();

            // Таблица для мастер-пароля
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS MasterPassword (
                    Id INTEGER PRIMARY KEY,
                    PasswordHash TEXT NOT NULL
                );";
            cmd.ExecuteNonQuery();
        }

        // --- Методы для PasswordEntries ---
        public ObservableCollection<PasswordEntry> GetAll()
        {
            var list = new ObservableCollection<PasswordEntry>();
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM PasswordEntries ORDER BY CreatedDate DESC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PasswordEntry
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Username = reader.GetString(2),
                    EncryptedPassword = reader.GetString(3),
                    Category = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    CreatedDate = reader.GetDateTime(5)
                });
            }
            return list;
        }

        public void Add(PasswordEntry entry)
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO PasswordEntries (Title, Username, EncryptedPassword, Category) 
                                VALUES (@t,@u,@p,@c)";
            cmd.Parameters.AddWithValue("@t", entry.Title);
            cmd.Parameters.AddWithValue("@u", entry.Username);
            cmd.Parameters.AddWithValue("@p", entry.EncryptedPassword);
            cmd.Parameters.AddWithValue("@c", entry.Category ?? "");
            cmd.ExecuteNonQuery();
        }

        public void Update(PasswordEntry entry)
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE PasswordEntries SET Title=@t, Username=@u, EncryptedPassword=@p, Category=@c
                                WHERE Id=@id";
            cmd.Parameters.AddWithValue("@t", entry.Title);
            cmd.Parameters.AddWithValue("@u", entry.Username);
            cmd.Parameters.AddWithValue("@p", entry.EncryptedPassword);
            cmd.Parameters.AddWithValue("@c", entry.Category ?? "");
            cmd.Parameters.AddWithValue("@id", entry.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(PasswordEntry entry)
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM PasswordEntries WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", entry.Id);
            cmd.ExecuteNonQuery();
        }

        // --- Методы для MasterPassword ---
        public bool IsMasterPasswordSet()
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM MasterPassword";
            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        public void SetMasterPassword(string hash)
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO MasterPassword (Id, PasswordHash) VALUES (1, @hash)";
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.ExecuteNonQuery();
        }

        public string GetMasterPasswordHash()
        {
            using var conn = new SqliteConnection(_connStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PasswordHash FROM MasterPassword WHERE Id=1";
            return (string)cmd.ExecuteScalar();
        }
    }
}
