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
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            try
            {
                proxy.ManagePermission(isAdd, rolename, permissions);
                Console.WriteLine("Manage allowed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Manage : {0}", e.Message);
            }
        }

        public void ManageRoles(bool isAdd, string rolename)
        {
            try
            {
                proxy.ManageRoles(isAdd, rolename);
                Console.WriteLine("Manage allowed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Manage : {0}", e.Message);
            }
        }
        public void WriteToDatabase(string message, byte[] sign, UserGroup clientGroup)
        {
            try
            {
                proxy.WriteToDatabase(message, sign, clientGroup);
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
    }
}
