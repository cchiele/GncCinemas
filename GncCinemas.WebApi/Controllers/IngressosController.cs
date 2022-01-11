using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GncCinemas.WebApi.Domain;
using GncCinemas.WebApi.Infraestrutura;
using GncCinemas.WebApi.Models;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace GncCinemas.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngressosController : Controller
    {
        private readonly IngressosRepository _ingressosRepository;
        private readonly SessoesRepository _sessoesRepository;
        private readonly ILogger<IngressosController> _logger;

        public IngressosController(IngressosRepository ingressosRepository, SessoesRepository sessoesRepository, ILogger<IngressosController> logger)
        {
            _ingressosRepository = ingressosRepository;
            _sessoesRepository = sessoesRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ObterComFiltros([FromQuery(Name = "id-sessao")] string idSessao, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Obtendo ingressos pelo filtro id-sessao={idSessao} ...", idSessao);

            if (!string.IsNullOrEmpty(idSessao))
                if (!Guid.TryParse(idSessao, out var guidSessao))
                    return BadRequest("Id da sessão é inválido");

            var ingressos = await _ingressosRepository.ObterAsync(idSessao, cancellationToken);

            _logger.LogInformation("Foram encontrado(s) {quantidadeIngressos} ingresso(s)", ingressos.Count());

            return Ok(ingressos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPelaChave(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Obtendo ingresso pelo ID {id} ...", id);

            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Id do ingresso é inválido");

            var ingresso = await _ingressosRepository.ObterPelaChaveAsync(guid, cancellationToken);
            if (ingresso == null)
                return NotFound();

            return Ok(ingresso);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarAsync([FromBody] IngressoInputModel ingressoInputModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Criando novo ingresso para a sessão de ID {id} ...", ingressoInputModel.IdSesao);

            if (!Guid.TryParse(ingressoInputModel.IdSesao, out var guid))
                return BadRequest("Id da sessão é inválido");

            var sessao = await _sessoesRepository.ObterPelaChaveAsync(guid, cancellationToken);
            if (sessao == null)
                return BadRequest("Id da sessão não encontrada");

            var ingresso = new Ingresso(sessao, ingressoInputModel.Quantidade);

            await _ingressosRepository.AdicionarAsync(ingresso, cancellationToken);

            await _ingressosRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Novo ingresso criado com o ID {id} ...", ingresso.ID);

            return CreatedAtAction(nameof(ObterPelaChave), new { id = ingresso.ID }, ingresso);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarAsync(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancelando o ingresso com ID {id} ...", id);

            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Id do ingresso é inválido");

            var ingresso = await _ingressosRepository.ObterPelaChaveAsync(guid, cancellationToken);
            if (ingresso == null)
                return BadRequest("Id do ingresso não encontrado");

            var sessao = await _sessoesRepository.ObterPelaChaveAsync(ingresso.IdSessao, cancellationToken);
            if (sessao == null)
                return BadRequest("Sessão do ingresso não encontrada");

            _logger.LogInformation("Cancelando as reservas do ingresso na sessão de ID {id} ...", sessao.ID);
            ingresso.Cancelar(sessao);

            // Não consegui fazer o método Deletar ser Async
            _ingressosRepository.Deletar(ingresso);

            await _ingressosRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Ingresso removido do sistema", id);

            return NoContent();
        }
    }
}
