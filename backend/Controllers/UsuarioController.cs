using backend.Helpers;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Security.Permissions;

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
        [SwaggerOperation("Buscar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Permission(Permissao.BuscarUsuario)]
        public async Task<ActionResult<UsuarioRow>> Get(int id)
        {
            var result = await _repository.Read(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Usuário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [SwaggerOperation("Listar Usuários")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [Permission(Permissao.ListarUsuarios)]
        public async Task<ActionResult<UsuarioResult>> Get([FromQuery] UsuarioQuery query)
        {
            var result = await _repository.Read(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Remover Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Permission(Permissao.RemoverUsuario)]
        public async Task<ActionResult<UsuarioRow>> Delete(int id)
        {
            var result = await _repository.Delete(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Usuário não encontrado", statusCode: StatusCodes.Status404NotFound);
        }

        [HttpPost]
        [SwaggerOperation("Criar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Permission(Permissao.CriarUsuario)]
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
        [SwaggerOperation("Editar Usuário")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Permission(Permissao.EditarUsuario)]
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
        [SwaggerOperation("Autenticar Usuário", "Gerar token de acesso")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioRow>> Authenticate(LoginForm form)
        {
            var (result, token) = await _repository.Authenticate(form);

            if (result == null)
                return Problem(detail: "Credencial inválida", statusCode: StatusCodes.Status401Unauthorized);
            
            HttpContext.Response.Headers.Authorization = $"Bearer {token}";
            return Ok(result);
        }

        [HttpHead("refresh")]
        [SwaggerOperation("Renovar Token", "Renovar token de acesso")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public void Refresh()
        {
            var token = _repository.GenerateToken(User.Claims);
            HttpContext.Response.Headers.Authorization = $"Bearer {token}";
        }
        /*
        TODO:
        - Authorization baseada em Policy ou Roles;
        - Endpoints p/ buscar e editar usuário autenticado;
            - Editar usuário deveria adotar o Patch, pois Senha não é Required;
            - Em editar usuário logado, SenhaAtual é Required, mas SenhaNova não, enquanto Perfil é ignorado.
        */
        [HttpGet("login")]
        [SwaggerOperation("Buscar Usuário Logado")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<UsuarioRow>> Get()
        {
            var id = Convert.ToInt32(User.FindFirst(x => x.Type == nameof(Usuario.Id))?.Value);
            var result = await _repository.Read(id);
            return (result != null) ?
                Ok(result) :
                Problem(detail: "Usuário não encontrado", statusCode: StatusCodes.Status401Unauthorized);
        }
        /*
        [HttpPatch("login")]
        [SwaggerOperation("Editar Usuário Logado")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<UsuarioRow>> Patch([FromBody] UsuarioForm form)
        {
            form.Id = Convert.ToInt32(User.FindFirst(x => x.Type == nameof(Usuario.Id))?.Value);
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
        */
    }
}
