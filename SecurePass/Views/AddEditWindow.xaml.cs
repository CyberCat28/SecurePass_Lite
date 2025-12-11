using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using password_manager.models;
using password_manager.Services;
using password_manager.ViewModels;

namespace password_manager.Views
{
    public partial class AddEditWindow : Window
    {
        private readonly AddEditViewModel _vm;
        private readonly MainViewModel _main;
        private readonly PasswordGenerator _pwdGen;

        public AddEditWindow(MainViewModel mainVm, PasswordEntry entry)
        {
            InitializeComponent();
            _main = mainVm;
            _pwdGen = new PasswordGenerator();

            _vm = new AddEditViewModel(mainVm.GetDatabase(), mainVm.GetEncryption(), entry);

            if (entry != null)
            {
                TitleBox.Text = entry.Title;
                UserBox.Text = entry.Username;
                CategoryBox.Text = entry.Category;
                string plain = mainVm.DecryptPassword(entry);
                PassBox.Password = plain;
                PassTextBox.Text = plain;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string password = PassBox.Visibility == Visibility.Visible ? PassBox.Password : PassTextBox.Text;
            _vm.Entry.Title = TitleBox.Text;
            _vm.Entry.Username = UserBox.Text;
            _vm.Entry.Category = CategoryBox.Text;
            _vm.Save(password);
            _main.LoadEntries();
            Close();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            int length = (int)LengthSlider.Value;
            bool upper = UpperCheck.IsChecked ?? true;
            bool lower = LowerCheck.IsChecked ?? true;
            bool digits = DigitsCheck.IsChecked ?? true;

            string password = _pwdGen.Generate(length, upper, lower, digits);

            if (string.IsNullOrEmpty(password))
            {
                password = GeneratePassword(length);
            }

            PassBox.Password = password;
            PassTextBox.Text = password;
        }

        private string GeneratePassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>?";
            var result = new StringBuilder(length);
            byte[] buffer = new byte[length];
            RandomNumberGenerator.Fill(buffer);
            for (int i = 0; i < length; i++)
                result.Append(chars[buffer[i] % chars.Length]);
            return result.ToString();
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (PassBox.Visibility == Visibility.Visible)
            {
                PassTextBox.Text = PassBox.Password;
                PassBox.Visibility = Visibility.Collapsed;
                PassTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                PassBox.Password = PassTextBox.Text;
                PassBox.Visibility = Visibility.Visible;
                PassTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            string password = PassBox.Visibility == Visibility.Visible ? PassBox.Password : PassTextBox.Text;
            if (!string.IsNullOrEmpty(password))
                Clipboard.SetText(password);
        }
    }
}