using System.Linq;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Auth.Controllers;
using static BCrypt.Net.BCrypt;

[Authorize(Roles = "admin")]
public class UsuarioController : Controller
{
    private readonly AppDbContext _db;

    public UsuarioController(AppDbContext db)
    {
        _db = db;
    }

    
    public IActionResult Index()
    {

        var usuarios = _db.Usuarios.AsNoTracking().OrderBy(x => x.Nome).ToList();
        return View(usuarios);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var usuario = new UsuarioModel();
        return View(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(UsuarioModel usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        var existente = _db.Usuarios.Any(u => u.Email == usuario.Email);
        if (existente)
        {
            ModelState.AddModelError("Email", "Já existe um usuário cadastrado com este e-mail.");
            return View(usuario);
        }

        usuario.Senha = HashPassword(usuario.Senha, 10);
        await _db.Usuarios.AddAsync(usuario);
        if (_db.SaveChanges() > 0)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError(string.Empty,
                "Ocorreu um erro desconhecido ao cadastrar seus dados. Tente novamente mais tarde.");
            return View(usuario);
        }
    }

    [HttpGet]
    public IActionResult Alterar(int id)
    {

        var usuario = _db.Usuarios.Find(id);
        if (usuario is null)
        {
            return RedirectToAction("Index");
        }
        return View(usuario);
    }

    [HttpPost]
    public IActionResult Alterar(int id, UsuarioModel usuario)
    {
        var usuarioOriginal = _db.Usuarios.Find(id);
        if (usuarioOriginal is null)
        {
            return RedirectToAction("Index");
        }

        usuarioOriginal.Nome = usuario.Nome;
        usuarioOriginal.Email = usuario.Email;
        usuarioOriginal.IsAdmin = usuario.IsAdmin;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Excluir(int id)
    {
        var usuarios = _db.Usuarios.Find(id);
        if (usuarios is null)
        {
            return RedirectToAction("Index");
        }
        return View(usuarios);
    }

    [HttpPost]
    public IActionResult ProcessarExclusao(int idUsuario)
    {
        var usuarioOriginal = _db.Usuarios.Find(idUsuario);
        if (usuarioOriginal is null)
        {
            return RedirectToAction("Index");
        }

        _db.Usuarios.Remove(usuarioOriginal);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}