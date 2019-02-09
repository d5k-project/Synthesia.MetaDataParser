namespace Synthesia.MetaDataParser.Models.Parts
{
    public class NotePart
    {
        public NotePart()
        {
            Length = 1;
        }

        public NotePart(PartType partType, int length = 1) : this()
        {
            PartType = partType;
            Length = length;
        }

        /// <summary>
        /// Part Type
        /// </summary>
        public PartType PartType { get; set; }

        /// <summary>
        /// Length
        /// </summary>
        public int Length { get; set; }
    }
}
