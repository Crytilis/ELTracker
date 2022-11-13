using ELTracker.Models;

namespace ELTracker.Managers;

internal interface IDonationManager
{
    Task<Total> GetTotals();
    Task UpdateTopDonor(string donor);
    Task UpdateTotalDonations(decimal amount);
    Task UpdateTotalDonors(int amount);
    Task<Donation> GetDonationByName(string name);
    Task<OperationResult> InsertDonation(Donation donation);
    Task<Donor> GetDonorById(string id);
    Task<Donor> GetDonorById(ulong id);
    Task<Donor> GetDonorByName(string name);
    Task<OperationResult> InsertDonor(Donor donor);
    Task<OperationResult> InsertDonor(ExtDonor donor);
    Task<List<Donor>> GetAllDonors();
    Task<ExtDonor> GetExtDonorById(string id);
    Task<ExtDonor> GetExtDonorById(ulong id);
    Task<ExtDonor> GetExtDonorByName(string name);
    Task<List<ExtDonor>> GetAllExtDonors();
}