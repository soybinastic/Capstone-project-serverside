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
using System.Data;
using System.Text;

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
        [HttpGet]
        [Route("download/{id}")]
        // [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> Download([FromRoute]int id, [FromQuery]string order_ref)
        {
            var recipient = await _db.Recipients.Where(r => r.Id == id)
                .Include(r => r.RecipientItems)
                .ThenInclude(r => r.HardwareProduct)
                .Include(r => r.Branch)
                .Include(r => r.Customer)
                .Include(r => r.Order)
                .FirstOrDefaultAsync();
            // PdfDocument doc = new PdfDocument();
            // PdfPage page = doc.Pages.Add();
            // PdfGraphics g = page.Graphics;
            // PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            // PdfGrid pdfGrid = new PdfGrid();

            // g.DrawString("Order Reference", font, PdfBrushes.Black, new PointF(0, 0));
            // pdfGrid.DataSource = recipient.RecipientItems;
            // pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));

            // MemoryStream stream = new MemoryStream();
            // doc.Save(stream);
            // stream.Position = 0;
            // doc.Close(true);

            // var file = File(stream, "application/pdf", "order_ref.pdf");

            //return Ok(); 
            var folder = Path.Combine("Resources", "OrderRefsPdf");
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folder);
            var fileName = GenerateRandomFileName();
            var pathToSave = Path.Combine(fullPath, fileName);

            using var document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            document.PageSettings.Margins.All = 50;

            PdfPage page = document.Pages.Add();
            PdfGrid pdfGrid = new PdfGrid();

            // DataTable table = new DataTable();
            // table.Columns.Add("Item Name");
            // table.Columns.Add("Price");
            // table.Columns.Add("Quantity");
            // table.Columns.Add("Total");
            // table.Rows.Add(new string[] { "Item Name", "Price", "Quantity", "Total" });
            // foreach (var item in items)
            // {
            //     table.Rows.Add(new object[] { item.ItemName, item.Price.ToString(), item.Quantity.ToString(), item.Total.ToString() });
            // }

            // pdfGrid.DataSource = table;
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 19, PdfFontStyle.Bold);

            page.Graphics.DrawString("Fastline Hardware", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(186, 0));

            PdfLayoutResult layoutResult = new PdfLayoutResult(page, new Syncfusion.Drawing.RectangleF(0, 0, page.Graphics.ClientSize.Width / 2, 95));
            font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);

            PdfGraphics g = page.Graphics;
            g.DrawRectangle(new PdfSolidBrush(new PdfColor(126, 151, 173)), new Syncfusion.Drawing.RectangleF(0, layoutResult.Bounds.Bottom + 40, g.ClientSize.Width, 30));

            PdfTextElement element = new PdfTextElement($"ORDER REF. NO. : {order_ref}", font);
            element.Brush = PdfBrushes.White;
            layoutResult = element.Draw(page, new Syncfusion.Drawing.PointF(10, layoutResult.Bounds.Bottom + 48));
            string date = "ORDER DATE: " + recipient.Order.OrderDate.ToString("MM/dd/yyyy");
            Syncfusion.Drawing.SizeF dateText = font.MeasureString(date);
            g.DrawString(date, font, element.Brush, new Syncfusion.Drawing.PointF(g.ClientSize.Width - dateText.Width - 10, layoutResult.Bounds.Y));

            element = new PdfTextElement("BILL TO ", new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            layoutResult = element.Draw(page, new Syncfusion.Drawing.PointF(10, layoutResult.Bounds.Bottom + 25));

            var strBuilder = new StringBuilder();
            strBuilder.Append(recipient.Branch.Name).Append(", ").Append(recipient.Branch.Address).AppendLine();
            strBuilder.Append(recipient.Customer.FirstName).Append(", ").Append(recipient.Customer.LastName).AppendLine();

            element = new PdfTextElement(strBuilder.ToString(), new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            layoutResult = element.Draw(page, new Syncfusion.Drawing.RectangleF(10, layoutResult.Bounds.Bottom + 3, g.ClientSize.Width / 2, 100));

            g.DrawLine(new PdfPen(new PdfColor(126, 151, 173), 0.70f), new Syncfusion.Drawing.PointF(0, layoutResult.Bounds.Bottom + 3), new Syncfusion.Drawing.PointF(g.ClientSize.Width, layoutResult.Bounds.Bottom + 3));

            DataTable table = new DataTable();
            table.Columns.Add("Item Name");
            table.Columns.Add("Price");
            table.Columns.Add("Quantity");
            table.Columns.Add("Total");
            // table.Rows.Add(new string[] { "Item Name", "Price", "Quantity", "Total" });
            foreach (var item in recipient.RecipientItems)
            {
                table.Rows.Add(new object[] { item.HardwareProduct.Name, item.HardwareProduct.CostPrice.ToString(), (item.Amount / (double)item.HardwareProduct.CostPrice).ToString(), item.Amount });
            }
            table.Rows.Add(new object[] { "", "", "", "Total of : " + recipient.TotalAmount });

            pdfGrid.DataSource = table;

            PdfGridCellStyle cellStyle = new PdfGridCellStyle();
            cellStyle.Borders.All = PdfPens.White;
            PdfGridRow header = pdfGrid.Headers[0];

            PdfGridCellStyle headerCellStyle = new PdfGridCellStyle();
            headerCellStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
            headerCellStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
            headerCellStyle.TextBrush = PdfBrushes.White;
            headerCellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);

            header.ApplyStyle(headerCellStyle);
            cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
            cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12f);
            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));
            //Creates the layout format for grid
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            // Creates layout format settings to allow the table pagination
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //Draws the grid to the PDF page.
            PdfGridLayoutResult gridResult = pdfGrid.Draw(page, new Syncfusion.Drawing.RectangleF(new Syncfusion.Drawing.PointF(0, layoutResult.Bounds.Bottom + 40), new Syncfusion.Drawing.SizeF(g.ClientSize.Width, g.ClientSize.Height - 100)), layoutFormat);

            if(!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            FileStream stream = new FileStream(pathToSave, FileMode.Create);
            document.Save(stream);
            stream.Close();

            document.Close(true);

            FileStream retrieveFileStream = new FileStream(pathToSave, FileMode.Open);

            return File(retrieveFileStream, "application/octet-stream", "foh_recipient.pdf");
        }

       
        [HttpGet]
        [Route("get-today-sales")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetTodaySales()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var todaySales = await _saleRepository.GetTodaySales(storeAdmin.BranchId);
            Console.WriteLine(todaySales);
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

        private string GenerateRandomFileName()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string time = $"{DateTime.Now.Hour.ToString()}{DateTime.Now.Minute.ToString()}{DateTime.Now.Second.ToString()}{DateTime.Now.Millisecond.ToString()}";
            return $"{year}{month}{day}{time}_fastonline_or.pdf";
        }
    }
}
