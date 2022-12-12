using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioController(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [Description("Buscar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioRow>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Usuário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [Description("Listar Usuários")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UsuarioResult>> Get([FromQuery] UsuarioQuery query)
        {
            var result = await _repository.Read(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Description("Remover Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Usuário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpPost]
        [Description("Criar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<UsuarioRow>> Post(UsuarioForm form)
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

            return CreatedAtAction(nameof(Get), new { id = result!.Id }, result);
        }

        [HttpPut("{id}")]
        [Description("Editar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<UsuarioRow>> Put(int id, [FromBody] UsuarioForm form)
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

            return Ok(result);
        }

        [HttpPost("login")]
        [Description("Autenticar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UsuarioRow>> Authenticate(LoginForm form)
        {
            var (result, token) = await _repository.Authenticate(form);

            if (result == null)
                return Problem(detail: "Credencial inválida", statusCode: StatusCodes.Status401Unauthorized);
            
            HttpContext.Response.Headers.Authorization = $"Bearer {token}";
            return Ok(result);
        }
        /*
        TODO:
        - Buscar e editar usuário autenticado;
        - Atualizar sessão (refresh token) a cada requisição;
        - Sair (cancel token).
        */
    }
}
