using EPIlist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namespace.Data;

namespace EPIlist.Controllers;
[ApiController]
[Route("EpiList/equipe")]
public class EquipeController : ControllerBase
{
 
    private readonly AppDataContext _ctx;
    public EquipeController(AppDataContext ctx) => _ctx = ctx;
    //Todas as entidades devem conter a inserção, remoção, alteração e listagem das informações em banco de dados;
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult CriarEquipeComUnidadeELider([FromBody] Equipe equipe)
    {
        if (equipe == null)
        {
            return BadRequest("Dados inválidos.");
        }

        // Aqui você pode adicionar a equipe ao contexto do Entity Framework
        // e definir a Unidade e o Líder com base nos IDs fornecidos

        Unidade? unidadeReq = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == equipe.UnidadeID);
        Usuario? LiderReq = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == equipe.LiderID);
        if(unidadeReq == null && LiderReq == null){
            return NotFound("Dados invalidos");
        }
        if(LiderReq.Cargo != "supervisor"){
            return NotFound("Lider sem o cargo previsto");
        }

        if (LiderReq.EquipeID != null)
        {
            return NotFound("SuperVisor não pode estar em outra equipe");
        }
        if (unidadeReq != null && LiderReq != null)
        {
            equipe.Unidade = unidadeReq;
            equipe.Lider = LiderReq;
            LiderReq.Equipe = equipe;
            _ctx.Equipes.Add(equipe);
            _ctx.Usuarios.Update(LiderReq);
            _ctx.SaveChanges();
            return Created("Equipe:", equipe);

        }
        return NotFound("Dados incorretos");

    }
    [HttpPut]
    [Route("adicionar-usuario/{equipeId}")]
    public IActionResult AdicionarUsuariosAEquipe(int equipeId, [FromBody] List<int> idsUsuarios)
    {
        var equipe = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipe == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        var usuariosParaAdicionar = _ctx.Usuarios
            .Where(u => idsUsuarios.Contains(u.UsuarioID) && u.Cargo == "colaborador")
            .ToList();

        if (usuariosParaAdicionar.Count == 0)
        {
            return BadRequest("Nenhum usuário válido fornecido para adicionar à equipe.");
        }

        foreach (var usuario in usuariosParaAdicionar)
        {
            usuario.EquipeID = equipeId;
            //equipe.Usuarios.Add(usuario);
        }

        _ctx.SaveChanges();

        return Ok($"Usuários ({string.Join(", ", usuariosParaAdicionar.Select(u => u.Nome))}) adicionados à equipe com sucesso.");
    }
    //delete para remover lider
    [HttpDelete]
    [Route("{equipeId}/remover-lider")]
    public IActionResult RemoverLiderDaEquipe(int equipeId)
    {
        var equipe = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipe == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        // Verificar se a equipe tem um líder atual
        if (equipe.LiderID == null)
        {
            return BadRequest("A equipe não tem um líder para remover.");
        }
        // Obter o usuário líder
        var lider = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == equipe.LiderID);

        if (lider == null)
        {
            return NotFound("Usuário líder não encontrado.{lider}");
        }
        // Remover a equipe da lista de usuários associados
        lider.Equipe = null;
        // Remover o líder da equipe (definindo LiderID como nulo)
        equipe.LiderID = null;
        equipe.Lider = null;


        // Salvar as alterações no banco de dados usando o Entity Framework
        _ctx.SaveChanges();

        return Ok("Líder removido da equipe com sucesso.");
    }
    //post para setar lider
    [HttpPost]
    [Route("{equipeId}/adicionar-lider/{liderId}")]
    public IActionResult AdicionarLiderAEquipe(int equipeId, int liderId)
    {
        var equipe = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipe == null)
        {
            return NotFound("Equipe não encontrada.");
        }
        if (equipe.LiderID != null)
        {
            return NotFound("Equipe ja esta com lider.");
        }

        var usuarioLider = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == liderId);

        if (usuarioLider == null)
        {
            return NotFound("Usuário líder não encontrado.");
        }
        // Defina o usuário líder como líder da equipe
        usuarioLider.Equipe = equipe;
        equipe.LiderID = usuarioLider.UsuarioID;
        equipe.Lider = usuarioLider;

        // Salvar as alterações no banco de dados usando o Entity Framework
        _ctx.SaveChanges();

        return Ok($"Usuário {usuarioLider.Nome} adicionado como líder da equipe com sucesso.");
    }
    //put para atualizar equipe
    [HttpPut]
    [Route("{equipeId}/atualizar")]
    public IActionResult AtualizarEquipe(int equipeId, [FromBody] Equipe equipeAtualizada)
    {
        var equipeExistente = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipeExistente == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        // Aplicar as atualizações nos campos desejados
        equipeExistente.NomeEquipe = equipeAtualizada.NomeEquipe;
        // Outras propriedades que você deseja atualizar

        // Salvar as alterações no banco de dados usando o Entity Framework
        _ctx.SaveChanges();

        return Ok("Equipe atualizada com sucesso.");
    }
    //put atualizar unidade
    [HttpPut]
    [Route("{equipeId}/atualizar-unidade/{novaUnidadeId}")]
    public IActionResult AtualizarUnidadeDaEquipe(int equipeId, int novaUnidadeId)
    {
        var equipeExistente = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipeExistente == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        var novaUnidade = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == novaUnidadeId);

        if (novaUnidade == null)
        {
            return NotFound("Unidade não encontrada.");
        }

        // Atualize a UnidadeID da equipe com o novo ID da unidade
        equipeExistente.UnidadeID = novaUnidadeId;
        equipeExistente.Unidade = novaUnidade;

        // Salve as alterações no banco de dados usando o Entity Framework
        _ctx.SaveChanges();

        return Ok($"Unidade da equipe atualizada com sucesso para a unidade com ID {novaUnidadeId}.");
    }
    [HttpDelete]
    [Route("{equipeId}/deletar")]
    public IActionResult DeletarEquipe(int equipeId)
    {
        var equipeExistente = _ctx.Equipes.FirstOrDefault(e => e.EquipeID == equipeId);

        if (equipeExistente == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        // Localize todos os usuários que pertencem a esta equipe
        var usuariosEquipe = _ctx.Usuarios.Where(u => u.EquipeID == equipeId).ToList();

        // Remova a equipe dos usuários pertencentes a ela
        foreach (var usuario in usuariosEquipe)
        {
            usuario.EquipeID = null;
        }
        // Agora, remova a equipe em si
        _ctx.Equipes.Remove(equipeExistente);

        // Salve as alterações no banco de dados usando o Entity Framework
        _ctx.SaveChanges();

        return Ok("Equipe e seus usuários relacionados excluídos com sucesso.");
    }
    //get para listar equipes
    [HttpGet]
    [Route("listar-equipes")]
    public IActionResult ObterTodasAsEquipes()
    {
        var equipesResponse = _ctx.Equipes
            .Select(e => new
            {
                e.EquipeID,
                e.NomeEquipe,
                Unidade = new
                {
                    e.Unidade.UnidadeID,
                    e.Unidade.Nome
                },
                Usuarios = e.Usuarios.Select(u => new
                {
                    u.UsuarioID,
                    u.Nome,
                    u.Cargo,
                    EPIS = u.UsuariosEpis.Select(ue => new
                    {
                        ue.Epi.EpiID,
                        ue.Epi.Descricao
                        // Outras propriedades do EPI que você deseja incluir
                    }).ToList()
                }).ToList()
            })
            .ToList();

        return Ok(equipesResponse);
}

    //get equipe
    [HttpGet]
    [Route("{equipeId}")]
    public IActionResult ObterEquipePorId(int equipeId)
    {
        
        var equipeResponse = _ctx.Equipes
            .Where(e => e.EquipeID == equipeId)
            .Select(e => new
            {
                e.EquipeID,
                e.NomeEquipe,
                Unidade = new
                {
                    e.Unidade.UnidadeID,
                    e.Unidade.Nome
                },
                Usuarios = e.Usuarios.Select(u => new
                {
                    u.UsuarioID,
                    u.Nome,
                    u.Cargo,
                    EPIS = u.UsuariosEpis.Select(ue => new
                    {
                        ue.Epi.EpiID,
                        ue.Epi.Descricao
                        // Outras propriedades do EPI que você deseja incluir
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefault();

        if (equipeResponse == null)
        {
            return NotFound("Equipe não encontrada.");
        }

        return Ok(equipeResponse);
    }


}