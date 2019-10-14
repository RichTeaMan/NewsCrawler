using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.Persistence.Postgres
{
    public class Source
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MAX_NEWS_SOURCE_LENGTH)]
        public string Name { get; set; }
    }
}
