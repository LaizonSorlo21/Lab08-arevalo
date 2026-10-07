using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public abstract class RepositorioBase
{
    private readonly string _cadenaConexion;

    protected RepositorioBase(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("BibliotecaDB")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
    }

    protected SqlConnection CrearConexion() => new(_cadenaConexion);
}
