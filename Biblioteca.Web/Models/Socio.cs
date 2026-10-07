using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class Socio
{
    public int SocioId { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    [StringLength(8)]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public bool Activo { get; set; } = true;
}
