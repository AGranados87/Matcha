using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcha.Modelo
{
    [Table("Citas")]
    public class Cita
    {
        public int CitaID { get; set; }

        [Required]
        public int PacienteID { get; set; }   // FK a Usuario

        [Required]
        public int EmpleadoID { get; set; }   // FK a Empleado (psicólogo)

        [StringLength(500)]
        public string? Motivo { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(10)]
        public string Hora { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("PacienteID")]
        public Usuario Paciente { get; set; }

        [ForeignKey("EmpleadoID")]
        public Empleado Psicologo { get; set; }
    }
}
