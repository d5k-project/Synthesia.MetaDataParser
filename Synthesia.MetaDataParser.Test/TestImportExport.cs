using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models;
using Synthesia.MetaDataParser.Test.Helpers;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestImportExport : BaseTest
    {
        #region Utilities

        protected bool CompareTwoSynthesiaMetadatas(SynthesiaMetadata metadata1, SynthesiaMetadata metadata2)
        {
            return CompareHelper.CompareWithJsonFormat(metadata1, metadata2);
        }

        protected bool CompareTwoSongs(Song song1, Song song2)
        {
            return CompareHelper.CompareWithJsonFormat(song1,song2);
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestImportDataFromPath()
        {
            var path = "Sample/SampleMetadata.synthesia";

            //import songs
            var songs = Parser.Parse(path);
            var song = songs.Songs.FirstOrDefault();

            //Check songs number
            Assert.AreEqual(1,songs.Songs.Count);

            //Check first song's name
            Assert.AreEqual("Freesia", song.Title);

            //Has three bookmarks
            Assert.AreEqual(3, song.Bookmarks.Count);

            //Has three tags
            Assert.AreEqual(3, song.Tags.Count);
        }

        [TestMethod]
        public void TestImportDataFromStream()
        {
            var path = "Sample/SampleMetadata.synthesia";
            var stream = File.Open(path, FileMode.Open);

            //import songs
            var songs = Parser.Parse(stream);

            //Check songs number
            Assert.AreEqual(1, songs.Songs.Count);
        }

        [TestMethod]
        public void TestExportDataToXml()
        {
            //Song
            var path = "Sample/SampleMetadata.synthesia";

            //import songs
            var metadata = Parser.Parse(path);

            //save songs 
            var xml = Parser.ToXml(metadata);

            //reload song
            var savedMetadata = Parser.Parse(xml);

            //compare two metadata is equal
            Assert.IsTrue(CompareTwoSynthesiaMetadatas(metadata, savedMetadata));
        }

        [TestMethod]
        public void TestExportDataToStream()
        {
            //Song
            var path = "Sample/SampleMetadata.synthesia";

            //import songs
            var metadata = Parser.Parse(path);

            //save songs 
            using (var stream = Parser.ToStream(metadata))
            {
                //save and reload song
                var savedMetadata = Parser.Parse(stream);

                //compare two songs is equal
                Assert.IsTrue(CompareTwoSynthesiaMetadatas(metadata, savedMetadata));
            }
        }

        #endregion
    }
}
