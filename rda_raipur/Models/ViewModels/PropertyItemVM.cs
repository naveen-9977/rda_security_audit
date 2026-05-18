namespace rda_raipur.Models.ViewModels
{
    public class PropertyItemVM
    {
        public int PropertyId { get; set; }
        public string Tender_Property_Id { get; set; }

        public bool IsSelected { get; set; }

        public string Scheme_Name { get; set; }
        public string Property_Type { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string Property_Number { get; set; }
        public string UserCategory { get; set; }

        public decimal Offset_Price { get; set; }
        public decimal EMD_Amount { get; set; }

        public string Property_Model { get; set; }
        public string Property_Size { get; set; }
        public string Property_Status { get; set; }
    }
}
