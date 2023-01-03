using backend.Models;
using backend.Services;
using backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mime;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioRepository _repository;

        public FuncionarioController(IFuncionarioRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Buscar Funcionário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioRow>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Funcionário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [SwaggerOperation("Listar Funcionários")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(FuncionarioResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioResult>> Get([FromQuery] FuncionarioQuery query)
        {
            var result = await _repository.Read(query);
            //return (result.RowCount > 0) ? Ok(result) : NotFound(result);
            return Ok(result);
        }

        [HttpGet("planilha")]
        [SwaggerOperation("Baixar Planilha de Funcionários")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<QueryFileResult> Download([FromQuery] FuncionarioQuery query)
        {
            var sheetName = "Funcionários";
            var result = await _repository.Download(query, sheetName);
            result.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            result.FileName = string.Format("{0}{1}.xlsx", sheetName, (result.PageCount > 1) ? $" (parte {result.PageNumber} de {result.PageCount})" : string.Empty);
            return result;
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Remover Funcionário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Funcionário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpDelete]
        [SwaggerOperation("Remover Funcionários")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FuncionarioRow>>> Delete([FromQuery] int[] id)
        {
            var result = await _repository.Delete(id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation("Criar Funcionário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FuncionarioRow>> Post(FuncionarioForm form)
        {
            form.Id = 0;
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

        [HttpPut("{id}")]
        [SwaggerOperation("Editar Funcionário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FuncionarioRow>> Put(int id, [FromBody] FuncionarioForm form)
        {
            form.Id = id;
            var (result, errors) = await _repository.Update(form);

            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                    ModelState.AddModelError(error.Key, message);
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            //return (result != null) ? Ok(result) : Problem(detail: "Funcionário não existe", statusCode: StatusCodes.Status404NotFound);
            return Ok(result);
        }
    }
}