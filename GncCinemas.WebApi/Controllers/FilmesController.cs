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
    public class FilmesController : Controller
    {
        private readonly FilmesRepository _filmesRepository;
        private readonly ILogger<FilmesController> _logger;

        public FilmesController(FilmesRepository filmesRepository, ILogger<FilmesController> logger)
        {
            _filmesRepository = filmesRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Obtendo lista completa de filmes ...");

            var filmes = await _filmesRepository.ObterAsync(cancellationToken);

            _logger.LogInformation("Foram encontrado(s) {x} filme(s)", filmes.Count());

            return Ok(filmes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPelaChave(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Obtendo filme pelo ID {id} ...", id);

            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Id inválido");

            var filme = await _filmesRepository.ObterAsync(guid, cancellationToken);
            if (filme == null)
                return NotFound();
            
            return Ok(filme);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarAsync([FromBody] FilmeInputModel filmeInputModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Criando novo filme ...");

            var filme = new Filme(filmeInputModel.Titulo, filmeInputModel.Duracao, filmeInputModel.Sinopse);

            await _filmesRepository.AdicionarAsync(filme, cancellationToken);

            await _filmesRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Novo filme criado com o ID {id} ...", filme.ID);

            return CreatedAtAction(nameof(ObterPelaChave), new { id = filme.ID }, filme);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAsync(string id, [FromBody] FilmeInputModel filmeInputModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Atualizando o filme de ID {id} ...", id);

            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Id inválido");

            var filme = await _filmesRepository.ObterAsync(guid);
            if (filme == null)
                return NotFound();

            filme.Titulo = filmeInputModel.Titulo;
            filme.Duracao = filmeInputModel.Duracao;
            filme.Sinopse = filmeInputModel.Sinopse;

            _filmesRepository.Atualizar(filme);

            await _filmesRepository.CommitAsync(cancellationToken);

            _logger.LogInformation("Filme de ID {id} foi atualizada", id);

            return Ok(filme);
        }
    }
}
