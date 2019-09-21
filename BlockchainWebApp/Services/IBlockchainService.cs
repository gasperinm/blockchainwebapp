using BlockchainWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Services
{
    public interface IBlockchainService
    {
        Task<List<Block>> GetBlockchain();
        Task<bool> AddBlock(string data);

        Task<List<Block>> TestGetBlockchain();
        Task<bool> TestAddBlock(string data);
        Task<bool> TestChangeBlock(int index, string data);
        double TestMineBlock(int difficulty);
        Task<List<Block>> TestIsBlockchainValid();
        Task<EmptyResp> TestMineBlock2(int index);
    }
}
