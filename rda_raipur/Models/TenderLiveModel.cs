using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class TenderLiveModel
    {
        [Key]
        public int TenderLiveId { get; set; }

        public string Tender_Property_Id { get; set; }
        public DateTime Tender_Start_Date { get; set; }
        public DateTime Tender_End_Date { get; set; }

        public string Property_Status { get; set; }
        public string Status { get; set; }

        //public List<PropertyListModel> Properties { get; set; }

        public List<PropertyListModel> Properties { get; set; } = new List<PropertyListModel>();
    }
}
