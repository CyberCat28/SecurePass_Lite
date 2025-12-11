using password_manager.models;
using password_manager.Services;
using password_manager.ViewModels;
using System.Linq;
using System.Windows;

namespace password_manager.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        private bool _showPasswords = false;

        public MainWindow(DatabaseService db, EncryptionService enc)
        {
            InitializeComponent();
            _vm = new MainViewModel(db, enc);
            ListEntries.ItemsSource = _vm.Entries;
            UpdatePasswords();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEditWindow(_vm, null);
            win.ShowDialog();
            RefreshList();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ListEntries.SelectedItem is PasswordEntry entry)
            {
                var win = new AddEditWindow(_vm, entry);
                win.ShowDialog();
                RefreshList();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ListEntries.SelectedItem is PasswordEntry entry)
            {
                _vm.DeleteEntry(entry);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            _vm.LoadEntries();
            ApplyFilter();
            UpdatePasswords();
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _vm.Entries.Where(en =>
                (string.IsNullOrEmpty(SearchTitleBox.Text) || en.Title.ToLower().Contains(SearchTitleBox.Text.ToLower())) &&
                (string.IsNullOrEmpty(SearchCategoryBox.Text) || (en.Category != null && en.Category.ToLower().Contains(SearchCategoryBox.Text.ToLower())))
            ).ToList();

            ListEntries.ItemsSource = filtered;
            UpdatePasswords();
        }

        private void TogglePasswords_Click(object sender, RoutedEventArgs e)
        {
            _showPasswords = !_showPasswords;
            UpdatePasswords();
        }

        private void UpdatePasswords()
        {
            foreach (PasswordEntry entry in ListEntries.Items)
            {
                entry.DisplayPassword = _showPasswords ? _vm.DecryptPassword(entry) :
                    new string('*', entry.Username.Length > 0 ? entry.Username.Length : 8);
            }
            ListEntries.Items.Refresh();
        }
    }
}
