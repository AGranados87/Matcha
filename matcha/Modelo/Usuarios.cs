using System.ComponentModel.DataAnnotations;

namespace matcha.Modelo
{
    public class Usuario
    {
        public int PacienteID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
        public int? EmpleadoID { get; set; }   // FK al psicólogo
        public string Psicologo{ get; set; }  // Nombre del psicólogo
    }
}