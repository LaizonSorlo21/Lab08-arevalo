using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class SociosController : Controller
{
    private readonly SocioRepositorio _socios;

    public SociosController(SocioRepositorio socios)
    {
        _socios = socios;
    }

    // GET: /Socios
    public async Task<IActionResult> Index()
    {
        var lista = await _socios.ListarActivosAsync();
        return View(lista);
    }

    // GET: /Socios/Create
    public IActionResult Create()
    {
        return View(new Socio());
    }

    // POST: /Socios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Socio socio)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _socios.InsertarAsync(socio);
            if (resultado > 0)
            {
                TempData["Mensaje"] = $"El socio \"{socio.Nombre}\" se registró correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(nameof(Socio.DNI), "Ya existe un socio registrado con ese DNI.");
        }

        return View(socio);
    }
}
