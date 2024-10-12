using System.ComponentModel.DataAnnotations.Schema;

namespace EPIlist.Models;
public class Unidade
{
    public int UnidadeID { get; set; }
    public string? Nome { get; set; }
    public List<UnidadeUsuario>? UnidadesUsuarios { get; set; }
    [NotMapped]
    public List<int>? UsuariosId { get; set; }
}
