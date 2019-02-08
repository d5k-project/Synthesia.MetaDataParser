using System.Collections.Generic;
using System.Linq;

namespace Synthesia.MetaDataParser.Models.Fingers
{
    /// <summary>
    /// 
    /// </summary>
    public class FingerHint : Dictionary<int,List<Finger>>
    {
        public void AddMeasure(int measureIndex, List<Finger> fingers = null)
        {
            if (ContainsKey(measureIndex))
                if(fingers != null && fingers.Any())
                    this[measureIndex] = fingers;
                else
                    return;

            Add(measureIndex, fingers);
        }
    }
}
