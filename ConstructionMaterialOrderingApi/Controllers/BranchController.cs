using ConstructionMaterialOrderingApi.Dtos.BranchDto;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;

        public BranchController(IBranchRepository branchRepository, IHardwareStoreUserRepository hardwareStoreUserRepository)
        {
            _branchRepository = branchRepository;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
        }

        [HttpPost]
        [Route("add-branch")]
        [Authorize(Roles = "StoreOwner,SuperAdmin")]
        public async Task<IActionResult> AddBranch([FromForm]AddBranchDto branchDto)
        {
            var hardwareStoreAppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreAppUserId);
            await _branchRepository.AddBranch(branchDto, hardwareStoreUserInfo.HardwareStoreId);
            return Ok(new { Success = 1, Message = "Addes Successfully."});
        }
        [HttpGet]
        [Route("get-branches")]
        [Authorize(Roles = "StoreOwner,SuperAdmin")]
        public async Task<IActionResult> GetBranches()
        {
            var hardwareStoreAppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreAppUserId);
            var branches = await _branchRepository.GetBranchesByStoreId(hardwareStoreUserInfo.HardwareStoreId);
            var json = ConvertToJson(branches);
            return Ok(json);
        }
        [HttpGet]
        [Route("get-branches/{storeId}")]
        public async Task<IActionResult> GetBranches(int storeId)
        {
            var branches = await _branchRepository.GetActiveBranches(storeId);
            var json = ConvertToJson(branches);
            // Console.WriteLine(HttpContext.User.Identity.IsAuthenticated);
            return Ok(json);
        }

        [HttpGet]
        [Route("get-branch/{branchId}")]
        public async Task<IActionResult> GetBranch(int branchId)
        {
            var branch = await _branchRepository.GetBranch(branchId);
            var json = ConvertToJson(branch);
            return Ok(json);
        }

        [HttpPut]
        [Route("update-branch/{branchId}")]
        [Authorize(Roles = "StoreOwner,SuperAdmin")]
        public async Task<IActionResult> UpdateBranch([FromForm]UpdateBranchDto branchDto,
            int branchId)
        {
            var hardwareStoreAppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreAppUserId);
            var result = await _branchRepository.UpdateBranch(branchDto, branchId, hardwareStoreUserInfo.HardwareStoreId);
            IActionResult returnValue = result ? Ok(new { Success = 1, Message = "Modified Successfully" }) :
                BadRequest(new { Success = 0, Message = "Failed to update" });
            return returnValue;
        }
        [HttpGet("all-branches")]
        public async Task<IActionResult> GetAllBranches([FromQuery(Name = "adjusted_km")]double adjustedKm)
        {
            try
            {
                string customerLat = HttpContext.Request.Headers["lat"];
                string customerLng = HttpContext.Request.Headers["lng"];

                double lat = customerLat.ToLatLng();
                double lng = customerLng.ToLatLng();
                

                var allBranches = await _branchRepository.GetAllBranches(lat, lng, adjustedKm);
                // Console.WriteLine(HttpContext.User.Identity.IsAuthenticated);
                return Ok(ConvertToJson(allBranches));

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery(Name = "search")]string search, 
            [FromQuery(Name = "adjusted_km")]double adjustedKm)
        {
            try
            {
                // get lat lang from request headers.
                string latString = HttpContext.Request.Headers["lat"];
                string lngString = HttpContext.Request.Headers["lng"];

                double lat = latString.ToLatLng();
                double lng = lngString.ToLatLng();
            
                var result = await _branchRepository.Search(search, lat, lng, adjustedKm);
                return Ok(ConvertToJson(result));
            }catch(Exception ex)
            {
                return BadRequest(new { Success = 0, Message = ex.Message });
            }
        }
        private string ConvertToJson<T>(T val)
        {
            var jsonObj = JsonConvert.SerializeObject(val, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return jsonObj;
        }
    }
}
