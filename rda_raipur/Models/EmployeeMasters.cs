using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rda_raipur.Models
{
    // ==========================================
    // 1. Department Master
    // ==========================================
    [Table("Department_Master")]
    public class Department_Master
    {
        [Key]
        public int dpt_id { get; set; }

        [Required]
        public string dpt_name_hi { get; set; }

        [Required]
        public string dpt_name_en { get; set; }

        public string? created_by { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }

    // ==========================================
    // 2. Designation Master
    // ==========================================
    [Table("Designation_Master")]
    public class Designation_Master
    {
        [Key]
        public int designation_id { get; set; }

        [Required]
        public string designation_name_hi { get; set; }

        [Required]
        public string designation_name_en { get; set; }

        public string? created_by { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }

    // ==========================================
    // 3. Employee Type Master
    // ==========================================
    [Table("EmpType_Master")]
    public class EmpType_Master
    {
        [Key]
        public int emp_type_id { get; set; }

        [Required]
        public string emp_type_name_hi { get; set; }

        [Required]
        public string emp_type_name_en { get; set; }

        public string? created_by { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? updated_Date { get; set; }
        public string? updated_by { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}