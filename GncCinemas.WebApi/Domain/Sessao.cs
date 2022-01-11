using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GncCinemas.WebApi.Domain
{
    public sealed class Sessao
    {
        private Guid _id;
        private DateTime _dataExibicao;
        private string _horarioInicio;
        private int _quantLugares;
        private int _quantLugaresReservados;
        private Decimal _valorIngresso;
        private Guid _idFilme;
        private String _hashConcorrencia;

        private Sessao() { }

        public Sessao(Guid id, DateTime dataExibicao, string horarioInicio, int quantLugares, Decimal valorIngresso, Filme filme, string hashConcorrencia)
        {
            ID = id;
            DataExibicao = dataExibicao;
            HorarioInicio = horarioInicio;
            QuantLugares = quantLugares;
            _quantLugaresReservados = 0;
            ValorIngresso = valorIngresso;
            IdFilme = filme.ID;
            _hashConcorrencia = hashConcorrencia;

            AtualizarHashConcorrencia();
        }

        public Sessao(DateTime dataExibicao, string horarioInicio, int quantLugares, Decimal valorIngresso, Filme filme)
            : this(Guid.NewGuid(), dataExibicao, horarioInicio, quantLugares, valorIngresso, filme, null)
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

        public DateTime DataExibicao
        {
            get => _dataExibicao;
            set
            {
                if (value < DateTime.Now)
                    throw new ValidationException("Data de exibicao deve ser superior a data atual");

                _dataExibicao = value;
            }
        }

        public string HorarioInicio
        {
            get => _horarioInicio;
            set
            {
                var time = value.Split(":");
                if (time.Length != 2)
                    throw new ValidationException("Hora especificada está no formato inválida. Formato aceito hh:mm");

                _horarioInicio = value;
            }
        }

        public int QuantLugares
        {
            get => _quantLugares;
            set
            {
                if (QuantLugaresReservados > 0 && value < QuantLugaresReservados)
                    throw new ValidationException($"Já existem {QuantLugaresReservados} lugare(s) vendido(s) nesta sessão, a nova quantidade de lugares não pode ser menor que a quantidade já vendida.");

                if (value <= 0 || value > 100)
                    throw new ValidationException("Quantidade de lugares deve estar entre 1 e 100");

                _quantLugares = value;
            }
        }

        public int QuantLugaresReservados
        {
            get => _quantLugaresReservados;
        }

        public Decimal ValorIngresso
        {
            get => _valorIngresso;
            set
            {
                if (value <= 0)
                    throw new ValidationException("Valor do ingresso deve ser superior a 0");

                _valorIngresso = value;
            }
        }

        public Guid IdFilme
        {
            get => _idFilme;
            set
            {
                _idFilme = value;
            }
        }

        public int QuantLugaresDisponiveis()
        {
            return (QuantLugares - QuantLugaresReservados);
        }

        public void ReservarLugares(int quantidadeReserva)
        {
            if (quantidadeReserva > QuantLugaresDisponiveis())
                throw new ValidationException("Quantidade de ingressos excede a capacidade máxima de lugares da sessão");

            _quantLugaresReservados += quantidadeReserva;

            AtualizarHashConcorrencia();
        }

        public void CancelarLugaresReservados(int quantidadeReserva)
        {
            if (_quantLugaresReservados >= quantidadeReserva)
                _quantLugaresReservados -= quantidadeReserva;
            else
                _quantLugaresReservados = 0;

            AtualizarHashConcorrencia();
        }

        private void AtualizarHashConcorrencia()
        {
            using var hash = MD5.Create();
            var data = hash.ComputeHash(
                Encoding.UTF8.GetBytes(
                    $"{ID}{DataExibicao}{HorarioInicio}{QuantLugares}{QuantLugaresReservados}{ValorIngresso}{IdFilme}"));

            var sBuilder = new StringBuilder();

            foreach (var tbyte in data)
                sBuilder.Append(tbyte.ToString("x2"));

            _hashConcorrencia = sBuilder.ToString();
        }

    }
}
