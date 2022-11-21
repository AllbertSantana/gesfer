using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarios;

        public FuncionarioController(IFuncionarioService funcionarios)
        {
            _funcionarios = funcionarios;
        }

        [HttpGet]
        public async Task<ActionResult<ListagemFuncionario>> Get(ConsultaFuncionario filter)
        {
            return await _funcionarios.Read(filter);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Funcionario>> Get(int id)
        {
            var funcionario = await _funcionarios.Read(id);

            if (funcionario == null)
                return NotFound();

            return funcionario;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Funcionario funcionario)
        {
            var id = await _funcionarios.CreateOrUpdate(funcionario);
            return Ok(new { funcionario.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _funcionarios.Delete(id);
            //if (id < 0) return NotFound();
            return Ok();
        }
    }
}