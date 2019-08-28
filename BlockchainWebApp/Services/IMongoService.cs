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
    }
}
