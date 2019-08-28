using BlockchainWebApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Services
{
    public class MongoService : IMongoService
    {
        private readonly MongoSettings _mongoSettings;

        public MongoService(IOptions<MongoSettings> options)
        {
            _mongoSettings = options.Value;
        }

        public async Task<List<MongoBlock>> GetAllDocuments()
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("cars_db");
                var collection = database.GetCollection<MongoBlock>("cars");

                var collectionList = await collection.AsQueryable().ToListAsync();

                return collectionList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void SaveNewCarData(Block block)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("cars_db");
                var collection = database.GetCollection<Block>("cars");

                await collection.InsertOneAsync(block);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
