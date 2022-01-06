using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // pokreni visual studio u admin modu
            //Debugger.Launch();

            // Ime servisnog korisnika
            string serviceCertCN =
                Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType =
                TcpClientCredentialType.Certificate;

            string hostAddressC2DB = "net.tcp://localhost:8080/DbService";
            string hostAddressC2C = "net.tcp://localhost:8090/ClientCommunication";

            ServiceHost host = new ServiceHost(typeof(DatabaseHandlingService));

            // endpoint za komunikaciju sa bazom podataka
            host.AddServiceEndpoint(typeof(IDatabaseHandling), binding, hostAddressC2DB);
            // endpoint za medjuklijentsku komunikaciju
            host.AddServiceEndpoint(typeof(IClientCommunications), binding, hostAddressC2C);

            // Trust Chain validacija
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode =
                X509RevocationMode.NoCheck;

            // Postavljanje servisnog sertifikata
            host.Credentials.ServiceCertificate.Certificate =
                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serviceCertCN);

            try
            {
                host.Open();

                Console.WriteLine(">> DbHandling Service is running...\nPress <enter> to stop ...");
                Console.ReadLine();

                Console.WriteLine("\n\n>> Service is shutting down...");
                Thread.Sleep(1500);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.ReadLine();
            }
            finally
            {
                host.Close();
            }
        }
    }
}
