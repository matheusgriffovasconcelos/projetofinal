using System.Linq;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth.Controllers;

[Authorize(Roles = "admin")]
public class CategoriaController : Controller
{
    private readonly AppDbContext _db;
    public CategoriaController(AppDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var categorias = _db.Categorias.AsNoTracking().OrderBy(x => x.Nome).ToList();
        return View(categorias);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var categoria = new CategoriaModel();
        return View(categoria);
    }

    [HttpPost]
    public IActionResult Cadastrar(CategoriaModel categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }
        _db.Categorias.Add(categoria);
        _db.SaveChanges();
        return RedirectToAction("Index");


    }

    [HttpGet]
    public IActionResult Alterar(int id)
    {
        var categoria = _db.Categorias.Find(id);
        if (categoria is null)
        {
            return RedirectToAction("Index");
        }
        return View(categoria);
    }

    [HttpPost]
    public IActionResult Alterar(int id, CategoriaModel categoria)
    {
        var categoriaOriginal = _db.Categorias.Find(id);
        if (categoriaOriginal is null)
        {
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
        {
            return View(categoria);
        }
        categoriaOriginal.Nome = categoria.Nome;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Excluir(int id)
    {
        var categoria = _db.Categorias.Find(id);
        if (categoria is null)
        {
            return RedirectToAction("Index");
        }
        return View(categoria);
    }

    [HttpPost]
    public IActionResult ProcessarExclusao(int id)
    {
        var categoriaOriginal = _db.Categorias.Find(id);
        if (categoriaOriginal is null)
        {
            return RedirectToAction("Index");
        }

        _db.Categorias.Remove(categoriaOriginal);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

}