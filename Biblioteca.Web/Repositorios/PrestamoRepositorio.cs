using System.Data;
using Biblioteca.Web.Models;
using Dapper;

namespace Biblioteca.Web.Repositorios;

public class PrestamoRepositorio : RepositorioBase
{
    public PrestamoRepositorio(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<PrestamoReporte>> ReporteAsync(DateTime desde, DateTime hasta)
    {
        await using var cn = CrearConexion();
        return await cn.QueryAsync<PrestamoReporte>(
            "dbo.usp_Prestamos_Reporte",
            new { Desde = desde.Date, Hasta = hasta.Date },
            commandType: CommandType.StoredProcedure);
    }
}
