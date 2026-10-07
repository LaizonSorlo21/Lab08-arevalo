using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class Libro
{
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es obligatorio.")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 20 caracteres.")]
    [Display(Name = "ISBN")]
    public string ISBN { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un autor.")]
    [Display(Name = "Autor")]
    public int AutorId { get; set; }

    [Display(Name = "Autor")]
    public string? NombreAutor { get; set; }

    [Required(ErrorMessage = "Ingrese la cantidad de ejemplares.")]
    [Range(0, 10000, ErrorMessage = "Los ejemplares deben estar entre 0 y 10000.")]
    [DataType(DataType.Text)]
    public int Ejemplares { get; set; }

    public bool Activo { get; set; } = true;
}
