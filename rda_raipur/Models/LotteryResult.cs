namespace rda_raipur.Models
{
    public class LotteryResult
    {
        public int Id { get; set; }

        public int ApplicantId { get; set; }

        public string ApplicantName { get; set; }

        public string FlatNo { get; set; }

        public string Category { get; set; }

        public DateTime CreatedDate { get; set; }

        public string PropertyType { get; set; } = string.Empty;
    }
}
