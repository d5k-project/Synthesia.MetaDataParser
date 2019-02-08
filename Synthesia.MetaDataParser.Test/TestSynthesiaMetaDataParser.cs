using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestSynthesiaMetaDataParser
    {
        #region Utilities

        protected Song SaveAndReloadSong(Song song)
        {
            var metadata = new SynthesiaMetadata
            {
                Songs = new List<Song>
                {
                    song
                }
            };
            var parser = new SynthesiaMetaDataParser();
            using (var stream = parser.ToStream(metadata))
            {
                return parser.Parse(stream).Songs?.FirstOrDefault();
            }
        }

        protected bool CompareTweoSongs(Song song1, Song song2)
        {
            return false;
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestImportDataFromPath()
        {
            var path = "";
            var parser = new SynthesiaMetaDataParser();
            var songs = parser.Parse(path);

            //TODO : Check song's property
        }

        [TestMethod]
        public void TestImportDataFromStream()
        {
            var path = "";
            var stream = File.Open(path, FileMode.Open);

            var parser = new SynthesiaMetaDataParser();
            var songs = parser.Parse(path);

            //TODO : Check song's property
        }

        [TestMethod]
        public void TestExportDataToStream()
        {
            //Song
            var song = new Song();
            var parser = new SynthesiaMetaDataParser();

            var savedSong = SaveAndReloadSong(song);

            //TODO: compare two song property
        }

        [TestMethod]
        public void TestEditData()
        {
            //TODO : Edit property
            var song = new Song();

            //TODO : save and reload song

            //TODO : compare two songs (maybe convert to json string and compare)
        }

        #endregion
    }
}
