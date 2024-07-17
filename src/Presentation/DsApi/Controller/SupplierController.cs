using Ds.Application;
using DsApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DsApi.Controller
{
    public class SupplierController : BaseController
    {
        private readonly ICommonService _service;

        public SupplierController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ResponseDto> SearchSupplier(string? term)
        {
            _response.Result = await _service.GetSuppliersAsync(term);

            return _response;
        }
    }
}
