namespace ClinicaWeb.Persistence.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId {  get; set; }
        public int PracticianId { get; set; }
        public int ProcedureId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime AppointmentDateTime { get; set; }

        public ICollection<User> Users { get; set; }
        public Procedure Procedure { get; set; }

    }
}
