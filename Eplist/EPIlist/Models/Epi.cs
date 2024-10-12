namespace EPIlist.Models;
public class Epi
{
    public int EpiID { get; set; }
    public string Descricao { get; set; }
    public int C_A { get; set; }
    public int Quantidade { get; set; }
    public List<UsuarioEpi>? EpisUsuario { get; set; }
}

