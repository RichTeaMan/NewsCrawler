using DocumentScanner.Integration.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DocumentScanner
{
    public class DocumentScannerService : IDisposable
    {
        private readonly ILogger logger;

        private readonly HttpClient client = new HttpClient();

        public DocumentScannerService(ILogger<DocumentScannerService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DocumentScannerResponse> ScanDocument(string document)
        {
            string url = "http://documentscanner.richteaman.com/scan";

            var values = new Dictionary<string, string>
            {
               { "document", document },
            };

            var content = new FormUrlEncodedContent(values);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var documentScannerResponse = JsonConvert.DeserializeObject<DocumentScannerResponse>(json);
                return documentScannerResponse;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
