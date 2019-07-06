using Newtonsoft.Json;

namespace DocumentScanner.Integration.Models
{
    public class DocumentScannerResponse
    {
        [JsonProperty("tokens")]
        public DocumentToken[] DocumentTokens { get; set; }

    }
}
