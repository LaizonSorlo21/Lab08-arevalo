using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers;

public class LibrosController : Controller
{
    private readonly LibroRepositorio _libros;

    public LibrosController(LibroRepositorio libros)
    {
        _libros = libros;
    }

    // GET: /Libros?buscar=texto
    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Title"] = "Libros";
        ViewData["Buscar"] = buscar;
        var lista = await _libros.ListarAsync(buscar);
        return View(lista);
    }

    // GET: /Libros/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var libro = await _libros.ObtenerPorIdAsync(id);
        if (libro is null) return NotFound();
        return View(libro);
    }

    // GET: /Libros/Create
    public async Task<IActionResult> Create()
    {
        await CargarAutoresAsync();
        return View(new Libro { Ejemplares = 1 });
    }

    // POST: /Libros/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Libro libro)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _libros.InsertarAsync(libro);
            if (resultado > 0)
            {
                TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe un libro con ese ISBN.");
        }

        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    // GET: /Libros/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var libro = await _libros.ObtenerPorIdAsync(id);
        if (libro is null) return NotFound();

        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    // POST: /Libros/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Libro libro)
    {
        if (id != libro.LibroId) return BadRequest();

        if (ModelState.IsValid)
        {
            var resultado = await _libros.ActualizarAsync(libro);
            if (resultado == 1)
            {
                TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se actualizó correctamente.";
                return RedirectToAction(nameof(Index));
            }

            if (resultado == 0) return NotFound();

            ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe otro libro con ese ISBN.");
        }

        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    // GET: /Libros/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _libros.ObtenerPorIdAsync(id);
        if (libro is null) return NotFound();
        return View(libro);
    }

    // POST: /Libros/Delete/5  (eliminación lógica: Activo = 0)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _libros.EliminarLogicoAsync(id);
        TempData["Mensaje"] = eliminado
            ? "El libro se eliminó correctamente."
            : "No se encontró el libro a eliminar.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarAutoresAsync(int? autorSeleccionado = null)
    {
        var autores = await _libros.ListarAutoresActivosAsync();
        ViewData["Autores"] = new SelectList(autores, nameof(Autor.AutorId), nameof(Autor.Nombre), autorSeleccionado);
    }
}
