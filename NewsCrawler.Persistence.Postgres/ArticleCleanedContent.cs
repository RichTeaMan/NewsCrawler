using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsCrawler.Persistence.Postgres
{
    public class ArticleCleanedContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string CleanedContent { get; set; }
    }
}
