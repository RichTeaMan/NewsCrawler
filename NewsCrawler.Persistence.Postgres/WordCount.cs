using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.Persistence.Postgres
{
    public class WordCount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MAX_WORD_LENGTH)]
        public string Word { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        [MaxLength(Constants.MAX_NEWS_SOURCE_LENGTH)]
        public string NewsSource { get; set; }
    }
}
