using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace NavnegruppenAdmin
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("personalInfo")]
        public PersonalInfo PersonalInfo { get; set; }

        [BsonElement("matches")]
        public List<string> Matches { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }
    }
}
