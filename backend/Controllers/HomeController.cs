using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : ControllerBase
    {
        public HomeController() { }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("error")]
        public IActionResult HandleError([FromServices] IHostEnvironment environment)
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            if (environment.IsDevelopment())
            {
                return Problem(
                    detail: exceptionHandlerFeature.Error.StackTrace,
                    title: exceptionHandlerFeature.Error.Message);
            }

            return Problem();//Problem(detail: exceptionHandlerFeature.Error.Message);
        }
    }
}
