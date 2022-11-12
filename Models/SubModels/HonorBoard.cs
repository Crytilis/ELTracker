namespace ELTracker.Models.SubModels
{
    public class HonorBoard
    {
        public ulong Channel { get; set; }
        public bool EmbedEnabled { get; set; }
        public ulong Embed { get; set; }
        public List<HonorMention> HonorableMentions { get; set; }

        public HonorBoard()
        {
            Channel = 0;
            HonorableMentions = new List<HonorMention>();
        }

        public HonorBoard(ulong id)
        {
            Channel = id;
            HonorableMentions = new List<HonorMention>();
        }
    }
}
