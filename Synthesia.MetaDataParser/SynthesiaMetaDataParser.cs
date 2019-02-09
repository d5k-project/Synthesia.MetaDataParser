using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Synthesia.MetaDataParser.Extensions;
using Synthesia.MetaDataParser.Models;
using Synthesia.MetaDataParser.Models.Fingers;
using Synthesia.MetaDataParser.Models.Parts;

namespace Synthesia.MetaDataParser
{
    public class SynthesiaMetaDataParser
    {
        #region Utilities

        /// <summary>
        /// XML -> SynthesiaMetadata
        /// </summary>
        /// <param name="document"></param>
        /// <param name="synthesiaMetadata"></param>
        protected virtual void GetSongProperty(XDocument document, SynthesiaMetadata synthesiaMetadata)
        {
            XElement top = document.Root;

            if (top == null || top.Name != "SynthesiaMetadata")
                throw new InvalidOperationException("Stream does not contain a valid Synthesia metadata file.");

            if (top.AttributeOrDefault("Version") != "1")
                throw new InvalidOperationException("Unknown Synthesia metadata version.  A newer version of this editor may be available.");

            synthesiaMetadata.Version = 1;

            XElement songs = document.Root?.Element("Songs");

            if (songs == null)
            {
                document.Root?.Add(songs = new XElement("Songs"));
                return;
            }

            foreach (XElement s in songs.Elements("Song"))
            {
                Song entry = new Song
                {
                    UniqueId = s.AttributeOrDefault("UniqueId"),
                    Title = s.AttributeOrDefault("Title"),
                    Subtitle = s.AttributeOrDefault("Subtitle"),

                    BackgroundImage = s.AttributeOrDefault("BackgroundImage"),

                    Composer = s.AttributeOrDefault("Composer"),
                    Arranger = s.AttributeOrDefault("Arranger"),
                    Copyright = s.AttributeOrDefault("Copyright"),
                    License = s.AttributeOrDefault("License"),
                    MadeFamousBy = s.AttributeOrDefault("MadeFamousBy"),

                    FingerHints = ConvertStringToFingerHints(s.AttributeOrDefault("FingerHints")),
                    HandParts = ConvertStringToHandParts(s.AttributeOrDefault("HandParts")),
                    Parts = ConvertStringToParts(s.AttributeOrDefault("Parts"))
                };

                if (int.TryParse(s.AttributeOrDefault("Rating"), out var rating))
                    entry.Rating = rating;
                if (int.TryParse(s.AttributeOrDefault("Difficulty"), out var difficulty))
                    entry.Difficulty = difficulty;

                entry.Tags = ConvertStringToTags(s.AttributeOrDefault("Tags"));

                entry.Bookmarks = ConvertStringToBookmarks(s.AttributeOrDefault("Bookmarks"));

                synthesiaMetadata.Songs.Add(entry);
            }
        }

        /// <summary>
        /// SynthesiaMetadata -> XML
        /// </summary>
        /// <param name="document"></param>
        /// <param name="synthesiaMetadata"></param>
        protected virtual void ApplySongProperty(XDocument document, SynthesiaMetadata synthesiaMetadata)
        {
            var songs = document.Root;
            //Set version
            songs.SetAttributeValueAndRemoveEmpty("Version", synthesiaMetadata.Version);

            foreach (var entry in synthesiaMetadata.Songs)
            {
                //Check xml cannot be null
                if(string.IsNullOrEmpty(entry.UniqueId))
                    throw new ArgumentNullException();

                //get element
                XElement element = songs.Elements("Song")
                    .FirstOrDefault(e => e.AttributeOrDefault("UniqueId") == entry.UniqueId);

                if (element == null)
                    songs.Add(element = new XElement("Song"));

                //Basic info
                element.SetAttributeValueAndRemoveEmpty("UniqueId", entry.UniqueId);
                element.SetAttributeValueAndRemoveEmpty("Title", entry.Title);
                element.SetAttributeValueAndRemoveEmpty("Subtitle", entry.Subtitle);

                //Image
                element.SetAttributeValueAndRemoveEmpty("BackgroundImage", entry.BackgroundImage);

                //Song producer
                element.SetAttributeValueAndRemoveEmpty("Composer", entry.Composer);
                element.SetAttributeValueAndRemoveEmpty("Arranger", entry.Arranger);
                element.SetAttributeValueAndRemoveEmpty("Copyright", entry.Copyright);
                element.SetAttributeValueAndRemoveEmpty("License", entry.License);
                element.SetAttributeValueAndRemoveEmpty("MadeFamousBy", entry.MadeFamousBy);

                //Rating
                element.SetAttributeValueAndRemoveEmpty("Rating", entry.Rating);
                element.SetAttributeValueAndRemoveEmpty("Difficulty", entry.Difficulty);

                //Tracks
                element.SetAttributeValueAndRemoveEmpty("FingerHints",
                    ConvertFingerHintsToString(entry.FingerHints));
                element.SetAttributeValueAndRemoveEmpty("HandParts", 
                    ConvertHandPartsToString(entry.HandParts));
                element.SetAttributeValueAndRemoveEmpty("Parts", 
                    ConvertPartsToString(entry.Parts));

                //Other property
                element.SetAttributeValueAndRemoveEmpty("Tags", 
                    ConvertTagsToString(entry.Tags));
                element.SetAttributeValueAndRemoveEmpty("Bookmarks",
                    ConvertBookmarksToString(entry.Bookmarks));
            }
        }

