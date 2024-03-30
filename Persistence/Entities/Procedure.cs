﻿namespace ClinicaWeb.Persistence.Entities
{
    public class Procedure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }

        public ProcedureCategory ProcedureCategory { get; set; }
    }
}
