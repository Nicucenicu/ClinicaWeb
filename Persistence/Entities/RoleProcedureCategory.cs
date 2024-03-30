using ClinicaWeb.Shared.Enums;

namespace ClinicaWeb.Persistence.Entities
{
    public class RoleProcedureCategory
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public int ProcedureCategoryId { get; set; }

        public ProcedureCategory ProcedureCategory { get; set; }

    }   
}
