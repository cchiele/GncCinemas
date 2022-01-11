using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GncCinemas.WebApi.Models
{
    public class IngressoInputModel
    {
        [Required]
        public string IdSesao { get; set; }

        [Required]
        public int Quantidade { get; set; }
    }
}
