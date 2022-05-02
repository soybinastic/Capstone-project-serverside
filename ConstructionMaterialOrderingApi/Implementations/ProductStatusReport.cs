using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class ProductStatusReport : IProductStatusReport
    {
        private readonly IWarehouseReportRepository _reportRepository;
        private readonly IProductStatusReportRepository _statusReportRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public ProductStatusReport(IWarehouseReportRepository reportRepository,
            IProductStatusReportRepository statusReportRepository, 
            INotificationRepository notificationRepository,
            IWarehouseRepository warehouseRepository)
        {
            _reportRepository = reportRepository;
            _statusReportRepository = statusReportRepository;
            _notificationRepository = notificationRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<(bool, string)> Add(AddProductStatusReportDto statusReportDto, int hardwareStoreId, int warehouseId)
        {
            if (statusReportDto.ReportDetail.ReportType == "ProductStatusReport")
            {
                var createdReport = await _reportRepository
                    .Add(statusReportDto.ReportDetail, hardwareStoreId, warehouseId);

                foreach (var report in statusReportDto.ProductStatusReports)
                {
                    await _statusReportRepository.Add(report, createdReport.Id);
                }

                var warehouses = await _warehouseRepository.GetWarehouses(hardwareStoreId);
                var warehouseAddingReport = await _warehouseRepository.GetWarehouse(warehouseId, hardwareStoreId);

                StringBuilder notificationText = new StringBuilder(warehouseAddingReport.Name);
                notificationText.Append(" added new product status report. Please check ");
                foreach(var warehouse in warehouses)
                {
                    if(warehouse.Id != warehouseId)
                    {
                        await _notificationRepository.PushWarehouseNotification(warehouse.Id, notificationText.ToString(), "ProductStatus");
                    }
                }

                return (true, "Added Successfully.");
            }

            return (false, "Cannot identify report type.");
        }
        public async Task<List<WarehouseReport>> GetReports(int hardwareStoreId)
        {
            var reports = await _reportRepository.GetWarehouseReports(hardwareStoreId, "ProductStatusReport");
            return reports;
        }

        public async Task<List<WarehouseProductStatusReport>> GetProductStatusReports(int warehouseReportId)
        {
            var statusReports = await _statusReportRepository.GetWarehouseProductStatusReports(warehouseReportId);
            return statusReports;
        }

        public async Task RemoveStatusReport(int statusReportId)
        {
            await _statusReportRepository.Remove(statusReportId);
        }

        public async Task AddProductStatusReport(int warehouseReportId, ProductStatusReportDto statusReportDto)
        {
            await _statusReportRepository.Add(statusReportDto, warehouseReportId);
        }

        public async Task<WarehouseReport> GetReport(int warehouseReportId)
        {
            var report = await _reportRepository
                .GetWarehouseReport(warehouseReportId, "ProductStatusReport");
            return report;

        }

        public async Task RemoveReport(int warehouseReportId)
        {
            var statusReports = await _statusReportRepository
                .GetWarehouseProductStatusReports(warehouseReportId); 

            if(statusReports != null)
            {
                foreach(var statusReport in statusReports)
                {
                    await _statusReportRepository.Remove(statusReport.Id);
                }
            }

            await _reportRepository.RemoveReport(warehouseReportId);
        }
    }
}
