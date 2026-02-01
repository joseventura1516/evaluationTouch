using InventarioSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventarioSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("low-stock-pdf")]
        public async Task<IActionResult> GenerateLowStockReport()
        {
            try
            {
                var pdfBytes = await _reportService.GenerateLowStockReportAsync();

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return BadRequest(new { message = "No se pudo generar el reporte" });

                return File(pdfBytes, "application/pdf", $"Reporte_Inventario_Bajo_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte PDF");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
