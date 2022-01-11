using GncCinemas.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace GncCinemas.WebApi.Infraestrutura
{
    public sealed class FilmesRepository
    {
        private readonly CinemasDbContext _cinemasDbContext;
        private readonly IConfiguration _configuracao;

        public FilmesRepository(CinemasDbContext cinemasDbContext, IConfiguration configuracao)
        {
            _cinemasDbContext = cinemasDbContext;
            _configuracao = configuracao;
        }

        public async Task AdicionarAsync(Filme filme, CancellationToken cancellationToken = default)
        {
            //_filmes.Add(filme);
            await _cinemasDbContext.AddAsync(filme, cancellationToken);
        }
        
        public async Task<IEnumerable<Filme>> ObterAsync(CancellationToken cancellationToken = default)
        {
            return await _cinemasDbContext.Filmes.ToListAsync(cancellationToken);
        }

        public async Task<Filme> ObterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _cinemasDbContext.Filmes.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);
        }

        public void Atualizar(Filme filme)
        {
            // Nada a fazer EF CORE fazer o Tracking da Entidade quando recuperamos a mesma
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _cinemasDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
