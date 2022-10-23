using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.EmployeeDtos;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEmployee newEmployee)
        {
            var roles = new string[] { UserRole.COMPANY_VALIDATOR, UserRole.USER_VALIDATOR, UserRole.ADMIN };
            var exist = roles.Any(r => r == newEmployee.Role);
            if (!exist) return BadRequest();

            var roleExist = await _roleManager.RoleExistsAsync(newEmployee.Role);
            if (!roleExist)
            {
                return BadRequest();
            }

            var userExist = await _userManager.FindByNameAsync(newEmployee.Username);
            if (userExist != null) return BadRequest();

            var user = new ApplicationUser
            {
                UserName = newEmployee.Username,
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName,
                RegisteredDate = DateTime.UtcNow,
                Email = "fastline.test@gmail.com"
            };
            var result = await _userManager.CreateAsync(user, newEmployee.Password);
            if (!result.Succeeded) return BadRequest(new { Succcess = 0, Errors = result.Errors.ToList(), Message = "Failed to create this employee." });

            var res = await _userManager.AddToRoleAsync(user, newEmployee.Role);
            if (!res.Succeeded) return BadRequest();

            var fastlineUser = await _userManager.FindByNameAsync(newEmployee.Username);
            var userRoles = await _userManager.GetRolesAsync(fastlineUser);
            await _db.FastlineUsers.AddAsync(new FastlineUser { UserId = fastlineUser.Id, Role = userRoles.FirstOrDefault() });
            await _db.SaveChangesAsync();
            return Ok(new { Success = 1, Message = "Success" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var employees = await _db.FastlineUsers.Select(u => new GetEmployeeDto
            {
                Id = u.Id,
                UserId = u.UserId,
                Role = u.Role,
                User = _db.Users.FirstOrDefault(x => x.Id == u.UserId)
            }).ToListAsync();
            return Ok(ConvertToJson(employees));
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