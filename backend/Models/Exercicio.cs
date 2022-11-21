namespace backend.Models
{
    public class Exercicio // Período Aquisitivo
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public Funcionario Funcionario { get; set; } = null!;
    }
}
