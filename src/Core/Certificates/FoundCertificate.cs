using System.Security.Cryptography.X509Certificates;

namespace LINQPadHelpers.Certificates
{
    public class FoundCertificate
    {
        public StoreName StoreName { get; set; }
        public StoreLocation StoreLocation { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public bool HasPrivateKey => this.Certificate.HasPrivateKey;
    }
}