using System;
using Certificate_Creator.Models;

namespace Certificate_Creator.Services
{
    public interface ICreatorService
    {
        bool GenerateRootCertificate(CertificateDataModel model);

        bool GenerateCertificate(CertificateDataModel model);
    }
}

