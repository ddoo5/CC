using System;
using System.Security.Cryptography.X509Certificates;

namespace Certificate_Creator.Models
{
    public class X509Certificate2WrapperModel
    {
        private X509Certificate2 _certificate;
        private string _certificateGroupName;
        private string _group;



        public X509Certificate2WrapperModel(X509Certificate2 certificate, string certificateGroupName, string group)
        {
            _certificate = certificate;
            _certificateGroupName = certificateGroupName;
            _group = group;
        }



        public X509Certificate2 Certificate
        {
            get
            {
                return _certificate;
            }
        }


        public string PublishedFor
        {
            get
            {
                return _certificate.GetNameInfo(X509NameType.SimpleName, false);
            }
        }


        public string Published
        {
            get
            {
                return _certificate.GetNameInfo(X509NameType.SimpleName, true);
            }
        }


        public string ExpirationDate
        {
            get
            {
                return _certificate.GetExpirationDateString();
            }
        }


        public string Group
        {
            get
            {
                return _group;
            }
        }


        public string GroupName
        {
            get
            {
                return _certificateGroupName;
            }
        }


        public override string ToString()
        {
            return $"Group: {Group}\n" +
                $"Group name: {GroupName}\n" +
                $"Published for: {PublishedFor}\n" +
                $"Expiration date: {ExpirationDate}";
        }
    }
}

