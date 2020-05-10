using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class GzipTextCompressor
    {
        public async Task<byte[]> Compress(string uncompressed)
        {
            using var mso = new MemoryStream();

            if (uncompressed != null)
            {
                var bytes = Encoding.UTF8.GetBytes(uncompressed);
                using var msi = new MemoryStream(bytes);
                using (var gzipStream = new GZipStream(mso, CompressionLevel.Optimal))
                {
                    await gzipStream.WriteAsync(msi.ToArray());
                }
            }
            return mso.ToArray();
        }

        public async Task<string> Decompress(byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gzipStream = new GZipStream(msi, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(mso);
            }
            string result = Encoding.UTF8.GetString(mso.ToArray());
            return result;
        }
    }
}
