﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsCrawler.Persistence
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MAX_URL_LENGTH)]
        public string Url { get; set; }

        [MaxLength(Constants.MAX_TITLE_LENGTH)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        [Required]
        public DateTimeOffset RecordedDate { get; set; }

        [Required]
        public bool IsIndexPage { get; set; }
    }
}
