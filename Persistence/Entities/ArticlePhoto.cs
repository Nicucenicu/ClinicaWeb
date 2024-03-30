using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicaWeb.Persistence.Entities
{
    public class ArticlePhoto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }

    public class ArticlesConfiguration : IEntityTypeConfiguration<ArticlePhoto>
    {
        public void Configure(EntityTypeBuilder<ArticlePhoto> builder)
        {
            builder.HasOne(ap => ap.Article)
               .WithMany(p => p.ArticlePhotos)
               .HasForeignKey(ap => ap.ArticleId);

            builder.HasOne(ap => ap.Photo)
                .WithOne()
                .HasForeignKey<ArticlePhoto>(ap => ap.PhotoId);
        }
    }
}