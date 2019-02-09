using System.Collections.Generic;
using Synthesia.MetaDataParser.Models.Fingers;
using Synthesia.MetaDataParser.Models.Parts;

namespace Synthesia.MetaDataParser.Models
{
    /// <summary>
    /// Song
    /// </summary>
    public class Song
    {
        public Song()
        {
            Tags = new List<string>();
            Bookmarks = new Dictionary<int, string>();
            FingerHints = new Dictionary<int, FingerHint>();
            HandParts = new Dictionary<int, string>();
            Parts = new Dictionary<int, Part>();
        }

        public Song(string uniqueId) : this()
        {
            UniqueId = uniqueId;
        }

        /// <summary>
        /// Unique Id
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Subtitle
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Background image(Path)
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Composer
        /// </summary>
        public string Composer { get; set; }

        /// <summary>
        /// Arranger
        /// </summary>
        public string Arranger { get; set; }

        /// <summary>
        /// Copyright
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// License
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Made famous by
        /// </summary>
        public string MadeFamousBy { get; set; }

        /// <summary>
        /// Finger hints
        /// </summary>
        public IDictionary<int,FingerHint> FingerHints { get; set; }

        /// <summary>
        /// Hand parts
        /// </summary>
        public IDictionary<int,string> HandParts { get; set; }

        /// <summary>
        /// Parts
        /// </summary>
        public IDictionary<int, Part> Parts { get; set; }

        /// <summary>
        /// Rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Difficulty
        /// </summary>
        public int? Difficulty { get; set; }

        /// <summary>
        /// Returns a copy of the list.  Use AddTag() and RemoveTag() to make changes.
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Returns a copy of the dictionary.
        /// </summary>
        public IDictionary<int, string> Bookmarks { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Title))
                return "(Unknown Song)";

            return string.IsNullOrEmpty(Subtitle) ? Title : $"{Title}, {Subtitle}";
        }
    }
}
