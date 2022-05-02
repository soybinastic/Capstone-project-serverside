using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Dtos.RequestDtos;
using ConstructionMaterialOrderingApi.Context;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class RequestProductRepository : IRequestProductRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(int requestId, RequestProductDto requestProductDto)
        {
            var newRequestProduct = new RequestProduct()
            {
                HardwareProductId = requestProductDto.HardwareProductId,
                RequestId = requestId,
                Quantity = requestProductDto.Quantity,
                IsComplete = false
            };

            _context.RequestProducts.Add(newRequestProduct);
            await _context.SaveChangesAsync();
        } 

        public async Task MakeCompleteRequestProduct(int hardwareProductId, int requestProductId)
        {
            var requestProduct = await _context.RequestProducts.Where(rp => rp.Id == requestProductId 
                && rp.HardwareProductId == hardwareProductId)
                .FirstOrDefaultAsync();
            if(requestProduct != null)
            {
                requestProduct.IsComplete = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
