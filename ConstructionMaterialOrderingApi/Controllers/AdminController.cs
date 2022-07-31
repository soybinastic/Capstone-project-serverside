using ConstructionMaterialOrderingApi.Dtos.AdminDtos;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IHardwareStoreRepository hardwareStoreRepository, IHardwareStoreUserRepository hardwareStoreUserRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _hardwareStoreRepository = hardwareStoreRepository;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
        } 
        [HttpPost]
        [Route("/api/admin/add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto roleDto)
        {
            var role = new IdentityRole()
            {
                Name = roleDto.RoleName
            }; 
            if(await _roleManager.RoleExistsAsync(roleDto.RoleName))
            {
                return BadRequest(new { Success = 0, Message = $"Role {roleDto.RoleName} is already exist."});
            }
            var result = await _roleManager.CreateAsync(role);
            IActionResult response = result.Succeeded ? Ok(new { Success = 1, Message = $"Role {roleDto.RoleName} is added successfully." }) : 
                BadRequest(new { Success = 0, Message = "Something went wrong."});

            return response;
        } 
        [HttpPost]
        [Route("/api/admin/add-admin")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDto addAdminDto)
        {
            string role = "Admin";
            var isAdminExist = await _userManager.FindByNameAsync(addAdminDto.UserName);
            if(isAdminExist != null)
            {
                return BadRequest(new { Success = 0, Messasge = "Admin username is already exist."});
            }
            var admin = new ApplicationUser()
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
                UserName = addAdminDto.UserName,
                Email = addAdminDto.Email,
                RegisteredDate = DateTime.Now
            };
            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { Success = 0, Messasge = "Role is not exist." });

            var result = await _userManager.CreateAsync(admin, addAdminDto.Password);
            if(result.Succeeded)
            {
                var adminUser = await _userManager.FindByNameAsync(addAdminDto.UserName);
                await _userManager.AddToRoleAsync(adminUser, role);
                return Ok(new { Success = 1, Message = "Added Successfully."});
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong."});
        }
        [HttpPost]
        [Route("/api/admin/register-hardware-store")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterHardwareStore([FromBody] RegisterHardwareStoreDto model)
        {
            string role = "StoreOwner";
            var isExist = await _userManager.FindByNameAsync(model.UserName);
            if(isExist != null)
            {
                return BadRequest(new { Success = 0, Message = "Username is already taken"});
            }

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { Success = 0, Message = "Role is not exist"}); 

            var hardwareAccount = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                RegisteredDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(hardwareAccount, model.Password);
            if (result.Succeeded)
            {
                var storeOwnerUser = new HardwareStoreUserDto()
                {
                    FirstName = hardwareAccount.FirstName,
                    LastName = hardwareAccount.LastName,
                    Email = hardwareAccount.Email,
                    UserName = hardwareAccount.UserName,
                    Role = "StoreOwner",
                    UserFrom = "Store"
                };
                var user = await _userManager.FindByNameAsync(model.UserName);
                await _userManager.AddToRoleAsync(user, role);
                var hardwareStore = _hardwareStoreRepository.AddHardwareStore(user.Id, model);
                await _hardwareStoreUserRepository.AddUser(storeOwnerUser, user.Id, hardwareStore.Id);
                return Ok(new { Success = 1, Message = $"{model.HardwareStoreName} is registered successfully."});
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong"});
        }
    }
}
