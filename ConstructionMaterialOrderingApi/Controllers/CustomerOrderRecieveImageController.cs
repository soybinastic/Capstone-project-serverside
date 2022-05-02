using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrderRecieveImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly ITransportAgentRepository _transportAgentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerOrderRecieveImageController(ApplicationDbContext context, IStoreAdminRepository storeAdminRepository
            , ITransportAgentRepository transportAgentRepository
            ,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _storeAdminRepository = storeAdminRepository;
            _transportAgentRepository = transportAgentRepository;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("add-customer-images/{orderId}")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> AddCustomerImages([FromRoute]int orderId)
        {
            int branchId = 0;
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userAppId);
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
                branchId = storeAdmin.BranchId;
            }
            else
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(userAppId);
                branchId = transportAgent.BranchId;
            }

            if(branchId == 0)
            {
                return BadRequest();
            }

            try
            {
                var imagesToReturn = new List<string>();
                var files = Request.Form.Files;
                var folderName = Path.Combine("Resources","OrderRecieveImages");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName); 

                if(!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                } 

                if(files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var newOrderRecieveImage = new CustomerOrderRecieveImage()
                        {
                            BranchId = branchId,
                            OrderId = orderId,
                            ImageFile = dbPath
                        };

                        await _context.CustomerOrderRecieveImages.AddAsync(newOrderRecieveImage);
                        await _context.SaveChangesAsync();

                        imagesToReturn.Add(dbPath);
                    }
                }

                return Ok(ConvertToJson(imagesToReturn));


            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-images/{orderId}")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> GetImages([FromRoute]int orderId)
        {
            int branchId = 0;
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userAppId);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
                branchId = storeAdmin.BranchId;
            }
            else
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(userAppId);
                branchId = transportAgent.BranchId;
            }

            if (branchId == 0)
            {
                return BadRequest();
            }

            var images = await _context.CustomerOrderRecieveImages.Where(image => image.BranchId == branchId && image.OrderId == orderId)
                .ToListAsync();

            return Ok(ConvertToJson(images));
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
