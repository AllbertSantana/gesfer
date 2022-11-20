using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        //private readonly ILogger<FuncionarioController> _logger;

        public FuncionarioController()// ILogger<FuncionarioController> logger
        {
            //_logger = logger;
        }

        [HttpGet]
        public IEnumerable<Funcionario> Get()
        {
            return new[] {
                new Funcionario { Id = 1, Nome = "Allbert", Cpf = "000.000.000-01", Matricula = "000001/1", DataVinculo = new DateOnly(2021, 1, 1) },
                new Funcionario { Id = 2, Nome = "Anderson", Cpf = "000.000.000-02", Matricula = "000002/1", DataVinculo = new DateOnly(2021, 1, 2) },
                new Funcionario { Id = 3, Nome = "Norberto", Cpf = "000.000.000-03", Matricula = "000003/1", DataVinculo = new DateOnly(2021, 1, 3) },
                new Funcionario { Id = 4, Nome = "Rosangela", Cpf = "000.000.000-04", Matricula = "000004/1", DataVinculo = new DateOnly(2021, 1, 4) }
            };
        }
    }
}