using password_manager.models;
using password_manager.Services;

namespace password_manager.ViewModels
{
    public class AddEditViewModel
    {
        private readonly DatabaseService _db;
        private readonly EncryptionService _enc;
        public PasswordEntry Entry { get; private set; }

        public AddEditViewModel(DatabaseService db, EncryptionService enc, PasswordEntry entry = null)
        {
            _db = db;
            _enc = enc;
            Entry = entry ?? new PasswordEntry();
        }

        public void Save(string password)
        {
            Entry.EncryptedPassword = _enc.Encrypt(password);
            if (Entry.Id == 0) _db.Add(Entry);
            else _db.Update(Entry);
        }
    }
}
