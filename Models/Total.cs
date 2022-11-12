using MongoDB.Bson.Serialization.Attributes;

namespace ELTracker.Models;

[Collection("Totals")]
public class Total
{
    [BsonId]
    public string Id { get; set; }
    public int Donors { get; set; }
    public decimal Amount { get; set; }
    public string TopDonor { get; set; }
}