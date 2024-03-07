using hagglehaul.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IOptions<JwtSettings> _settings;
        private readonly IUserCoreService _userCoreService;
        private readonly IRiderProfileService _riderProfileService;
        private readonly IDriverProfileService _driverProfileService;

        public AuthenticationController(
            IOptions<JwtSettings> settings,
            IUserCoreService userCoreService,
            IRiderProfileService riderProfileService,
            IDriverProfileService driverProfileService
        )
        {
            _settings = settings;
            _userCoreService = userCoreService;
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
        }

        [HttpPost]
        [Route("register")]
        [SwaggerOperation(Summary = "Register a new user")]
        [SwaggerResponse(StatusCodes.Status200OK, "User registered successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "One or more fields are empty, the user already exists, or invalid role")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.Role))
                return BadRequest(new { Error = "One or more fields are empty" });
            
            var existingUser = await _userCoreService.GetAsync(model.Email);
            if (existingUser is not null)
                return BadRequest(new { Error = "User already exists" });

            if (model.Role != "rider" && model.Role != "driver")
                return BadRequest(new { Error = "Role must either be \"rider\" or \"driver\"" });

            _userCoreService.CreatePasswordHash(model.Password, out var hash, out var salt);
            var userCore = new UserCore
            {
                Email = model.Email,
                PasswordHash = hash,
                Salt = salt,
                Role = model.Role
            };

            await _userCoreService.CreateAsync(userCore);

            if (model.Role == "rider")
            {
                var riderProfile = new RiderProfile
                {
                    Email = model.Email,
                    NumRatings = 0
                };
                await _riderProfileService.CreateAsync(riderProfile);
            }
            else
            {
                var driverProfile = new DriverProfile
                {
                    Email = model.Email,
                    NumRatings = 0
                };
                await _driverProfileService.CreateAsync(driverProfile);
            }

            return await Login(new Login { Email = model.Email, Password = model.Password });
        }

        [HttpPost]
        [Route("login")]
        [SwaggerOperation(Summary = "Login as an existing user")]
        [SwaggerResponse(StatusCodes.Status200OK, "User logged in successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "One or more fields are empty")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid email or password")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password))
                return BadRequest(new { Error = "One or more fields are empty" });
            
            var userCore = await _userCoreService.GetAsync(model.Email);

            if (userCore is not null &&
                _userCoreService.ComparePasswordToHash(model.Password, userCore.PasswordHash, userCore.Salt)
                )
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                authClaims.Add(new Claim(ClaimTypes.Role, userCore.Role));

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret));

                var token = new JwtSecurityToken(
                    issuer: _settings.Value.ValidIssuer,
                    audience: _settings.Value.ValidAudience,
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        
        [HttpGet]
        [Route("role")]
        [Authorize]
        [SwaggerOperation(Summary = "Get the role of the current user")]
        [SwaggerResponse(StatusCodes.Status200OK, "Role retrieved successfully")]
        public async Task<String> Role()
        {
            ClaimsPrincipal currentUser = this.User;
            return currentUser.FindFirstValue(ClaimTypes.Role);
        }
    }
}
