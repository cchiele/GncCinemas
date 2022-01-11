using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GncCinemas.WebApi.Domain;
using GncCinemas.WebApi.Infraestrutura.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace GncCinemas.WebApi.Infraestrutura
{
    public class CinemasDbContext : DbContext
    {
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }

        public CinemasDbContext(DbContextOptions options) : base(options)
        {
        }
        
        /*
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FilmeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessaoTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IngressoTypeConfiguration());
        }

    }
}
