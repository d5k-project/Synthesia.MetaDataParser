using System;
using System.IO;
using Synthesia.MetaDataParser.Model;

namespace Synthesia.MetaDataParser
{
    public class SynthesiaMetaDataParser
    {
        public Song Parser(string path)
        {
            return Parser(File.Open(path, FileMode.Open));
        }

        public Song Parser(Stream stream)
        {
            return null;
        }

        public Stream Export(Song song)
        {
            return null;
        }
    }
}
