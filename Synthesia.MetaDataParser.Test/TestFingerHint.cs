using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synthesia.MetaDataParser.Models.Fingers;

namespace Synthesia.MetaDataParser.Test
{
    [TestClass]
    public class TestFingerHint : BaseTest
    {
        #region Tests

        [TestMethod]
        public void TestStringToFingerHint()
        {
            var fingerHintsString =
                "6780 m14: ----6060680690690786868686868686868686869686868-86868686868 t1:  m2: 52431----6";

            //get finger hints
            var fingerHints = Parser.TestConvertStringToFingerHints(fingerHintsString);

            //Track0 and Track1
            Assert.AreEqual(2, fingerHints.Count);

            //Track1 has one measure
            Assert.AreEqual(1, fingerHints[1].Count);

            //Track1, measure2 has 10 finger hints
            Assert.AreEqual(10,fingerHints[1][2].Count);

            //Track1, measure2 hint[0] is Left5
            Assert.AreEqual(Finger.Left5, fingerHints[1][2][0]);
        }

        [TestMethod]
        public void TestFingerHintToString()
        {
            var fingerHints = new Dictionary<int,FingerHint>
            {
                {
                    0,
                    new FingerHint()
                },
                {
                    1,
                    new FingerHint
                    {
                        {
                            0,
                            new List<Finger>
                            {
                                Finger.Right1,//Do
                                Finger.Right1,//Do
                                Finger.Right4,//So
                                Finger.Right4,//So
                                Finger.Right5,//Ra
                                Finger.Right5,//Ra
                                Finger.Right4//So
                            }
                        },
                        {
                            1,
                            new List<Finger>
                            {
                                Finger.Right4,//Fa
                                Finger.Right4,//Fa
                                Finger.Right3,//Mi
                                Finger.Right3,//Mi
                                Finger.Right2,//Ra
                                Finger.Right2,//Ra
                                Finger.Right1//Do
                            }
                        }
                    }
                }
            };

            //get finger hints string
            var fingerHintsString = Parser.TestConvertFingerHintsToString(fingerHints);

            //Track0 and Track1
            Assert.AreEqual("t1:  6699009 m1: 9988776", fingerHintsString);
        }

        #endregion
    }
}
