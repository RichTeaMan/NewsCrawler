using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NewsCrawler.Exceptions
{
    public class HttpClientException : Exception
    {
        public string Url { get; }

        public HttpStatusCode StatusCode { get; }

        public HttpClientException(string url, HttpStatusCode statusCode) : base($"{statusCode} Could not fetch from {url}.")
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            StatusCode = statusCode;
        }
    }
}
