using DsApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DsApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        public readonly ResponseDto _response;
        public BaseController()
        {
            _response = new ResponseDto
            {
                IsSuccess = true
            };
        }
    }
}
