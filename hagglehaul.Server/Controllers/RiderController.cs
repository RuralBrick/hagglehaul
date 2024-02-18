using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hagglehaul.Server.Models;
using System.Runtime.InteropServices;

namespace hagglehaul.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiderController : ControllerBase
    {
        private readonly IRiderProfileService _riderProfileService;
        private readonly IDriverProfileService _driverProfileService;
        private readonly IUserCoreService _userCoreService;
        private readonly ITripService _tripService;
        private readonly IBidService _bidService;

        public RiderController(IRiderProfileService riderProfileService, IDriverProfileService driverProfileService, IUserCoreService userCoreService, ITripService tripService, IBidService bidService)
        {
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
        }

        [Authorize]
        [HttpPost]
        [Route("modifyAcc")]
        public async Task<IActionResult> ModifyAccountDetails([FromBody] RiderUpdate riderUpdate )
        {
            ClaimsPrincipal currentUser = this.User;
  
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            bool changingPassword = !String.IsNullOrEmpty(riderUpdate.NewPassword);
            if (String.IsNullOrEmpty(riderUpdate.CurrentPassword) && changingPassword)
            {
                return BadRequest(new { Error = "Can't make a new password" });
            }
            
            //check role for error
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            UserCore userCore = await _userCoreService.GetAsync(email);
            
            if (changingPassword)
            {

                if (!_userCoreService.ComparePasswordToHash(riderUpdate.CurrentPassword, userCore.PasswordHash,userCore.Salt))
                {
                    return BadRequest(new { Error = "Current Password is invalid" });
                }
                
            }
            
            if (changingPassword) {
                Console.WriteLine("changing pass");
                _userCoreService.CreatePasswordHash(riderUpdate.NewPassword, out var newHash, out var newSalt);
                userCore.PasswordHash = newHash;
                userCore.Salt = newSalt;
            }
            if (!String.IsNullOrEmpty(riderUpdate.Name))
            {
                userCore.Name = riderUpdate.Name;
            }
            if (!String.IsNullOrEmpty(riderUpdate.Phone))
            {
                userCore.Phone = riderUpdate.Phone;
            }
            await _userCoreService.UpdateAsync(email, userCore);
            return Ok();
        }
    }

}
