namespace backend.Models
{
    public class Exercicio // Período Aquisitivo
    {
        public uint Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}
