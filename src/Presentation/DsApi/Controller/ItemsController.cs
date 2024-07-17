using Ds.Application;
using DsApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DsApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : BaseController
    {
        private readonly ICommonService _service;

        public ItemsController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ResponseDto> SearchItems(string? term)
        {
            _response.Result = await _service.GetItemsAsync(term);

            return _response;
        }
    }
}
