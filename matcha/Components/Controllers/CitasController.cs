using Dapper;
using Microsoft.Data.SqlClient;
using matcha.Modelo;

namespace matcha.Components.Services
{
    public class CitasController
    {
        private readonly string _connectionString;

        public CitasController(string connectionString)
        {
            _connectionString = connectionString;
        }

       
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
SELECT e.EmpleadoID, e.UserName, e.Email, e.RolID, r.Nombre AS RolNombre
FROM Empleados e
LEFT JOIN Roles r ON e.RolID = r.ID
WHERE e.Activo = 1 AND e.RolID = 2
ORDER BY e.UserName;";
            var result = await conn.QueryAsync<Empleado>(sql);
            return result.ToList();
        }

        
        public async Task<List<Usuario>> GetPacientesPorPsicologoAsync(int empleadoId)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
SELECT u.PacienteID, u.Nombre, u.Email, u.EmpleadoID
FROM Usuarios u
WHERE u.EmpleadoID = @empleadoId
   OR u.EmpleadoID IS NULL
ORDER BY CASE WHEN u.EmpleadoID = @empleadoId THEN 0 ELSE 1 END, u.Nombre;";
            var result = await conn.QueryAsync<Usuario>(sql, new { empleadoId });
            return result.ToList();
        }

      
        public async Task<List<Usuario>> GetPacientesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
SELECT u.PacienteID, u.Nombre, u.Email, u.EmpleadoID,
       ISNULL(e.UserName,'') AS Psicologo
FROM Usuarios u
LEFT JOIN Empleados e ON u.EmpleadoID = e.EmpleadoID
ORDER BY u.Nombre;";
            var result = await conn.QueryAsync<Usuario>(sql);
            return result.ToList();
        }

      
        public async Task<List<Cita>> GetCitasPorEmpleadoAsync(int empleadoId)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<Cita>(@"
SELECT CitaID, PacienteID, EmpleadoID, Motivo, Fecha, Hora, FechaCreacion
FROM Citas
WHERE EmpleadoID = @empleadoId
ORDER BY Fecha, Hora;", new { empleadoId });
            return result.ToList();
        }

        public async Task InsertarCitaAsync(Cita cita)
        {
            using var conn = new SqlConnection(_connectionString);

            // Normaliza hora "HH:mm"
            var hora = (cita.Hora ?? "").Trim();
            if (hora.Length >= 5) hora = hora[..5];

            // Check anti-solape
            var existe = await conn.ExecuteScalarAsync<int>(@"
SELECT COUNT(1)
FROM Citas
WHERE EmpleadoID = @EmpleadoID
  AND Fecha = @Fecha
  AND Hora = @Hora;",
                new { cita.EmpleadoID, Fecha = cita.Fecha.Date, Hora = hora });

            if (existe > 0)
                throw new InvalidOperationException("Ese horario ya está ocupado para este psicólogo.");

            await conn.ExecuteAsync(@"
INSERT INTO Citas (PacienteID, EmpleadoID, Motivo, Fecha, Hora)
VALUES (@PacienteID, @EmpleadoID, @Motivo, @Fecha, @Hora);",
                new
                {
                    cita.PacienteID,
                    cita.EmpleadoID,
                    cita.Motivo,
                    Fecha = cita.Fecha.Date,
                    Hora = hora
                });
        }
    }
}
