using System.Text.Json.Serialization;

namespace EPIlist.Models;
public class UnidadeUsuario
{
        public int UsuarioID { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }
        // Chave estrangeira para Epi
        public int UnidadeID { get; set; }
        [JsonIgnore]
        public Unidade Unidade { get; set; }
}
