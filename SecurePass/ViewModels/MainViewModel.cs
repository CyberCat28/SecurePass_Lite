using password_manager.models;
using password_manager.Services;
using System.Collections.ObjectModel;

namespace password_manager.ViewModels
{
    public class MainViewModel
    {
        private readonly DatabaseService _db;
        private readonly EncryptionService _enc;
        public ObservableCollection<PasswordEntry> Entries { get; set; }

        public MainViewModel(DatabaseService db, EncryptionService enc)
        {
            _db = db;
            _enc = enc;
            LoadEntries();
        }

        public void LoadEntries()
        {
            Entries = _db.GetAll();
        }

        public void DeleteEntry(PasswordEntry entry)
        {
            _db.Delete(entry);
            LoadEntries();
        }

        public string DecryptPassword(PasswordEntry entry)
        {
            return _enc.Decrypt(entry.EncryptedPassword);
        }

        public EncryptionService GetEncryption() => _enc;
        public DatabaseService GetDatabase() => _db;
    }
}
