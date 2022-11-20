﻿namespace backend.Models
{
    public class Funcionario
    {
        public uint Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; } // Padrão @"\d{3}\.\d{3}\.\d{3}\.-\d{2}"
        public string Matricula { get; set; } // Padrão @"\d+\/\d+"
        public DateOnly DataVinculo { get; set; } // Início das Atividades
    }
}
