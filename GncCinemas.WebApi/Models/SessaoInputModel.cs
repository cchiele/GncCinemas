using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GncCinemas.WebApi.Models
{
    public class SessaoInputModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = "Data de exibição está no formato inválido")]
        public DateTime DataExibicao { get; set; }

        [Required]
        public string HorarioInicio { get; set; }

        [Required]
        public int QuantLugares { get; set; }

        [Required]
        public Decimal ValorIngresso { get; set; }

        [Required]
        public string IdFilme { get; set; }
    }
}
