using MongoDB.Bson.Serialization.Attributes;

namespace NavnegruppenAdmin
{
    public class PersonalInfo
    {
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("partner")]
        public string Partner { get; set; }
    }
}
