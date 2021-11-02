using ConstructionMaterialOrderingApi.Dtos.CustomerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetCustomerDto> GetCustomerByAccountId(string customerAccountId)
        {
            var customer = await _context.Customers.Where(c => c.AccountId == customerAccountId)
                .FirstOrDefaultAsync();

           if(customer != null)
            {
                var customerDto = new GetCustomerDto()
                {
                    CustomerId = customer.Id,
                    AccountId = customer.AccountId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Address = customer.Address,
                    ContactNo = customer.ContactNo,
                    Email = customer.Email
                };
                return customerDto;
            }

            return null;
        }

        public async Task RegisterCustomer(CustomerRegisterDto model, string accountId)
        {
            var customer = new Customer()
            {
                AccountId = accountId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Address = model.Address,
                ContactNo = model.ContactNo
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }
    }
}
