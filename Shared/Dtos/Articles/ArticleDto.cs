using ClinicaWeb.Shared.Dtos.Photos;
using System.ComponentModel.DataAnnotations;

namespace ClinicaWeb.Shared.Dtos.Articles
{
    public class ArticleDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Subtitle { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Author { get; set; }
        public List<int> PhotoIds { get; set; } = new List<int>();
        public List<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
    }
}
