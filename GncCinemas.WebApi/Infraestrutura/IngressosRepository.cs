using GncCinemas.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GncCinemas.WebApi.Infraestrutura
{
    public sealed class IngressosRepository
    {
        private readonly CinemasDbContext _cinemasDbContext;
        private readonly IConfiguration _configuracao;

        public IngressosRepository(CinemasDbContext cinemasDbContext, IConfiguration configuracao)
        {
            _cinemasDbContext = cinemasDbContext;
            _configuracao = configuracao;
        }

        public async Task AdicionarAsync(Ingresso ingresso, CancellationToken cancellationToken = default)
        {
            await _cinemasDbContext.AddAsync(ingresso, cancellationToken);
        }

        public async Task<IEnumerable<Ingresso>> ObterAsync(string idIngresso, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(idIngresso))
            {
                if (!Guid.TryParse(idIngresso, out var guid))
                    throw new Exception("ID do ingresso é inválida.");

                return await _cinemasDbContext.Ingressos.Where(c => c.IdSessao == guid).ToListAsync(cancellationToken);
            }
            else
            {
                return await _cinemasDbContext.Ingressos.ToListAsync(cancellationToken);
            }
        }

        public async Task<Ingresso> ObterPelaChaveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _cinemasDbContext.Ingressos.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);
        }

        public void Atualizar(Ingresso ingresso)
        {
            // Nada a fazer EF CORE fazer o Tracking da Entidade quando recuperamos a mesma
        }
        public void Deletar(Ingresso ingresso)
        {
            // TODO: Aparentemente não tem um método EntryAsync

            _cinemasDbContext.Entry(ingresso).State = EntityState.Deleted;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _cinemasDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
