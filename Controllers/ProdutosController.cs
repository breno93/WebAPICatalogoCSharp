using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace APICatalogo.Controllers
{
    //[Route("[controller]")] é uma variável de espaço reservado que será substituída
    //pelo nome do controlador.
    //ProdutosController, então [Route("[controller]")] será equivalente a
    //[Route("Produtos")]
    [Route("[controller]")]

    //é um controlador da API. Ele habilita vários
    //comportamentos específicos da API, tratamento automático
    //de respostas HTTP, formatação de corpo de solicitação e resposta,
    //e tratamento de validação de modelo.
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //Uma variável AppDbContext chamada _context,
        //que será usada para acessar o banco de dados.
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {

            try
            {
                var produtos = _context.Produtos.AsNoTracking().ToList();

                if (produtos is null)
                {
                    return NotFound("Produtos não encontrados");
                }
                return produtos;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tratar a sua solicitação");
            }
            
        }

        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            try
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound("Produto não encontrado");
                }
                return produto;
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tratar a sua solicitação");
            }

            
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                return BadRequest("Dados inválidos");
            }

            //Aqui eu estou adicionando o produto
            _context.Produtos.Add(produto); 

            //Aqui eu estou persistindo no banco de dados
            _context.SaveChanges();

            //CreatedAtRouteResult recurso do ASPNET que retorna
            //o 201 created no header
            return new CreatedAtRouteResult("ObterProduto", 
                new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest("Dados inválidos");
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            //var produto = _context.Produtos.Find(id);

            if (produto is null)
            {
                return NotFound("Produto não localizado");
            }
            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(produto);
        }
    }
}
