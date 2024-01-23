﻿using hagglehaul.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace hagglehaul.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IOptions<JwtSettings> _settings;
        private readonly UserCoreService _userCoreService;

        public AuthenticationController(IOptions<JwtSettings> settings, UserCoreService userCoreService)
        {
            _settings = settings;
            _userCoreService = userCoreService;
        }

        protected void CreatePasswordHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(128 / 8);
            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
        
        protected bool ComparePasswordToHash(string password, string hash, string salt)
        {
            string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return newHash == hash;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var existingUser = await _userCoreService.GetAsync(model.Email);
            if (existingUser is not null)
                return BadRequest("User already exists");
            
            CreatePasswordHash(model.Password, out var hash, out var salt);
            var userCore = new UserCore
            {
                Email = model.Email,
                PasswordHash = hash,
                Salt = salt,
                Role = model.Role
            };

            await _userCoreService.CreateAsync(userCore);

            return await Login(new Login { Email = model.Email, Password = model.Password });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var userCore = await _userCoreService.GetAsync(model.Email);

            if (userCore is not null &&
                ComparePasswordToHash(model.Password, userCore.PasswordHash, userCore.Salt)
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
    }
}
