using System;
using System.Collections.Generic;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class SummarySaleDto
    {
        public DateTime MonthSale { get; set; }
        public List<TotalSaleDto> DailySales { get; set; }
        public double TotalMonthSales { get; set; }

        public SummarySaleDto(List<TotalSaleDto> dailySales, DateTime monthSale, double totalMonthSales)
        {
            DailySales = new List<TotalSaleDto>();

            DailySales.AddRange(dailySales);
            MonthSale = monthSale;
            TotalMonthSales = totalMonthSales;
        }
        public SummarySaleDto()
        {
            DailySales = new List<TotalSaleDto>();
        }

    }
}
