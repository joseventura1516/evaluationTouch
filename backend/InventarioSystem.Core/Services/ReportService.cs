using InventarioSystem.Core.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualBasic;

namespace InventarioSystem.Core.Services
{
    
    public class ReportService : IReportService
    {
        
        private readonly IProductRepository _productRepository;

        public ReportService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<byte[]> GenerateLowStockReportPdfAsync()
        {
            
            var lowStockProducts = await _productRepository.GetLowStockProductsAsync(5);

            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            writer.PageEvent = new PageNummberEvent();
            document.Open();

            // Título
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DarkGray);
            var title = new Paragraph("Reporte de Productos con Inventario Bajo", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Fecha de generación
            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Gray);
            var date = new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 20
            };
            document.Add(date);

            // Tabla
            var table = new PdfPTable(6)
            {
                WidthPercentage = 100,
                SpacingBefore = 10
            };
            table.SetWidths(new float[] { 3f, 4f, 2f, 2f, 2f,2f });

            // Encabezados
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.White);
            var headerColor = new BaseColor(52, 73, 94);

            AddHeaderCell(table, "Nombre", headerFont, headerColor);
            AddHeaderCell(table, "Categoría", headerFont, headerColor);
            AddHeaderCell(table, "Precio", headerFont, headerColor);
            AddHeaderCell(table, "Cantidad", headerFont, headerColor);
            AddHeaderCell(table, "Estado", headerFont, headerColor);
            AddHeaderCell(table, "Fecha", headerFont, headerColor);
            
            // Datos
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Black);
            var warningColor = new BaseColor(231, 76, 60);
            var alternateColor = new BaseColor(245, 245, 245);

            int rowIndex = 0;
            foreach (var product in lowStockProducts)
            {
                var bgColor = rowIndex % 2 == 0 ? BaseColor.White : alternateColor;

                AddCell(table, product.Name, cellFont, bgColor);
                AddCell(table, product.Category, cellFont, bgColor);
                AddCell(table, $"${product.Price:N2}", cellFont, bgColor);

                // Celda de cantidad con color de advertencia
                var quantityCell = new PdfPCell(new Phrase(product.Quantity.ToString(), cellFont))
                {
                    BackgroundColor = warningColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 8
                };
                table.AddCell(quantityCell);

                AddCell(table, "Bajo", cellFont, bgColor);
                AddCell(table, product.CreatedAt.ToString("dd/MM/yyyy"), cellFont, bgColor);
                rowIndex++;
            }

            document.Add(table);

            // Resumen
            var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DarkGray);
            var summary = new Paragraph($"\nTotal de productos con inventario bajo: {lowStockProducts.Count()}", summaryFont)
            {
                SpacingBefore = 20
            };
            document.Add(summary);
            
            document.Close();
            writer.Close();

            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateInventoryReportPdfAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();

            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            writer.PageEvent = new PageNummberEvent();
            document.Open();

            // Título
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DarkGray);
            var title = new Paragraph("Reporte General de Inventario", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Fecha de generación
            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Gray);
            var date = new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 20
            };
            document.Add(date);

            // Tabla
            var table = new PdfPTable(7)
            {
                WidthPercentage = 100,
                SpacingBefore = 10
            };
            table.SetWidths(new float[] { 3f, 3f, 2f, 2f, 2f, 2f,2f });

            // Encabezados
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.White);
            var headerColor = new BaseColor(52, 73, 94);

            AddHeaderCell(table, "Nombre", headerFont, headerColor);
            AddHeaderCell(table, "Descripción", headerFont, headerColor);
            AddHeaderCell(table, "Categoría", headerFont, headerColor);
            AddHeaderCell(table, "Precio", headerFont, headerColor);
            AddHeaderCell(table, "Cantidad", headerFont, headerColor);
            AddHeaderCell(table, "Estado", headerFont, headerColor);
            AddHeaderCell(table, "Fecha", headerFont, headerColor);

            // Datos
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.Black);
            var warningColor = new BaseColor(231, 76, 60);
            var normalColor = new BaseColor(46, 204, 113);
            var alternateColor = new BaseColor(245, 245, 245);

            int rowIndex = 0;
            foreach (var product in allProducts)
            {
                var bgColor = rowIndex % 2 == 0 ? BaseColor.White : alternateColor;
                var isLowStock = product.Quantity < 5;

                AddCell(table, product.Name, cellFont, bgColor);
                AddCell(table, TruncateText(product.Description, 30), cellFont, bgColor);
                AddCell(table, product.Category, cellFont, bgColor);
                AddCell(table, $"${product.Price:N2}", cellFont, bgColor);

                var quantityCell = new PdfPCell(new Phrase(product.Quantity.ToString(), cellFont))
                {
                    BackgroundColor = isLowStock ? warningColor : bgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 8
                };
                table.AddCell(quantityCell);

                var statusCell = new PdfPCell(new Phrase(isLowStock ? "Bajo" : "Normal", cellFont))
                {
                    BackgroundColor = isLowStock ? warningColor : normalColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 8
                };
                table.AddCell(statusCell);
                AddCell(table, product.CreatedAt.ToString("dd/MM/yyyy"), cellFont, bgColor);
                rowIndex++;
            }

            document.Add(table);

            // Resumen
            var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DarkGray);
            var totalProducts = allProducts.Count();
            var lowStockCount = allProducts.Count(p => p.Quantity < 5);
            var totalValue = allProducts.Sum(p => p.Price * p.Quantity);

            document.Add(new Paragraph($"\nResumen:", summaryFont) { SpacingBefore = 20 });
            document.Add(new Paragraph($"Total de productos: {totalProducts}", cellFont));
            document.Add(new Paragraph($"Productos con inventario bajo: {lowStockCount}", cellFont));
            document.Add(new Paragraph($"Valor total del inventario: ${totalValue:N2}", cellFont));
            
            document.Close();
            writer.Close();

            return memoryStream.ToArray();
        }

        private static void AddHeaderCell(PdfPTable table, string text, Font font, BaseColor bgColor)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = bgColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 10
            };
            table.AddCell(cell);
        }

        private static void AddCell(PdfPTable table, string text, Font font, BaseColor bgColor)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = bgColor,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 8
            };
            table.AddCell(cell);
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength - 3) + "...";
        }
    }

    public class PageNummberEvent : PdfPageEventHelper
    {
        private PdfContentByte cb;
        private BaseFont bf;
        private int pageNummber;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            bf=BaseFont.CreateFont(BaseFont.HELVETICA,BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent
;           //base.OnOpenDocument(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            pageNummber= writer.PageNumber;
            string text = $"Pagina {pageNummber}";
            float x = document.PageSize.Width - 100;
            float y = document.PageSize.GetBottom(30);

            cb.BeginText();
            cb.SetFontAndSize(bf,10);
            cb.ShowTextAligned(Element.ALIGN_RIGHT,text,x,y,0);
            cb.EndText();
            //base.OnEndPage(writer, document);
        }
    }
}
