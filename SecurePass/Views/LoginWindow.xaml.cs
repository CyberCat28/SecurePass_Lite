using password_manager.Services;
using System.Windows;

namespace password_manager.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService _db;
        private bool _firstRun;

        public LoginWindow()
        {
            InitializeComponent();
            _db = new DatabaseService();
            _firstRun = !_db.IsMasterPasswordSet();

            if (_firstRun)
            {
                InfoLabel.Content = "Создайте мастер-пароль";
                ConfirmLabel.Visibility = Visibility.Visible;
                ConfirmBox.Visibility = Visibility.Visible;
                LoginButton.Content = "Сохранить";
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string master = MasterBox.Password;

            if (master.Length < 4)
            {
                MessageBox.Show("Слабый мастер пароль (минимум 4 символа)");
                return;
            }

            if (_firstRun)
            {
                string confirm = ConfirmBox.Password;
                if (master != confirm)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                // сохраняем хеш
                string hash = Utils.HashPassword(master);
                _db.SetMasterPassword(hash);
                MessageBox.Show("Мастер-пароль установлен!");
            }
            else
            {
                // проверяем существующий мастер-пароль
                string hash = Utils.HashPassword(master);
                string stored = _db.GetMasterPasswordHash();
                if (hash != stored)
                {
                    MessageBox.Show("Неверный мастер-пароль");
                    return;
                }
            }

            var enc = new EncryptionService(master);
            var main = new MainWindow(_db, enc);
            main.Show();
            Close();
        }
    }
}
