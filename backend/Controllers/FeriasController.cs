using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeriasController : ControllerBase
    {
        private readonly IFeriasRepository _repository;

        public FeriasController(IFeriasRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FeriasQuery query)
        {
            var (status, result) = await _repository.Read(query);
            return StatusCode((int)status, result);
        }

    }
}