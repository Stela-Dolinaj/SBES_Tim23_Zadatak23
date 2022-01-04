using Contracts;
using Contracts.Enums;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class WCFClient2Client : ChannelFactory<IClientCommunications>,
        IClientCommunications, IDisposable
    {
        IClientCommunications proxy;
        readonly UserGroup myGroup = UserGroup.NULL;

        public WCFClient2Client(NetTcpBinding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
            WindowsIdentity myIdentity = WindowsIdentity.GetCurrent();

            // Ime klijentskog korisnika
            string clientCertCN = Formatter.ParseName(myIdentity.Name);

            // Grupa klijenta
            //myGroup = CertManager.GetMyGroup(myIdentity);

            // Trust Chain validacija
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            X509Certificate2 clientCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertCN);

            myGroup = CertManager.GetMyGroupFromCert(clientCert);

            // Postavljanje klijentskog sertifikata
            this.Credentials.ClientCertificate.Certificate = clientCert;

            proxy = this.CreateChannel();
        }

        public void TestCommunication()
        {
            try
            {
                proxy.TestCommunication();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR]: " + e.Message);
            }
        }

        public bool SendMessage(string messageForClients, UserGroup clientGroup)
        {
            try
            {
                return proxy.SendMessage(messageForClients, clientGroup);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR]: " + e.Message);
                return false;
            }
        }
    }
}
