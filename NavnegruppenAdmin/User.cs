using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NavnegruppenAdmin;
using System;
using System.Collections.Generic;

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

    [BsonElement("likedNames")]
    public List<string> LikedNames { get; set; }

    [BsonElement("matches")]
    public List<string> Matches { get; set; }

    [BsonElement("__v")]
    public int Version { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    // Partner's unique identifier - now nullable
    [BsonElement("partner")]
    public ObjectId? PartnerId { get; set; }
}
