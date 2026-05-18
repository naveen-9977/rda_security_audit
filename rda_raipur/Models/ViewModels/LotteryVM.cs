namespace rda_raipur.Models.ViewModels
{
    public class LotteryVM
    {
        public string Category { get; set; }
        public string PropertyType { get; set; }
        public decimal OffsetPrice { get; set; }
        public int TotalFlats { get; set; }

        public List<ApplicantVM> Applicants { get; set; }
        public List<string> AvailableFlats { get; set; }

        public List<LotteryResultVM> Results { get; set; }
    }
    public class FlatVM
    {
        public string FlatNo { get; set; }
        public bool IsAllotted { get; set; }
    }
    public class ApplicantVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LotteryResultVM
    {
        public string ApplicantName { get; set; }
        public string FlatNo { get; set; }
    }
}
