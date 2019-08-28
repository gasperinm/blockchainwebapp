using BlockchainWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public bool AddBlock(string data)
        {
            List<Block> blockchain = GetBlockchain();

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

            newBlock.Hash = ComputeSha256Hash(newBlock.Index, newBlock.Timestamp, newBlock.Data, newBlock.PreviousHash, "0");
            newBlock = MineBlock(1, newBlock);

            if (!IsBlockchainValid())
            {
                return false;
            }

            blockchain.Add(newBlock);

            SaveToFile(blockchain);

            return true;
        }

        public async Task<bool> AddBlock2(string data)
        {
            List<Block> blockchain = await GetBlockchain2();

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

            newBlock.Hash = ComputeSha256Hash(newBlock.Index, newBlock.Timestamp, newBlock.Data, newBlock.PreviousHash, "0");
            newBlock = MineBlock(1, newBlock);

            if (!(await IsBlockchainValid2()))
            {
                return false;
            }

            blockchain.Add(newBlock);

            _mongoService.SaveNewCarData(newBlock);

            return true;
        }

        public List<Block> GetBlockchain()
        {
            List<Block> blockchain = new List<Block>();

            try
            {
                string contents = File.ReadAllText("blockchain.json");

                blockchain = JsonConvert.DeserializeObject<List<Block>>(contents);

                return blockchain;
            }

            catch (Exception ex)
            {
                blockchain = AddGenesisBlock();

                return blockchain;
            }
        }

        public async Task<List<Block>> GetBlockchain2()
        {
            List<Block> blockchain = new List<Block>();

            var mongoBlockchain = await _mongoService.GetAllDocuments();

            if (mongoBlockchain == null)
            {
                return null;
            }

            if (mongoBlockchain.Count < 1)
            {
                blockchain = AddGenesisBlock2();

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
                block.Hash = ComputeSha256Hash(block.Index, block.Timestamp, block.Data, block.PreviousHash, block.Nonce.ToString());
            }

            return block;
        }

        private bool IsBlockchainValid()
        {
            List<Block> blockchain = GetBlockchain();

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

            //for (int i = 1; i < blockchain.Count; i++)
            //{
            //    Block currentBlock = blockchain[i];
            //    Block previousBlock = blockchain[i - 1];

            //    string calculatedHash = ComputeSha256Hash(currentBlock.Index, currentBlock.Timestamp, currentBlock.Data, currentBlock.PreviousHash, "0");

            //    if (currentBlock.Hash != calculatedHash)
            //    {
            //        return false;
            //    }

            //    if (currentBlock.PreviousHash != previousBlock.Hash)
            //    {
            //        return false;
            //    }
            //}

            return true;
        }

        private async Task<bool> IsBlockchainValid2()
        {
            List<Block> blockchain = await GetBlockchain2();

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
            //List<Block> blockchain = GetBlockchain();
            List<Block> blockchain = new List<Block>();

            //if (blockchain == null)
            //{
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

            SaveToFile(blockchain);

            return blockchain;
            //}
        }

        private List<Block> AddGenesisBlock2()
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

            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

            //byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            //SHA256Managed hashstring = new SHA256Managed();
            //byte[] hash = hashstring.ComputeHash(bytes);
            //string hashString = string.Empty;
            //foreach (byte x in hash)
            //{
            //    hashString += String.Format("{0:x2}", x);
            //}
            //return hashString;
        }

        private bool SaveToFile(List<Block> blockchain)
        {
            string contents = JsonConvert.SerializeObject(blockchain);

            try
            {
                File.WriteAllText("blockchain.json", contents);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
