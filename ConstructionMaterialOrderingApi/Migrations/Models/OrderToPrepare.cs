namespace ConstructionMaterialOrderingApi.Models
{
    public class OrderToPrepare
    {
        public int Id { get; set; }
        public int SalesClerkId { get; set; }
        public SalesClerk SalesClerk { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}