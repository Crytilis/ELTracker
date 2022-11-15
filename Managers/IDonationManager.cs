using ELTracker.Models;

namespace ELTracker.Managers;

internal interface IDonationManager
{
    Task<Total> GetTotals();
    Task UpdateTopDonor(string donor);
    Task UpdateTotalDonations(decimal amount);
    Task UpdateTotalDonors(int amount);
    Task<Donation> GetDonationByName(string name);
    Task<bool> CheckDonationExists(string name);
    Task<OperationResult> InsertDonation(Donation donation);
    Task<List<Donation>> GetAllDonations();
    Task<OperationResult> DonationSentToDonors(Donation donation, bool sentToDonors);
    Task<bool> CheckDonorExists(string name);
    Task<Donor> GetDonorById(string id);
    Task<Donor> GetDonorById(ulong id);
    Task<Donor> GetDonorByName(string name);
    Task<OperationResult> InsertDonor(Donor donor);
    Task<OperationResult> ReplaceDonorRecord(Donor oldRecord, Donor newRecord);
    Task<OperationResult> InsertDonor(ExtDonor donor);
    Task<List<Donor>> GetAllDonors();
    Task<bool> CheckExtDonorExists(string name);
    Task<ExtDonor> GetExtDonorById(string id);
    Task<ExtDonor> GetExtDonorById(ulong id);
    Task<ExtDonor> GetExtDonorByName(string name);
    Task<List<ExtDonor>> GetAllExtDonors();
}