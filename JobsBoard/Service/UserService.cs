using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;  // Required for IHttpContextAccessor
using System.Security.Claims;

namespace JobsBoard.Service
{
    [Authorize]
    public class UserService
    {
        private readonly UserManager<JobsBoardUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor to inject UserManager and IHttpContextAccessor
        public UserService(UserManager<JobsBoardUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // Method to get the current user using IHttpContextAccessor


        public string check()
        {
            return "Hello";
        }
        public async Task<JobsBoardUser> GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    var currentUser = await _userManager.FindByIdAsync(userId);
                    return currentUser;
                }
            }

            return null;
        }

    }
}
