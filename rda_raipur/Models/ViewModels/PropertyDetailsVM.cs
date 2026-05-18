namespace rda_raipur.Models.ViewModels
{
    public class PropertyDetailsVM
    {
        public int Property_Id { get; set; }
        public string Scheme_Name { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public decimal? Area { get; set; }
        public string Description { get; set; }
        public string Property_Number { get; set; }

        public string sector_name { get; set; }

        public string block_name_en { get; set; }

        public string Flat_name_en { get; set; }

        public string res_category_name_en { get; set; }

        public decimal Buildup_Area { get; set; }
        public decimal Carpet_Area { get; set; }
        public string allotement_type_name_en { get;set; }

       public string Tender_Property_Id { get; set; }
    }
}
