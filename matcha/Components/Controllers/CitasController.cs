// Controllers/CitasController.cs
using Dapper;
using Microsoft.Data.SqlClient;
using Matcha.Modelo;
using System.Data;

namespace Matcha.Controllers
{
    public interface ICitasService
    {
        Task<IEnumerable<CitaSchedulerDto>> GetAsync(DateTime? desde, DateTime? hasta, int? empleadoId);
        Task<CitaSchedulerDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CitaUpsert dto);
        Task<int> UpdateAsync(int id, CitaUpsert dto);
        Task<int> DeleteAsync(int id);
    }

    // Ojo: no es un Web API; es un servicio DI con nombre "Controller"
    public class CitasController : ICitasService
    {
        private readonly string _cs;
        public CitasController(IConfiguration cfg) => _cs = cfg.GetConnectionString("SqlConnection")!;
        private IDbConnection Create() => new SqlConnection(_cs);

        public async Task<IEnumerable<CitaSchedulerDto>> GetAsync(DateTime? desde, DateTime? hasta, int? empleadoId)
        {
            var from = desde ?? DateTime.Today.AddDays(-3);
            var to = hasta ?? DateTime.Today.AddDays(10);

            const string sqlBase = @"
                SELECT CitaID, PacienteID, PacienteNombre, EmpleadoID, EmpleadoNombre,
                       [Text], Descripcion, Inicio, Fin, AllDay, Location
                FROM dbo.vwCitasScheduler WITH (NOLOCK)
                WHERE Inicio < @Hasta AND Fin > @Desde";

            var sql = empleadoId.HasValue
                ? sqlBase + " AND EmpleadoID = @EmpleadoID ORDER BY Inicio"
                : sqlBase + " ORDER BY Inicio";

            using var cn = Create();
            var rows = await cn.QueryAsync<CitaSchedulerDto>(sql, new { Desde = from, Hasta = to, EmpleadoID = empleadoId });
            return rows;
        }

        public async Task<CitaSchedulerDto?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT TOP 1 CitaID, PacienteID, PacienteNombre, EmpleadoID, EmpleadoNombre,
                       [Text], Descripcion, Inicio, Fin, AllDay, Location
                FROM dbo.vwCitasScheduler WITH (NOLOCK)
                WHERE CitaID = @Id";

            using var cn = Create();
            return await cn.QueryFirstOrDefaultAsync<CitaSchedulerDto>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(CitaUpsert dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Inicio == default) throw new ArgumentException("Inicio requerido.", nameof(dto));

            var fecha = dto.Inicio.Date;
            var hora = dto.Inicio.ToString("HH:mm");

            const string sql = @"
                INSERT INTO dbo.Citas (PacienteID, EmpleadoID, Motivo, Fecha, Hora, FechaCreacion)
                VALUES (@PacienteID, @EmpleadoID, @Motivo, @Fecha, @Hora, SYSUTCDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            using var cn = Create();
            var newId = await cn.ExecuteScalarAsync<int>(sql, new
            {
                dto.PacienteID,
                dto.EmpleadoID,
                Motivo = dto.Motivo ?? "",
                Fecha = fecha,
                Hora = hora
            });
            return newId;
        }

        public async Task<int> UpdateAsync(int id, CitaUpsert dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var fecha = dto.Inicio.Date;
            var hora = dto.Inicio.ToString("HH:mm");

            const string sql = @"
                UPDATE dbo.Citas
                   SET PacienteID = @PacienteID,
                       EmpleadoID = @EmpleadoID,
                       Motivo     = @Motivo,
                       Fecha      = @Fecha,
                       Hora       = @Hora
                 WHERE CitaID    = @CitaID";

            using var cn = Create();
            return await cn.ExecuteAsync(sql, new
            {
                CitaID = id,
                dto.PacienteID,
                dto.EmpleadoID,
                Motivo = dto.Motivo ?? "",
                Fecha = fecha,
                Hora = hora
            });
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string sql = @"DELETE FROM dbo.Citas WHERE CitaID = @Id;";
            using var cn = Create();
            return await cn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
