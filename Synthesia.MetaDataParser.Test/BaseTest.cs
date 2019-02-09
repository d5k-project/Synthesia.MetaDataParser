using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models.Fingers;
using Synthesia.MetaDataParser.Models.Parts;

namespace Synthesia.MetaDataParser.Test
{
    public class BaseTest
    {
        public TestSynthesiaMetaDataParser Parser = null;

        [TestInitialize]
        public void Initialize()
        {
            Parser = new TestSynthesiaMetaDataParser();
        }

        public class TestSynthesiaMetaDataParser : SynthesiaMetaDataParser
        {
            public IDictionary<int, FingerHint> TestConvertStringToFingerHints(string fingerHintsString)
                => ConvertStringToFingerHints(fingerHintsString);

            public string TestConvertFingerHintsToString(IDictionary<int, FingerHint> fingerHints)
                => ConvertFingerHintsToString(fingerHints);


            public IDictionary<int, string> TestConvertStringToHandParts(string handPartsString)
                => ConvertStringToHandParts(handPartsString);

            public string TestConvertHandPartsToString(IDictionary<int, string> handParts)
                => ConvertHandPartsToString(handParts);

            public IDictionary<int, Part> TestConvertStringToParts(string partsString)
                => ConvertStringToParts(partsString);

            public string TestConvertPartsToString(IDictionary<int, Part> parts)
                => ConvertPartsToString(parts);

            public IList<string> TestConvertStringToTags(string tagsString)
                => ConvertStringToTags(tagsString);

            public string TestConvertTagsToString(IList<string> tags)
                => ConvertTagsToString(tags);

            public IDictionary<int, string> TestConvertStringToBookmarks(string bookmarksString)
                => ConvertStringToBookmarks(bookmarksString);

            public string TestConvertBookmarksToString(IDictionary<int, string> bookmarks)
                => ConvertBookmarksToString(bookmarks);
        }
    }
}
