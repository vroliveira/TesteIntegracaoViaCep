using ExemploTestes.Models;
using ExemploTestes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExemploTestes.Controllers
{
    [Route("api/endereco")]
    [ApiController]
    public class EnderecoViaCepController : ControllerBase
    {
        private readonly IViaCepService _viaCepService;

        public EnderecoViaCepController(IViaCepService viaCepService)
        {
            _viaCepService = viaCepService;
        }

        [HttpGet("{cep}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Endereco>> ObterEnderecoPorCep(string cep)
        {
            return Ok(await _viaCepService.BuscaCep(cep));
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> ObterEndereco()
        {
            return Ok("Coloque o caminho na pesquisa para pequisa pelo cep: http://localhost:{suaPorta}/api/endereco/{SeuCepParaPesquisar}");
        }
    }
}
