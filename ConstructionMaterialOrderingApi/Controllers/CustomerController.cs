using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Dtos.CustomerDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConstructionMaterialOrderingApi.Helpers;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICustomerRepository _cutomerRepository;

        public CustomerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cutomerRepository = customerRepository;
        } 

        [HttpPost]
        [Route("/api/customer/register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterDto model)
        {
            string role = UserRole.CUSTOMER;
            var isExist = await _userManager.FindByNameAsync(model.UserName);
            if(isExist != null)
            {
                return BadRequest(new { Success = 0, Message = "Username is already taken."});
            }
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole{ Name = role});
            }
                

            var customerUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = "customer@gmail.com",
                FirstName = model.FirstName,
                LastName = model.LastName, 
                RegisteredDate = DateTime.Now
            };
            var result = await _userManager.CreateAsync(customerUser, model.Password);
            if(result.Succeeded)
            {
                var userCustomer = await _userManager.FindByNameAsync(model.UserName);
                await _userManager.AddToRoleAsync(userCustomer, role);
                await _cutomerRepository.RegisterCustomer(model, userCustomer.Id);

                return Ok(new { Success = 1, Messsage = "Registered successfully."});
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong."});

        }
        
        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update([FromBody]UpdateCustomerDto customerDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cutomerRepository.Update(customerDto, userAppId);
            return result ? Ok(new { Success = 1, Message = "Personal Information Successfully updated."}) : BadRequest(new { Success = 0, Message = "Failed to update."});
        }

        [HttpGet]
        [Route("/api/customer/get-customer-info")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerInfo = await _cutomerRepository.GetCustomerByAccountId(customerUserAccountId);

            return Ok(customerInfo);
        }
    }
}
