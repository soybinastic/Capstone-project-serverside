using ConstructionMaterialOrderingApi.Dtos.AccountDtos;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHardwareStoreRepository _storeRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IHardwareStoreRepository storeRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _storeRepository = storeRepository;
        } 

        [HttpPost]
        [Route("/api/account/login")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if(result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var role = await _userManager.GetRolesAsync(user);
                var token = GenerateToken(user.Id, user.UserName, user.Email, role);
                await _storeRepository.SignIn(user.Id, role);
                return Ok(new { Success = 1, Message = "Login successfully", Token = token});
            }
            return BadRequest(new { Success = 0, Message = "Username or Password are invalid."});
        } 

        [HttpPost]
        [Route("/api/account/logout")] 
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(accountId);
            if(user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                await _storeRepository.SignOut(accountId, role);
                await _signInManager.SignOutAsync();
                return Ok(new { Success = 1, Message = "Logout successfully."});
            }
            return BadRequest(new { Success = 0, Message = "Something went wrong."});
        }
        private string GenerateToken(string userId,string userName, string email, IList<string> role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("MY_SECRET_KEY_FOR_CONSTRUCTION_MATERIAL_ORDERING@2021_BY_BISOYVINUYA*");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
    }
}
