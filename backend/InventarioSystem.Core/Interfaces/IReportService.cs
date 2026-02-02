namespace InventarioSystem.Core.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateLowStockReportPdfAsync();
        Task<byte[]> GenerateInventoryReportPdfAsync();
    }
}
