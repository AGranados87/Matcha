namespace Matcha.Modelo;

public class CitaSchedulerDto
{
    public int CitaID { get; set; }
    public int PacienteID { get; set; }
    public string PacienteNombre { get; set; } = "";
    public int EmpleadoID { get; set; }
    public string EmpleadoNombre { get; set; } = "";
    public string Text { get; set; } = "";           // Título del evento
    public string? Descripcion { get; set; }         // Tooltip / detalle
    public DateTime Inicio { get; set; }             // StartProperty
    public DateTime Fin { get; set; }                // EndProperty
    public bool AllDay { get; set; }                
    public string? Location { get; set; }
}

public class CitaUpsert
{
    public int? CitaID { get; set; }
    public int PacienteID { get; set; }
    public int EmpleadoID { get; set; }
    public DateTime Inicio { get; set; }             
    public int DuracionMin { get; set; } = 60;       
    public string? Motivo { get; set; }
}