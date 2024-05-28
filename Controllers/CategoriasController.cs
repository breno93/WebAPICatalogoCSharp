using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CategoriasController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public string GetValores()
        {
            var valor1 = _configuration["chave1"];
            var valor2 = _configuration["chave2"];

            var secao1 = _configuration["secao1:chave2"];

            return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
        }

        //com FromServices
        [HttpGet("UsandoFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoFromService([FromServices] IMeuServico
            meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        //Sem FromServices
        [HttpGet("UsandoSemFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoSemFromService(IMeuServico
            meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
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
            throw new Exception("Exceção ao retornar a categoria pelo id");
            //string[] teste = null;
            //if (teste.Length > 0)
            //{
                
            //}

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
