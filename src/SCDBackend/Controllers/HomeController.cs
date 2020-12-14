using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace SCDBackend.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Running";
        }

    }
}
