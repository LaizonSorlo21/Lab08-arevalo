using System.Data;
using Biblioteca.Web.Models;
using Dapper;

namespace Biblioteca.Web.Repositorios;

public class SocioRepositorio : RepositorioBase
{
    public SocioRepositorio(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Socio>> ListarActivosAsync()
    {
        await using var cn = CrearConexion();
        return await cn.QueryAsync<Socio>("dbo.usp_Socios_ListarActivos", commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        await using var cn = CrearConexion();
        var existe = await cn.ExecuteScalarAsync<int>(
            "dbo.usp_Socios_ExisteDNI",
            new { DNI = dni },
            commandType: CommandType.StoredProcedure);
        return existe == 1;
    }

    /// <summary>Devuelve el nuevo SocioId, o -1 si el DNI ya existe.</summary>
    public async Task<int> InsertarAsync(Socio socio)
    {
        await using var cn = CrearConexion();
        return await cn.ExecuteScalarAsync<int>(
            "dbo.usp_Socios_Insertar",
            new { socio.DNI, socio.Nombre, Email = string.IsNullOrWhiteSpace(socio.Email) ? null : socio.Email.Trim() },
            commandType: CommandType.StoredProcedure);
    }
}
