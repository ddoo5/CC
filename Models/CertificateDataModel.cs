using System;
using System.Security.Cryptography.X509Certificates;

namespace Certificate_Creator.Models
{
    public sealed class CertificateDataModel
    {
        public X509Certificate2? RootCertificate { get; set;}

        public int CertificateDuration { get; set; }

        public string? CertificateName { get; set; }

        public string? Password { get; set; }

        public string? PasswordConfirm { get; set; }

        public string? OutFolder { get; set; }
    }
}

