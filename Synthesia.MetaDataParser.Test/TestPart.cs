using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models.Parts;

namespace Synthesia.MetaDataParser.Test
{
    /// <summary>
    /// See : https://github.com/Synthesia-LLC/metadata-editor/wiki/Hand-Parts
    /// </summary>
    [TestClass]
    public class TestPart : BaseTest
    {
        #region Tests

        [TestMethod]
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

        [TestMethod]
        public void TestExportPart()
        {
            var parts = new Dictionary<int, Part>
            {
                {
                    0,
                    new Part(PartType.Left)//Track 0 is left hand
                },
                {
                    1,
                    new Part(PartType.Right)//Track 1 is right hand
                    {
                        {
                            10,
                            new List<NotePart>
                            {
                                new NotePart(PartType.Left),//Set measure 10 is left at note 0
                            }
                        },
                        {
                            15,
                            new List<NotePart>
                            {
                                new NotePart(PartType.Ignore,10),//Set measure 15 is ignore at note 0~9
                            }
                        }
                    }
                }
            };

            //get finger hints string
            var fingerHintsString = Parser.TestConvertPartsToString(parts);

            //Track0 and Track1
            Assert.AreEqual("t0:LA t1:RA m10:Lm15:-10", fingerHintsString);
        }

        #endregion
    }
}
