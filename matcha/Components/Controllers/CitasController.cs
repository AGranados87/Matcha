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

        // --- Pacientes ---
        public async Task<List<Usuario>> GetPacientesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"SELECT u.PacienteID, u.Nombre, u.Email, u.EmpleadoID, e.UserName AS Psicologo
                        FROM Usuarios u
                        LEFT JOIN Empleados e ON u.EmpleadoID = e.EmpleadoID";
            var result = await conn.QueryAsync<Usuario>(sql);
            return result.ToList();
        }

        // --- Empleados (psicólogos) ---
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"SELECT e.EmpleadoID, e.UserName, e.Email, e.PasswordHash, e.FechaCreacion, 
                               e.Activo, e.RolID, r.Nombre AS RolNombre
                        FROM Empleados e
                        LEFT JOIN Roles r ON e.RolID = r.ID
                        WHERE e.Activo = 1";
            var result = await conn.QueryAsync<Empleado>(sql);
            return result.ToList();
        }

        // --- Todas las citas ---
        public async Task<List<Cita>> GetCitasAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<Cita>("SELECT * FROM Citas");
            return result.ToList();
        }

        // --- Citas por psicólogo ---
        public async Task<List<Cita>> GetCitasPorEmpleadoAsync(int empleadoId)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<Cita>(
                "SELECT * FROM Citas WHERE EmpleadoID = @empleadoId",
                new { empleadoId });
            return result.ToList();
        }

        // --- Crear cita ---
        public async Task InsertarCitaAsync(Cita cita)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Citas (PacienteID, EmpleadoID, Motivo, Fecha, Hora)
                        VALUES (@PacienteID, @EmpleadoID, @Motivo, @Fecha, @Hora)";
            await conn.ExecuteAsync(sql, cita);
        }
    }
}
