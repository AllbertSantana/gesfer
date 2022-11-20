﻿namespace backend.Models
{
    public class Ferias // Período de Gozo
    {
        public uint Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public Exercicio Exercicio { get; set; } = null!;
    }
}