using BlockchainWebApp.Models;
using BlockchainWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainWebApp.Controllers
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    public class BlockchainController : Controller
    {
        private readonly IBlockchainService _blockchainService;

        public BlockchainController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockchain()
        {
            //List<Block> blockchain = _blockchainService.GetBlockchain();
            List<Block> blockchain = await _blockchainService.GetBlockchain();

            if (blockchain == null)
            {
                return BadRequest();
            }

            return Ok(blockchain);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlock([FromBody] CarData carData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //bool resp = _blockchainService.AddBlock(JsonConvert.SerializeObject(carData));
            bool resp = await _blockchainService.AddBlock(JsonConvert.SerializeObject(carData));

            if (!resp)
            {
                return BadRequest();
            }

            return Ok(new EmptyResp());
        }
    }
}
