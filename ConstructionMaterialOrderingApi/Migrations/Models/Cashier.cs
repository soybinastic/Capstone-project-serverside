namespace ConstructionMaterialOrderingApi.Models
{
    public class Cashier
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public int HardwareStoreId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
    }
}