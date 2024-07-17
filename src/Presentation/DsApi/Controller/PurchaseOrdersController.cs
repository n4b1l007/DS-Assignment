using Ds.Application;
using Ds.Application.Dto;
using DsApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DsApi.Controller
{
    public class PurchaseOrdersController : BaseController
    {
        private readonly ICommonService _service;

        public PurchaseOrdersController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetPurchaseOrders([FromQuery] int start = 0, [FromQuery] int length = 10, [FromQuery] string search = "", [FromQuery] string orderColumn = "Id", [FromQuery] string orderDir = "dsc")
        {
            var pageIndex = (start / length) + 1;

            var result = await _service.GetPagedPurchaseOrdersAsync(pageIndex, length, search, orderColumn, orderDir);

            var response = new
            {
                draw = pageIndex,
                recordsTotal = result.TotalRecords,
                recordsFiltered = result.TotalRecords,
                data = result.Data
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<PurchaseOrderDto> GetPurchaseOrderById(int id)
        {
            var result = await _service.GetPurchaseOrderById(id);
            return result;
        }

        [HttpPost]
        public async Task<ResponseDto> InsertOrUpdatePurchaseOrderAndDetails([FromBody] PurchaseOrderDto purchaseOrder)
        {
            _response.Result = await _service.InsertOrUpdatePurchaseOrderAndDetailsAsync(purchaseOrder);
            return _response;
        }

        [HttpDelete("{id}")]
        public async Task<ResponseDto> DeletePurchaseOrder(int id)
        {
            var purchaseOrder = await _service.GetPurchaseOrderById(id);
            if (purchaseOrder.Id != null)
            {
                await _service.DeletePurchaseOrder(id);

            }
            _response.Message = $"Purchase Order {purchaseOrder.OrderNumber} Deleted";
            return _response;
        }
    }
}
