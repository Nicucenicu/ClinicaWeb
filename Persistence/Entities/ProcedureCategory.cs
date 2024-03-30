namespace ClinicaWeb.Persistence.Entities
{
    public class ProcedureCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Procedure> Procedures { get; set; }
    }
}
