using Microsoft.AspNetCore.Mvc;
using Ds.Application;
using QuestPDF.Fluent;

namespace DsApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ICommonService _service;

        public ReportsController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetPurchaseOrderPdf(int id)
        {
            var purchaseOrder = await _service.GetPurchaseOrderById(id);
            if (purchaseOrder == null)
                return NotFound();

            var document = new PurchaseOrderDocument(purchaseOrder);
            var pdfBytes = document.GeneratePdf();
            //return File(pdfBytes, "application/pdf", $"PurchaseOrder_{purchaseOrder.OrderNumber}.pdf");
            var base64String = Convert.ToBase64String(pdfBytes);

            return Ok(new { PdfContent = base64String });
        }
    }
}
