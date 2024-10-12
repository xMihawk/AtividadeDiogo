namespace EPIlist.Models;
public class Equipe
{
    public int EquipeID { get; set; }
    public string NomeEquipe { get; set; }
    public List<Usuario>? Usuarios { get; set; }
    public Unidade? Unidade { get; set; }
    public int UnidadeID {get; set; }
    public int? LiderID { get; set; } // Pode ser nulo se a equipe não tiver um líder
    public Usuario? Lider { get; set; }
}
