using BlockchainWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Services
{
    public interface IMongoService
    {
        Task<List<MongoBlock>> GetAllDocuments();
        void SaveNewCarData(Block block);

        Task<List<MongoBlock>> TestGetAllDocuments();
        void TestSaveNewData(Block block);
        void TestChangeData(int index, string data);
        Task<Block> TestGetBlock(int index);
        void TestSaveMinedBlock(int index, string hash);
        void TestSaveNonce(int index, int nonce);
    }
}
