using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestTag : BaseTest
    {
        [TestMethod]
        public void TestStringToTag()
        {
            const string tagString = "tag001;tag002;tag003";

            //get tags
            var tags = Parser.TestConvertStringToTags(tagString);

            Assert.AreEqual(3,tags.Count);
            Assert.AreEqual("tag001",tags[0]);
            Assert.AreEqual("tag003", tags[2]);
        }

        [TestMethod]
        public void TestTagToString()
        {
            var tags = new List<string>
            {
                "tag001",
                "tag002",
                "tag003",
            };

            //get tags string
            var tagsString = Parser.TestConvertTagsToString(tags);

            Assert.AreEqual("tag001;tag002;tag003", tagsString);
        }
    }
}
