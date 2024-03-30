using System.ComponentModel.DataAnnotations;

namespace ClinicaWeb.Shared.Dtos.Appointments
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PracticianId { get; set; }
        [Required]
        public int ProcedureId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime AppointmentDateTime { get; set; }
    }
}
