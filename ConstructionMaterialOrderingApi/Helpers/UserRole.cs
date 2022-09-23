using System.Collections.Generic;
namespace ConstructionMaterialOrderingApi.Helpers
{
    public class UserRole
    {
        public const string ADMIN = "Admin";
        public const string CUSTOMER = "Customer";
        public const string TRANSPORT_AGENT = "TransportAgent";
        public const string STORE_OWNER = "StoreOwner";
        public const string STORE_ADMIN = "StoreAdmin";
        public const string WAREHOUSE_ADMIN = "WarehouseAdmin";
        public const string SUPER_ADMIN = "SuperAdmin";
        public const string CASHIER = "Cashier";
        public const string SALES_CLERK = "SalesClerk";

        public static List<string> AllUserRoles()
        {
            return new List<string>
                {
                    ADMIN, CUSTOMER, TRANSPORT_AGENT, 
                    STORE_OWNER, STORE_ADMIN,
                    WAREHOUSE_ADMIN, SUPER_ADMIN,
                    CASHIER, SALES_CLERK
                };
        }
    }
}