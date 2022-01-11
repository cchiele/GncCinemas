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
    public sealed class IngressoTypeConfiguration : IEntityTypeConfiguration<Ingresso>
    {
        public void Configure(EntityTypeBuilder<Ingresso> builder)
        {
            builder.ToTable("Ingressos");
            builder.HasKey(c => c.ID);
            builder.Property(c => c.IdSessao).HasColumnType("uniqueidentifier");
            builder.Property(c => c.Quantidade).HasColumnType("int");
            builder.Property(c => c.ValorTotal).HasColumnType("Decimal");
        }
    }
}
