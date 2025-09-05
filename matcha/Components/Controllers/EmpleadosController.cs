using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using matcha.Modelo;

namespace matcha.Components.Controllers
{
    public class EmpleadosController
    {
        private readonly string _connectionString;
        private readonly PasswordHasher<Empleado> _passwordHasher;

        public EmpleadosController(string connectionString)
        {
            _connectionString = connectionString;
            _passwordHasher = new PasswordHasher<Empleado>();
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var empleados = await conn.QueryAsync<Empleado>(@"
                SELECT e.EmpleadoID, e.UserName, e.Email, e.PasswordHash, e.Activo, e.RolID, r.Nombre AS RolNombre
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

        public async Task InsertarEmpleadoAsync(Empleado empleado, string password)
        {
            // Hashear la contraseña
            empleado.PasswordHash = _passwordHasher.HashPassword(empleado, password);

            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@"
                INSERT INTO Empleados (UserName, Email, PasswordHash, Activo, RolID)
                VALUES (@UserName, @Email, @PasswordHash, @Activo, @RolID)",
                empleado);
        }

        public async Task ActualizarEmpleadoAsync(Empleado empleado, string? password = null)
        {
            using var conn = new SqlConnection(_connectionString);

            // Si se proporciona nueva contraseña, actualizar todo incluyendo password
            if (!string.IsNullOrWhiteSpace(password))
            {
                empleado.PasswordHash = _passwordHasher.HashPassword(empleado, password);
                await conn.ExecuteAsync(@"
                    UPDATE Empleados
                    SET UserName=@UserName, Email=@Email, Activo=@Activo, RolID=@RolID, 
                        PasswordHash=@PasswordHash
                    WHERE EmpleadoID=@EmpleadoID",
                    empleado);
            }
            else
            {
                // Si no se proporciona nueva contraseña, no actualizar el PasswordHash
                await conn.ExecuteAsync(@"
                    UPDATE Empleados
                    SET UserName=@UserName, Email=@Email, Activo=@Activo, RolID=@RolID
                    WHERE EmpleadoID=@EmpleadoID",
                    empleado);
            }
        }

        public async Task EliminarEmpleadoAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DELETE FROM Empleados WHERE EmpleadoID=@EmpleadoID", new { EmpleadoID = id });
        }

        public async Task<Empleado?> AutenticarAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Email o password vacíos");
                    return null;
                }

                using var conn = new SqlConnection(_connectionString);

                var empleado = await conn.QueryFirstOrDefaultAsync<Empleado>(
                    "SELECT EmpleadoID, UserName, Email, PasswordHash, Activo, RolID FROM Empleados WHERE Email = @Email AND Activo = 1",
                    new { Email = email.Trim() });

                if (empleado == null)
                {
                    Console.WriteLine($"No se encontró empleado con email: {email}");
                    return null;
                }

                if (string.IsNullOrEmpty(empleado.PasswordHash))
                {
                    Console.WriteLine("PasswordHash está vacío en la base de datos");
                    return null;
                }

                Console.WriteLine($"Empleado encontrado: {empleado.Email}, verificando contraseña...");

                var resultado = _passwordHasher.VerifyHashedPassword(empleado, empleado.PasswordHash, password.Trim());

                Console.WriteLine($"Resultado de verificación: {resultado}");

                return resultado == PasswordVerificationResult.Success ? empleado : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AutenticarAsync: {ex.Message}");
                return null;
            }
        }
    }
}