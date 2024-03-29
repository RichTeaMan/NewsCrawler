﻿using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleTitleFetcherService : INewsArticleTitleFetcherService
    {
        private readonly Regex titleNodeRegex = new Regex(@"[^\|]+");

        public string FetchTitle(string articleContent)
        {
            string title = null;
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var titleNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasTitleAttribute(n));
            title = titleNode?.InnerText?.Trim();
            if (title == null)
            {
                string titleText = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Name == "title")?.InnerText?.Trim();
                if (titleText != null) {
                    var match = titleNodeRegex.Match(titleText);
                    if (match.Success)
                    {
                        title = match.Value.Trim();
                    }
                }
            }
            return title;
        }

        private bool HasTitleAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Name == "h1" && htmlNode.Attributes.Any(attr => attr.Name == "class" && (attr.Value?.Contains("content__headline") == true));
        }
    }
}
