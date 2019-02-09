using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestBookmark : BaseTest
    {
        [TestMethod]
        public void TestStringToBookmark()
        {
            const string bookmarkString = "1,bookmark001;2,bookmark002;3,bookmark003";

            //get tags
            var tags = Parser.TestConvertStringToBookmarks(bookmarkString);

            Assert.AreEqual(3, tags.Count);
            Assert.AreEqual("bookmark001", tags[1]);
            Assert.AreEqual("bookmark003", tags[3]);
        }

        [TestMethod]
        public void TestBookmarkToString()
        {
            var bookmarks = new Dictionary<int,string>
            {
                {1,"bookmark001"},
                {2,"bookmark002"},
                {3,"bookmark003"}
            };

            //get tags
            var bookmarksString = Parser.TestConvertBookmarksToString(bookmarks);

            Assert.AreEqual("1,bookmark001;2,bookmark002;3,bookmark003", bookmarksString);
        }
    }
}
