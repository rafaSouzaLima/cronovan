namespace webapp.Models;

public class Estudante : Usuario {
    public string? ResponsavelId { get; set; }
    public Responsavel? Responsavel { get; set; }
}