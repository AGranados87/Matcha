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
                INSERT INTO Empleados (UserName, Email, PasswordHash, Activo, RolID)
                VALUES (@UserName, @Email, @PasswordHash, @Activo, @RolID)",
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

        public async Task<Empleado?> AutenticarAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            using var conn = new SqlConnection(_connectionString);

            var empleado = await conn.QueryFirstOrDefaultAsync<Empleado>(
                "SELECT * FROM Empleados WHERE Email = @Email AND Activo = 1",
                new { Email = email.Trim() });

            if (empleado == null)
                return null;

            var hash = HashPassword(password.Trim());

            return string.Equals(hash, empleado.PasswordHash, StringComparison.Ordinal) ? empleado : null;
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                password = "";

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);  // <-- igual que tu DB
        }


    }
}
