// Path: Models/Document_Verification/ViewModels/SchemeWiseCountVM.cs
namespace rda_raipur.Models.Document_Verification.ViewModels
{
    public class SchemeWiseCountVM
    {
        public string? SchemeName { get; set; }
        public int TotalApplications { get; set; }
        public int PendingVerification { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public string PropertyType { get; set; }
    }
}