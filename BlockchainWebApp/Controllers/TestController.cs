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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly IBlockchainService _blockchainService;

        public TestController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        [HttpGet]
        public IActionResult TestMineBlock(int difficulty)
        {
            var resp = _blockchainService.TestMineBlock(difficulty);

            return Ok(resp);
        }

        public async Task<IActionResult> TestMineBlock2(int index)
        {
            var resp = await _blockchainService.TestMineBlock2(index);

            return Ok(resp);
        }

        public async Task<IActionResult> TestGetBlockchain()
        {
            var resp = await _blockchainService.TestGetBlockchain();

            return Ok(resp);
        }

        public async Task<IActionResult> TestAddBlock(string data)
        {
            await _blockchainService.TestAddBlock(data);

            return BadRequest();
        }

        public async Task<IActionResult> TestChangeData(int index, string newData)
        {
            await _blockchainService.TestChangeBlock(index, newData);

            return Ok(new EmptyResp());
        }

        public async Task<List<Block>> TestBlockchainValid()
        {
            var resp = await _blockchainService.TestIsBlockchainValid();

            return resp;
        }
    }
}
