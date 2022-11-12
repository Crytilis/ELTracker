using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ELTracker.Models;
using MongoDB.Driver;

namespace ELTracker.Managers
{
    internal class DonationManager
    {
        private readonly IMongoCollection<Donation> _donations;
        private readonly IMongoCollection<Donor> _donors;
        private readonly IMongoCollection<ExtDonor> _extdonors;
        private readonly IMongoCollection<Total> _totals;

        public DonationManager(IMongoDatabase database)
        {
            _donations = database.GetCollection<Donation>(GetCollectionName(typeof(Donation)));
            _donors = database.GetCollection<Donor>(GetCollectionName(typeof(Donor)));
            _extdonors = database.GetCollection<ExtDonor>(GetCollectionName(typeof(ExtDonor)));
            _totals = database.GetCollection<Total>(GetCollectionName(typeof(Total)));
        }

        private static string GetCollectionName(ICustomAttributeProvider collectionType)
        {
            return ((CollectionAttribute)collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault())?.CollectionName;
        }

        
    }
}
