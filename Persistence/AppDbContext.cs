using ClinicaWeb.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaWeb.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProcedureCategory> ProceduresCategories { get; set; }
        public DbSet<RoleProcedureCategory> RankProcedureCategories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<ArticlePhoto> ArticlesPhotos { get; set; }
        public DbSet<ResetPasswordField> ResetPasswordFields { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
    }
}
