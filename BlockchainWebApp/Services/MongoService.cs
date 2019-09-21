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

        public async Task<List<MongoBlock>> TestGetAllDocuments()
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<MongoBlock>("test");

                var collectionList = await collection.AsQueryable().ToListAsync();

                return collectionList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void TestSaveNewData(Block block)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<Block>("test");

                await collection.InsertOneAsync(block);
            }
            catch (Exception ex)
            {

            }
        }

        public async void TestChangeData(int index, string data)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<Block>("test");

                //var collectionList = await collection.AsQueryable().ToListAsync();
                //var addedBlock = collectionList.Where(b => b.Index == index).FirstOrDefault();

                var filter = Builders<Block>.Filter.Eq(b => b.Index, index);
                var update = Builders<Block>.Update.Set(b => b.Data, data);

                //addedBlock.Data = data;

                var resp = await collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<Block> TestGetBlock(int index)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<MongoBlock>("test");

                var collectionList = await collection.AsQueryable().ToListAsync();
                var addedBlock = collectionList.Where(b => b.Index == index).FirstOrDefault();

                Block block = new Block
                {
                    Index = addedBlock.Index,
                    Data = addedBlock.Data,
                    Hash = addedBlock.Hash,
                    Nonce = addedBlock.Nonce,
                    PreviousHash = addedBlock.PreviousHash,
                    Timestamp = addedBlock.Timestamp
                };

                return block;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void TestSaveMinedBlock(int index, string hash)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<Block>("test");


                var filter = Builders<Block>.Filter.Eq(b => b.Index, index);
                var update = Builders<Block>.Update.Set(b => b.Hash, hash);

                var resp = await collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {

            }
        }

        public async void TestSaveNonce(int index, int nonce)
        {
            try
            {
                var client = new MongoClient(_mongoSettings.ConnectionString);
                var database = client.GetDatabase("test_db");
                var collection = database.GetCollection<Block>("test");


                var filter = Builders<Block>.Filter.Eq(b => b.Index, index);
                var update = Builders<Block>.Update.Set(b => b.Nonce, nonce);

                var resp = await collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
