using System;
using ConstructionMaterialOrderingApi.Dtos.CustomerDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Helpers
{
    public static class ShippingFee
    {
        public static double Get(GetCustomerDto customer, Branch branch)
        {
            var kmRoundValue = Math.Round(Coordinates.DistanceBetweenPlaces(customer.Longitude, customer.Latitude, 
                branch.Longitude, branch.Latitude));
            Console.WriteLine($"Distance KM: {kmRoundValue}");    
            return kmRoundValue > 1 ? (kmRoundValue * 10) : 0;
        }
    }
}