using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace hagglehaul.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MongoTestController : ControllerBase
    {
        private readonly ILogger<MongoTestController> _logger;
        private readonly MongoTestService _mongoTestService;

        public MongoTestController(ILogger<MongoTestController> logger, MongoTestService mongoTestService)
        {
            _logger = logger;
            _mongoTestService = mongoTestService;
        }

        [HttpGet(Name = "GetMongoTest")]
        public async Task<List<MongoTest>> Get()
        {
            return await _mongoTestService.GetAsync();
        }
    }
}
