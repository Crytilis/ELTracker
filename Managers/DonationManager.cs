using System.Reflection;
using ELTracker.Models;
using MongoDB.Driver;

namespace ELTracker.Managers
{
    internal class DonationManager
    {
        private readonly IMongoCollection<Donation> _donations;
        private readonly IMongoCollection<Donor> _donors;
        private readonly IMongoCollection<ExtDonor> _extDonors;
        private readonly IMongoCollection<Total> _totals;

        public DonationManager(IMongoDatabase database)
        {
            _donations = database.GetCollection<Donation>(GetCollectionName(typeof(Donation)));
            _donors = database.GetCollection<Donor>(GetCollectionName(typeof(Donor)));
            _extDonors = database.GetCollection<ExtDonor>(GetCollectionName(typeof(ExtDonor)));
            _totals = database.GetCollection<Total>(GetCollectionName(typeof(Total)));
        }

        private static string GetCollectionName(ICustomAttributeProvider collectionType)
        {
            return ((CollectionAttribute)collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault())?.CollectionName;
        }

        // Totals Crud Tasks
        public async Task<Total> GetTotals()
        {
            var filter = Builders<Total>.Filter.Eq("_id", $"EL{DateTime.Now.Year}Totals");
            return await _totals.Find(filter).FirstOrDefaultAsync();
        }
        public async Task UpdateTopDonor(string donor)
        {
            var filter = Builders<Total>.Filter.Eq("_id", $"EL{DateTime.Now.Year}Totals");
            var updateDef = Builders<Total>.Update.Set(u => u.TopDonor, donor);
            await _totals.UpdateOneAsync(filter, updateDef, new UpdateOptions{IsUpsert = true});
        }
        public async Task UpdateTotalDonations(decimal amount)
        {
            var filter = Builders<Total>.Filter.Eq("_id", $"EL{DateTime.Now.Year}Totals");
            var updateDef = Builders<Total>.Update.Set(u => u.Amount, amount);
            await _totals.UpdateOneAsync(filter, updateDef, new UpdateOptions{IsUpsert = true});
        }
        public async Task UpdateTotalDonors(int amount)
        {
            var filter = Builders<Total>.Filter.Eq("_id", $"EL{DateTime.Now.Year}Totals");
            var updateDef = Builders<Total>.Update.Set(u => u.Donors, amount);
            await _totals.UpdateOneAsync(filter, updateDef, new UpdateOptions { IsUpsert = true });
        }


        // Donations Crud Tasks
        public async Task<Donation> GetDonationByName(string name)
        {
            var filter = Builders<Donation>.Filter.Eq("DisplayName", name);
            return await _donations.FindAsync<Donation>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<OperationResult> InsertDonation(Donation donation)
        {
            var result = new OperationResult();
            try
            {
                await _donations.InsertOneAsync(donation);
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


        // Donors Crud Tasks
        private async Task<Donor> GetDonorById(string id)
        {
            var filter = Builders<Donor>.Filter.Eq("_id", id);
            return await _donors.FindAsync<Donor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<Donor> GetDonorById(ulong id)
        {
            var filter = Builders<Donor>.Filter.Eq("_id", Convert.ToString(id));
            return await _donors.FindAsync<Donor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<Donor> GetDonorByName(string name)
        {
            var filter = Builders<Donor>.Filter.Eq("Username", name);
            return await _donors.FindAsync<Donor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<OperationResult> InsertDonor(Donor donor)
        {
            var result = new OperationResult();
            try
            {
                await _donors.InsertOneAsync(donor);
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
        public async Task<List<Donor>> GetAllDonors()
        {
            var filter = Builders<Donor>.Filter.Empty;
            return await _donors.Find(filter).ToListAsync();
        }


        // ExtDonors Crud Tasks (Manually added donors)
        private async Task<ExtDonor> GetExtDonorById(string id)
        {
            var filter = Builders<ExtDonor>.Filter.Eq("_id", id);
            return await _extDonors.FindAsync<ExtDonor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<ExtDonor> GetExtDonorById(ulong id)
        {
            var filter = Builders<ExtDonor>.Filter.Eq("_id", Convert.ToString(id));
            return await _extDonors.FindAsync<ExtDonor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<ExtDonor> GetExtDonorByName(string name)
        {
            var filter = Builders<ExtDonor>.Filter.Eq("Username", name);
            return await _extDonors.FindAsync<ExtDonor>(filter).Result.FirstOrDefaultAsync();
        }
        public async Task<OperationResult> InsertDonor(ExtDonor donor)
        {
            var result = new OperationResult();
            try
            {
                await _extDonors.InsertOneAsync(donor);
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
        public async Task<List<ExtDonor>> GetAllExtDonors()
        {
            var filter = Builders<ExtDonor>.Filter.Empty;
            return await _extDonors.Find(filter).ToListAsync();
        }
    }
}
