using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ELTracker.Models;

[Collection("Donations")]
[BsonIgnoreExtraElements]
internal class Donation
{
    [BsonId]
    [JsonProperty("donorID")]
    public string DonorId { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }
        
    [JsonProperty("modifiedDateUTC")]
    public string CreatedDateUtc { get; set; }

    [JsonProperty("sumDonations")]
    public decimal Amount { get; set; }
}