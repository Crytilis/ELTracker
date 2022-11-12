using ELTracker.Models.SubModels;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ELTracker.Models;

[Collection("Servers")]
public class Server
{
    [BsonId]
    public string Id { get; set; }

    public string GuildName { get; set; }

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<int, DonationTierDetails> DonationTiers { get; set; }

    public HonorBoard HonorBoard { get; set; }

}