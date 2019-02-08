﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Synthesia.MetaDataParser.Extensions;
using Synthesia.MetaDataParser.Models;

namespace Synthesia.MetaDataParser
{
    public class SynthesiaMetaDataParser
    {
        #region Utilities

        protected virtual void ApplySongProperty(XElement songs, Song entry)
        {
            XElement element = (from e in songs.Elements("Song") where e.AttributeOrDefault("UniqueId") == entry.UniqueId select e).FirstOrDefault();

            if (element == null)
                songs.Add(element = new XElement("Song"));

            element.SetAttributeValueAndRemoveEmpty("UniqueId", entry.UniqueId);
            element.SetAttributeValueAndRemoveEmpty("Title", entry.Title);
            element.SetAttributeValueAndRemoveEmpty("Subtitle", entry.Subtitle);

            element.SetAttributeValueAndRemoveEmpty("BackgroundImage", entry.BackgroundImage);

            element.SetAttributeValueAndRemoveEmpty("Composer", entry.Composer);
            element.SetAttributeValueAndRemoveEmpty("Arranger", entry.Arranger);
            element.SetAttributeValueAndRemoveEmpty("Copyright", entry.Copyright);
            element.SetAttributeValueAndRemoveEmpty("License", entry.License);
            element.SetAttributeValueAndRemoveEmpty("MadeFamousBy", entry.MadeFamousBy);

            element.SetAttributeValueAndRemoveEmpty("Rating", entry.Rating);
            element.SetAttributeValueAndRemoveEmpty("Difficulty", entry.Difficulty);

            element.SetAttributeValueAndRemoveEmpty("FingerHints", entry.FingerHints);
            element.SetAttributeValueAndRemoveEmpty("HandParts", entry.HandParts);
            element.SetAttributeValueAndRemoveEmpty("Parts", entry.Parts);
            element.SetAttributeValueAndRemoveEmpty("Tags", string.Join(";", entry.Tags.ToArray()));
            element.SetAttributeValueAndRemoveEmpty("Bookmarks", string.Join(";", from b in entry.Bookmarks select (string.IsNullOrWhiteSpace(b.Value) ? b.Key.ToString() : string.Join(",", b.Key.ToString(), b.Value))));
        }

        #endregion

        #region Methods

        public Song Parser(string path)
        {
            return Parser(File.Open(path, FileMode.Open));
        }

        public Song Parser(Stream stream)
        {
            XDocument m_document;

            using (var reader = new StreamReader(stream))
                m_document = XDocument.Load(reader, LoadOptions.None);

            XElement top = m_document.Root;

            if (top == null || top.Name != "SynthesiaMetadata")
                throw new InvalidOperationException("Stream does not contain a valid Synthesia metadata file.");

            if (top.AttributeOrDefault("Version") != "1")
                throw new InvalidOperationException("Unknown Synthesia metadata version.  A newer version of this editor may be available.");

            XElement songs = m_document.Root.Element("Songs");
            if (songs == null) m_document.Root.Add(songs = new XElement("Songs"));

            var song = new Song();
            ApplySongProperty(songs, song);

            return song;
        }

        public Stream Export(Song song)
        {
            return null;
        }

        #endregion
    }
}
