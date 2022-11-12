namespace ELTracker.Models.SubModels;

public class DonationTierDetails
{
    public string Name { get; set; }
    public decimal Threshold { get; set; }
    public ulong Role { get; set; }

    public DonationTierDetails(string name, decimal threshold, ulong role)
    {
        Name = name;
        Threshold = threshold;
        Role = role;
    }
}