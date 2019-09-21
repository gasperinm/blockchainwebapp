using BlockchainWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainWebApp.Services
{
    public class BlockchainService : IBlockchainService
    {
        private readonly IMongoService _mongoService;

        public BlockchainService(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<bool> AddBlock(string data)
        {
            List<Block> blockchain = await GetBlockchain();

            Block latestBlock = blockchain.LastOrDefault();

            if (latestBlock == null)
            {
                return false;
            }

            Block newBlock = new Block
            {
                Index = latestBlock.Index + 1,
                Timestamp = DateTime.Now.ToString(),
                Data = data,
                PreviousHash = latestBlock.Hash,
            };

            newBlock.Hash = ComputeSha256Hash(newBlock.Index, 
                                              newBlock.Timestamp,
                                              newBlock.Data,
                                              newBlock.PreviousHash,
                                              "0");

            newBlock = MineBlock(1, newBlock);

            if (!(await IsBlockchainValid()))
            {
                return false;
            }

            blockchain.Add(newBlock);

            _mongoService.SaveNewCarData(newBlock);

            return true;
        }

        public async Task<List<Block>> GetBlockchain()
        {
            List<Block> blockchain = new List<Block>();

            var mongoBlockchain = await _mongoService.GetAllDocuments();

            if (mongoBlockchain == null)
            {
                return null;
            }

            if (mongoBlockchain.Count < 1)
            {
                blockchain = AddGenesisBlock();

                return blockchain;
            }

            foreach (var mongoBlock in mongoBlockchain)
            {
                blockchain.Add(new Block
                {
                    Index = mongoBlock.Index,
                    Data = mongoBlock.Data,
                    Timestamp = mongoBlock.Timestamp,
                    Hash = mongoBlock.Hash,
                    PreviousHash = mongoBlock.PreviousHash,
                    Nonce = mongoBlock.Nonce
                });
            }

            return blockchain;
        }

        public double TestMineBlock(int difficulty)
        {
            #region Test block
            Block testBlock = new Block
            {
                Index = 0,
                Timestamp = DateTime.Now.ToString(),
                Data = JsonConvert.SerializeObject(new CarData
                {
                    VehicleName = "",
                    License = "",
                    Registration = "",
                    Date = "",
                    Owners = "",
                    Vin = ""
                }),
                PreviousHash = "0"
            };

            testBlock.Hash = ComputeSha256Hash(testBlock.Index,
                                              testBlock.Timestamp,
                                              testBlock.Data,
                                              testBlock.PreviousHash,
                                              "0");
            #endregion

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            Block minedBlock = MineBlock(difficulty, testBlock);

            stopwatch.Stop();

            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public async Task<EmptyResp> TestMineBlock2(int index)
        {
            Block block = await _mongoService.TestGetBlock(index);

            if (block == null)
            {
                return null;
            }

            block.Hash = ComputeSha256Hash(block.Index, block.Timestamp, block.Data, block.PreviousHash, "0");

            Block minedBlock = MineBlock(1, block);

            _mongoService.TestSaveMinedBlock(minedBlock.Index, minedBlock.Hash);
            _mongoService.TestSaveNonce(minedBlock.Index, minedBlock.Nonce);

            return new EmptyResp();
        }

        public async Task<List<Block>> TestGetBlockchain()
        {
            List<Block> blockchain = new List<Block>();

            var mongoBlockchain = await _mongoService.TestGetAllDocuments();

            if (mongoBlockchain == null)
            {
                return null;
            }

            if (mongoBlockchain.Count < 1)
            {
                Block genesisBlock = new Block
                {
                    Index = 0,
                    Timestamp = DateTime.Now.ToString(),
                    Data = "Genesis block",
                    PreviousHash = "0"
                };

                genesisBlock.Hash = ComputeSha256Hash(genesisBlock.Index, genesisBlock.Timestamp, genesisBlock.Data, genesisBlock.PreviousHash, "0");

                _mongoService.TestSaveNewData(genesisBlock);
            }

            foreach (var mongoBlock in mongoBlockchain)
            {
                blockchain.Add(new Block
                {
                    Index = mongoBlock.Index,
                    Data = mongoBlock.Data,
                    Timestamp = mongoBlock.Timestamp,
                    Hash = mongoBlock.Hash,
                    PreviousHash = mongoBlock.PreviousHash,
                    Nonce = mongoBlock.Nonce
                });
            }

            return blockchain;
        }

        public async Task<bool> TestAddBlock(string data)
        {
            List<Block> blockchain = await TestGetBlockchain();

            Block latestBlock = blockchain.LastOrDefault();

            if (latestBlock == null)
            {
                Block genesisBlock = new Block
                {
                    Index = latestBlock.Index + 1,
                    Timestamp = DateTime.Now.ToString(),
                    Data = "Genesis block",
                    PreviousHash = "0"
                };

                genesisBlock.Hash = ComputeSha256Hash(genesisBlock.Index, genesisBlock.Timestamp, genesisBlock.Data, genesisBlock.PreviousHash, "0");

                _mongoService.TestSaveNewData(genesisBlock);
            }

            Block block = new Block
            {
                Index = latestBlock.Index + 1,
                Timestamp = DateTime.Now.ToString(),
                Data = data,
                PreviousHash = latestBlock.Hash
            };

            block.Hash = ComputeSha256Hash(block.Index, block.Timestamp, block.Data, block.PreviousHash, "0");

            _mongoService.TestSaveNewData(block);

            await TestMineBlock2(block.Index);

            return true;
        }

        public async Task<bool> TestChangeBlock(int index, string data)
        {
            _mongoService.TestChangeData(index, data);

            return true;
        }

        public async Task<List<Block>> TestIsBlockchainValid()
        {
            List<Block> blockchain = await TestGetBlockchain();
            List<Block> invalidBlocks = new List<Block>();

            Block previousBlock = null;

            foreach (var block in blockchain)
            {
                if (previousBlock != null)
                {
                    //Block minedBlock = MineBlock(1, block);

                    string calculatedHash = ComputeSha256Hash(block.Index, block.Timestamp, block.Data, block.PreviousHash, block.Nonce.ToString());

                    if (block.Hash != calculatedHash || block.PreviousHash != previousBlock.Hash)
                    {
                        invalidBlocks.Add(block);
                    }
                }

                previousBlock = block;
            }

            if (invalidBlocks.Count >= 1)
            {
                return invalidBlocks;
            }

            return null;
        }

        private Block MineBlock(int difficulty, Block block)
        {
            string wantedSubstring = string.Empty;

            for (int i = 0; i < difficulty; i++)
            {
                wantedSubstring += "0";
            }

            while (block.Hash.Substring(0, difficulty) != wantedSubstring)
            {
                string substring = block.Hash.Substring(0, difficulty);
                block.Nonce++;
                block.Hash = ComputeSha256Hash(block.Index, 
                                               block.Timestamp,
                                               block.Data, 
                                               block.PreviousHash, 
                                               block.Nonce.ToString());
            }

            return block;
        }

        private async Task<bool> IsBlockchainValid()
        {
            List<Block> blockchain = await GetBlockchain();

            Block previousBlock = null;

            foreach (var block in blockchain)
            {
                if (previousBlock != null)
                {
                    string calculatedHash = ComputeSha256Hash(block.Index, block.Timestamp, block.Data, block.PreviousHash, block.Nonce.ToString());

                    if (block.Hash != calculatedHash)
                    {
                        return false;
                    }

                    if (block.PreviousHash != previousBlock.Hash)
                    {
                        return false;
                    }
                }

                previousBlock = block;
            }

            return true;
        }

        private List<Block> AddGenesisBlock()
        {
            List<Block> blockchain = new List<Block>();

            blockchain = new List<Block>();

            Block genesisBlock = new Block
            {
                Index = 0,
                Timestamp = DateTime.Now.ToString(),
                Data = "Genesis block",
                PreviousHash = "0"
            };

            genesisBlock.Hash = ComputeSha256Hash(genesisBlock.Index, genesisBlock.Timestamp, JsonConvert.SerializeObject(genesisBlock.Data), genesisBlock.PreviousHash, "0");

            blockchain.Add(genesisBlock);

            _mongoService.SaveNewCarData(genesisBlock);

            return blockchain;
        }

        private string ComputeSha256Hash(int index, string timestamp, string data, string previousHash, string nonce)
        {
            string rawData = index.ToString() + timestamp + data + previousHash + nonce;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
 
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
