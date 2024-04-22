using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            //return _context.Categorias.ToList();
            return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            //AsNoTracking torna a consulta nao rastreável
            //(porem apenas pra consultas que nao vao ser alteradas)
            try
            {
                //Erro proposital para teste
                //throw new DataMisalignedException();
                return _context.Categorias.AsNoTracking().ToList();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tratar a sua solicitação");
            }

            
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            try
            {
                if (categoria == null)
                {
                    return NotFound($"Categoria com id = {id}, nao foi encontrada...");
                }
                return categoria;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tratar a sua solicitação");
            }

            
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            //aqui estou utilizando a verificação explícita
            if (categoria == null)
            {
                return BadRequest("Dados inválidos");
            }

            _context.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        
        //aqui eu tenho que analisar se o id que eu estou informando
        //é o mesmo id de categoria
        public ActionResult Put(int id, Categoria categoria)
        {
            if (categoria == null)
            {
                return BadRequest("Dados inválidos");
            }

            //aqui eu estou mudando o estado da categoria para 'modified'
            //utilizando o EntityFrameworkCore
            _context.Entry(categoria).State = EntityState.Modified; 
            _context.SaveChanges();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        { 
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com id= {id} não encontrada");
            }
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return Ok(categoria);
        }
    }
}
