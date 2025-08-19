using Dapper;
using matcha.Modelo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace matcha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsuariosController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection()
        {
            return new SqlConnection(_config.GetConnectionString("SqlConnection"));
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            using var db = Connection();
            var sql = "SELECT Id, Nombre, Email FROM Usuarios";
            var usuarios = await db.QueryAsync<Usuario>(sql);
            return Ok(usuarios);
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            using var db = Connection();
            var sql = "SELECT Id, Nombre, Email FROM Usuarios WHERE Id = @Id";
            var usuario = await db.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });

            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult> PostUsuario(Usuario usuario)
        {
            using var db = Connection();
            var sql = @"INSERT INTO Usuarios (Nombre, Email)
                    VALUES (@Nombre, @Email);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = await db.ExecuteScalarAsync<int>(sql, usuario);
            usuario.Id = id;

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();

            using var db = Connection();
            var sql = "UPDATE Usuarios SET Nombre=@Nombre, Email=@Email WHERE Id=@Id";

            var rows = await db.ExecuteAsync(sql, usuario);
            if (rows == 0) return NotFound();

            return NoContent();
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            using var db = Connection();
            var sql = "DELETE FROM Usuarios WHERE Id=@Id";

            var rows = await db.ExecuteAsync(sql, new { Id = id });
            if (rows == 0) return NotFound();

            return NoContent();
        }
    }
}
