using MongoDB.Driver;
using System.Reflection;
using ELTracker.Models;
using ELTracker.Models.SubModels;

namespace ELTracker.Managers;

internal class ServerManager : IServerManager
{
    private readonly IMongoCollection<Server> _servers;

    public ServerManager(IMongoDatabase database)
    {
        _servers = database.GetCollection<Server>(GetCollectionName(typeof(Server)));
    }

    private static string GetCollectionName(ICustomAttributeProvider collectionType)
    {
        return ((CollectionAttribute)collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault())?.CollectionName;
    }

    public async Task<Server> GetById(ulong id)
    {
        var filter = Builders<Server>.Filter.Eq("_id", id.ToString());
        return await _servers.FindAsync<Server>(filter).Result.FirstOrDefaultAsync();
    }

    public async Task<Server> GetByName(string name)
    {
        var filter = Builders<Server>.Filter.Eq("ServerName", name);
        return await _servers.FindAsync<Server>(filter).Result.FirstOrDefaultAsync();
    }

    public async Task<OperationResult> Insert(Server server)
    {
        var result = new OperationResult();
        try
        {
            await _servers.InsertOneAsync(server);
        }
        catch (MongoWriteException e)
        {
            result.Succeeded = false;
            result.ErrorMessage = e.Message;
            return result;
        }

        result.Succeeded = true;
        return result;
    }

    public async Task UpdateBoard(Server guild, HonorBoard board)
    {
        var filter = Builders<Server>.Filter.Eq("_id", guild.Id);
        var updateDef = Builders<Server>.Update.Set(x => x.HonorBoard, board);
        await _servers.UpdateOneAsync(filter, updateDef, new UpdateOptions { IsUpsert = true });
    }

    public async Task UpdateDonationTier(Server guild, int tier, DonationTierDetails tierDetails)
    {
        var filter = Builders<Server>.Filter.Eq("_id", guild.Id);
        var updateDef = Builders<Server>.Update.Set(x => x.DonationTiers[tier], tierDetails);
        await _servers.UpdateOneAsync(filter, updateDef, new UpdateOptions { IsUpsert = true });
    }

    public async Task UpdateEmbedId(Server guild, ulong embedId)
    {
        var filter = Builders<Server>.Filter.Eq("_id", guild.Id);
        var updateDef = Builders<Server>.Update.Set(x => x.HonorBoard.Embed, embedId);
        await _servers.UpdateOneAsync(filter, updateDef, new UpdateOptions { IsUpsert = true });
    }

    public async Task UpdateTiers(Server guild, Dictionary<int, DonationTierDetails> donationTiers)
    {
        var filter = Builders<Server>.Filter.Eq("_id", guild.Id);
        var updateDef = Builders<Server>.Update.Set(x => x.DonationTiers, donationTiers);
        await _servers.UpdateOneAsync(filter, updateDef, new UpdateOptions { IsUpsert = true });
    }
}