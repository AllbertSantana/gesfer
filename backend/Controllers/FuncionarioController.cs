using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Net;

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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FuncionarioQuery query)
        {
            var (status, result) = await _repository.Read(query);
            return StatusCode((int)status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var (status, result) = await _repository.Read(id);
            return StatusCode((int)status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(FuncionarioForm form)
        {
            var (status, result) = await _repository.CreateOrUpdate(form);

            if (status == HttpStatusCode.BadRequest)
            {
                foreach (var error in (Dictionary<string, string[]>)result!)
                {
                    foreach (var message in error.Value)
                        ModelState.AddModelError(error.Key, message);
                }
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);
            }

            if (status == HttpStatusCode.Created)
                return CreatedAtAction(nameof(Get), new { id = ((Funcionario)result!).Id }, result);
            return StatusCode((int)status, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (status, result) = await _repository.Delete(id);
            return StatusCode((int)status, result);
        }
    }
}