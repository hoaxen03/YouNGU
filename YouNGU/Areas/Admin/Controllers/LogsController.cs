using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using YouNGU.Areas.Admin.Models;
using YouNGU.Repositories;
using ClosedXML.Excel;
using System.IO;
using System.Drawing;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class LogsController : Controller
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogRepository _logRepository;

        public LogsController(ILogger<LogsController> logger, ILogRepository logRepository)
        {
            _logger = logger;
            _logRepository = logRepository;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(
            string category = null,
            string level = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null,
            int page = 1)
        {
            const int pageSize = 20;

            // Lấy dữ liệu nhật ký từ repository
            var logs = await _logRepository.GetLogsAsync(
                level,
                category,
                fromDate,
                toDate,
                searchTerm,
                page,
                pageSize);

            // Lấy tổng số nhật ký để tính số trang
            var totalLogs = await _logRepository.GetLogsCountAsync(
                level,
                category,
                fromDate,
                toDate,
                searchTerm);

            // Lấy danh sách các loại nhật ký và cấp độ
            var categories = await _logRepository.GetLogCategoriesAsync();
            var levels = await _logRepository.GetLogLevelsAsync();

            // Tạo view model
            var viewModel = new LogsViewModel
            {
                Logs = logs.Select(l => new LogViewModel
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Level = l.Level,
                    Message = l.Message,
                    Category = l.Category,
                    UserId = l.UserId,
                    Exception = l.Exception,
                    AdditionalInfo = l.AdditionalInfo,
                    RequestPath = l.RequestPath,
                    RequestMethod = l.RequestMethod,
                    SourceContext = l.SourceContext,
                    ActionName = l.ActionName,
                    ApplicationName = l.ApplicationName,
                    MachineName = l.MachineName
                }),
                Category = category,
                Level = level,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
                TotalLogs = totalLogs,
                Categories = categories.ToList(),
                Levels = levels.ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // Lấy chi tiết nhật ký từ repository
            var log = await _logRepository.GetLogByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            // Tạo view model
            var viewModel = new LogViewModel
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Message = log.Message,
                Category = log.Category,
                UserId = log.UserId,
                Exception = log.Exception,
                AdditionalInfo = log.AdditionalInfo,
                RequestPath = log.RequestPath,
                RequestMethod = log.RequestMethod,
                SourceContext = log.SourceContext,
                ActionName = log.ActionName,
                ApplicationName = log.ApplicationName,
                MachineName = log.MachineName
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Clear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear(string category)
        {
            // Xóa nhật ký từ repository
            await _logRepository.ClearLogsAsync(category);

            TempData["SuccessMessage"] = $"Đã xóa nhật ký {(string.IsNullOrEmpty(category) ? "tất cả" : category)} thành công.";
            return RedirectToAction(nameof(Index), new { category });
        }

        [HttpPost]
        [Route("DeleteLog/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLog(int id)
        {
            // Xóa một nhật ký cụ thể từ repository
            await _logRepository.DeleteLogAsync(id);

            TempData["SuccessMessage"] = $"Đã xóa nhật ký #{id} thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Export")]
        public async Task<IActionResult> Export(
            string category = null,
            string level = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null)
        {
            // Lấy tất cả log phù hợp với điều kiện lọc (không phân trang)
            var logs = await _logRepository.GetLogsAsync(level, category, fromDate, toDate, searchTerm, 1, int.MaxValue);

            // Tạo file Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Logs");

                // Tạo header
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Thời gian";
                worksheet.Cell(1, 3).Value = "Cấp độ";
                worksheet.Cell(1, 4).Value = "Loại";
                worksheet.Cell(1, 5).Value = "Nội dung";
                worksheet.Cell(1, 6).Value = "Người dùng";
                worksheet.Cell(1, 7).Value = "Đường dẫn";
                worksheet.Cell(1, 8).Value = "Phương thức";
                worksheet.Cell(1, 9).Value = "Nguồn";

                // Định dạng header
                var headerRange = worksheet.Range(1, 1, 1, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Điền dữ liệu
                int row = 2;
                foreach (var log in logs)
                {
                    worksheet.Cell(row, 1).Value = log.Id;
                    worksheet.Cell(row, 2).Value = log.Timestamp;
                    worksheet.Cell(row, 3).Value = log.Level;
                    worksheet.Cell(row, 4).Value = log.Category;
                    worksheet.Cell(row, 5).Value = log.Message;
                    worksheet.Cell(row, 6).Value = log.UserId;
                    worksheet.Cell(row, 7).Value = log.RequestPath;
                    worksheet.Cell(row, 8).Value = log.RequestMethod;
                    worksheet.Cell(row, 9).Value = log.SourceContext;

                    row++;
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Columns().AdjustToContents();

                // Tạo file Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    // Tạo tên file
                    string fileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}