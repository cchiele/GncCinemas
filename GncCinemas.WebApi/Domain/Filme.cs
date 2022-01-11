using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GncCinemas.WebApi.Domain
{
    public sealed class Filme
    {
        private Guid _id;
        private String _titulo;
        private int _duracao;
        private String _sinopse;

        private Filme() { }

        public Filme(Guid id, string titulo, int duracao, string sinopse)
        {
            ID = id;
            Titulo = titulo;
            Duracao = duracao;
            Sinopse = sinopse;
        }

        public Filme(string titulo, int duracao, string sinopse) : this(Guid.NewGuid(), titulo, duracao, sinopse)
        {
        }

        public Guid ID
        {
            get => _id;
            private set
            {
                _id = value;
            }
        }

        public string Titulo
        {
            get => _titulo;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("Título do filme deve ser informado");
                }
                else
                {
                    _titulo = value;
                }
            }
        }

        public int Duracao
        {
            get => _duracao;
            set
            {
                if (value <= 0)
                {
                    throw new ValidationException("Duração do filme deve ser superior a 0 minutos");
                }
                else
                {
                    _duracao = value;
                }
            }
        }

        public string Sinopse
        {
            get => _sinopse;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("Sinopse do filme deve ser informado");
                }
                else
                {
                    _sinopse = value;
                }
            }
        }

    }
}
