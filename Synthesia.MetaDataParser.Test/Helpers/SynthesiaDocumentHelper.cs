using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Synthesia.MetaDataParser.Test.Helpers
{
    public class SynthesiaDocumentHelper
    {
        public static Stream GetStreamByFileName(string fileName)
        {
            return new StreamReader("Sample/" + fileName).BaseStream;
        }

        public static string GetFileStringByFileName(string fileName)
        {
            // Open the text file using a stream reader.
            using (var sr = new StreamReader("Sample/" + fileName))
            {
                // Read the stream to a string, and write the string to the console.
                var lines = sr.ReadToEnd();
                return lines;
            }
        }
    }
}
