using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GncCinemas.WebApi.Models
{
    public class FilmeInputModel
    {
        [Required]
        public string Titulo { get; set; }

        [Required]
        public int Duracao { get; set; }

        [Required]
        public string Sinopse { get; set; }

    }
}
