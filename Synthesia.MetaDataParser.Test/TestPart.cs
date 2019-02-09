using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models.Parts;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestPart : BaseTest
    {
        #region Tests

        public void TestParsePart()
        {
            var partsString = "t0:RA t1:LA";

            //get parts
            var parts = Parser.TestConvertStringToParts(partsString);

            //Track0 and Track1
            Assert.AreEqual(2, parts.Count);

            //Track1 has one part
            Assert.AreEqual(1, parts[1].Count);

            //Track1, part1 is "all part"
            Assert.AreEqual(PartType.Left, parts[1][0][0].PartType);
        }

        public void TestExportPart()
        {

        }

        #endregion
    }
}
