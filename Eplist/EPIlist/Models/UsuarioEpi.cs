using System.Text.Json.Serialization;

namespace EPIlist.Models;
public class UsuarioEpi
{
        public int UsuarioID { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }
        // Chave estrangeira para Epi
        public int EpiID { get; set; }
        [JsonIgnore]
        public Epi Epi { get; set; }
}
