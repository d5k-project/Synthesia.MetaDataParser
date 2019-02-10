using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestSongProperty : BaseTest
    {
        [TestMethod]
        public void TestSongOtherProperty()
        {
            //Create a song
            var song = CreateTestSong();

            //Export to xml format
            var exportXml = Parser.ToXml(new SynthesiaMetadata
            {
                Songs = new List<Song>
                {
                    song
                }
            });

            //Import and get first song
            var importSong = Parser.Parse(exportXml).Songs.FirstOrDefault()
                ?? throw new ArgumentNullException();

            //Compare two object's property
            Assert.AreEqual(song.Title,importSong.Title);
            Assert.AreEqual(song.Subtitle, importSong.Subtitle);
            Assert.AreEqual(song.BackgroundImage, importSong.BackgroundImage);

            Assert.AreEqual(song.Composer, importSong.Composer);
            Assert.AreEqual(song.Arranger, importSong.Arranger);
            Assert.AreEqual(song.Copyright, importSong.Copyright);
            Assert.AreEqual(song.License, importSong.License);
            Assert.AreEqual(song.MadeFamousBy, importSong.MadeFamousBy);

            Assert.AreEqual(song.Difficulty, importSong.Difficulty);
            Assert.AreEqual(song.Rating, importSong.Rating);
        }
    }
}
