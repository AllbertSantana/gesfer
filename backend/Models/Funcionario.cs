namespace backend.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!; // Padrão @"\d{3}\.\d{3}\.\d{3}\.-\d{2}"
        public string Matricula { get; set; } = null!; // Padrão @"\d+\/\d+"
        public DateOnly DataVinculo { get; set; } // Início das Atividades
    }
}
