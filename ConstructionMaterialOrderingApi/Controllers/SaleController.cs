using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ConstructionMaterialOrderingApi.Dtos.SaleDtos;
using ConstructionMaterialOrderingApi.Context;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf.Grid;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly IBestSellingReportRepository _bestSellingReportRepository;
        private readonly IWareHouseAdminRepository _warehouseAdminRepository;
        private readonly ISaleItemRepository _saleItemRepository;
        private readonly ApplicationDbContext _db;
        public SaleController(ISaleRepository saleRepository, IStoreAdminRepository storeAdminRepository, 
            IBestSellingReportRepository bestSellingReportRepository,
            IWareHouseAdminRepository wareHouseAdminRepository,
            ISaleItemRepository saleItemRepository, 
            ApplicationDbContext db)
        {
            _saleRepository = saleRepository;
            _storeAdminRepository = storeAdminRepository;
            _bestSellingReportRepository = bestSellingReportRepository;
            _warehouseAdminRepository = wareHouseAdminRepository;
            _saleItemRepository = saleItemRepository;
            _db = db;
        } 
        // endpoint to be fix soon.
        [HttpPost]
        [Route("download/{id}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> Download([FromRoute]int id)
        {
            var recipient = await _db.Recipients.Where(r => r.Id == id)
                .Include(r => r.RecipientItems)
                .FirstOrDefaultAsync();
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.Pages.Add();
            PdfGraphics g = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            PdfGrid pdfGrid = new PdfGrid();

            g.DrawString("Order Reference", font, PdfBrushes.Black, new PointF(0, 0));
            pdfGrid.DataSource = recipient.RecipientItems;
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            doc.Close(true);

            var file = File(stream, "application/pdf", "order_ref.pdf");

            return Ok(file);
        }

       
        [HttpGet]
        [Route("get-today-sales")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetTodaySales()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var todaySales = await _saleRepository.GetTodaySales(storeAdmin.BranchId);
            return Ok(ConvertToJson(todaySales));
        } 

        [HttpGet]
        [Route("get-sales")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetSales([FromQuery]DateTime date, [FromQuery]string filterBy)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            double totalSales = await _saleRepository.GetSales(storeAdmin.BranchId, date, filterBy);
            return Ok(ConvertToJson(totalSales));
        }

        [HttpGet]
        [Route("get-bestselling-products")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery]DateTime date, [FromQuery]string filterBy)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            var bestSellingProducts = await _saleRepository.GetBestSellingProduct(storeAdmin.BranchId, date, filterBy);
            return Ok(ConvertToJson(bestSellingProducts));
        }

        [HttpGet]
        [Route("get-monthly-totalsales")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetMonthlyTotalSale([FromQuery]int year)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var totalSales = await _saleRepository.GetTotalSales(storeAdmin.BranchId, year);
            return Ok(ConvertToJson(totalSales));
        }

        [HttpPost]
        [Route("add-bestselling-report")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> AddBestSellingReport([FromBody]BestSellingReportDto bestSellingReportDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            await _bestSellingReportRepository.AddReport(bestSellingReportDto, storeAdmin.BranchId);
            return Ok(new { Success = 1, Message = "Successfully added"});
        }

        [HttpGet]
        [Route("get-reports-by-branch")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetReportsByBranch()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var reports = await _bestSellingReportRepository.GetReportByBranchId(storeAdmin.BranchId);
            return Ok(ConvertToJson(reports));
        }

        [HttpGet]
        [Route("get-reports-by-warehouse")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> GetReportsByWarehouse()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdmin = await _warehouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var reports = await _bestSellingReportRepository.GetReportsByWarehouseId(warehouseAdmin.WarehouseId);
            return Ok(ConvertToJson(reports));
        }

        [HttpPost]
        [Route("add-sale-report")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> AddSaleReport([FromBody]SaleReportDto saleReportDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            await _saleRepository.AddSaleReport(saleReportDto, storeAdmin.BranchId);
            return Ok(new { Success = 1, Message = "Added report successfully."});
        }

        [HttpGet]
        [Route("get-sale-reports")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetSaleReports()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var saleReports = await _saleRepository.GetSaleReports(storeAdmin.BranchId);
            return Ok(ConvertToJson(saleReports));
        }

        [HttpGet]
        [Route("get-month-summary-sales")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetMonthSummarySales([FromQuery]DateTime date)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var summarySales = await _saleRepository.GetMonthSalesSummary(storeAdmin.BranchId, date);
            return Ok(ConvertToJson(summarySales));
        }

        [HttpGet]
        [Route("get-sale-item")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetSaleItem([FromQuery]DateTime date, [FromQuery]string filterBy)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var saleItems = await _saleItemRepository.GetSaleItems(storeAdmin.BranchId, date, filterBy);
            return Ok(ConvertToJson(saleItems));
        }

        private string ConvertToJson<T>(T obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return jsonObj;
        }
    }
}
