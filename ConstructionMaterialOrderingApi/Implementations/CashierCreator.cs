using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class CashierCreator : IHardwareStoreUser
    {
        private readonly IBranchUserRepository<Cashier> _cashierRepository;
        public CashierCreator(IBranchUserRepository<Cashier> cashierRepository)
        {
            _cashierRepository = cashierRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _cashierRepository.Add(new Cashier 
                {
                    AccountId = applicationUserId,
                    BranchId = storeUserDto.BranchId,
                    HardwareStoreId = hardwareStoreId,
                    Name = $"{storeUserDto.FirstName} {storeUserDto.LastName}"
                });
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var cashier = await _cashierRepository.GetByAccountId(accountId);
            var user = new UserInformationDto
            {
                AccountId = cashier.AccountId,
                BranchId = cashier.BranchId,
                Role = UserRole.CASHIER,
                HardwareStoreId = cashier.HardwareStoreId,
                Id = cashier.Id,
                Name = cashier.Name
            };
            return user;
        }
    }
}