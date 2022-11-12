using MongoDB.Bson.Serialization.Attributes;

namespace ELTracker.Models
{
    [Collection("ExtDonors")]
    public class ExtDonor : ICloneable
    {
        [BsonId]
        public string Id { get; set; }
        public string Username { get; set; }
        public DateTime AddedOn { get; set; }
        public decimal DonationAmount { get; set; }
        public string AssignedTier { get; set; }

        public object Clone()
        {
            return (ExtDonor)MemberwiseClone();
        }
    }
}
