using System.ComponentModel.DataAnnotations;

namespace rda_raipur.Models
{
    public class Better_Location
    {

        [Key]
        public int better_location_id { get; set; }

        [Required]
        [StringLength(200)]

        public string better_location_name_en { get; set; }

        [Required]
        public string better_location_name_hi { get; set; }

        [Required]
        public string short_name { get; set; }

        [Required]
        public int percentage { get; set; }
         
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        [Required]
        public int updated_by { get; set; }

        //public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }
    }
}