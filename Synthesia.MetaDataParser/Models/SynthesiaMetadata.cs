using System;
using System.Collections.Generic;
using System.Text;

namespace Synthesia.MetaDataParser.Models
{
    public class SynthesiaMetadata
    {
        public SynthesiaMetadata()
        {
            Songs = new List<Song>();
        }

        public int Version { get; set; }

        public IList<Song> Songs { get; set; }
    }
}
