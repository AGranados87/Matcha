using System.ComponentModel.DataAnnotations.Schema;

namespace matcha.Modelo
{
    [Table("Empleados")]
    public class Empleado
    {
        public int EmpleadoID { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public int? RolID { get; set; }  
        public string? RolNombre { get; set; } 
    }

}
