using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace NavnegruppenAdmin
{
    // Class managing database
    public class DatabaseManager
    {
        // MongoDB collections
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<AdminUser> _adminUserCollection;
        private readonly IMongoCollection<Ticket> _ticketCollection;

        // Properties for access to the AdminUser and Ticket collections.
        public IMongoCollection<AdminUser> AdminUserCollection => _adminUserCollection;
        public IMongoCollection<Ticket> TicketCollection => _ticketCollection;

        // Constructor initializing MongoDB client and collections.
        public DatabaseManager(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _userCollection = database.GetCollection<User>("User");
            _adminUserCollection = database.GetCollection<AdminUser>("AdminUser");
            _ticketCollection = database.GetCollection<Ticket>("Ticket");
        }

        // Retrieves a user by their username.
        public User GetUserByUsername(string username)
        {
            return _userCollection.Find(u => u.Username == username).FirstOrDefault();
        }

        // Validates admin user credentials.
        public bool ValidateAdminUser(string username, string password)
        {
            var adminUser = _adminUserCollection.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return adminUser != null;
        }

        // Gets all tickets from the database.
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

        // Updates the status of a specified ticket.
        public void UpdateTicketStatus(string ticketId, string newStatus)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticketId);
            var update = Builders<Ticket>.Update.Set(t => t.Status, newStatus);
            _ticketCollection.UpdateOne(filter, update);
        }

        // Updates a user's information in the database.
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

                try
                {
                    var result = _userCollection.UpdateOne(filter, update);
                    if (result.ModifiedCount == 0)
                    {
                        Console.WriteLine("No documents were updated.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                }
            }
        }

        // Retrieves the top liked names across all users.
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

        // Unsets the partner ID for a specific user.
        public void UnsetUserPartner(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Unset(u => u.PartnerId);
            try
            {
                _userCollection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unsetting user partner: {ex.Message}");
            }
        }

        // Inserts a new user into the collection.
        public void InsertUser(User user)
        {
            _userCollection.InsertOne(user);
        }

        // Deletes a user from the collection.
        public void DeleteUser(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            _userCollection.DeleteOne(filter);
        }

        // Retrieves all users from the collection.
        public List<User> GetAllUsers()
        {
            return _userCollection.Find(_ => true).ToList();
        }

        // Searches for users matching a specific keyword.
        public List<User> SearchUsers(string keyword)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Username, new BsonRegularExpression(keyword, "i")),
                Builders<User>.Filter.Regex(u => u.PersonalInfo.FirstName, new BsonRegularExpression(keyword, "i")),
                Builders<User>.Filter.Regex(u => u.PersonalInfo.LastName, new BsonRegularExpression(keyword, "i")),
                Builders<User>.Filter.Regex(u => u.Email, new BsonRegularExpression(keyword, "i")));
            return _userCollection.Find(filter).ToList();
        }
    }
}
