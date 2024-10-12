using EPIlist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namespace.Data;

namespace EPIlist.Controllers;
[ApiController]
[Route("EpiList/Unidade")]
public class UnidadeController : ControllerBase
{
    private readonly AppDataContext _ctx;
    public UnidadeController(AppDataContext ctx) => _ctx = ctx;
    //Todas as entidades devem conter a inserção, remoção, alteração e listagem das informações em banco de dados;
    //GET: Epilist/Unidade/listar
    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            // Buscar todas as unidades
            var unidadesComUsuarios = _ctx.Unidades
            .Include(u => u.UnidadesUsuarios)
                .ThenInclude(uu => uu.Usuario)
            .ToList();

            // Criar uma lista para armazenar as informações de todas as unidades e suas equipes associadas
            var resultado = new List<object>();

            foreach (var unidade in unidadesComUsuarios)
            {
                // Obtenha as equipes associadas à unidade atual
                var equipesDaUnidade = _ctx.Equipes
                    .Where(e => e.UnidadeID == unidade.UnidadeID)
                    .Select(e => new
                    {
                        EquipeID = e.EquipeID,
                        // Outras propriedades da equipe, se necessário
                    })
                    .ToList();
                // Obtenha informações dos usuários associados à unidade atual
                var usuariosDaUnidade = unidade.UnidadesUsuarios
                    .Select(uu => new
                    {
                        UsuarioID = uu.Usuario.UsuarioID,
                        NomeUsuario = uu.Usuario.Nome,
                        Email = uu.Usuario.Email,
                        Telefone = uu.Usuario.Telefone,
                        CPF = uu.Usuario.CPF,
                        Cargo = uu.Usuario.Cargo

                    })
                    .ToList();

                var unidadeInfo = new
                {
                    UnidadeID = unidade.UnidadeID,
                    Nome = unidade.Nome,
                    Equipes = equipesDaUnidade,
                    Usuarios = usuariosDaUnidade
                };
                resultado.Add(unidadeInfo);
            }
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    // // GET: Epilist/Unidade/id
    [HttpGet]
    [Route("{id}")]
    public IActionResult ObterUnidadeComUsuariosEEquipes(int id)
    {
        try
        {
            // Buscar a unidade com informações de usuários associados
            var unidadeComUsuarios = _ctx.Unidades
                .Include(u => u.UnidadesUsuarios)
                    .ThenInclude(uu => uu.Usuario)
                .FirstOrDefault(u => u.UnidadeID == id);

            if (unidadeComUsuarios == null)
            {
                return NotFound("Unidade não encontrada.");
            }

            // Obtenha as equipes associadas à unidade atual
            var equipesDaUnidade = _ctx.Equipes
                .Where(e => e.UnidadeID == unidadeComUsuarios.UnidadeID)
                .Select(e => new
                {
                    EquipeID = e.EquipeID,
                    NomeEquipe = e.NomeEquipe
                    // Outras propriedades da equipe, se necessário
                })
                .ToList();

            // Obtenha informações dos usuários associados à unidade atual
            var usuariosDaUnidade = unidadeComUsuarios.UnidadesUsuarios
            .Select(uu => new
                {
                    UsuarioID = uu.Usuario.UsuarioID,
                    NomeUsuario = uu.Usuario.Nome,
                    Email = uu.Usuario.Email,
                    Telefone = uu.Usuario.Telefone,
                    CPF = uu.Usuario.CPF,
                    Cargo = uu.Usuario.Cargo
                })
            .ToList();

            var unidadeInfo = new
            {
                UnidadeID = unidadeComUsuarios.UnidadeID,
                Nome = unidadeComUsuarios.Nome,
                Equipes = equipesDaUnidade,
                Usuarios = usuariosDaUnidade
            };

            return Ok(unidadeInfo);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    // }
    //Post: Epilist/Usuario/cadastrar
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Unidade unidade)
    {
        try
        {
            if (unidade.UsuariosId != null && unidade.UsuariosId.Any() && unidade.UsuariosId.Count() == 2)
            {
                List<UnidadeUsuario> unidadeUsuarios = new List<UnidadeUsuario>();
                foreach (var usuarioID in unidade.UsuariosId)
                {
                    Usuario? usuario = _ctx.Usuarios.Find(usuarioID);
                    if (usuario == null)
                    {
                        return NotFound($"Usuario com ID {usuarioID} não encontrado.");
                    }

                    //validaçao de usuarios diferentes e outra validaçao juntar de usuarios com um cargo tecnico e getor
                    var usuarioUnidade = new UnidadeUsuario
                    {
                        Usuario = usuario,
                        Unidade = unidade
                    };
                    unidadeUsuarios.Add(usuarioUnidade);
                }
                var encontrouTecnico = false;
                var encontrouGestor = false;

                foreach (var usuario in unidadeUsuarios)
                {
                    if (usuario.Usuario.Cargo == "tecnico")
                    {
                        encontrouTecnico = true;
                    }
                    else if (usuario.Usuario.Cargo == "gestor")
                    {
                        encontrouGestor = true;
                    }
                }
                if (encontrouTecnico && encontrouGestor)
                {
                    _ctx.UnidadeUsuarios.AddRange(unidadeUsuarios);
                    _ctx.Add(unidade);
                    _ctx.SaveChanges();
                    return Created("", unidade);
                }
                else
                {
                    return NotFound("Cargos inválido");
                }
            }
            return NotFound("Nao a usuario suficiente");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    //put atualizar A unidade

    [HttpPut]
    [Route("{id}/Atualizar")]
    public IActionResult AtualizarUnidade(int id, [FromBody] Unidade novaUnidade)
    {
        try
        {
            Unidade unidadeExistente = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == id);

            if (unidadeExistente == null)
            {
                return NotFound("Unidade não encontrada.");
            }

            // Atualizar as informações da unidade com base nos dados fornecidos
            unidadeExistente.Nome = novaUnidade.Nome;

            _ctx.SaveChanges();

            return Ok("Unidade atualizada com sucesso.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    //put para adicionar usuario
    [HttpPut]
    [Route("AdicionarUsuario/{idUnidade}/{idUsuario}")]
    public IActionResult AdicionarUsuario(int idUnidade, int idUsuario)
    {
        try
        {
            var novaUnidade = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == idUnidade);

            if (novaUnidade == null)
            {
                return NotFound("Unidade não encontrada.");
            }
            Usuario novoUsuario = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == idUsuario);

            if (novoUsuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (novoUsuario.Cargo == "gestor" && novoUsuario.UsuariosUnidades != null && novoUsuario.UsuariosUnidades.Any())
            {
                return BadRequest("O usuário ja estar associado a uma unidade.");
            }

            // Verificar se o cargo do usuário é válido (Técnico ou Gestor)
            if (novoUsuario.Cargo != "tecnico" && novoUsuario.Cargo != "gestor")
            {
                return BadRequest("O cargo do usuário deve ser 'Técnico' ou 'Gestor'.");
            }

            // Verificar se já existe um técnico ou gestor na unidade
            bool tecnicoExistente = _ctx.UnidadeUsuarios.Any(uu =>
                uu.UnidadeID == idUnidade &&
                uu.Usuario.Cargo == "tecnico");

            bool gestorExistente = _ctx.UnidadeUsuarios.Any(uu =>
                uu.UnidadeID == idUnidade &&
                uu.Usuario.Cargo == "gestor");

            if ((novoUsuario.Cargo == "tecnico" && tecnicoExistente) ||
                (novoUsuario.Cargo == "gestor" && gestorExistente))
            {
                return BadRequest($"Já existe um {novoUsuario.Cargo} nesta unidade.");
            }

            // Adicionar o usuário à unidade através da classe intermediária UnidadeUsuario
            var unidadeUsuario = new UnidadeUsuario
            {
                Usuario = novoUsuario,
                Unidade = novaUnidade
            };

            _ctx.UnidadeUsuarios.Add(unidadeUsuario);
            _ctx.SaveChanges();

            return Ok("Usuário adicionado com sucesso.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    //delete usuario da unidade
    [HttpDelete]
    [Route("DeletarUsuario/{unidadeId}/{usuarioId}")]
    public IActionResult DeletarUsuarioDaUnidade(int unidadeId, int usuarioId)
    {
        try
        {
            // Verificar se a unidade e o usuário existem
            var unidade = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == unidadeId);
            var usuario = _ctx.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (unidade == null || usuario == null)
            {
                return NotFound("Unidade ou usuário não encontrado.");
            }

            // Verificar se o usuário está associado à unidade através da classe intermediária
            var unidadeUsuario = _ctx.UnidadeUsuarios.FirstOrDefault(uu =>
                uu.UnidadeID == unidadeId &&
                uu.UsuarioID == usuarioId);

            if (unidadeUsuario == null)
            {
                return NotFound("Usuário não está associado a esta unidade.");
            }

            // Remover o usuário da unidade através da classe intermediária
            _ctx.UnidadeUsuarios.Remove(unidadeUsuario);
            _ctx.SaveChanges();

            return Ok("Usuário removido da unidade com sucesso.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    //deletar unidade
    [HttpDelete]
    [Route("DeletarUnidade/{unidadeId}")]
    public IActionResult DeletarUnidade(int unidadeId)
    {
        try
        {
            // Verificar se a unidade existe
            var unidade = _ctx.Unidades.FirstOrDefault(u => u.UnidadeID == unidadeId);

            if (unidade == null)
            {
                return NotFound("Unidade não encontrada.");
            }

            // Obter todas as equipes associadas a esta unidade
            var equipesDaUnidade = _ctx.Equipes.Where(e => e.UnidadeID == unidadeId).ToList();

            foreach (var equipe in equipesDaUnidade)
            {
                // Obter todos os usuários associados a esta equipe
                var usuariosDaEquipe = _ctx.Usuarios.Where(u => u.EquipeID == equipe.EquipeID).ToList();

                // Desassociar a equipe de cada usuário
                foreach (var usuario in usuariosDaEquipe)
                {
                    usuario.Equipe = null;
                    usuario.EquipeID = null;
                }

                // Remover a equipe
                _ctx.Equipes.Remove(equipe);
            }

            // Remover a unidade
            _ctx.Unidades.Remove(unidade);

            _ctx.SaveChanges();

            return Ok("Unidade, equipes associadas e usuários associados removidos com sucesso.");
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}