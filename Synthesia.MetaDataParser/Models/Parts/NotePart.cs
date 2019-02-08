namespace Synthesia.MetaDataParser.Models.Parts
{
    public class NotePart : IPart
    {
        /// <summary>
        /// Part Type
        /// </summary>
        public PartType PartType { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// All
        /// </summary>
        public bool All { get; set; }
    }
}
