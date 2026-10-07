using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class PrestamosController : Controller
{
    private readonly PrestamoRepositorio _prestamos;

    public PrestamosController(PrestamoRepositorio prestamos)
    {
        _prestamos = prestamos;
    }

    // GET: /Prestamos/Reporte?desde=2026-01-01&hasta=2026-01-31
    public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
    {
        var hoy = DateTime.Today;
        var fechaDesde = (desde ?? hoy.AddDays(-30)).Date;
        var fechaHasta = (hasta ?? hoy).Date;

        ViewData["Desde"] = fechaDesde.ToString("yyyy-MM-dd");
        ViewData["Hasta"] = fechaHasta.ToString("yyyy-MM-dd");

        if (fechaDesde > fechaHasta)
        {
            ModelState.AddModelError(string.Empty, "La fecha \"desde\" no puede ser posterior a la fecha \"hasta\".");
            return View(Enumerable.Empty<PrestamoReporte>());
        }

        var resultado = await _prestamos.ReporteAsync(fechaDesde, fechaHasta);
        return View(resultado);
    }
}
