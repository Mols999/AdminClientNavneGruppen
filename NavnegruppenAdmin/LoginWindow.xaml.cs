using MongoDB.Driver;
using System;
using System.Windows;
using System.Windows.Input;

namespace NavnegruppenAdmin
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IMongoCollection<AdminUser> _adminUserCollection;

        public LoginWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager("mongodb://localhost:27017", "NewbornNamesDB");
            _adminUserCollection = _databaseManager.AdminUserCollection;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string enteredPassword = PasswordBox.Password;

           
            var adminUser = _adminUserCollection.Find(u => u.Username == username && u.Password == enteredPassword).FirstOrDefault();

            if (adminUser != null)
            {
                MainWindow mainWindow = new MainWindow(_databaseManager);
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                ErrorMessage.Text = "Invalid username or password.";
            }
        }
    }
}
