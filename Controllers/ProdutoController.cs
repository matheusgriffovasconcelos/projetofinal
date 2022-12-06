using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Auth.Controllers;

[Authorize(Roles = "admin")]
public class ProdutoController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ProdutoController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var produtos = _db.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToList();
        return View(produtos);
    }

    private void CarregarCategorias(int? idCategoria = null)
    {
        var categorias = _db.Categorias.OrderBy(c => c.Nome).ToList();
        var categoriasSelectList = new SelectList(
            categorias, "Id", "Nome", idCategoria);
        ViewBag.Categorias = categoriasSelectList;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastrar()
    {
        CarregarCategorias();
        var produto = new ProdutoModel();
        return View(produto);
    }

    [AllowAnonymous]
    [Authorize]
    [HttpPost]
    public IActionResult Cadastrar(ProdutoModel produto)
    {
        if (!ModelState.IsValid)
        {
            CarregarCategorias(produto.IdCategoria);
            return View(produto);
        }
        _db.Produtos.Add(produto);
        if (_db.SaveChanges() > 0)
        {
            var caminhoImagem = $"{_env.WebRootPath}//img//produto//{produto.Id.ToString("D6")}.jpg";
            SalvarUploadImagemAsync(caminhoImagem, produto.ArquivoImagem).Wait();
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Alterar(int id)
    {
        var produto = _db.Produtos.Find(id);
        if (produto is null)
        {
            return RedirectToAction("Index");
        }
        CarregarCategorias(produto.IdCategoria);
        return View(produto);
    }

    [HttpPost]
    public IActionResult Alterar(int id, ProdutoModel produto)
    {
        var produtoOriginal = _db.Produtos.Find(id);
        if (produtoOriginal is null)
        {
            return RedirectToAction("Index");
        }
        ModelState.Remove("ArquivoImagem");
        if (!ModelState.IsValid)
        {
            CarregarCategorias(produto.IdCategoria);
            return View(produto);
        }
        produtoOriginal.Nome = produto.Nome;
        produtoOriginal.Estoque = produto.Estoque;
        produtoOriginal.Preco = produto.Preco;
        produtoOriginal.IdCategoria = produto.IdCategoria;
        _db.SaveChanges();
        if (produto.ArquivoImagem is not null)
        {
            var caminhoImagem = $"{_env.WebRootPath}//img//produto//{produto.Id.ToString("D6")}.jpg";
            SalvarUploadImagemAsync(caminhoImagem, produto.ArquivoImagem).Wait();
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Excluir(int id)
    {
        var produto = _db.Produtos.Find(id);
        if (produto is null)
        {
            return RedirectToAction("Index");
        }
        return View(produto);
    }

    [HttpPost]
    public IActionResult ProcessarExclusao(int id)
    {
        var produtoOriginal = _db.Produtos.Find(id);
        if (produtoOriginal is null)
        {
            return RedirectToAction("Index");
        }
        _db.Produtos.Remove(produtoOriginal);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public async Task<bool> SalvarUploadImagemAsync(
        string caminhoArquivoImagem, IFormFile imagem,
        bool salvarQuadrada = true)
    {
        if (imagem is null)
        {
            return false;
        }
        var ms = new MemoryStream();
        await imagem.CopyToAsync(ms);
        ms.Position = 0;
        var img = await Image.LoadAsync(ms);

        if (salvarQuadrada)
        {
            var tamanho = img.Size();
            var ladoMenor = (tamanho.Height < tamanho.Width) ? tamanho.Height : tamanho.Width;
            img.Mutate(i =>
                i.Resize(new ResizeOptions()
                {
                    Size = new Size(ladoMenor, ladoMenor),
                    Mode = ResizeMode.Crop
                })
            );
        }

        await img.SaveAsJpegAsync(caminhoArquivoImagem);
        return true;
    }
}