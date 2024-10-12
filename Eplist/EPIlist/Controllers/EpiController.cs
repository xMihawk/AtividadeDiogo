using EPIlist.Models;
using Microsoft.AspNetCore.Mvc;
using Namespace.Data;

namespace EPIlist.Controllers;
[ApiController]
[Route("EpiList/EPI")]
public class EpiController : ControllerBase
{
    
    private readonly AppDataContext _ctx;
    public EpiController(AppDataContext ctx) => _ctx = ctx;

    //GET: Epilist/EPI/listar
    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            List<Epi> Epis = _ctx.Epis.ToList();
            return Epis.Count == 0 ? NotFound() : Ok(Epis);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    // GET: Epilist/Epi/id
    [HttpGet]
    [Route("{id}")]
    public IActionResult EPIId(int id)
    {
        Epi epi = _ctx.Epis.FirstOrDefault(e => e.EpiID == id);
        return epi == null ? NotFound("") : Ok(epi);
    }

    // POST: Epilist/Epi/cadastrar
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Epi epi)
    {
        try
        {
                    _ctx.Add(epi);
        _ctx.SaveChanges();
            return Created("", epi);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //Put: Epilist/Epi/autalizar
    [HttpPut]
    [Route("Atualizar/{id}")]
    public IActionResult AtualizarEPI(int id, [FromBody] Epi epiAtualizado)
    {
        if (epiAtualizado == null)
        {
            return BadRequest("Dados do EPI inválidos.");
        }

        Epi epi = _ctx.Epis.FirstOrDefault(e => e.EpiID == id);

        if (epi == null)
        {
            return NotFound("EPI não encontrado.");
        }

        epi.Descricao = epiAtualizado.Descricao;
        epi.C_A = epiAtualizado.C_A;
        epi.Quantidade = epiAtualizado.Quantidade;

             _ctx.Epis.Update(epi);
        _ctx.SaveChanges();
        return Ok(epi);
    }
    //delete: Epilist/Epi/deletar/id
    [HttpDelete]
    [Route("Deletar/{id}")]
    public IActionResult DeletarEPI(int id)
    {
        Epi epiExistente = _ctx.Epis.FirstOrDefault(e => e.EpiID == id);
        if (epiExistente == null)
        {
            return NotFound("EPI não encontrado.");
        }
                _ctx.Epis.Remove(epiExistente);
        _ctx.SaveChanges();
        return NoContent();
    }
}

