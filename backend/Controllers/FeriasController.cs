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
            return (result != null) ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Description("Listar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(FuncionarioResult))]
        public async Task<ActionResult<FeriasResult>> Get([FromQuery] FeriasQuery query)
        {
            var result = await _repository.Read(query);
            //return (result.RowCount > 0) ? Ok(result) : NotFound(result);
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
            return (result != null) ? Ok(result) : NotFound();
        }

        [HttpPost("funcionario/{id}")]
        [Description("Criar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
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

            //if (result == null) return NoContent();
            return CreatedAtAction(nameof(Get), new { id = result!.Id }, result);
        }

        [HttpPut("exercicio/{id}")]
        [Description("Editar Férias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FeriasGroupByExercicio>> Put(int id, [FromBody] ExercicioForm form)
        {
            form.Id = id;
            form.FuncionarioId = 0;// TODO: remodel ExercicioForm to disreguard FuncionarioId so it won't be updated.
            var (result, errors) = await _repository.Update(form);

            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                    ModelState.AddModelError(error.Key, message);
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            return (result != null) ? Ok(result) : NotFound();
        }
    }
}