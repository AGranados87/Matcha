using Dapper;
using Microsoft.Data.SqlClient;
using matcha.Modelo;

namespace matcha.Components.Controllers
{
    public class UsuariosController
    {
        private readonly string _connectionString;

        public UsuariosController(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var usuarios = await conn.QueryAsync<Usuario>(@"
                SELECT
                    u.PacienteID,
                    u.Nombre,
                    u.Email,
                    u.EmpleadoID,
                    e.UserName AS Psicologo
                FROM Usuarios u
                LEFT JOIN Empleados e ON u.EmpleadoID = e.EmpleadoID
                ORDER BY u.PacienteID ASC;");
            return usuarios.ToList();
        }

        public async Task<List<Empleado>> GetPsicologosAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var psicologos = await conn.QueryAsync<Empleado>(@"
                SELECT EmpleadoID, UserName 
                FROM Empleados 
                WHERE RolID = 2
                ORDER BY UserName;");
            return psicologos.ToList();
        }

        public async Task InsertarUsuarioAsync(Usuario usuario)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@"
                INSERT INTO Usuarios (Nombre, Email, EmpleadoID)
                VALUES (@Nombre, @Email, @EmpleadoID);",
                usuario);
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(@"
                UPDATE Usuarios
                SET Nombre = @Nombre, Email = @Email, EmpleadoID = @EmpleadoID
                WHERE PacienteID = @PacienteID;",
                usuario);
        }

        public async Task EliminarUsuarioAsync(int pacienteId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DELETE FROM Usuarios WHERE PacienteID = @PacienteID", new { PacienteID = pacienteId });
        }
    }
}
