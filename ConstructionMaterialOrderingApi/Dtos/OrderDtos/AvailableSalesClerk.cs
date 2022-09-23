using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class AvailableSalesClerk
    {
        public SalesClerk SalesClerk { get; set; }
        public bool IsAvailable { get; set; }
    }
}