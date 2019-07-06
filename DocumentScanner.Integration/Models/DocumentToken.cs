using Newtonsoft.Json;

namespace DocumentScanner.Integration.Models
{
    public class DocumentToken
    {
        [JsonProperty("dep")]
        public string Dep { get; set; }

        [JsonProperty("isAlpha")]
        public bool IsAlpha { get; set; }

        [JsonProperty("isStop")]
        public bool IsStop { get; set; }

        [JsonProperty("lemma")]
        public string Lemma { get; set; }

        /// <summary>
        /// Gets part of speech.
        /// </summary>
        [JsonProperty("pos")]
        public string PartOfSpeech { get; set; }

        [JsonProperty("shape")]
        public string Shape { get; set; }

        /// <summary>
        /// The detailed part of speech tag.
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public bool IsNoun()
        {
            return PartOfSpeech == Constants.NounPos;
        }

        public bool IsProperNoun()
        {
            return PartOfSpeech == Constants.ProperNounPos;
        }
    }
}
