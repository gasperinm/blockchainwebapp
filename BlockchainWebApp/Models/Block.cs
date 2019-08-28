using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Models
{
    public class Block
    {
        public int Index { get; set; }
        public string Timestamp { get; set; }
        public string Data { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }
    }
}