        protected virtual IDictionary<int, FingerHint> ConvertStringToFingerHints(string fingerHintsString)
        {
            var fingerHintsDictionary = new Dictionary<int, FingerHint>();

            //Do not convert if string is empty
            if (string.IsNullOrEmpty(fingerHintsString))
                return fingerHintsDictionary;

            //split tracks
            var hint = "0:  " + fingerHintsString;
            var trackStrings = hint.Split('t');

            foreach (var trackString in trackStrings)
            {
                var fingerHints = new FingerHint();

                var trackInfo = trackString.Split(":  ");

                //get track id and track infos
                var trackId = int.Parse(trackInfo[0]);
                var measureInfos = trackInfo[1].Split("m").Where(x=>!string.IsNullOrWhiteSpace(x));

                foreach (var measureInfo in measureInfos)
                {
                    var measureInfoSplit = (measureInfo.Contains(":") ? measureInfo : "0:" + measureInfo).Split(':');

                    //get measure id and fingers in measure
                    var measureId = int.Parse(measureInfoSplit[0]);
                    var fingers = measureInfoSplit[1].Replace(" ", String.Empty).Select(c => (Finger) c).ToList();

                    //collect
                    fingerHints.AddMeasure(measureId, fingers);
                }

                //collect
                fingerHintsDictionary.Add(trackId, fingerHints);
            }

            return fingerHintsDictionary;
        }

        protected virtual string ConvertFingerHintsToString(IDictionary<int, FingerHint> fingerHints)
        {
            if (fingerHints == null)
                return null;

            var result = "";

            foreach (var fingerHint in fingerHints)
            {
                if (fingerHint.Key != 0)
                    result = result + "t" + fingerHint.Key + ":  ";

                foreach (var measureInfo in fingerHint.Value)
                {
                    if(measureInfo.Key!=0)
                        result = result + " m" + fingerHint.Key + ": ";

                    result = result + string.Concat(measureInfo.Value.Select(x => (char)x));
                }
            }

            return result;
        }

        protected virtual IDictionary<int, string> ConvertStringToHandParts(string handPartsString)
        {
            //TODO : Note : HandParts is too lod so not implement now
            return new Dictionary<int, string>();
        }

        protected virtual string ConvertHandPartsToString(IDictionary<int, string> handParts)
        {
            //TODO : Note : HandParts is too lod so not implement now
            return null;
        }

        protected virtual IDictionary<int, Part> ConvertStringToParts(string partsString)
        {
            return new Dictionary<int, Part>();
        }

        protected virtual string ConvertPartsToString(IDictionary<int, Part> parts)
        {
            return null;
        }

        protected virtual IList<string> ConvertStringToTags(string tagsString)
        {
            return tagsString?.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();
        }

        protected virtual string ConvertTagsToString(IList<string> tags)
        {
            return string.Join(";", tags.ToArray());
        }

        protected virtual IDictionary<int, string> ConvertStringToBookmarks(string bookmarksString)
        {
            var entry = new Dictionary<int,string>();
            if (bookmarksString != null)
            {
                foreach (var b in bookmarksString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int comma = b.IndexOf(',');

                    int.TryParse(comma == -1 ? b : b.Substring(0, comma), out var measure);
                    if (measure == 0) continue;

                    string description = "";
                    if (comma != -1) description = b.Substring(comma + 1);

                    entry.Add(measure, description);
                }
            }

            return entry;
        }

        protected virtual string ConvertBookmarksToString(IDictionary<int, string> bookmarks)
        {
            return string.Join(";",
                bookmarks.Select(b =>
                    (string.IsNullOrWhiteSpace(b.Value)
                        ? b.Key.ToString()
                        : string.Join(",", b.Key.ToString(), b.Value))));
        }

        protected char FingerToChar(Finger finger)
        {
            return (char) finger;
        }

        protected Finger CharToFinger(char c)
        {
            return (Finger) c;
        }

        protected char PartTypeToChar(PartType partType)
        {
            return (char)partType;
        }

        protected PartType CharToPartType(char c)
        {
            return (PartType)c;
        }

        #endregion

        #region Methods

        public SynthesiaMetadata Parse(string path)
        {
            var stream = File.Open(path, FileMode.Open);
            if(stream == null)
                throw new FileNotFoundException();

            return Parse(stream);
        }

        public SynthesiaMetadata Parse(Stream stream)
        {
            XDocument mDocument;

            using (var reader = new StreamReader(stream))
                mDocument = XDocument.Load(reader, LoadOptions.None);

            var metadata = new SynthesiaMetadata();

            GetSongProperty(mDocument, metadata);

            return metadata;
        }

        public XDocument ToXml(SynthesiaMetadata song)
        {
            XDocument document = new XDocument(new XElement("SynthesiaMetadata", (new XAttribute("Version", "1"))));

            //apply
            ApplySongProperty(document, song);

            return document;
        }

        public Stream ToStream(SynthesiaMetadata song)
        {
            var xmlFile = ToXml(song);

            var stream = new MemoryStream();

            xmlFile.Save(stream);

            return stream;
        }

        public void SaveToFile(SynthesiaMetadata song, string path)
        {
            var document = ToXml(song);

            using (FileStream output = File.Create(path))
                document.Save(output);
        }

        #endregion
    }
}
