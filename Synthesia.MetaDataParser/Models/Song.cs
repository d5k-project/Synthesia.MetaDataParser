using System;
using System.Collections.Generic;

namespace Synthesia.MetaDataParser.Models
{
    public class Song
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Subtitile
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
        public string FingerHints { get; set; }

        /// <summary>
        /// Hand parts
        /// </summary>
        public string HandParts { get; set; }

        /// <summary>
        /// Parts
        /// </summary>
        public string Parts { get; set; }

        /// <summary>
        /// Rating
        /// </summary>
        private int? m_rating;
        public int? Rating
        {
            get => m_rating;
            set => m_rating = value.HasValue ? Math.Max(0, Math.Min(100, value.Value)) : (int?) null;
        }

        /// <summary>
        /// Difficulty
        /// </summary>
        private int? m_difficulty;
        public int? Difficulty
        {
            get => m_difficulty;
            set => m_difficulty = value.HasValue ? Math.Max(0, Math.Min(100, value.Value)) : (int?)null;
        }

        /// <summary>
        /// Returns a copy of the list.  Use AddTag() and RemoveTag() to make changes.
        /// </summary>
        public IList<string> Tags { get; set; } = new List<string>();


        /// <summary>
        /// Returns a copy of the dictionary.
        /// </summary>
        public Dictionary<int, string> Bookmarks { get; set; } = new Dictionary<int, string>();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Title))
                return "(Unknown Song)";

            return string.IsNullOrEmpty(Subtitle) ? Title : $"{Title}, {Subtitle}";
        }
    }
}
