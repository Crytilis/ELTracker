using ELTracker.Models;
using ELTracker.Models.SubModels;

namespace ELTracker.Managers;

internal interface IServerManager
{
    Task<Server> GetById(ulong id);
    Task<Server> GetByName(string name);
    Task<OperationResult> Insert(Server server);
    Task UpdateBoard(Server guild, HonorBoard board);
    Task UpdateDonationTier(Server guild, int tier, DonationTierDetails tierDetails);
    Task UpdateEmbedId(Server guild, ulong embedId);
    Task UpdateTiers(Server guild, Dictionary<int, DonationTierDetails> donationTiers);
}