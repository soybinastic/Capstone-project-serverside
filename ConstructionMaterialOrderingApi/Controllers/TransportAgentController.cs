using ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportAgentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly ITransportAgentRepository _transportAgentRepository;

        public TransportAgentController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            IHardwareStoreRepository hardwareStoreRepository, ITransportAgentRepository transportAgentRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _hardwareStoreRepository = hardwareStoreRepository;
            _transportAgentRepository = transportAgentRepository;
        } 

        [HttpPost]
        [Route("/api/transportagent/add-transp-agent")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> AddTransportAgent([FromBody] CreateTransportAgentDto model)
        {
            string role = "TransportAgent";

            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUserAccountId);

            var isExist = await _userManager.FindByNameAsync(model.UserName);
            if(isExist != null)
            {
                return BadRequest(new { Success = 0, Message = "Username is already exist"});
            }

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest(new { Success = 0, Message = "Role is not exist" });

            var storeTransportAgentUser = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = "none",
                RegisteredDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(storeTransportAgentUser, model.Password); 
            if(result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                await _userManager.AddToRoleAsync(user, role);
                await _transportAgentRepository.AddTransportAgent(model, hardwareStore.HardwareStoreId, user.Id);
                return Ok(new { Success = 1, Message = "Transport agent successfully added."});
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong"});

        }
    }
}
