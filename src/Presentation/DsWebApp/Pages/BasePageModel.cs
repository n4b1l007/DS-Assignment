using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DsWebApp.Pages
{
    public class BasePageModel : PageModel
    {
        public string ApiBaseUrl { get; private set; }
        protected readonly IConfiguration Configuration;

        public BasePageModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            ViewData["ApiBaseUrl"] = Configuration["AppSettings:ApiBaseUrl"]; // Assuming Configuration is injected or accessible
            base.OnPageHandlerExecuting(context);
        }
    }

}
