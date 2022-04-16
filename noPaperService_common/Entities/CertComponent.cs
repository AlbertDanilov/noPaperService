using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    public class CertComponent
    {
        public X509Certificate2 cert = null;
        public String thumbprint;
        public String valid;
        public String issued;
        public String fullIssued;
        public String organization;
        public String subject;
        public String fullSubject;
        public String serialNumber;

        public CertComponent(X509Certificate2 cert)
        {
            this.cert = cert;
        }
    }
}
