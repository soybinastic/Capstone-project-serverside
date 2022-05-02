using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using ConstructionMaterialOrderingApi.Dtos.CompanyRegisterDtos;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyRegisterController : ControllerBase
    {
        private readonly ICompanyRegisterRepository _companyRegisterRepository;
        public CompanyRegisterController(ICompanyRegisterRepository companyRegisterRepository)
        {
            _companyRegisterRepository = companyRegisterRepository;
        } 

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm]RegisterDto registerDto)
        {
            await _companyRegisterRepository.Register(registerDto);
            return Ok(new { Success = 1, Message = "Successfully registered."});
        }

        [HttpGet]
        [Route("get/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            var companyRegister = await _companyRegisterRepository.Get(id);
            return Ok(ConvertToJson(companyRegister));
        }
        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var companyRegisters = await _companyRegisterRepository.GetAll();
            return Ok(ConvertToJson(companyRegisters));
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _companyRegisterRepository.Delete(id);
            return result ? Ok() : BadRequest();
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
