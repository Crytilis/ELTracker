using MongoDB.Bson.Serialization.Attributes;

namespace ELTracker.Models;

[Collection("Donors")]
public class Donor : ICloneable
{
    [BsonId]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Alias { get; set; }
    public DateTime LastDonatedOn { get; set; }
    public decimal DonationAmount { get; set; }
    public string AssignedTier { get; set; }

    public object Clone()
    {
        return (Donor)MemberwiseClone();
    }
}