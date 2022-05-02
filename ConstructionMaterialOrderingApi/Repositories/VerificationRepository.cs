using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.CustomerVerificationDtos;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly ApplicationDbContext _context;
        public VerificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        private async Task<string> UploadImage(IFormFile file, string directoryFolder)
        {
            try
            {

                var folderName = Path.Combine("Resources", directoryFolder);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (file?.Length > 0)
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

                return string.Empty;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<(bool, string)> Post(VerificationDetail verificationDetail, int customerId)
        {
            var customer = await _context.Customers.Where(c => c.Id == customerId).FirstOrDefaultAsync();

            if(customer == null)
            {
                return (false, "User not found!");
            }

            var customerDetail = await _context.CustomerVerifications.Where(cv => cv.CustomerId == customerId)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync();

            if(customerDetail != null && customerDetail.Customer.IsVerified)
            {
                return (false, "You are already verified by the system.");
            }

            var isUserAlreadyFill = await _context.CustomerVerifications.AnyAsync(cv => cv.CustomerId == customerId);

            if (isUserAlreadyFill)
            {
                return (true, "You already fill your verification details. Please wait to verified your account by the Admin.");
            }

            var newVerificationDetail = new CustomerVerification()
            {
                CustomerId = customerId,
                FirstName = verificationDetail.FirstName,
                MiddleName = verificationDetail.MiddleName,
                LastName = verificationDetail.LastName,
                Address = verificationDetail.Address,
                BirthDate = verificationDetail.BirthDate,
                Age = verificationDetail.Age,
                NbiImageFile = await UploadImage(verificationDetail.Nbi, "NBIFiles"),
                BarangayClearanceImageFile = await UploadImage(verificationDetail.BarangayClearance, "BarangayClearanceFiles"),
                GovernmentIdImageFile = await UploadImage(verificationDetail.GovernmentId, "GovernmentIdFiles"),
                BankStatementImageFile = await UploadImage(verificationDetail.BankStatement, "BankStatementFiles"),
                DateSubmitted = DateTime.Now
            };

            await _context.CustomerVerifications.AddAsync(newVerificationDetail);
            await _context.SaveChangesAsync();
            return (true, "Submitted succesfully");
        }

        public async Task<List<CustomerVerification>> GetAll()
        {
            var customerVerificationsDetails = await _context.CustomerVerifications.Include(cv => cv.Customer)
                    .OrderByDescending(cv => cv.DateSubmitted)
                    .ToListAsync();

            return customerVerificationsDetails;
        }
        public async Task<CustomerVerification> Get(int id)
        {
            var customerVerificationDetails = await _context.CustomerVerifications.Where(cv => cv.Id == id)
                    .Include(cv => cv.Customer)
                    .FirstOrDefaultAsync();

            return customerVerificationDetails;
        }

        public async Task<(bool, string)> VerifyCustomer(int customerId)
        {
            var customer = await _context.Customers.Where(c => c.Id == customerId)
                .FirstOrDefaultAsync();
            if (customer != null)
            {
                if (!customer.IsVerified)
                {
                    customer.IsVerified = true;
                    await _context.SaveChangesAsync();

                    return (true, "Verified Successfully.");
                }

                return (true, "This user is already verified.");
            }

            return (false, "Customer not found");
        }
    }
}
