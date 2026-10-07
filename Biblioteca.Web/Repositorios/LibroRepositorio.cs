using System.Data;
using Biblioteca.Web.Models;
using Dapper;

namespace Biblioteca.Web.Repositorios;

public class LibroRepositorio : RepositorioBase
{
    public LibroRepositorio(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Libro>> ListarAsync(string? titulo = null)
    {
        await using var cn = CrearConexion();
        if (string.IsNullOrWhiteSpace(titulo))
        {
            return await cn.QueryAsync<Libro>("dbo.usp_Libros_Listar", commandType: CommandType.StoredProcedure);
        }

        return await cn.QueryAsync<Libro>(
            "dbo.usp_Libros_BuscarPorTitulo",
            new { Titulo = titulo.Trim() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        await using var cn = CrearConexion();
        return await cn.QuerySingleOrDefaultAsync<Libro>(
            "dbo.usp_Libros_ObtenerPorId",
            new { LibroId = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Autor>> ListarAutoresActivosAsync()
    {
        await using var cn = CrearConexion();
        return await cn.QueryAsync<Autor>("dbo.usp_Autores_ListarActivos", commandType: CommandType.StoredProcedure);
    }

    /// <summary>Devuelve el nuevo LibroId, o -1 si el ISBN ya existe.</summary>
    public async Task<int> InsertarAsync(Libro libro)
    {
        await using var cn = CrearConexion();
        return await cn.ExecuteScalarAsync<int>(
            "dbo.usp_Libros_Insertar",
            new { libro.Titulo, libro.ISBN, libro.AutorId, libro.Ejemplares },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>Devuelve 1 si actualizó, 0 si no existe, -1 si el ISBN pertenece a otro libro.</summary>
    public async Task<int> ActualizarAsync(Libro libro)
    {
        await using var cn = CrearConexion();
        return await cn.ExecuteScalarAsync<int>(
            "dbo.usp_Libros_Actualizar",
            new { libro.LibroId, libro.Titulo, libro.ISBN, libro.AutorId, libro.Ejemplares },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>Eliminación lógica (Activo = 0). Devuelve true si se modificó el libro.</summary>
    public async Task<bool> EliminarLogicoAsync(int id)
    {
        await using var cn = CrearConexion();
        var filas = await cn.ExecuteScalarAsync<int>(
            "dbo.usp_Libros_EliminarLogico",
            new { LibroId = id },
            commandType: CommandType.StoredProcedure);
        return filas > 0;
    }
}
