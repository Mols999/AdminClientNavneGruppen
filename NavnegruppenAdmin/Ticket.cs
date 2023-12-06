using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("sender")]
    public string Sender { get; set; }

    [BsonElement("message")]
    public string Message { get; set; }

    [BsonElement("personalInfo")]
    public PersonalInfo PersonalInfo { get; set; }

    [BsonElement("timestamp")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime Timestamp { get; set; }

    [BsonElement("status")]
    public string Status { get; set; }
}

public class PersonalInfo
{
    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }
}
