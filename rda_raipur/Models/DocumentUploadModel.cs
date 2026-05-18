namespace rda_raipur.Models
{
    public class DocumentUploadModel
    {
        public int ApplicantId { get; set; }

        public IFormFile Photo { get; set; }
        public IFormFile Signature { get; set; }
        public IFormFile IdentityProof { get; set; }
        public IFormFile AddressProof { get; set; }
        public IFormFile IncomeCertificate { get; set; }
        public IFormFile CategoryCertificate { get; set; }
    }
}
