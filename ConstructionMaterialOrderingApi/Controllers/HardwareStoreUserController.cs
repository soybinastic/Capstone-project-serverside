using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Implementations;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardwareStoreUserController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly IHardwareStoreUserHandler _hardwareStoreUserHandler;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;

        public HardwareStoreUserController(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, IHardwareStoreUserRepository hardwareStoreUserRepository,
            IHardwareStoreUserHandler hardwareStoreUserHandler,
            IWareHouseAdminRepository wareHouseAdminRepository,
            IStoreAdminRepository storeAdminRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _hardwareStoreUserHandler = hardwareStoreUserHandler;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _storeAdminRepository = storeAdminRepository;
        } 

        [HttpGet]
        [Route("/api/hardwarestoreuser/get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles =  _roleManager.Roles.ToList();
            var jsonRoles = ConvertToJson(roles);
            return Ok(jsonRoles);
        }
        [HttpPost]
        [Route("add-user")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> AddUser([FromBody] HardwareStoreUserDto storeDto)
        {
            var hardwareStoreAppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreAppUserId);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreAppUserId);
            var isUserNameExist = await _userManager.FindByNameAsync(storeDto.UserName);
            if(isUserNameExist != null)
            {
                return BadRequest(new { Success = 0, Message = $"Username of {storeDto.UserName} is already taken."});
            } 

            if(!await _roleManager.RoleExistsAsync(storeDto.Role))
            {
                return BadRequest(new { Success = 0, Message = "User role is exist."});
            }

            var hardwareStoreUser = new ApplicationUser()
            {
                UserName = storeDto.UserName,
                Email = storeDto.Email,
                FirstName = storeDto.FirstName,
                LastName = storeDto.LastName,
                RegisteredDate = DateTime.Now
            };
            var result = await _userManager.CreateAsync(hardwareStoreUser, storeDto.Password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(hardwareStoreUser.UserName);
                await _userManager.AddToRoleAsync(user, storeDto.Role);
                await _hardwareStoreUserRepository.AddUser(storeDto, user.Id, hardwareStoreUserInfo.HardwareStoreId);
                var handlerInstance = _hardwareStoreUserHandler.GetHardwareStoreUserInstance(storeDto.Role);
                _hardwareStoreUserHandler.CreateUser(handlerInstance, storeDto, user.Id, hardwareStoreUserInfo.HardwareStoreId);
                
                return Ok(new { Success = 1, Message = $"User {storeDto.UserName} is added Successfully."});
            }

            return BadRequest(new { Success = 0, Message = "This user cannot be identified by the system."});
        }
        [HttpGet]
        [Route("get-users")]
        [Authorize(Roles = "StoreOwner,SuperAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var hardwareStoreAppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreAppUserId);
            var hardwareStoreUsers = await _hardwareStoreUserRepository.GetUsers(hardwareStoreUserInfo.HardwareStoreId);
            var json = ConvertToJson(hardwareStoreUsers);
            return Ok(json);
        } 
        [HttpGet]
        [Route("get-user/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            return Ok(await _hardwareStoreUserRepository.GetUserById(userId));
        }
        [HttpGet]
        [Route("user-logged-in-info")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin,TransportAgent,StoreAdmin,Cashier,SalesClerk")]
        public async Task<IActionResult> UserLoggedInfo()
        {
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(appUserId);
            var jsonResult = ConvertToJson(hardwareStoreUserInfo);
            //IHardwareStoreUser hardwareStoreUserInstance = _hardwareStoreUserHandler
            //    .GetHardwareStoreUserInstance(hardwareStoreUserInfo.Role);
            //var hardwareStoreUser = _hardwareStoreUserHandler
            //    .GetUser(hardwareStoreUserInstance, hardwareStoreUserInfo.ApplicationUserAccountId);

            return Ok(jsonResult);
        } 
        [HttpGet]
        [Route("get-warehouse-admin-info")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> GetWareHouseAdmin()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminnfo = await _wareHouseAdminRepository
                .GetWarehouseAdminByAccountId(userId);
            var jsonResult = ConvertToJson(warehouseAdminnfo);
            return Ok(jsonResult);
        }
        [HttpGet]
        [Route("get-storeadmin-info")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetStoreAdminInfo()
        {
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdminInfo = await _storeAdminRepository.GetStoreAdminByAccountId(appUserId);
            return Ok(ConvertToJson(storeAdminInfo));
        }

        private string ConvertToJson<T>(T obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return jsonObj;
        }
    }
}
