using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace ELTracker.Models;

[Collection("Donations")]
internal class Donation
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [JsonIgnore]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }
        
    [JsonProperty("modifiedDateUTC")]
    public string CreatedDateUtc { get; set; }

    [JsonProperty("sumDonations")]
    public decimal Amount { get; set; }

    [JsonIgnore]
    public bool SentToDonors { get; set; }
}