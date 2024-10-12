using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EPIlist.Models;
public class Usuario
{   
    
    public int UsuarioID { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Senha { get; set; }
    public string CPF { get; set; }
    public string Cargo { get; set; }
    [JsonIgnore]
    public Equipe? Equipe { get; set; }
    public int? EquipeID { get; set; }
    public List<UsuarioEpi>? UsuariosEpis { get; set; }
    public List<UnidadeUsuario>? UsuariosUnidades { get; set; }
    [NotMapped]
    public List<int>? episId { get; set; }
}