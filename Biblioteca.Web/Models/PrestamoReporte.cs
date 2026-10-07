using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class PrestamoReporte
{
    public string Socio { get; set; } = string.Empty;
    public string Libro { get; set; } = string.Empty;

    [Display(Name = "Fecha de préstamo")]
    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }

    [Display(Name = "Fecha límite")]
    [DataType(DataType.Date)]
    public DateTime FechaLimite { get; set; }

    public string Estado { get; set; } = string.Empty;
}
