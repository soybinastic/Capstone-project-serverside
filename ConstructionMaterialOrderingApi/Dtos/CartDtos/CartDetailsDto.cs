using System;
using System.Collections.Generic;

namespace ConstructionMaterialOrderingApi.Dtos.CartDtos
{
    public class CartDetailsDto
    {
        public double ShippingFee { get; set; }
        public List<GetProductToCartDto> CartItems { get; set; }

        public CartDetailsDto(double shippingFee, List<GetProductToCartDto> cartItems)
        {
            ShippingFee = shippingFee;
            CartItems = cartItems;
        }
    }
}