using EPIlist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namespace.Data;

namespace EPIlist.Controllers;
[ApiController]
[Route("EpiList/Usuario")]
public class UsuarioController : ControllerBase
{

    private readonly AppDataContext _ctx;
    public UsuarioController(AppDataContext ctx) => _ctx = ctx;
    //Todas as entidades devem conter a inserção, remoção, alteração e listagem das informações em banco de dados;
    //GET: Epilist/Usuario/listar
    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            var usuariosComEpis = _ctx.Usuarios
            .Include(u => u.UsuariosEpis)
            .ThenInclude(ue => ue.Epi)
            .ToList();

            var resultado = usuariosComEpis.Select(u => new
            {
                UsuarioID = u.UsuarioID,
                Nome = u.Nome,
                Email = u.Email,
                Telefone = u.Telefone,
                Senha = u.Senha,
                CPF = u.CPF,
                Cargo = u.Cargo,
                Equipe = u.EquipeID,
                Epis = u.UsuariosEpis.Select(ue => new
                {
                    EpiID = ue.Epi.EpiID,
                    Descricao = ue.Epi.Descricao,
                    C_A = ue.Epi.C_A,
                    Quantidade = ue.Epi.Quantidade
                }).ToList()

            }).ToList();
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    // GET: Epilist/Usuario/id
    [HttpGet]
    [Route("{id}")]
    public IActionResult UsuarioId(int id)
    {
        try
        {
            var usuario = _ctx.Usuarios
            .Include(u => u.UsuariosEpis)
            .ThenInclude(ue => ue.Epi)
            .FirstOrDefault(u => u.UsuarioID == id);;
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var usuarioEPIResponse = new
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                CPF = usuario.CPF,
                Senha = usuario.Senha,
                Cargo = usuario.Cargo,
                Equipe = usuario.EquipeID,
                EPIS = usuario.UsuariosEpis.Select(ue => new
                {
                    EpiID = ue.Epi.EpiID,
                    Descricao = ue.Epi.Descricao,
                    C_A = ue.Epi.C_A,
                    Quantidade = ue.Epi.Quantidade
                    // Outras propriedades do EPI que você deseja incluir
                }).ToList()
            };

            return Ok(usuarioEPIResponse);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    //Post: Epilist/Usuario/cadastrar
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Usuario usuario)
    {
        try
        {
            if (usuario.episId != null && usuario.episId.Any())
            {
                List<UsuarioEpi> usuarioEpis = new List<UsuarioEpi>();
                foreach (var epiId in usuario.episId)
                {
                    Epi? epi = _ctx.Epis.Find(epiId);
                    if (epi == null)
                    {
                        return NotFound($"Epi com ID {epiId} não encontrado.");
                    }
                    if (epi.Quantidade >= 1)
                    {
                        var usuarioEpi = new UsuarioEpi
                        {
                            Usuario = usuario,
                            Epi = epi
                        };
                        usuarioEpis.Add(usuarioEpi);
                        epi.Quantidade = epi.Quantidade - 1;
                        _ctx.Epis.Update(epi);
                        _ctx.SaveChanges();
                    }
                    else
                    {
                        return NotFound($"Epi com ID {epiId} sem estoque.");
                    }
                }
                _ctx.UsuarioEpis.AddRange(usuarioEpis);
            }
            _ctx.Add(usuario);
            _ctx.SaveChanges();
            return Created("", usuario);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    //delete: Epilist/usuario/deletar/id
    [HttpDelete]
    [Route("Deletar/{id}")]
    public IActionResult DeletarEPI(int id)
    {
        var usuario = _ctx.Usuarios.Include(u => u.Equipe).FirstOrDefault(u => u.UsuarioID == id);
        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }
        // Pesquise as equipes em que o usuário líder está alocado
        var equipesDoLider = _ctx.Equipes.Where(e => e.LiderID == usuario.UsuarioID).ToList();
        // Remova o usuário da equipe definindo o LiderID como nulo
        foreach (var equipe in equipesDoLider)
        {
            equipe.LiderID = 0;
            equipe.Lider = null;
        }
        _ctx.Usuarios.Remove(usuario);
        _ctx.SaveChanges();
        return NoContent();
    }
    //adicionar epi
    [HttpPut]
    [Route("{id}/AdicionarEPIs")]
    public IActionResult AdicionarEPIsAoUsuario(int id, [FromBody] List<int> epiIds)
    {
        try
        {
            epiIds = epiIds.Distinct().ToList();
            Usuario usuario = _ctx.Usuarios.Include(u => u.UsuariosEpis).FirstOrDefault(u => u.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            // Verifique se os EPIs existem e estão disponíveis
            var epis = _ctx.Epis.Where(epi => epiIds.Contains(epi.EpiID) && epi.Quantidade > 0).ToList();

            if (epis.Count != epiIds.Count)
            {
                return BadRequest("Alguns dos EPIs selecionados não estão disponíveis.");
            }
            foreach (var epiId in epiIds)
            {
                // Verifique se o usuário já possui o EPI com base no ID do EPI
                if (!usuario.UsuariosEpis.Any(ue => ue.EpiID == epiId))
                {
                    var epi = epis.FirstOrDefault(e => e.EpiID == epiId);
                    if (epi != null)
                    {
                        usuario.UsuariosEpis.Add(new UsuarioEpi { UsuarioID = usuario.UsuarioID, EpiID = epi.EpiID });
                        epi.Quantidade--; // Reduza a quantidade disponível do EPI
                    }
                }
            }
            _ctx.SaveChanges();
            return Ok("EPIs adicionados com sucesso ao usuário.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    [HttpPut]
    [Route("{id}/RemoverEPIs")]
    public IActionResult RemoverEPIsDoUsuario(int id, [FromBody] List<int> epiIds)
    {
        try
        {
            Usuario usuario = _ctx.Usuarios.Include(u => u.UsuariosEpis).FirstOrDefault(u => u.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            // Verificar se o usuário possui os EPIs a serem removidos
            var epiIdsParaRemover = usuario.UsuariosEpis
                .Where(ue => epiIds.Contains(ue.EpiID))
                .Select(ue => ue.EpiID)
                .ToList();

            // Remover os EPIs do usuário
            foreach (var epiId in epiIdsParaRemover)
            {
                var usuarioEpiParaRemover = usuario.UsuariosEpis.FirstOrDefault(ue => ue.EpiID == epiId);
                if (usuarioEpiParaRemover != null)
                {
                    usuario.UsuariosEpis.Remove(usuarioEpiParaRemover);

                    // Aumentar a quantidade disponível do EPI
                    var epi = _ctx.Epis.FirstOrDefault(e => e.EpiID == epiId);
                    if (epi != null)
                    {
                        epi.Quantidade++;
                    }
                }
            }
            _ctx.SaveChanges();
            return Ok("EPIs removidos com sucesso do usuário.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    [HttpPut]
    [Route("{id}/Atualizar")]
    public IActionResult AtualizarUsuario(int id, [FromBody] Usuario novoUsuario)
    {
        try
        {
            Usuario usuarioExistente = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == id);

            if (usuarioExistente == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            Equipe? equipenova = _ctx.Equipes.FirstOrDefault(u => u.EquipeID == novoUsuario.EquipeID);
            if (equipenova == null)
            {
                usuarioExistente.Nome = novoUsuario.Nome;
                usuarioExistente.Email = novoUsuario.Email;
                usuarioExistente.Telefone = novoUsuario.Telefone;
                usuarioExistente.Senha = novoUsuario.Senha;
                usuarioExistente.CPF = novoUsuario.CPF;
                usuarioExistente.Cargo = novoUsuario.Cargo;
                usuarioExistente.EquipeID = null;
            }
            else
            {
                // Atualizar as informações do usuário com base nos dados fornecidos
                usuarioExistente.Nome = novoUsuario.Nome;
                usuarioExistente.Email = novoUsuario.Email;
                usuarioExistente.Telefone = novoUsuario.Telefone;
                usuarioExistente.Senha = novoUsuario.Senha;
                usuarioExistente.CPF = novoUsuario.CPF;
                usuarioExistente.Cargo = novoUsuario.Cargo;
                usuarioExistente.EquipeID = novoUsuario.EquipeID;
            }

            _ctx.Usuarios.Update(usuarioExistente);
            _ctx.SaveChanges();

            return Ok(usuarioExistente);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
