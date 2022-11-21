using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        //private readonly ILogger<FuncionarioController> _logger;
        private static List<Funcionario>? _funcionarios;

        public FuncionarioController()// ILogger<FuncionarioController> logger
        {
            //_logger = logger;

            if (_funcionarios == null)
            {
                _funcionarios = new() {
                    new() { Id = 1, Nome = "Allbert", Cpf = "000.000.000-01", Matricula = "000001/1", DataVinculo = new DateOnly(2021, 1, 1) },
                    new() { Id = 2, Nome = "Anderson", Cpf = "000.000.000-02", Matricula = "000002/1", DataVinculo = new DateOnly(2021, 1, 2) },
                    new() { Id = 3, Nome = "Norberto", Cpf = "000.000.000-03", Matricula = "000003/1", DataVinculo = new DateOnly(2021, 1, 3) },
                    new() { Id = 4, Nome = "Rosangela", Cpf = "000.000.000-04", Matricula = "000004/1", DataVinculo = new DateOnly(2021, 1, 4) }
                };
            }
        }

        [HttpGet]
        public IEnumerable<Funcionario> Get()
        {
            return _funcionarios!;
        }

        [HttpGet]
        public ActionResult<Funcionario> Get(int id)
        {
            var funcionario = _funcionarios!.FirstOrDefault(f => f.Id == id);

            if (funcionario == null)
                return NotFound();

            return funcionario;
        }

        [HttpPost]
        public ActionResult Post(Funcionario funcionario)
        {
            var index = -1;
            if (funcionario.Id > 0)
                index = _funcionarios!.FindIndex(f => f.Id == funcionario.Id);

            if (index < 0) // create
            {
                funcionario.Id = _funcionarios!.Max(f => f.Id) + 1;
                _funcionarios!.Add(funcionario);
            }
            else // update
            {
                _funcionarios![index] = funcionario;
            }
            
            return Ok(new { funcionario.Id });
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var index = _funcionarios!.FindIndex(f => f.Id == id);

            if (index < 0)
                return NotFound();

            _funcionarios.RemoveAt(index);

            return Ok();
        }
    }
}