using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioRepository _repository;

        public FuncionarioController(IFuncionarioRepository funcionarios)
        {
            _repository = funcionarios;
        }

        [HttpGet]
        public async Task<IActionResult> Get(ConsultaFuncionario filter)
        {
            var (status, result) = await _repository.Read(filter);
            return StatusCode((int)status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var (status, result) = await _repository.Read(id);
            return StatusCode((int)status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Funcionario funcionario)
        {
            var (status, result) = await _repository.CreateOrUpdate(funcionario);
            
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