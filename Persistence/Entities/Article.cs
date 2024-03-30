namespace ClinicaWeb.Persistence.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public ICollection<ArticlePhoto> ArticlePhotos { get; set; }
    }
}
