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
    public class WCFClient2Db : ChannelFactory<IDatabaseHandling>, IDatabaseHandling, IDisposable
    {
        IDatabaseHandling proxy;

        public WCFClient2Db(NetTcpBinding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
            WindowsIdentity myIdentity = WindowsIdentity.GetCurrent();

            // Ime klijentskog korisnika
            string clientCertCN = Formatter.ParseName(myIdentity.Name);

            // Trust Chain validacija
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            X509Certificate2 clientCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertCN);

            // Postavljanje klijentskog sertifikata
            this.Credentials.ClientCertificate.Certificate = clientCert;

            proxy = this.CreateChannel();
        }
        public void WriteToTempDb(string message, byte[] sign)
        {
            try
            {
                proxy.WriteToTempDb(message, sign);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR]: " + e.Message);
            }
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

        public void WriteToPressureDb(string message, byte[] sign)
        {
            try
            {
                proxy.WriteToPressureDb(message, sign);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR]: " + e.Message);
            }
        }

        public void WriteToSoundDb(string message, byte[] sign)
        {
            try
            {
                proxy.WriteToSoundDb(message, sign);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR]: " + e.Message);
            }
        }
    }
}
