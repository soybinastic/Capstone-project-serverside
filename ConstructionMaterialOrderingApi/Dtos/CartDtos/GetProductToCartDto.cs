using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CartDtos
{
    public class GetProductToCartDto
    {
        //[JsonPropertyName("cartId")]
        public int CartId { get; set; }
        //[JsonPropertyName("branchId")]
        public int BranchId { get; set; }
        //[JsonPropertyName("hardwareStoreId")]
        public int HardwareStoreId { get; set; }
        //[JsonPropertyName("customerId")]
        public int CustomerId { get; set; }
        //[JsonPropertyName("productId")]
        public int ProductId { get; set; }
        //[JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        //[JsonPropertyName("productName")]
        public string ProductName { get; set; }
        //[JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; }
        //[JsonPropertyName("productBrand")]
        public string ProductBrand { get; set; }
        //[JsonPropertyName("productQuality")]
        public string ProductQuality { get; set; }
        //[JsonPropertyName("productPrice")]
        public double ProductPrice { get; set; }
        //[JsonPropertyName("productQuantity")]
        public int ProductQuantity { get; set; }
    }
}
