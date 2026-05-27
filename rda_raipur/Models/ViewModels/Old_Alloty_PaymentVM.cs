using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models.ViewModels
{
    public class Old_Alloty_PaymentVM
    {
        public Old_Alloty_Payment_Details Payment { get; set; }
        public List<SelectListItem> PaymentModes { get; set; }
        public string Alloty_Unique_Id { get; set; }
        // Dropdown
        //public List<SelectListItem> PaymentModes { get; set; }
    }
}
