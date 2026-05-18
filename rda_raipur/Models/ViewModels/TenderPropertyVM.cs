namespace rda_raipur.Models.ViewModels
{
    public class TenderPropertyVM
    {

        //public int scheme_id { get; set; }
        //public int sector_id { get; set; }
        //public int block_id { get; set; }
        //public int Flat_id { get; set; }

        //public string House_No { get; set; }

        //public string Tender_Property_Id { get; set; }

        //public decimal offset_Price { get; set; }

        //public decimal EMD_Amount { get; set; }

        //public decimal Super_Buildup_Area { get; set; }

        //public decimal Buildup_Area { get; set; }

        //public decimal Carpet_Area { get; set; }

        //public string created_by { get; set; }

        //public DateTime? Create_Date { get; set; }

        //public DateTime? updated_Date { get; set; }

        //public string updated_by { get; set; }

        //public bool IsActive { get; set; }

        //public bool IsDeleted { get; set; }


        public bool IsSelected { get; set; }

        public int PropertyId { get; set; }
        public string Tender_Property_Id { get; set; }
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
