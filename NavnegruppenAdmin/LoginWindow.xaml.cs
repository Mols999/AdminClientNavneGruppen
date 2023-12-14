using MongoDB.Driver;
using System;
using System.Windows;
using System.Windows.Input;

namespace NavnegruppenAdmin
{
    //LoginWindow which handles the login functionality.
    public partial class LoginWindow : Window
    {
        // DatabaseManager instance 
        private readonly DatabaseManager _databaseManager;
        
        // Collection for AdminUser
        private readonly IMongoCollection<AdminUser> _adminUserCollection;

        // Constructor for the LoginWindow and Initialize
        public LoginWindow()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager("mongodb+srv://Mols:ID4EY0Cqr80zSnH2@cluster0.euyeftl.mongodb.net", "NewbornNamesCloudDB");
            // Retrieve the admin user collection
            _adminUserCollection = _databaseManager.AdminUserCollection;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the username and password entered by the user.
            string username = UsernameTextBox.Text;
            string enteredPassword = PasswordBox.Password;

            // Query the admin user collection to find a user matching the entered credentials.
            var adminUser = _adminUserCollection.Find(u => u.Username == username && u.Password == enteredPassword).FirstOrDefault();

            // Check if a matching admin user was found.
            if (adminUser != null)
            {
                // If a user is found, open the main window and hide the login window.
                MainWindow mainWindow = new MainWindow(_databaseManager);
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                this.Hide();
            }
            else
            {
                // If no matching user is found, display an error message.
                ErrorMessage.Text = "Invalid username or password.";
            }
        }
    }
}
