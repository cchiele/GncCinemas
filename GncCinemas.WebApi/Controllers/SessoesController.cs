using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GncCinemas.WebApi.Domain;
using GncCinemas.WebApi.Infraestrutura;
using GncCinemas.WebApi.Models;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace GncCinemas.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessoesController : Controller
    {
        private readonly SessoesRepository _sessoesRepository;
        private readonly FilmesRepository _filmesRepository;
        private readonly ILogger<SessoesController> _logger;

        public SessoesController(SessoesRepository sessoesRepository, FilmesRepository filmesRepository, ILogger<SessoesController> logger)
        {
            _sessoesRepository = sessoesRepository;
            _filmesRepository = filmesRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ObterComFiltros(
            [FromQuery(Name = "id-filme")] string idFilme,
            [FromQuery(Name = "data-exibicao")][DataType(DataType.Date)] DateTime dataExibicao,
            CancellationToken cancellationToken)
        {
            // TODO: Nas linhas acima do FromQuery, conseguimos dar um Name e mudar o tipo para Date, seria bom conseguir colocar um Format (yyyy-mm-dd) para aparecer no input do swagger

            _logger.LogInformation("Obtendo lista de sessões pelos filtros id-filme={idFilme}, data-exibicao={dataExibicao} ...", idFilme, dataExibicao.ToString());

            var sessoes = await _sessoesRepository.ObterAsync(idFilme, dataExibicao, cancellationToken);

            _logger.LogInformation("Foram encontrada(s) {x} sessões", sessoes.Count());

            return Ok(sessoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPelaChave(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Obtendo sessão pelo ID {id} ...", id);

            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Id da sessão é inválido");

            var sessao = await _sessoesRepository.ObterPelaChaveAsync(guid, cancellationToken);
            if (sessao == null)
                return NotFound();

            return Ok(sessao);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarAsync([FromBody] SessaoInputModel sessaoInputModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Criando nova sessão para o filme de ID {id} ...", sessaoInputModel.IdFilme);

            if (!Guid.TryParse(sessaoInputModel.IdFilme, out var guid))
                return BadRequest("Id do Filme é inválido");

            var filme = await _filmesRepository.ObterAsync(guid, cancellationToken);
            if (filme == null)
                return NotFound();

            var sessao = new Sessao(sessaoInputModel.DataExibicao, sessaoInputModel.HorarioInicio, sessaoInputModel.QuantLugares, sessaoInputModel.ValorIngresso, filme);

            await _sessoesRepository.AdicionarAsync(sessao, cancellationToken);

            await _sessoesRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Nova sessão criada com o ID {id} ...", sessao.ID);

            return CreatedAtAction(nameof(ObterPelaChave), new { id = sessao.ID }, sessao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAsync(string id, [FromBody] SessaoInputModel sessaoInputModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Atualizando a sessão de ID {id} ...", id);

            if (!Guid.TryParse(id, out var guidSessao))
                return BadRequest("Id da sessão é inválido");

            var sessao = await _sessoesRepository.ObterPelaChaveAsync(guidSessao);
            if (sessao == null)
                return NotFound();

            if (!Guid.TryParse(sessaoInputModel.IdFilme, out var guidFilme))
                return BadRequest("Id do filme é inválido");

            var filme = await _filmesRepository.ObterAsync(guidFilme, cancellationToken);
            if (filme == null)
                return BadRequest("Id do filme não encontrado");

            sessao.DataExibicao = sessaoInputModel.DataExibicao;
            sessao.HorarioInicio = sessaoInputModel.HorarioInicio;
            sessao.QuantLugares = sessaoInputModel.QuantLugares;
            sessao.ValorIngresso = sessaoInputModel.ValorIngresso;
            sessao.IdFilme = filme.ID;

            _sessoesRepository.Atualizar(sessao);

            await _sessoesRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Sessão de ID {id} foi atualizada", id);

            return Ok(sessao);
        }
    }
}
