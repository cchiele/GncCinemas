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
    public sealed class SessoesRepository
    {
        private readonly CinemasDbContext _cinemasDbContext;
        private readonly IConfiguration _configuracao;

        public SessoesRepository(CinemasDbContext cinemasDbContext, IConfiguration configuracao)
        {
            _cinemasDbContext = cinemasDbContext;
            _configuracao = configuracao;
        }

        public async Task AdicionarAsync(Sessao sessao, CancellationToken cancellationToken = default)
        {
            await _cinemasDbContext.AddAsync(sessao, cancellationToken);
        }

        public async Task<IEnumerable<Sessao>> ObterAsync(string idFilme, DateTime dataExibicao, CancellationToken cancellationToken = default)
        {
            // TODO: Não sei se o código abaixo é a melhor maneira de implementar os filtros, provavelmente não é, principalmente a forma de trabalahr com as datas.

            string strDataExibicao = dataExibicao.ToString();
           
            if (!string.IsNullOrEmpty(idFilme) && strDataExibicao != "01/01/0001 00:00:00")
            {
                if (!Guid.TryParse(idFilme, out var guid))
                    throw new Exception("ID do Filme é inválida.");

                return await _cinemasDbContext.Sessoes.Where(c => c.IdFilme == guid && c.DataExibicao == dataExibicao).ToListAsync(cancellationToken);
            }
            else if (!string.IsNullOrEmpty(idFilme) && strDataExibicao == "01/01/0001 00:00:00")
            {
                if (!Guid.TryParse(idFilme, out var guid))
                    throw new Exception("ID do Filme é inválida.");

                return await _cinemasDbContext.Sessoes.Where(c => c.IdFilme == guid).ToListAsync(cancellationToken);
            }
            else if (string.IsNullOrEmpty(idFilme) && strDataExibicao != "01/01/0001 00:00:00")
            {
                return await _cinemasDbContext.Sessoes.Where(c => c.DataExibicao == dataExibicao).ToListAsync(cancellationToken);
            }
            else
            {
                return await _cinemasDbContext.Sessoes.ToListAsync(cancellationToken);
            }
        }

        public async Task<Sessao> ObterPelaChaveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _cinemasDbContext.Sessoes.FirstOrDefaultAsync(c => c.ID == id, cancellationToken);
        }

        public void Atualizar(Sessao sessao)
        {
            // Nada a fazer EF CORE fazer o Tracking da Entidade quando recuperamos a mesma
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _cinemasDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
