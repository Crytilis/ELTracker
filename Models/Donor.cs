using MongoDB.Bson.Serialization.Attributes;

namespace ELTracker.Models;

[Collection("Donors")]
public class Donor : ICloneable
{
    [BsonId]
    public string Id { get; set; }
    public string Username { get; set; }
    public List<string> DonationAliases { get; set; }
    public decimal DonationAmount { get; set; }
    public string AssignedTier { get; set; }

    public object Clone()
    {
        return (Donor)MemberwiseClone();
    }
}