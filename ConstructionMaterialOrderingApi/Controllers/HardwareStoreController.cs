using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardwareStoreController : ControllerBase
    {
        private readonly IHardwareStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransportAgentRepository _transportAgentRepository;

        public HardwareStoreController(IHardwareStoreRepository storeRepository, UserManager<ApplicationUser> userManager,
            ITransportAgentRepository transportAgentRepository)
        {
            _storeRepository = storeRepository;
            _userManager = userManager;
            _transportAgentRepository = transportAgentRepository;
        } 
        [HttpGet]
        [Route("/api/hardwarestore/get-hardware-stores")]
        public async Task<IActionResult> GetAllHardwareStore()
        {
            var stores = await _storeRepository.GetAllHardwareStore();
            var hardwareStoresJson = JsonConvert.SerializeObject(stores, new JsonSerializerSettings
            { 
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return Ok(hardwareStoresJson);
        } 
        [HttpGet]
        [Route("/api/hardwarestore/get-hardwarestore-info")]
        [Authorize(Roles = "StoreOwner,TransportAgent")]
        public async Task<IActionResult> GetHardwareStoreInfo()
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _storeRepository.GetHardware(hardwareStoreUser.Id);
                var hardwareStoreInfo = await _storeRepository.GetHardwareByStoreId(hardwareStore.HardwareStoreId);
                return Ok(hardwareStoreInfo);
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var hardwareStoreTranspAgent = await _transportAgentRepository.GetTransportAgentByAccountId(hardwareStoreUser.Id);
                var hardwareStoreInfo = await _storeRepository.GetHardwareByStoreId(hardwareStoreTranspAgent.HardwareStoreId);
                return Ok(hardwareStoreInfo);
            }
            return BadRequest(new { Success = 0, Message = "Something went wrong."});
        }

        [HttpGet]
        [Route("/api/hardwarestore/get-hardwarestore/{storeId}")]
        public async Task<IActionResult> GetHardwareStore(int storeId)
        {
            var hardwareStore = await _storeRepository.GetHardwareByStoreId(storeId);
            return Ok(hardwareStore);
        }
    }
}
