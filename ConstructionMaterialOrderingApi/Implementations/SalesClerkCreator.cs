using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class SalesClerkCreator : IHardwareStoreUser
    {
        private readonly IBranchUserRepository<SalesClerk> _salesClerkRepository;
        public SalesClerkCreator(IBranchUserRepository<SalesClerk> salesClerkRepository)
        {
            _salesClerkRepository = salesClerkRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _salesClerkRepository.Add(new SalesClerk
                {
                    AccountId = applicationUserId,
                    BranchId = storeUserDto.BranchId,
                    HardwareStoreId = hardwareStoreId,
                    Name = storeUserDto.FirstName + " " + storeUserDto.LastName
                });
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var salesClerk = await _salesClerkRepository.GetByAccountId(accountId);
            var user = new UserInformationDto
            {
                Id = salesClerk.Id,
                BranchId = salesClerk.BranchId,
                AccountId = salesClerk.AccountId,
                HardwareStoreId = salesClerk.HardwareStoreId,
                Role = UserRole.SALES_CLERK,
                Name = salesClerk.Name
            };
            return user;
        }
    }
}