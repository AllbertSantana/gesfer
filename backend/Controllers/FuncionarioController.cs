using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Net;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioRepository _repository;

        public FuncionarioController(IFuncionarioRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Description($"Listar Funcionários")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(FuncionarioResult))]
        public async Task<ActionResult<FuncionarioResult>> Get([FromQuery] FuncionarioQuery query)
        {
            var result = await _repository.Read(query);
            //return (result.RowCount > 0) ? Ok(result) : NotFound(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Description($"Buscar Funcionário")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<FuncionarioRow>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Description($"Criar Funcionário")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
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
        [Description("Editar Funcionário")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
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

        [HttpDelete("{id}")]
        [Description($"Remover Funcionário")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<FuncionarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ? Ok(result) : NotFound();
        }

        // TODO: STOP USING STATUS CODE AND DOCUMENT ALL RESPONSES ON SWAGGER

    }
}