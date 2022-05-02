using ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class RecieveProductReport : IRecieveProductReport
    {
        private readonly IWarehouseReportRepository _warehouseReportRepository;
        private readonly IRecieveProductRepository _recieveProductRepository;

        public RecieveProductReport(IWarehouseReportRepository warehouseReportRepository,
            IRecieveProductRepository recieveProductRepository)
        {
            _warehouseReportRepository = warehouseReportRepository;
            _recieveProductRepository = recieveProductRepository;
        }

        public async Task<(bool, string)> Add(AddRecieveProductDto recieveProductDto,
            int hardwareStoreId, int warehouseId)
        {
            string message = "";

            if (recieveProductDto.ReportDetail.ReportType == "RecieveProductReport")
            {
                var newReport = await _warehouseReportRepository.Add(recieveProductDto.ReportDetail, hardwareStoreId, warehouseId);

                foreach (var recieveProduct in recieveProductDto.RecieveProducts)
                {
                    if(recieveProduct.Quantity > 0)
                    {
                        await _recieveProductRepository.Add(recieveProduct, newReport.Id, warehouseId);
                    }
                    else
                    {
                        message = $"Product with id of {recieveProduct.HardwareProductId} have no quantity.";
                    }
                }

                return (true, message);
            }

            message = "System cannot identified the report type.";

            return (false, message);
        }
        public async Task<bool> UpdateRecieveProduct(RecieveProductDto recieveProductDto, int recieveProductId, int warehouseId)
        {
            if(recieveProductDto.Quantity > 0)
            {
                await _recieveProductRepository.Update(recieveProductDto, recieveProductId, warehouseId);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<RecieveProduct>> GetRecieveProducts(int warehouseReportId)
        {
            var recieveProducts = await _recieveProductRepository.GetRecieveProducts(warehouseReportId);
            return recieveProducts;
        }
        public async Task<RecieveProduct> GetRecieveProduct(int recieveProductId)
        {
            var recieveProduct = await _recieveProductRepository.GetRecieveProduct(recieveProductId);
            return recieveProduct;
        }
        public async Task<WarehouseReport> GetReciveProductReport(int warehouseReportId)
        {
            var report = await _warehouseReportRepository.GetWarehouseReport(warehouseReportId, "RecieveProductReport");
            return report;
        }
        public async Task<List<WarehouseReport>> GetReciveProductReports(int warehouseId, int hardwareStoreId)
        {
            var reports = await _warehouseReportRepository.GetWarehouseReportsWithWarehouseId(hardwareStoreId, "RecieveProductReport", warehouseId);
            return reports;
        }

        public async Task<bool> AddRecieveProduct(RecieveProductDto recieveProductDto, int warehouseReportId, int warehouseId)
        {
            if(recieveProductDto.Quantity > 0)
            {
                await _recieveProductRepository.Add(recieveProductDto, warehouseReportId, warehouseId);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
