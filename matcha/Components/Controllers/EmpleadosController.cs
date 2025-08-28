using Dapper;
using Microsoft.Data.SqlClient;
using matcha.Modelo;

namespace matcha.Components.Controllers
{
    public class EmpleadosController
    {
        private readonly string _connectionString;

        public EmpleadosController(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var empleados = await conn.QueryAsync<Empleado>(@"
                SELECT e.EmpleadoID, e.UserName, e.Email, e.Activo, e.RolID, r.Nombre AS RolNombre
                FROM Empleados e
                LEFT JOIN Roles r ON e.RolID = r.ID
                ORDER BY e.EmpleadoID ASC");
            return empleados.ToList();
        }

        public async Task<List<Rol>> GetRolesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var roles = await conn.QueryAsync<Rol>("SELECT ID, Nombre FROM Roles");
            return roles.ToList();
        }

        public async Task InsertarEmpleadoAsync(Empleado empleado)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@"
                INSERT INTO Empleados (UserName, Email, PasswordHash, Salt, Activo, RolID)
                VALUES (@UserName, @Email, @PasswordHash, @Salt, @Activo, @RolID)",
                empleado);
        }

        public async Task ActualizarEmpleadoAsync(Empleado empleado)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@"
                UPDATE Empleados
                SET UserName=@UserName, Email=@Email, Activo=@Activo, RolID=@RolID
                WHERE EmpleadoID=@EmpleadoID",
                empleado);
        }

        public async Task EliminarEmpleadoAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DELETE FROM Empleados WHERE EmpleadoID=@EmpleadoID", new { EmpleadoID = id });
        }
    }
}
