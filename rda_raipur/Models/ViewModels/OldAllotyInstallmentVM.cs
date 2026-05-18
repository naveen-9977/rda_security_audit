using rda_raipur.Models;

namespace rda_raipur.Models.ViewModels
{
    public class OldAllotyInstallmentVM
    {
        public string Alloty_Unique_Id { get; set; }

        public decimal Sale_Value { get; set; }
        public decimal Total_Property_Cost { get; set; }
        public decimal Basic_Property_Price { get; set; }
        public decimal Final_Property_Cost { get; set; }
        public decimal Additional_Property_Cost { get; set; }

    }
}
