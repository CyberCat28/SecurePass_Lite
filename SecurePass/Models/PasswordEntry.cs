using System;

namespace password_manager.models
{
    public class PasswordEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }

        // Для отображения пароля в ListView
        public string DisplayPassword { get; set; }
    }
}
