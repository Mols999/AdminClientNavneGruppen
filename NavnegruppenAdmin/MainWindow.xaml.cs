using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MongoDB.Driver;

namespace NavnegruppenAdmin
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseManager _databaseManager;
        private User SelectedUser { get; set; }

        public MainWindow(DatabaseManager databaseManager)
        {
            InitializeComponent();
            _databaseManager = databaseManager;
            UpdateUserList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string username = SearchTextBox.Text.Trim();
            SelectedUser = _databaseManager.GetUserByUsername(username);
            UserInfoPanel.DataContext = SelectedUser;
        }

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
                UpdateUserList();
            }
        }

        private void UserListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserListView.SelectedItem is User selectedUser)
            {
                UserInfoPanel.DataContext = selectedUser;
                UsernameTextBox.Text = selectedUser.Username;
                FirstNameTextBox.Text = selectedUser.PersonalInfo.FirstName;
                LastNameTextBox.Text = selectedUser.PersonalInfo.LastName;
                EmailTextBox.Text = selectedUser.Email;
                SelectedUser = selectedUser;
            }
        }

        private void SplitPairButton_Click(object sender, RoutedEventArgs e)
        {
         //need
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            _databaseManager.UpdateUser(SelectedUser);
        }
    }
}