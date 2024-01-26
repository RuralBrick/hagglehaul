using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Authentication;
using System.Security.Claims;

namespace hagglehaul.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoTestController : ControllerBase
    {
        private readonly IMongoTestService _mongoTestService;

        public MongoTestController(IMongoTestService mongoTestService)
        {
            _mongoTestService = mongoTestService;
        }

        [HttpGet]
        [Route("insecure")]
        public async Task<List<MongoTest>> Get()
        {
            return await _mongoTestService.GetAsync();
        }

        [HttpGet]
        [Route("secure")]
        [Authorize]
        public async Task<List<MongoTest>> GetSecure()
        {
            ClaimsPrincipal currentUser = this.User;
            var username = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            var results = await _mongoTestService.GetAsync();
            foreach (var result in results)
            {
                result.Test = $"{result.Test} Username: {username} Role: {role}";
            }

            return results;
        }
    }
}
