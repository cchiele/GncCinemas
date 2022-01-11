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
    public sealed class SessaoTypeConfiguration : IEntityTypeConfiguration<Sessao>
    {
        public void Configure(EntityTypeBuilder<Sessao> builder)
        {
            builder.ToTable("sessoes");
            builder.HasKey(c => c.ID);
            builder.Property(c => c.DataExibicao).HasColumnName("dataexibicao").HasColumnType("date");
            builder.Property(c => c.HorarioInicio).HasColumnType("varchar(5)");
            builder.Property(c => c.QuantLugares).HasColumnType("int");
            builder.Property(c => c.QuantLugaresReservados).HasColumnType("int");
            builder.Property(c => c.ValorIngresso).HasColumnType("Decimal");
            builder.Property(c => c.IdFilme).HasColumnType("uniqueidentifier");
            builder.Property("_hashConcorrencia").HasColumnName("hashconcorrencia").HasConversion<string>().IsConcurrencyToken();
        }
    }
}
