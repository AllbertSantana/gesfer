using backend.Models;
using backend.Services;
using backend.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mime;

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
        [Description("Buscar Funcion�rio")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioRow>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Description("Listar Funcion�rios")]
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
        [Description("Baixar Planilha de Funcion�rios")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Type = typeof(FileResult))]
        public async Task<FileResult> Download([FromQuery] FuncionarioQuery query)
        {
            var name = "Funcion�rios";
            var result = await _repository.Read(query);
            var content = await Export.ToSpreadsheet(result.Items, name);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{name}.xlsx");
        }

        [HttpDelete("{id}")]
        [Description("Remover Funcion�rio")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FuncionarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ? Ok(result) : NotFound();
        }

        [HttpDelete]
        [Description("Remover Funcion�rios")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FuncionarioRow>>> Delete([FromQuery] int[] id)
        {
            var result = await _repository.Delete(id);
            return Ok(result);
        }

        [HttpPost]
        [Description("Criar Funcion�rio")]
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
        [Description("Editar Funcion�rio")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

            return (result != null) ? Ok(result) : NotFound();
        }
    }
}