using BlockchainWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Services
{
    public interface IBlockchainService
    {
        List<Block> GetBlockchain();
        bool AddBlock(string data);

        Task<List<Block>> GetBlockchain2();
        Task<bool> AddBlock2(string data);
    }
}
