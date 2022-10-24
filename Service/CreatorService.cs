using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using Certificate_Creator.Models;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace Certificate_Creator.Services
{
    public class CreatorService : ICreatorService
    {
        /// <summary>
        /// This method creating under root or subsidiary certificate
        /// </summary>
        /// <param name="model">Certificate data</param>
        /// <returns>'true' if successfully</returns>
        /// <exception cref="Exception">if something wrong</exception>
        public bool GenerateCertificate(CertificateDataModel model)
        {
            //get root certificate
            X509Certificate rootCertificateInternal = DotNetUtilities.FromX509Certificate(model.RootCertificate);

            //generating pair keys
            SecureRandom secRnd = new();
            RsaKeyPairGenerator keyGen = new();
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(new Org.BouncyCastle.Math.BigInteger("10001", 16), new SecureRandom(), 512, 25); //todo: change key size
            keyGen.Init(param);
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            string subject = "CN=" + model.CertificateName;

            string p12File = model.OutFolder + @"/" + model.CertificateName + ".p12";
            string crtFile = model.OutFolder + @"/" + model.CertificateName + ".crt";

            byte[] serialNumber = Guid.NewGuid().ToByteArray();
            serialNumber[0] = (byte)(serialNumber[0] & 0x7F);

            //sertificate settings
            X509V3CertificateGenerator certificateGen = new X509V3CertificateGenerator();
            certificateGen.SetSerialNumber(new Org.BouncyCastle.Math.BigInteger(1, serialNumber));
            certificateGen.SetIssuerDN(rootCertificateInternal.IssuerDN);
            certificateGen.SetNotBefore(DateTime.Now.ToUniversalTime());
            certificateGen.SetNotAfter(DateTime.Now.ToUniversalTime() + new TimeSpan(model.CertificateDuration * 182, 0, 0, 0));
            certificateGen.SetSubjectDN(new Org.BouncyCastle.Asn1.X509.X509Name(subject));
            certificateGen.SetPublicKey(keyPair.Public);
            certificateGen.SetSignatureAlgorithm("MD5WITHRSA");
            certificateGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(rootCertificateInternal.GetPublicKey()));
            certificateGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keyPair.Public));
            KeyUsage keyUsage = new KeyUsage(model.CertificateName.EndsWith("CA") ? 182 : 176);
            certificateGen.AddExtension(X509Extensions.KeyUsage, true, keyUsage);
            ArrayList keyPruposes = new();
            //keyPruposes.Add(KeyPurposeID.IdKPServerAuth); //server
            keyPruposes.Add(KeyPurposeID.IdKPCodeSigning); //code sign
            //keyPruposes.Add(KeyPurposeID.IdKPEmailProtection);  //email
            certificateGen.AddExtension(X509Extensions.ExtendedKeyUsage, true, new ExtendedKeyUsage(keyPruposes));
            if (model.CertificateName.EndsWith("CA"))
                certificateGen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

            //get ready to sign certificate
            FieldInfo fieldInfo = typeof(X509V3CertificateGenerator).GetField("tbsGen", BindingFlags.NonPublic | BindingFlags.Instance);
            V3TbsCertificateGenerator v3TbsCertificate = (V3TbsCertificateGenerator)fieldInfo.GetValue(certificateGen);
            TbsCertificateStructure tbsCertificate = v3TbsCertificate.GenerateTbsCertificate();

            //counting md5 for certificate body
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] tbsCertificateHash = md5.ComputeHash(tbsCertificate.GetDerEncoded());

            //sign by .NET
            RSAPKCS1SignatureFormatter signatureFormatter = new();
            signatureFormatter.SetHashAlgorithm("MD5");
            signatureFormatter.SetKey(model.RootCertificate.PrivateKey);
            byte[] certificateSign = signatureFormatter.CreateSignature(tbsCertificateHash);

            //formatting certificate
            X509Certificate signCertificate = new X509Certificate(
                new X509CertificateStructure(
                    tbsCertificate,
                    new AlgorithmIdentifier(
                        PkcsObjectIdentifiers.MD5WithRsaEncryption),
                    new Org.BouncyCastle.Asn1.DerBitString(certificateSign)));

            //creating standard storage .p12
            try
            {
                using(FileStream fs = new FileStream(p12File, FileMode.CreateNew))
                {
                    Pkcs12Store p12 = new();
                    X509CertificateEntry certificateEntry = new(signCertificate);
                    X509CertificateEntry rootCertificateEntry = new(rootCertificateInternal);
                    p12.SetKeyEntry(model.CertificateName, new AsymmetricKeyEntry(keyPair.Private), new X509CertificateEntry[] {certificateEntry, rootCertificateEntry});
                    p12.Save(fs, model.Password.ToCharArray(), secRnd);
                }
            }
            catch(Exception a)
            {
                throw new Exception($"In the process of conservation, an error occurred: {a.Message}");
            }


            return true;
        }


        /// <summary>
        /// This method creating root certificate
        /// </summary>
        /// <param name="model">Certificate data</param>
        /// <returns>'true' if successfully</returns>
        /// <exception cref="Exception">if something wrong</exception>
        public bool GenerateRootCertificate(CertificateDataModel model)
        {
            //generating pair keys
            SecureRandom secRnd = new();
            RsaKeyPairGenerator keyGen = new();
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(new Org.BouncyCastle.Math.BigInteger("10001", 16), new SecureRandom(), 512, 25); //todo: change key size
            keyGen.Init(param);
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            //common name
            string issuer = "CN=" + model.CertificateName;

            //path and name
            string p12File = model.OutFolder + @"/" + model.CertificateName + ".p12";
            string crtFile = model.OutFolder + @"/" + model.CertificateName + ".crt";

            //serial number
            byte[] serialNumber = Guid.NewGuid().ToByteArray();
            serialNumber[0] = (byte)(serialNumber[0] & 0x7F);

            //sertificate settings
            X509V3CertificateGenerator certificateGen = new X509V3CertificateGenerator();
            certificateGen.SetSerialNumber(new Org.BouncyCastle.Math.BigInteger(1, serialNumber));
            certificateGen.SetIssuerDN(new Org.BouncyCastle.Asn1.X509.X509Name(issuer));
            certificateGen.SetNotBefore(DateTime.Now.ToUniversalTime());
            certificateGen.SetNotAfter(DateTime.Now.ToUniversalTime() + new TimeSpan(model.CertificateDuration * 182, 0, 0, 0));
            certificateGen.SetSubjectDN(new Org.BouncyCastle.Asn1.X509.X509Name(issuer));
            certificateGen.SetPublicKey(keyPair.Public);
            certificateGen.SetSignatureAlgorithm("MD5WITHRSA");
            certificateGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(keyPair.Public));
            certificateGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keyPair.Public));
            certificateGen.AddExtension(X509Extensions.BasicConstraints, false, new BasicConstraints(false));

            X509Certificate rootCertificate = certificateGen.Generate(keyPair.Private);

            byte[] rawRootCert = rootCertificate.GetEncoded();

            //saving close part
            try
            {
                using(FileStream fs = new FileStream(p12File, FileMode.CreateNew))
                {
                    Pkcs12Store pkcs12 = new();
                    X509CertificateEntry certificateEntry = new(rootCertificate);
                    pkcs12.SetKeyEntry(model.CertificateName, new AsymmetricKeyEntry(keyPair.Private),new X509CertificateEntry[] {certificateEntry} );
                    pkcs12.Save(fs, model.Password.ToCharArray(), secRnd);
                }
            }
            catch(Exception a)
            {
                throw new Exception($"In the process of conservation, an error occurred: {a.Message}");
            }

            //saving open part
            try
            {
                using(FileStream fs = new FileStream(crtFile, FileMode.CreateNew))
                {
                    fs.Write(rawRootCert, 0, rawRootCert.Length);
                    return true;
                }
            }
            catch(Exception a)
            {
                throw new Exception($"In the process of conservation, an error occurred: {a.Message}");
            }
        }
    }
}

