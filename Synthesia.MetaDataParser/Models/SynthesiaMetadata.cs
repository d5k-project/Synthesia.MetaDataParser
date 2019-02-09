using System;
using System.Collections.Generic;
using System.Text;

namespace Synthesia.MetaDataParser.Models
{
    public class SynthesiaMetadata
    {
        public SynthesiaMetadata()
        {
            Version = 1;
            Songs = new List<Song>();
        }

        public int Version { get; set; }

        public IList<Song> Songs { get; set; }
    }
}
