using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace NavnegruppenAdmin
{
    public class DatabaseManager
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<AdminUser> _adminUserCollection;
        private readonly IMongoCollection<Ticket> _ticketCollection; 
        public IMongoCollection<AdminUser> AdminUserCollection => _adminUserCollection;
        public IMongoCollection<Ticket> TicketCollection => _ticketCollection; 

        public DatabaseManager(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _userCollection = database.GetCollection<User>("Users");
            _adminUserCollection = database.GetCollection<AdminUser>("AdminUser");
            _ticketCollection = database.GetCollection<Ticket>("Ticket"); 
        }

        public User GetUserByUsername(string username)
        {
            return _userCollection.Find(u => u.Username == username).FirstOrDefault();
        }

        public bool ValidateAdminUser(string username, string password)
        {
            var adminUser = _adminUserCollection.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return adminUser != null;
        }


        public List<Ticket> GetAllTickets()
        {
            try
            {
                return _ticketCollection.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching tickets: {ex.Message}");
                return new List<Ticket>();
            }
        }


        public void UpdateTicketStatus(string ticketId, string newStatus)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticketId);
            var update = Builders<Ticket>.Update.Set(t => t.Status, newStatus);
            _ticketCollection.UpdateOne(filter, update);
        }



        public void UpdateUser(User user)
        {
            if (user != null)
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<User>.Update
                    .Set(u => u.PersonalInfo.FirstName, user.PersonalInfo.FirstName)
                    .Set(u => u.PersonalInfo.LastName, user.PersonalInfo.LastName)
                    .Set(u => u.Email, user.Email)
                    .Set(u => u.Username, user.Username)
                    .Set(u => u.Password, user.Password); 

                _userCollection.UpdateOne(filter, update);
            }
        }



        public void InsertUser(User user)
        {
            _userCollection.InsertOne(user);
        }

        public void DeleteUser(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            _userCollection.DeleteOne(filter);
        }

        public List<User> GetAllUsers()
        {
            return _userCollection.Find(_ => true).ToList();
        }

        public List<User> SearchUsers(string keyword)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Username, new BsonRegularExpression(keyword, "i")), // Case-insensitive username search
                Builders<User>.Filter.Regex(u => u.PersonalInfo.FirstName, new BsonRegularExpression(keyword, "i")), // Case-insensitive first name search
                Builders<User>.Filter.Regex(u => u.PersonalInfo.LastName, new BsonRegularExpression(keyword, "i")), // Case-insensitive last name search
                Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression(keyword, "i"))); // Case-insensitive email search

            return _userCollection.Find(filter).ToList();
        }
    }
}
