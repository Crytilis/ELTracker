namespace ELTracker.Models.SubModels
{
    public class HonorMention
    {
        public ulong MessageId { get; set; }
        public ulong ThreadId { get; set; }
        public decimal MentionAmount { get; set; }
        public string Tier { get; set; }
        public ulong Role { get; set; }
        public List<HonorableDonor> Donors { get; set; }

        public HonorMention(decimal amount, string name, ulong role)
        {
            MessageId = 0;
            ThreadId = 0;
            MentionAmount = amount;
            Tier = name;
            Role = role;
            Donors = new List<HonorableDonor>();
        }

        public void AddDonor(string donor, string tier, bool mentioned)
        {
            if (Donors.All(x => x.Name != donor))
            {
                Donors.Add(new HonorableDonor(donor, tier, mentioned));
            }
        }

        public void UpdateDonor(string donor, bool mentioned)
        {
            var updDonor = Donors.FirstOrDefault(x => x.Name == donor);
            if (updDonor != null) updDonor.Mentioned = mentioned;
        }

        public HonorableDonor GetDonor(string donor)
        {
            return Donors.FirstOrDefault(x => x.Name == donor);
        }

        internal void UpdateDonor(HonorableDonor mentioned)
        {
            var index = Donors.FindIndex(x => x.Name == mentioned.Name);
            if (index > -1) Donors[index] = mentioned;
        }
    }
}
