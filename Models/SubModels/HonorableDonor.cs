namespace ELTracker.Models.SubModels
{
    public class HonorableDonor
    {
        public string Name { get; set; }
        public string Tier { get; set; }
        public bool Mentioned { get; set; }

        public HonorableDonor(string name, string tier, bool mentioned)
        {
            Name = name;
            Tier = tier;
            Mentioned = mentioned;
        }
    }
}
