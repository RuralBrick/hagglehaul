using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Authentication;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace hagglehaul.Server.Controllers
{
    /// <summary>
    /// Test controller for frontend to database connection.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MongoTestController : ControllerBase
    {
        private readonly IMongoTestService _mongoTestService;

        public MongoTestController(IMongoTestService mongoTestService)
        {
            _mongoTestService = mongoTestService;
        }

        /// <summary>
        /// Test frontend to database endpoint without authentication.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("insecure")]
        [SwaggerOperation(Summary = "Test frontend to database endpoint without authentication")]
        public async Task<List<MongoTest>> Get()
        {
            return await _mongoTestService.GetAsync();
        }

        /// <summary>
        /// Test frontend to database endpoint with authentication.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("secure")]
        [Authorize]
        [SwaggerOperation(Summary = "Test frontend to database endpoint with authentication")]
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
