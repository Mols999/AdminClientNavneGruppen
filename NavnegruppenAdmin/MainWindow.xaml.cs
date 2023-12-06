using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            UpdateAllTicketList();
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
                PasswordBox.Clear();
                SelectedUser = selectedUser;
            }
        }


        private void CloseTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (TicketListView.SelectedItem is Ticket selectedTicket)
            {
                _databaseManager.UpdateTicketStatus(selectedTicket.Id, "Closed");
                UpdateAllTicketList();
            }
            else
            {
                MessageBox.Show("Please select a ticket to close.", "No Ticket Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void UpdateAllTicketList()
        {
            var ticketList = _databaseManager.GetAllTickets();
            TicketListView.ItemsSource = ticketList;
        }


        private void SplitPairButton_Click(object sender, RoutedEventArgs e)
        {
         //need
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
            {
                SelectedUser.Username = UsernameTextBox.Text;
                SelectedUser.PersonalInfo.FirstName = FirstNameTextBox.Text;
                SelectedUser.PersonalInfo.LastName = LastNameTextBox.Text;
                SelectedUser.Email = EmailTextBox.Text;

            
                if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    SelectedUser.Password = PasswordBox.Password;
                }

               
                _databaseManager.UpdateUser(SelectedUser);

                MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                PasswordBox.Clear();
            }
            else
            {
                MessageBox.Show("No user selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }







    }
}