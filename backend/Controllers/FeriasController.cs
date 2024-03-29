using backend.Models;
using backend.Services;
using backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet("funcionario/{funcionarioId}/exercicio/{exercicioId}")]
        [SwaggerOperation("Buscar F�rias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Permission(Permissao.BuscarFerias)]
        public async Task<ActionResult<FeriasGroupByExercicio>> Get(int funcionarioId, int exercicioId)
        {
            var result = await _repository.Read(funcionarioId, exercicioId);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Per�odo aquisitivo n�o encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet("funcionario")]
        [SwaggerOperation("Listar F�rias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Permission(Permissao.ListarFerias)]
        public async Task<ActionResult<FeriasGroupByFuncionario>> Get([FromQuery] FeriasFilter filter)
        {
            var result = await _repository.Read(filter);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Funcion�rio n�o encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet("funcionario/{id}/planilha")]
        [SwaggerOperation("Baixar Planilha de F�rias")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Type = typeof(FileResult))]
        [Permission(Permissao.BaixarPlanilhaFerias)]
        public async Task<FileResult> Download(int id)
        {
            var sheetName = "F�rias";
            var content = await _repository.Download(id, sheetName);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{sheetName} do Funcion�rio {id}.xlsx");
        }

        [HttpDelete("funcionario/{funcionarioId}/exercicio/{exercicioId}")]
        [SwaggerOperation("Remover F�rias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Permission(Permissao.RemoverFerias)]
        public async Task<ActionResult<FuncionarioRow>> Delete(int funcionarioId, int exercicioId)
        {
            var result = await _repository.Delete(funcionarioId, exercicioId);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Per�odo aquisitivo n�o encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpPost("funcionario/{id}")]
        [SwaggerOperation("Criar F�rias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Permission(Permissao.CriarFerias)]
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

            //return (result != null) ? CreatedAtAction(nameof(Get), new { id = result!.Id }, result) : Problem(detail: "Funcion�rio n�o existe", statusCode: StatusCodes.Status404NotFound);
            return CreatedAtAction(nameof(Get), new { id = result!.Id }, result);
        }

        [HttpPut("funcionario/{funcionarioId}/exercicio/{exercicioId}")]
        [SwaggerOperation("Editar F�rias")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Permission(Permissao.EditarFerias)]
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

            //return (result != null) ? Ok(result) : Problem(detail: "Per�odo aquisitivo n�o existe", statusCode: StatusCodes.Status404NotFound);
            return Ok(result);
        }
    }
}