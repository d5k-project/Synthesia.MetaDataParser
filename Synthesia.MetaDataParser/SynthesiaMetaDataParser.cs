using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                document.Root?.Add(new XElement("Songs"));
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
            XElement top = document.Root;

            if (top == null || top.Name != "SynthesiaMetadata")
                document.Add(top = new XElement("SynthesiaMetadata", new XAttribute("Version", synthesiaMetadata.Version)));

            //Set version
            top.SetAttributeValueAndRemoveEmpty("Version", synthesiaMetadata.Version);

            //Set songs
            var songs = top.Element("Songs");
            if (songs == null)
            {
                top.Add(songs = new XElement("Songs"));
            }

            //Add each song in songs field
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

        protected virtual IDictionary<int, IDictionary<int, string>> ConvertToTrackFormat(string formatString)
        {
            if(string.IsNullOrEmpty(formatString))
                return new Dictionary<int, IDictionary<int, string>>();

            //get tracks
            var trackStrings = formatString.Replace(" ",string.Empty)
                .Split('t').Where(t => !string.IsNullOrEmpty(t))
                .ToDictionary(k => int.Parse(k.Split(':').FirstOrDefault())
                    , v =>
                    {
                        var measure = ("t" + v)
                            .Split('m').Where(m => !string.IsNullOrEmpty(m) && m.Split(':').Count(x=>!string.IsNullOrEmpty(x)) == 2)
                            .ToDictionary(k =>
                                {
                                    var keyString = k.Split(':').FirstOrDefault();

                                    if (keyString != null && keyString.Contains('t'))
                                        return -1;

                                    return int.Parse(keyString);
                                },
                                vv => vv.Split(':').LastOrDefault());

                        return (IDictionary<int,string>)measure;
                    });

            return trackStrings;
        }

        public List<string> SpecialSplit(string input)
        {
            var result = new List<string>();

            var currentString = new StringBuilder(4);
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (currentString.Length > 0)
                {
                    // Determine whether we're at constraints or not.
                    var firstCharLetter = currentString[0] >= 'A' && currentString[0] <= 'Z';
                    var atMaxLetterLength = firstCharLetter && currentString.Length == 4;
                    var atMaxNumberLength = !firstCharLetter && currentString.Length == 3;

                    // Split if at max letter/number length, or if we're on a letter.
                    var mustSplit = atMaxLetterLength || atMaxNumberLength || c >= 'A' && c <= 'Z' || c == '-';

                    if (mustSplit)
                    {
                        // If we must split our string, then verify we're not leaving an orphaned '0'.
                        if (c == '0')
                        {
                            // Go back a letter, take it out of the new string, and set our `c` to it.
                            i--;
                            currentString.Length--;
                            c = input[i];
                        }

                        // Add and clear the string to our result.
                        result.Add(currentString.ToString());
                        currentString.Clear();
                    }
                }

                // Add our `c` to the string.
                currentString.Append(c);
            }

            // Add our string to the result.
            result.Add(currentString.ToString());

            return result;
        }

        protected virtual IDictionary<int, FingerHint> ConvertStringToFingerHints(string fingerHintsString)
        {
            if(string.IsNullOrEmpty(fingerHintsString))
                return new Dictionary<int, FingerHint>();

            var dict = ConvertToTrackFormat("0:" +fingerHintsString);

            //convert to wanted format
            return dict.ToDictionary(k => k.Key,
                v =>
                {
                    var fingerHint = new FingerHint();

                    var measures = v.Value;
                    foreach (var measure in measures)
                    {
                        //if -1 then replace to 0
                        var measureKey = measure.Key == -1 ? 0 : measure.Key;
                        var fingers = measure.Value.Select(c => (Finger)c).ToList();
                        fingerHint.AddMeasure(measureKey, fingers);
                    }
                    return fingerHint;
                });
        }

        protected virtual string ConvertFingerHintsToString(IDictionary<int, FingerHint> fingerHints)
        {
            if (fingerHints == null || !fingerHints.Any())
                return null;

            var result = "";

            foreach (var fingerHint in fingerHints)
            {
                if (fingerHint.Key != 0)
                    result = result + "t" + fingerHint.Key + ":  ";

                foreach (var measureInfo in fingerHint.Value)
                {
                    if(measureInfo.Key!=0)
                        result = result + " m" + measureInfo.Key + ": ";

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
            if (string.IsNullOrEmpty(partsString))
                return new Dictionary<int, Part>();

            var dict = ConvertToTrackFormat(partsString);

            return dict.ToDictionary(k => k.Key,
                v =>
                {
                    var part = new Part();

                    var noteParts = v.Value;
                    foreach (var notePart in noteParts)
                    {
                        var notePartKey = notePart.Key;
                        var notePartValue = notePart.Value;

                        if (notePartKey == -1)
                        {
                            //Set all key
                            part.AllPartType = (PartType)notePartValue[0];
                        }
                        else
                        {
                            var notes = new List<NotePart>();
                            var notesStr = SpecialSplit(notePartValue);
                            foreach (var noteStr in notesStr)
                            {
                                var note = new NotePart
                                {
                                    PartType = (PartType)noteStr[0]
                                };

                                if (noteStr.Any(char.IsDigit))
                                    note.Length = int.Parse(noteStr.Substring(1, noteStr.Length - 1));

                                notes.Add(note);
                            }

                            //set partial key
                            part.Add(notePartKey,notes);
                        }
                    }

                    return part;
                });
        }

        protected virtual string ConvertPartsToString(IDictionary<int, Part> parts)
        {
            if (parts == null || !parts.Any())
                return null;

            var result = "";

            foreach (var part in parts)
            {
                var partKey = part.Key;
                var partValue = part.Value;
                result = result + "t" + partKey + ":" + (char)partValue.AllPartType + "A ";

                foreach (var notePart in partValue)
                {
                    result = result + "m" + notePart.Key + ":";

                    foreach (var singleNotePart in notePart.Value)
                    {
                        result = result + (char) singleNotePart.PartType;

                        if (singleNotePart.Length > 1)
                            result = result + singleNotePart.Length;
                    }
                }
            }

            return result;
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

        #endregion

        #region Methods

        public SynthesiaMetadata Parse(string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                if (stream == null)
                    throw new FileNotFoundException();

                return Parse(stream);
            }
        }

        public SynthesiaMetadata Parse(Stream stream)
        {
            if (stream == null)
                throw new FileNotFoundException();

            var document = XDocument.Load(stream, LoadOptions.None);

            return Parse(document);
        }

        public SynthesiaMetadata Parse(XDocument document)
        {
            var metadata = new SynthesiaMetadata();

            GetSongProperty(document, metadata);

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
            stream.Flush();//Adjust this if you want read your data 
            stream.Position = 0;

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
