using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MongoDB.Driver;
using static NavnegruppenAdmin.MainWindow;

namespace NavnegruppenAdmin
{
    public partial class MainWindow : Window
    {
        // DatabaseManager instance for interacting with the MongoDB database.
        private readonly DatabaseManager _databaseManager;

        // Currently selected user in the application.
        private User SelectedUser { get; set; }

        // Constructor for the MainWindow.
        public MainWindow(DatabaseManager databaseManager)
        {
            InitializeComponent();
            _databaseManager = databaseManager; 
            // Load initial data for display.
            LoadTopLikedNames();
            UpdateUserList();
            UpdateAllTicketList();
        }

        // Loads and displays the top liked names.
        private void LoadTopLikedNames()
        {
            var topLikedNames = _databaseManager.GetTopLikedNames();
            TopLikedNamesListView.ItemsSource = topLikedNames;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string username = SearchTextBox.Text.Trim();
            SelectedUser = _databaseManager.GetUserByUsername(username);
            UserInfoPanel.DataContext = SelectedUser; // Update UI with selected user's data.
        }

        // Updates and displays the user list.
        private void UpdateUserList()
        {
            var userList = _databaseManager.GetAllUsers();
            UserListView.ItemsSource = userList;
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                _databaseManager.DeleteUser(selectedUser.Id);
                UpdateUserList(); // Refresh user list after deletion.
            }
        }

        // Event handler for user list double-click.
        private void UserListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                UserInfoPanel.DataContext = selectedUser; // Update UI with selected user's data.
                // Populate the form with user data.
                UsernameTextBox.Text = selectedUser.Username;
                FirstNameTextBox.Text = selectedUser.PersonalInfo.FirstName;
                LastNameTextBox.Text = selectedUser.PersonalInfo.LastName;
                EmailTextBox.Text = selectedUser.Email;
                PasswordBox.Clear();
                SelectedUser = selectedUser;
            }
        }

        private void CloseTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (TicketListView.SelectedItem is Ticket selectedTicket)
            {
                _databaseManager.UpdateTicketStatus(selectedTicket.Id, "Closed");
                UpdateAllTicketList(); // Refresh ticket list after status update.
            }
            else
            {
                MessageBox.Show("Please select a ticket to close.", "No Ticket Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Updates and displays all tickets in the ListView.
        private void UpdateAllTicketList()
        {
            var ticketList = _databaseManager.GetAllTickets();
            TicketListView.ItemsSource = ticketList;
        }

   
        private void SplitPairButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null && SelectedUser.PartnerId != null)
            {
                _databaseManager.UnsetUserPartner(SelectedUser.Id);
                MessageBox.Show("Partner unset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedUser = _databaseManager.GetUserByUsername(SelectedUser.Username); // Update the selected user.
                UserInfoPanel.DataContext = SelectedUser; // Update UI.
            }
            else
            {
                MessageBox.Show("No user selected or user has no partner.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

 
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
            {
                // Update selected user's data with form values.
                SelectedUser.Username = UsernameTextBox.Text;
                SelectedUser.PersonalInfo.FirstName = FirstNameTextBox.Text;
                SelectedUser.PersonalInfo.LastName = LastNameTextBox.Text;
                SelectedUser.Email = EmailTextBox.Text;
                if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    SelectedUser.Password = PasswordBox.Password;
                }
                _databaseManager.UpdateUser(SelectedUser); // Send update to database.
                MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                PasswordBox.Clear(); // Clear the password box.
            }
            else
            {
                MessageBox.Show("No user selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

  
        public class UserService
        {
            private readonly IMongoCollection<User> _userCollection;

            // Constructor to initialize the MongoDB collection.
            public UserService(string connectionString, string databaseName)
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _userCollection = database.GetCollection<User>("User");
            }

            // Retrieves the top liked names from all users.
            public List<KeyValuePair<string, int>> GetTopLikedNames()
            {
                var nameCounts = new Dictionary<string, int>();
                var users = _userCollection.Find(_ => true).ToList();
                foreach (var user in users)
                {
                    foreach (var name in user.LikedNames)
                    {
                        if (nameCounts.ContainsKey(name))
                        {
                            nameCounts[name]++;
                        }
                        else
                        {
                            nameCounts[name] = 1;
                        }
                    }
                }
                var topNames = nameCounts.OrderByDescending(x => x.Value).Take(10).ToList();
                return topNames;
            }
        }
    }
}
