using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GncCinemas.WebApi.Domain
{
    public sealed class Ingresso
    {
        private Guid _id;
        private Guid _idSessao;
        private int _quantidade;
        private Decimal _valorTotal;

        private Ingresso() { }

        public Ingresso(Guid id, Sessao sessao, int quantidade)
        {
            ID = id;
            IdSessao = sessao.ID;
            Quantidade = quantidade;
            ValorTotal = (sessao.ValorIngresso * quantidade);

            sessao.ReservarLugares(quantidade);
        }

        public Ingresso(Sessao sessao, int quantidade) : this(Guid.NewGuid(), sessao, quantidade)
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

        public Guid IdSessao
        {
            get => _idSessao;
            private set
            {
                _idSessao = value;
            }
        }

        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (value <= 0)
                    throw new ValidationException("Quantidade de ingressos deve ser superior a 0");

                _quantidade = value;
            }
        }

        public Decimal ValorTotal
        {
            get => _valorTotal;
            set
            {
                if (value <= 0)
                    throw new ValidationException("Valor total do ingresso deve ser superior a 0");

                _valorTotal = value;
            }
        }
        
        public void Cancelar(Sessao sessao)
        {
            sessao.CancelarLugaresReservados(Quantidade);
        }
    }
}
