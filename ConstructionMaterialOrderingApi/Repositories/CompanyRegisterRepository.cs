using System;
using System.Collections.Generic;
using System.Linq;
using ConstructionMaterialOrderingApi.Dtos.CompanyRegisterDtos;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class CompanyRegisterRepository : ICompanyRegisterRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRegisterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Register(RegisterDto registerDto)
        {
            var newCompanyRegister = new CompanyRegister()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CompanyName = registerDto.CompanyName,
                Address = registerDto.Address,
                ZipCode = registerDto.Zip,
                Region = registerDto.Region,
                Country = registerDto.Country,
                PhoneNumber = registerDto.PhoneNumber,
                EmailAddress = registerDto.EmailAddress,
                BusinessPermitImageFile = await UploadBusinessPermitFile(registerDto.BusinessPermitImageFile),
                RegisteredDate = DateTime.Now
            };

            await _context.CompanyRegisters.AddAsync(newCompanyRegister);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> Delete(int companyRegisterId)
        {
            var companyRegister = await _context.CompanyRegisters.Where(c => c.Id == companyRegisterId)
                .FirstOrDefaultAsync();
            if (companyRegister != null)
            {
                _context.CompanyRegisters.Remove(companyRegister);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<CompanyRegister> Get(int companyRegisterId)
        {
            var companyRegister = await _context.CompanyRegisters.Where(c => c.Id == companyRegisterId)
                .FirstOrDefaultAsync();
            return companyRegister;
        }
        public async Task<List<CompanyRegister>> GetAll()
        {
            var companyRegisters = await _context.CompanyRegisters.ToListAsync();
            return companyRegisters;
        }

        private async Task<string> UploadBusinessPermitFile(IFormFile file)
        {
            try
            {
                var folderName = Path.Combine("Resources", "BusinessPermitFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return dbPath;
                }

                return "";

            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
