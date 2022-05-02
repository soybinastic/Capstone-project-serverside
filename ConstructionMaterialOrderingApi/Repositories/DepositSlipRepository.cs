using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Dtos.DepositSlipDtos;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class DepositSlipRepository : IDepositSlipRepository
    {
        private readonly ApplicationDbContext _context;
        public DepositSlipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(DepositSlipDto depositSlipDto, int branchId)
        {
            await _context.DepositSlips.AddAsync(new DepositSlip
            {
                BranchId = branchId,
                Depositor = depositSlipDto.Depositor,
                DateDeposited = depositSlipDto.DateDeposited,
                BankName = depositSlipDto.BankName,
                Amount = depositSlipDto.AmountDeposited
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<DepositSlip>> GetDepositSlips(int branchId)
        {
            var depositSlips = await _context.DepositSlips.Where(ds => ds.BranchId == branchId)
                .OrderByDescending(ds => ds.DateDeposited)
                .ToListAsync();

            return depositSlips;

        }

        public async Task<DepositSlip> GetDepositSlip(int branchId, int depositSlipId)
        {
            var depositSlip = await _context.DepositSlips.Where(ds => ds.BranchId == branchId && ds.Id == depositSlipId)
                .FirstOrDefaultAsync();

            return depositSlip;
        }

        public async Task<bool> Update(int branchId, int depositSlipId, DepositSlipDto depositSlipDto)
        {
            var depositSlipToUpdate = await _context.DepositSlips.Where(ds => ds.Id == depositSlipId
                && ds.BranchId == branchId)
                .FirstOrDefaultAsync();

            if (depositSlipToUpdate == null)
                return false;

            depositSlipToUpdate.Depositor = depositSlipDto.Depositor;
            depositSlipToUpdate.BankName = depositSlipDto.BankName;
            depositSlipToUpdate.DateDeposited = depositSlipDto.DateDeposited;
            depositSlipToUpdate.Amount = depositSlipDto.AmountDeposited;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
