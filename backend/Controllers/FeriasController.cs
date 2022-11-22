using backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeriasController : ControllerBase
    {
        public FeriasController()
        {
        }
        
        [HttpGet]
        public Task<IActionResult> Get(ConsultaFerias filter)
        {
            throw new NotImplementedException();
            /*
            var feriasQuery = (new List<Ferias>())
                .Where(ferias => ferias.DataInicio >= filter.Arguments.InicioFerias)
                .Where(ferias => ferias.DataFim <= filter.Arguments.FimFerias)
                .Where(ferias => ferias.Exercicio.DataInicio >= filter.Arguments.InicioExercicio)
                .Where(ferias => ferias.Exercicio.DataFim <= filter.Arguments.FimExercicio)
                .Where(ferias => ferias.Exercicio.Funcionario.Matricula == filter.Arguments.Matricula)
                .Where(ferias => ferias.Exercicio.Funcionario.Nome == filter.Arguments.Nome);

            //var exerciciosQuery = (new List<ExercicioDto>())
            //    .Where(exercicio => exercicio.Saldo >= filter.Arguments.Saldo);
            */
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}