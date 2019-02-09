using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synthesia.MetaDataParser.Models.Parts
{
    public class Part : Dictionary<int, List<NotePart>>
    {
        public PartType AllPartType { get; set; }

        public PartType SearchPartType(int measure,int index)
        {
            if (this.ContainsKey(measure))
            {
                var sum = 0;
                return this[measure].FirstOrDefault(x =>
                {
                    sum = sum + x.Size;
                    return sum > index;
                })?.PartType ?? AllPartType;
            }

            return AllPartType;
        }
    }
}
