using Microsoft.EntityFrameworkCore;
using rda_raipur.Models;
using rda_raipur.Models.otp;
using rda_raipur.Models.site;
using rda_raipur.Models.PermissionModel;
using rda_raipur.Models.Document_Verification;
using rda_raipur.Models.AllotyInstallmentDueDetails;
using rda_raipur.Models.Property;

namespace rda_raipur.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MasterState> MasterStates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SiteNews> SiteNews { get; set; }
        public DbSet<SiteGallery> SiteGalleries { get; set; }
        public DbSet<SiteCarousel> SiteCarousels { get; set; }
        public DbSet<OtpLog> OtpLogs { get; set; }
        public DbSet<Scheme_Master> Scheme_Master { get; set; }
        public DbSet<Sector_Master> Sector_Master { get; set; }
        public DbSet<Block_Master> Block_Masters { get; set; }
        public DbSet<AllotmentType_Master> AllotementTypeMasters { get; set; }
        public DbSet<ApplicantModel> Applicants { get; set; }
        public DbSet<UserCategory_Master> UserCategoryMasters { get; set; }
        public DbSet<PropertyModelMaster> PropertyModelMasters { get; set; }
        public DbSet<PropertyModelTypeMaster> PropertyModelTypeMasters { get; set; }
        public DbSet<FlatTypeMaster> FlatTypeMasters { get; set; }
        public DbSet<GalleryFolder> GalleryFolders { get; set; }
        public DbSet<SitePopup> SitePopups { get; set; }
        public DbSet<SiteContact> SiteContacts { get; set; }
        public DbSet<ContactQuery> ContactQueries { get; set; }
        public DbSet<Brochure> Brochures { get; set; }

        // 🔥 PROPERTY MODULE 🔥
        public DbSet<TenderPropertyCreation> TenderProperties { get; set; }

        
        public DbSet<MasterDirection> MasterDirections { get; set; }

        public DbSet<Tender_Live_Master> Tender_Live_Masters { get; set; }
        public DbSet<Tender_Live_Mapping> Tender_Live_Mappings { get; set; }
        public DbSet<UserProfileDetail> UserProfileDetails { get; set; }
        public DbSet<BookingProfileDetail> BookingProfileDetails { get; set; }
        public DbSet<PropertyBooking> PropertyBookings { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<LotteryResult> LotteryResults { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Department_Master> Department_Masters { get; set; }
        public DbSet<Designation_Master> Designation_Masters { get; set; }
        public DbSet<EmpType_Master> EmpType_Masters { get; set; }
        public DbSet<AppModule> AppModules { get; set; }
        public DbSet<EmployeePermission> EmployeePermissions { get; set; }
        public DbSet<DocumentVerificationLog> DocumentVerificationLogs { get; set; }
        public DbSet<rda_raipur.Models.AllotyRegistration.AllotyRegistration> AllotyRegistration { get; set; }
        public DbSet<rda_raipur.Models.PropertyMaster> Properties { get; set; }

        public DbSet<rda_raipur.Models.Better_Location> Better_Location { get; set; }

        public DbSet<Old_Alloty_Payment_Details> OldAllotyPaymentDetails { get; set; }
        public DbSet<AlloteePaymentDetail> AlloteePaymentDetails { get; set; }
        public DbSet<IpIiOldData> IpIiOldDatas { get; set; }
        public DbSet<AllotyInstallmentDueDetails> Alloty_Installment_Due_Details { get; set; }
        public DbSet<AllotyPaymentDetails> Alloty_Payment_Details { get; set; }

        public DbSet<OldAllotyInstallmentMaster> OldAllotyInstallmentMasters { get; set; }
        public DbSet<OldAllotyInstallmentDetail> OldAllotyInstallmentDetail { get; set; }
        public DbSet<SiteScheme> SiteSchemes { get; set; }

        public DbSet <PropertieDetailsCalulation> PropertieDetailsCalulation { get; set; }
        //public object UserProfiles { get; internal set; }
        public DbSet<UserProfileDetail> UserProfiles { get; set; }
    }
}