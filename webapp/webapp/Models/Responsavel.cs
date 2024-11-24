namespace webapp.Models;

public class Responsavel : Usuario {
    public ICollection<Estudante> Estudantes { get; set; } = new List<Estudante>();
}