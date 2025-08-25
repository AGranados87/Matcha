using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcha.Modelo
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        public int EmpleadoID { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string Salt { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        public bool Activo { get; set; } = true;
    }
}
