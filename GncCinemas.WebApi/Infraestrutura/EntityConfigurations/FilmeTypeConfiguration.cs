using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GncCinemas.WebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GncCinemas.WebApi.Infraestrutura.EntityConfigurations
{
    public sealed class FilmeTypeConfiguration : IEntityTypeConfiguration<Filme>
    {
        public void Configure(EntityTypeBuilder<Filme> builder)
        {
            builder.ToTable("filmes");
            builder.HasKey(c => c.ID);
            builder.Property(c => c.Titulo).HasColumnType("varchar(50)");
            builder.Property(c => c.Duracao).HasColumnType("int");
            builder.Property(c => c.Sinopse).HasColumnType("varchar(100)");
        }
    }
}
