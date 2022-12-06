using System.Security.Claims;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BCrypt.Net.BCrypt;

namespace Auth.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Proibida()
    {
        return View();
    }

    [Authorize]
    public IActionResult Restrita()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    public IActionResult Administrar()
    {
        return View();
        // return RedirectToAction("Index", "Usuario");
    }

    public IActionResult Login()
    {
        var model = new LoginViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var usuario = _db.Usuarios.FirstOrDefault(u => u.Email == login.Email);
        if (usuario is null)
        {
            ModelState.AddModelError("Email", "Usuário não encontrado.");
            return View(login);
        }


        var senhaEstaCorreta = Verify(login.Senha, usuario.Senha);

        if (senhaEstaCorreta)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            usuario.IsAdmin ? new Claim(ClaimTypes.Role, "admin") : null,
        };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = true,
                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),           
                IsPersistent = login.Lembrar,
            };

            //metodo que efetivamente realiza o login
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                        authProperties);

            Console.WriteLine($"Usuário {usuario.Email} logou às {DateTime.Now}.");

            if (returnUrl is null)
            {
                return RedirectToAction(nameof(Index));
            }

            return Redirect(returnUrl);
        }
        else
        {
            ModelState.AddModelError("Senha", "Senha não confere");
            return View(login);
        }
    }

    public async Task<IActionResult> Logout(string returnUrl = null)
    {
        //metodo que efetivamente realiza logout
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        if (returnUrl is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return Redirect(returnUrl);
    }

    public IActionResult Cadastrar()
    {
        var model = new UsuarioModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(UsuarioModel usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }
        //verifico se o email usado para cadastrar o usuario ja nao pertence a outro usuario
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
}