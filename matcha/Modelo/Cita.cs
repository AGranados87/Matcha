using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcha.Modelo
{
    [Table("Citas")]
    public class Cita
    {
        public int CitaID { get; set; }

        public int PacienteID { get; set; }   // FK a Usuario

        public int EmpleadoID { get; set; }   // FK a Empleado (psicólogo)

        [StringLength(500)]
        public string? Motivo { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(10)]
        public string Hora { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("PacienteID")]
        public Usuario Paciente { get; set; }

        [ForeignKey("EmpleadoID")]
        public Empleado Psicologo { get; set; }
    }
}
