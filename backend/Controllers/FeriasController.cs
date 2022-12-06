using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
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

        [HttpGet("exercicio/{id}")]
        [Description("Buscar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeriasGroupByExercicio>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Período aquisitivo não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [Description("Listar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FeriasResult>> Get([FromQuery] FeriasQuery query)
        {
            var result = await _repository.Read(query);
            return Ok(result);
        }

        [HttpDelete("exercicio/{id}")]
        [Description("Remover Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Período aquisitivo não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpPost("funcionario/{id}")]
        [Description("Criar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FeriasGroupByExercicio>> Post(int id, [FromBody] ExercicioForm form)
        {
            form.Id = 0;
            form.FuncionarioId = id;
            var (result, errors) = await _repository.Create(form);

            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                    ModelState.AddModelError(error.Key, message);
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            //return (result != null) ? CreatedAtAction(nameof(Get), new { id = result!.Id }, result) : Problem(detail: "Funcionário não existe", statusCode: StatusCodes.Status404NotFound);
            return CreatedAtAction(nameof(Get), new { id = result!.Id }, result);
        }

        [HttpPut("funcionario/{funcionarioId}/exercicio/{exercicioId}")]
        [Description("Editar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FeriasGroupByExercicio>> Put(int funcionarioId, int exercicioId, [FromBody] ExercicioForm form)
        {
            form.Id = exercicioId;
            form.FuncionarioId = funcionarioId;
            var (result, errors) = await _repository.Update(form);

            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                    ModelState.AddModelError(error.Key, message);
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            //return (result != null) ? Ok(result) : Problem(detail: "Período aquisitivo não existe", statusCode: StatusCodes.Status404NotFound);
            return Ok(result);
        }
    }
}