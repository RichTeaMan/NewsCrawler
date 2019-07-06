using System;

namespace NewsCrawler.WebUI.Models
{
    public class WordFrequency
    {
        public int Frequency { get; }

        public string Word { get; }

        public WordFrequency(int frequency, string word)
        {
            Frequency = frequency;
            Word = word ?? throw new ArgumentNullException(nameof(word));
        }
    }
}
