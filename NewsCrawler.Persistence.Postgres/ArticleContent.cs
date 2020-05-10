using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace NewsCrawler.Persistence.Postgres
{
#pragma warning disable CS0612 // Type or member is obsolete
    public class ArticleContent
    {
        [Key]
        public int Id { get; set; }

        [Column("Content", TypeName = "text")]
        [Obsolete]
        public string TextContent { get; set; }


        [Column]
        public byte[] CompressedContent { get; set; }

        [Required]
        public CompressionType CompressionType { get; set; } = CompressionType.Gzip;

        [NotMapped]
        public string Content
        {
            get
            {
                if (CompressionType == CompressionType.None)
                {
                    return TextContent;
                }
                else
                {
                    var compressor = new GzipTextCompressor();
                    return compressor.Decompress(CompressedContent).Result;
                }
            }
            set
            {
                if (CompressionType == CompressionType.None)
                {
                    TextContent = value;
                }
                else
                {
                    var compressor = new GzipTextCompressor();
                    CompressedContent = compressor.Compress(value).Result;
                }
            }
        }

        /// <summary>
        /// Determines if compression is required and applies. Returns true if this record should be saved.
        /// </summary>
        public async Task<bool> UpdateCompression()
        {
            bool updateRequired = false;
            if (CompressionType == CompressionType.None)
            {
                var compressor = new GzipTextCompressor();
                var compressed = await compressor.Compress(TextContent);
                TextContent = null;

                CompressedContent = compressed;
                CompressionType = CompressionType.Gzip;
                updateRequired = true;
            }
            else if (CompressionType == CompressionType.Gzip)
            {
                if (TextContent != null)
                {
                    TextContent = null;
                    updateRequired = true;
                }
            }
            return updateRequired;
        }
    }
#pragma warning restore CS0612 // Type or member is obsolete
}
