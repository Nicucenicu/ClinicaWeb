using System.ComponentModel.DataAnnotations;

namespace ClinicaWeb.Shared.Dtos.Appointments
{
    public class CreateAppointmentDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PracticianId { get; set; }
        [Required]
        public int ProcedureId { get; set; }
        [Required]
        public DateTime AppointmentDateTime { get; set; }
    }
}
