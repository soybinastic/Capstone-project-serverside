using System.Security.Claims;
using System;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,StoreAdmin")]
        public async Task<IActionResult> GetAll()
        {
            string branchIdString = HttpContext.Request.Query["branchId"].ToString();
            int branchId = Convert.ToInt32(branchIdString);
            
            Console.WriteLine(branchIdString);

            var dashboards = await _dashboardRepository.GetAll();

            if(branchId > 0)
            {
                dashboards = dashboards.Where(d => d.BranchId == branchId).ToList();
            }

            return Ok(ConvertToJson(dashboards));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,StoreAdmin")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var dashboard = await _dashboardRepository.GetById(id);
            return Ok(ConvertToJson(dashboard));
        }

        [HttpGet("by-branch/{branchId}")]
        [Authorize(Roles = "Admin,StoreAdmin")]
        public async Task<IActionResult> GetDashboardsByBranch([FromRoute]int branchId)
        {
            var dashboards = await _dashboardRepository.GetAll(branchId: branchId);
            return Ok(ConvertToJson(dashboards));
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