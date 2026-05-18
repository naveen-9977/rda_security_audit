namespace rda_raipur.Models.ViewModels
{
    public class AllotyReportVM
    {
        public string Name { get; set; }
        public string Block { get; set; }

        public decimal BasicPrice { get; set; }
        public decimal FinalCost { get; set; }
        public decimal AdditionalCost { get; set; }

        public decimal TotalInstallment { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalWithGST { get; set; }

        public decimal LastAmount { get; set; }
        public decimal LastGST { get; set; }

        public decimal RegAmount { get; set; }
        public decimal RegGST { get; set; }

        public decimal MaintAmount { get; set; }
        public decimal MaintGST { get; set; }

        public decimal Interest { get; set; }
        public decimal IntGST { get; set; }

        public decimal TotalReceivable { get; set; }
        public decimal Received { get; set; }
        public decimal Balance { get; set; }

        public decimal Waiver { get; set; }
        public decimal GSTWaiver { get; set; }

        public decimal FinalPayable { get; set; }
    }
}
