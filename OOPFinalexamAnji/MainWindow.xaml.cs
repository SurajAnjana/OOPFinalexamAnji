using System;
using System.Threading.Tasks;
using System.Windows;
using PasswordResetApp.Classes;

namespace PasswordResetApp
{
    public partial class MainWindow : Window
    {
        private const string FilePath = "password.txt";
        private PasswordManager passwordManager;
        private BruteForceAttack bruteForceAttack;

        public MainWindow()
        {
            InitializeComponent();
            passwordManager = new PasswordManager();
            ThreadSlider.ValueChanged += ThreadSlider_ValueChanged;
        }

        private void ThreadSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ThreadCountTextBlock != null)
            {
                ThreadCountTextBlock.Text = ThreadSlider.Value.ToString();
            }
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordTextBox.Text;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password.");
                return;
            }

            passwordManager.CreateAndStorePassword(password, FilePath);
            string encryptedPassword = System.IO.File.ReadAllText(FilePath);
            EncryptedPasswordTextBlock.Text = encryptedPassword;
            MessageBox.Show("Password saved successfully.");
        }

        private async void StartAttack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string encryptedPassword = System.IO.File.ReadAllText(FilePath);
                int maxThreads = (int)ThreadSlider.Value;
                bruteForceAttack = new BruteForceAttack(encryptedPassword, maxThreads);

                var result = await bruteForceAttack.StartAttack();
                ResultTextBlock.Text = $"Password: {result.password}\nDuration: {result.duration}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
